using System.ComponentModel.DataAnnotations;

namespace StoreBack.ViewModels {

public class AddBranchViewModel
{

    [Required]
    public string BranchName { get; set; }

    
}
}
