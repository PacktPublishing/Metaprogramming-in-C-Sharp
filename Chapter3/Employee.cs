using System.ComponentModel.DataAnnotations;

namespace Chapter3;

public record Employee(
    [Required]
    string FirstName,

    [Required]
    string LastName);
