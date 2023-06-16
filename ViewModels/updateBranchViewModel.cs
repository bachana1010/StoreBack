using System.ComponentModel.DataAnnotations;

namespace StoreBack.ViewModels {

public class UpdateBranchViewModel
{
    [Required]
    public string BranchName { get; set; }

}
}
