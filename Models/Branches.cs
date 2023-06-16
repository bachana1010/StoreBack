using System.ComponentModel.DataAnnotations.Schema;

namespace StoreBack.Models
{
public class Branches
{
    public int Id { get; set; }
    public int OrganizationId { get; set; }
    public string BrancheName { get; set; }
    public int AddedByUserId { get; set; }

    public DateTime? DeletedAt { get; set; }  // New property

    [NotMapped]
    public User? AddedByUser { get; set; }

    [NotMapped]
    public Organization? Organization { get; set; }

    [NotMapped]
    public ICollection<User>? Users { get; set; }
}

}