using System.ComponentModel.DataAnnotations;

namespace StoreBack.ViewModels {

public class RegisterOrganizationViewModel
{
    [Required]
    public string OrganizationName { get; set; }

    [Required]
    public string Address { get; set; }

    public string Email { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }
}
}