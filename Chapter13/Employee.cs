using System.ComponentModel.DataAnnotations;

namespace Chapter13;

public record Employee(
    [Required]
    string FirstName,

    [Required]
    string LastName);
