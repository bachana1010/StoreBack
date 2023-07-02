

using System.ComponentModel.DataAnnotations;



    
namespace StoreBack.ViewModels 
{
    public class DashboardDataViewModel
    {
        [Display(Name = "User Count")]
        public int UserCount { get; set; }

        [Display(Name = "Operator Count")]
        public int OperatorCount { get; set; }

        [Display(Name = "Manager Count")]
        public int ManagerCount { get; set; }

        [Display(Name = "Goods In Count")]
        public int GoodsInCount { get; set; }

        [Display(Name = "Goods Out Count")]
        public int GoodsOutCount { get; set; }

        [Display(Name = "Product Count")]
        public int ProductCount { get; set; }

        [Display(Name = "Branch Count")]
        public int BranchCount { get; set; }
    }
}


