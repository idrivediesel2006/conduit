using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Data
{
    public class Tag
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string DisplayName { get; set; }

        public ICollection<Editorial> Editorials { get; set; }

        public Tag()
        {
            Editorials = new HashSet<Editorial>();
        }
    }
}
