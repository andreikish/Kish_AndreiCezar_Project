using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kish_AndreiCezar_Project.Models;

public class Customer
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress]
    public string? Email { get; set; }

    [Phone]
    public string? Phone { get; set; }

    public ICollection<ServiceTicket> ServiceTickets { get; set; } = new List<ServiceTicket>();
}

