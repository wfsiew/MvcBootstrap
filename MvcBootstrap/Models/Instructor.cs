using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;

namespace MvcBootstrap.Models
{
    public class Instructor : Person
    {
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Hire Date")]
        public DateTime HireDate { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual OfficeAssignment OfficeAssignment { get; set; }

        public string GetCourses(bool ishtml)
        {
            IEnumerable<Course> l = Courses ?? new List<Course>();
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < l.Count(); i++)
            {
                Course o = l.ElementAt(i);
                string s = string.Format("{0} {1}", o.CourseID, o.Title);

                if (i < l.Count() - 1)
                {
                    if (ishtml)
                        sb.Append(s + "<br/>");

                    else
                        sb.AppendLine(s);
                }

                else
                    sb.Append(s);
            }

            return sb.ToString();
        }
    }
}