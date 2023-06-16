using System.ComponentModel.DataAnnotations;

namespace StoreBack.ViewModels {

public class UpdateserViewModel
{
    public string Email { get; set; }

    [Required]
    public string UserName { get; set; }

    // Removed [Required] from Password
    public string? Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

}
}
