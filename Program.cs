using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Servicii de infrastructura
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=autoservice.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddGrpc();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<PredictionEngineService>();

var app = builder.Build();

// Creare si seed baza de date
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapStaticAssets();

// Rute API/atribute + MVC
app.MapControllers();
// Endpoints MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Endpoints gRPC
app.MapGrpcService<MaintenanceService>();

app.Run();
