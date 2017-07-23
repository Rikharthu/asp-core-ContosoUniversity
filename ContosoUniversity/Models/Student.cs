using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class Student : Person
    {
                [Display(Name = "Enrollment Date")]
        [DataType(DataType.Date)]  // mark this type as Date for browsers support (HTML5 features and etc)
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)] // specify the desired date format instead of using database defaults
        // ApplyFormatInEditMode - persist formatting in edit fields too 
        public DateTime EnrollmentDate { get; set; }

        // This is a navigation property that holds all enrollments for current student
        // ICollection will be created as HashSet by default
        public ICollection<Enrollment> Enrollments { get; set; }

    }
}
