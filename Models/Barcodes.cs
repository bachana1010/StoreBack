using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBack.Models
{
public class Barcodes
{
    [Key]
    public int Id { get; set; }

    public string Barcode { get; set; }
    
    public string Name { get; set; }

    public float Price { get; set; }

    public string Unit { get; set; }

    [ForeignKey("Branch")]
    public int BranchId { get; set; }

    [ForeignKey("Organization")]
    public int OrganizationId { get; set; }


    [NotMapped]
    public Branches Branch { get; set; }

    [NotMapped]
    public Organization Organization { get; set; }
}

}