using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace StoreBack.Models {

    public class Organization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        public string Email { get; set; }

   
        public ICollection<User> Users { get; set; }
        public ICollection<Branches> Branches { get; set; }
    }
}
