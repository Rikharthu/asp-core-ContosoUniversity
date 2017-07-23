using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public enum Grade
    {
        A, B, C, D, F
    }

    public class Enrollment
    {
        // Primary Key
        public int EnrollmentID { get; set; }
        public int CourseID { get; set; }
        // Foreign Key, corresponding navigation property is Student
        public int StudentID { get; set; }
        // ? = nullable
        [DisplayFormat(NullDisplayText = "No grade")]
        public Grade? Grade { get; set; }

        // Navigation properties
        public Course Course { get; set; }
        public Student Student { get; set; }

        /* !!! ACHTUNG !!!
         * Entity Framework interprets a property as a foreign key property if it's named 
         * <navigation property name><primary key property name> (for example, 
         * StudentID for the Student navigation property since the Student entity's 
         * primary key is ID). Foreign key properties can also be named simply 
         * <primary key property name> (for example, CourseID since the Course entity's 
         * primary key is CourseID).
         */

    }
}
