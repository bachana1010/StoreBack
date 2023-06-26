using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Required for ICollection

namespace StoreBack.Models {

    // public enum Role
    // {
    //     Administrator,
    //     Operator,
    //     Manager
    // }

    public class User
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Organization")]
        public int OrganizationId { get; set; }

        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public int RoleId {get; set;}

        public string? Role { get; set; }

        [ForeignKey("Branch")]
        public int? BranchId { get; set; }

        public DateTime? DeletedAt { get; set; }

        // Navigation properties
        public Organization Organization { get; set; }

        public Branches Branch { get; set; }

        // Add this line
        public ICollection<Branches> Branches { get; set; }
    }
}
