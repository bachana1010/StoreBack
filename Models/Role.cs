using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic; // Required for ICollection

namespace StoreBack.Models {

    public class Role
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Key { get; set; }
    }
}
