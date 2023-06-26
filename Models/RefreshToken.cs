using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using StoreBack.Models;

namespace StoreBack.Models {

    public class RefreshTokens
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        public DateTime Expires { get; set; }

        [Required]
        [ForeignKey("Users")]
        public int UserId { get; set; }

        public DateTime? Revoked { get; set; } // This should be nullable

        // Navigation property
        public virtual User User { get; set; }
    }
}
