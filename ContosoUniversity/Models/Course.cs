using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public class Course
    {
        // This lets us enter the PK instead of having the database generate it
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Number")]
        public int CourseID { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [Range(0, 5)]
        public int Credits { get; set; }

        public int DepartmentID { get; set; }

        // Navigation property
        public Department Department { get; set; }
        // A Course entity can be related to any number of enrollments
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<CourseAssignment> CourseAssignments { get; set; }

        /* The Entity Framework doesn't require you to add a foreign key property to your data model 
         * when you have a navigation property for a related entity. EF automatically creates foreign 
         * keys in the database wherever they are needed and creates shadow properties for them. 
         * But having the foreign key in the data model can make updates simpler and more efficient. 
         * For example, when you fetch a course entity to edit, the Department entity is null if you 
         * don't load it, so when you update the course entity, you would have to first fetch the 
         * Department entity. When the foreign key property DepartmentID is included in the data model, 
         * you don't need to fetch the Department entity before you update. */

    }
}
