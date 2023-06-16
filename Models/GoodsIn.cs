using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBack.Models
{
public class Goodsin
{
    [Key]
    public int Id { get; set; }

    [ForeignKey("Branch")]
    public int BranchId { get; set; }

    public DateTime EntryDate { get; set; }

    public float Quantity { get; set; }
    
    [ForeignKey("User")]
    public int OperatorUserId { get; set; }


    [ForeignKey("Organization")]
    public int OrganizationId { get; set; }


    [ForeignKey("Barcode")]
    public int BarcodeId { get; set; }



    [NotMapped]
    public Branches Branch { get; set; }

    [NotMapped]
    public Organization Organization { get; set; }

    [NotMapped]
    public User user { get; set; }

    [NotMapped]
    public Barcodes Barcodes { get; set; }
}
}

