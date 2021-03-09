using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Conduit.Data
{
    [Table("People")]
    public class Person
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(200)]
        public string Bio { get; set; }

        [Required]
        [StringLength(200)]
        public string Image { get; set; }

        public Account Account { get; set; }
        public virtual ICollection<Follow> FollowerNavigations { get; set; }
        public virtual ICollection<Follow> FollowingNavigations { get; set; }

        public Person()
        {
            FollowerNavigations = new HashSet<Follow>();
            FollowingNavigations = new HashSet<Follow>();
        }
    }
}
