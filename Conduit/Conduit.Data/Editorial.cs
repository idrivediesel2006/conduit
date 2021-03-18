using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Conduit.Data
{
    public class Editorial
    {
        [Key]
        public int ID { get; set; }

        [Required]
        public int PersonId { get; set; }

        [Required]
        [StringLength(50)]
        public string Slug { get; set; }

        [Required]
        [StringLength(50)]
        public string Title { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [Required]
        [StringLength(50)]
        public string Body { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdateAt { get; set; }

        public Person Person { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<Commentary> Commentaries { get; set; }
        public ICollection<Favorite> Favorites { get; set; }

        public Editorial()
        {
            Tags = new HashSet<Tag>();
            Commentaries = new HashSet<Commentary>();
            Favorites = new HashSet<Favorite>();
        }
    }
}
