using Kish_AndreiCezar_Project.Data;
using Kish_AndreiCezar_Project.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=autoservice.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<PredictionEngineService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
    
    try
    {
        var connection = db.Database.GetDbConnection();
        connection.Open();
        using var command = connection.CreateCommand();
        
        command.CommandText = "PRAGMA table_info(CarModels);";
        using var reader = command.ExecuteReader();
        var hasVehicleType = false;
        while (reader.Read())
        {
            if (reader.GetString(1) == "VehicleType")
            {
                hasVehicleType = true;
                break;
            }
        }
        reader.Close();
        
        if (!hasVehicleType)
        {
            command.CommandText = "ALTER TABLE CarModels ADD COLUMN VehicleType TEXT NOT NULL DEFAULT 'Car';";
            command.ExecuteNonQuery();
            
            command.CommandText = @"
                UPDATE CarModels SET VehicleType = 'SUV' WHERE Name LIKE '%SUV%' OR Name LIKE '%Suv%';
                UPDATE CarModels SET VehicleType = 'Truck' WHERE Name LIKE '%Truck%' OR Name LIKE '%Van%';
                UPDATE CarModels SET VehicleType = 'Bus' WHERE Name LIKE '%Bus%';
                UPDATE CarModels SET VehicleType = 'Motorcycle' WHERE Name LIKE '%Motorcycle%' OR Name LIKE '%Bike%';
            ";
            command.ExecuteNonQuery();
        }
        
        connection.Close();
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogWarning(ex, "Nu s-a putut actualiza schema bazei de date. Poate fi necesar să ștergeți baza de date.");
    }
}

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

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
