using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kish_AndreiCezar_Project.Models;

public class Mechanic
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [StringLength(120)]
    public string Specialty { get; set; } = string.Empty;

    [Range(0, 60)]
    public int YearsExperience { get; set; }

    public ICollection<ServiceTicket> ServiceTickets { get; set; } = new List<ServiceTicket>();
}

