using Grpc.Core;
using Kish_AndreiCezar_Project.Protos;

namespace Kish_AndreiCezar_Project.Services;

public class MaintenanceService : MaintenanceAdvisor.MaintenanceAdvisorBase
{
    public override Task<MaintenanceReply> GetSchedule(MaintenanceRequest request, ServerCallContext context)
    {
        var tasks = new List<string> { "Inspectie vizuala", "Diagnoza rapida" };

        if (request.MileageKm > 150000)
        {
            tasks.Add("Inlocuire distributie");
            tasks.Add("Verificare compresie motor");
        }
        else if (request.MileageKm > 80000)
        {
            tasks.Add("Schimb lichid frana");
            tasks.Add("Curatare EGR");
        }
        else
        {
            tasks.Add("Schimb ulei + filtre");
            tasks.Add("Rotire anvelope");
        }

        if (!string.IsNullOrWhiteSpace(request.CarModel) && request.CarModel.ToLower().Contains("diesel"))
        {
            tasks.Add("Regenerare DPF asistata");
        }

        var severity = request.MileageKm switch
        {
            > 180000 => "Critica",
            > 120000 => "Ridicata",
            > 60000 => "Medie",
            _ => "Redusa"
        };

        var reply = new MaintenanceReply
        {
            Recommendation = $"Plan pentru {request.CarModel} la {request.MileageKm} km",
            Severity = severity
        };
        reply.SuggestedTasks.AddRange(tasks);

        return Task.FromResult(reply);
    }
}

