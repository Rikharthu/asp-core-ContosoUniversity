using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Models
{
    public abstract class Person
    {
        // By default Entity Framework interprets property named ID or classnameID as the primary key
        public int ID { get; set; }

        // add string length validation
        [StringLength(50)]
        [RegularExpression(@"^[A-Z]+[a-zA-Z''-'\s]*$")] // to prevent user from entering whitespace characters
        [Required] // makes this required field in forms (can be replaced with MinimumLength: [StringLength(50, MinimumLength=1)])
        [Display(Name = "Last Name")] // affects @Html.DisplayFor(modelItem => item.LastName)
        public string LastName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "First name cannot be longer than 50 characters.")]
        [Column("FirstName")] // we want the column in the database to be called "FirstName" instead of using property name "FirstMidName"
        public string FirstMidName { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return LastName + ", " + FirstMidName;
            }
            //! note that it has only the get accessor, thus column in the database wont be generated!
        }

    }
}
