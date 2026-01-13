using System.ComponentModel.DataAnnotations;

namespace Kish_AndreiCezar_Project.Models;

public class ServiceTicket
{
    public int Id { get; set; }

    public int CarModelId { get; set; }
    public CarModel? CarModel { get; set; }

    public int CustomerId { get; set; }
    public Customer? Customer { get; set; }

    public int MechanicId { get; set; }
    public Mechanic? Mechanic { get; set; }

    [DataType(DataType.Date)]
    public DateOnly IntakeDate { get; set; }

    [Required, StringLength(200)]
    public string Complaint { get; set; } = string.Empty;

    [Range(0, 1000000)]
    public int MileageKm { get; set; }

    [StringLength(40)]
    public string Status { get; set; } = "In lucru";

    [Range(0, 200)]
    public float EstimatedHours { get; set; }

    [Range(0, 100000)]
    public float EstimatedCost { get; set; }
}

