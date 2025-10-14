using System.ComponentModel.DataAnnotations;

namespace web.Domain.Entities;

public class User
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Email { get; set; }
}
