using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace Kish_AndreiCezar_Project.Models;

public class Manufacturer
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string Country { get; set; } = string.Empty;

    public ICollection<CarModel> Models { get; set; } = new List<CarModel>();
}

