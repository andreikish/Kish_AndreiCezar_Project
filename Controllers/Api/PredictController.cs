using Kish_AndreiCezar_Project.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kish_AndreiCezar_Project.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PredictController : ControllerBase
{
    private readonly PredictionEngineService _prediction;

    public PredictController(PredictionEngineService prediction)
    {
        _prediction = prediction;
    }

    [HttpPost]
    public ActionResult<PredictionResponse> Post([FromBody] PredictionRequest request)
    {
        var cost = _prediction.PredictCost(request.MileageKm, request.EstimatedHours, request.IsPremiumBrand);
        return Ok(new PredictionResponse { PredictedCost = cost });
    }
}

public class PredictionRequest
{
    public int MileageKm { get; set; }
    public float EstimatedHours { get; set; }
    public bool IsPremiumBrand { get; set; }
}

public class PredictionResponse
{
    public float PredictedCost { get; set; }
}

