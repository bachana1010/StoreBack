namespace StoreBack.Models
{
    public class GetBarcodeBalanceViewModel
    {
        public int Id {get; set;}
        public string Barcode { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public float Quantity { get; set; }

        public string Unit { get; set; }

        public string? BranchName { get; set; }
    }
}
