using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class OfficeAssignment
    {
        // EF can't automatically recognize this as the primary key of this entity
        // because its name doesn't follow the ID or classnameID convention => use [Key] attribute
        [Key]
        public int InstructorID { get; set; }
        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; }

        // Navigation property
        public Instructor Instructor { get; set; }
    }
}