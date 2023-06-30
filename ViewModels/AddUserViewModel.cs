using System.ComponentModel.DataAnnotations;

namespace StoreBack.ViewModels {

public class AddUserViewModel
{
    public string Email { get; set; }

    [Required]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Role { get; set; }

    public int? BranchId { get; set; }
}


}
