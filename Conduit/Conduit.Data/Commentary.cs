using System;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Data
{
    public class Commentary
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        public int EditorialId { get; set; }

        [Required]
        [StringLength(200)]
        public string Body { get; set; }

        [Required]
        public DateTime CreateAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }

        public Person Person { get; set; }
        public Editorial Editorial { get; set; }
    }
}
