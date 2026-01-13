using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kish_AndreiCezar_Project.Models;

public class CarModel
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Name { get; set; } = string.Empty;

    [Range(1950, 2100)]
    public int YearFrom { get; set; }

    [Required, StringLength(20)]
    [Display(Name = "Tip")]
    public string VehicleType { get; set; } = "Car";

    public int ManufacturerId { get; set; }
    public Manufacturer? Manufacturer { get; set; }

    public ICollection<ServiceTicket> ServiceTickets { get; set; } = new List<ServiceTicket>();
}

