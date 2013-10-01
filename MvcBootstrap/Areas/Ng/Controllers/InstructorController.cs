using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.Helpers;
using MvcBootstrap.Models;
using MvcBootstrap.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Areas.Ng.Controllers
{
    public class InstructorController : Controller
    {
        private IInstructorRepository repository;

        public InstructorController(IInstructorRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            InstructorIndexData viewModel = new InstructorIndexData();

            if (searchString != null)
                page = 1;

            var instructors = repository.GetInstructors()
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(x => x.Department));

            if (!string.IsNullOrEmpty(keyword))
            {
                instructors = instructors.Where(x => x.LastName.ToUpper().Contains(keyword) ||
                    x.FirstMidName.ToUpper().Contains(keyword));
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    instructors = instructors.OrderByDescending(x => x.LastName);
                    break;

                case "FirstName":
                    instructors = instructors.OrderBy(x => x.FirstMidName);
                    break;

                case "FirstName_desc":
                    instructors = instructors.OrderByDescending(x => x.FirstMidName);
                    break;

                case "Date":
                    instructors = instructors.OrderBy(x => x.HireDate);
                    break;

                case "Date_desc":
                    instructors = instructors.OrderByDescending(x => x.HireDate);
                    break;

                case "Loc":
                    instructors = instructors.OrderBy(x => x.OfficeAssignment.Location);
                    break;

                case "Loc_desc":
                    instructors = instructors.OrderByDescending(x => x.OfficeAssignment.Location);
                    break;

                default:
                    instructors = instructors.OrderBy(x => x.LastName);
                    break;
            }

            int pageSize = Constants.PAGE_SIZE;
            int pageNumber = (page ?? 1);

            var l = instructors.ToPagedList(pageNumber, pageSize);
            var lx = l.Select(x => new
            {
                PersonID = x.PersonID,
                LastName = x.LastName,
                FirstMidName = x.FirstMidName,
                HireDate = x.HireDate,
                OfficeAssignment = new { Location = x.OfficeAssignment == null ? null : x.OfficeAssignment.Location },
                Courses = x.Courses == null ? null : x.Courses.Select(c => new { CourseID = c.CourseID, Title = c.Title })
            });
            Pager pager = new Pager(l.TotalItemCount, l.PageNumber, l.PageSize);
            Dictionary<string, object> res = new Dictionary<string, object>
            {
                { "pager", pager },
                { "model", lx }
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Courses(int? id)
        {
            if (id != null)
            {
                var courses = repository.GetInstructors()
                    .Include(i => i.Courses.Select(x => x.Department))
                    .Where(i => i.PersonID == id.Value).Single().Courses;
                var lx = courses.Select(x => new
                {
                    PersonID = id,
                    CourseID = x.CourseID,
                    Title = x.Title,
                    Department = new { Name = x.Department == null ? null : x.Department.Name }
                });

                return Json(lx, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<byte>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Enrollments(int? id, int? courseID)
        {
            if (id != null && courseID != null)
            {
                var enrollments = repository.GetInstructors()
                    .Include(i => i.Courses.Select(x => x.Department))
                    .Where(i => i.PersonID == id.Value).Single().Courses
                    .Where(i => i.CourseID == courseID).Single().Enrollments;
                var lx = enrollments.Select(x => new
                {
                    Student = new { FullName = x.Student == null ? null : x.Student.FullName },
                    Grade = x.Grade == null ? Enrollment.NO_GRADE : Enum.GetName(typeof(Grade), x.Grade)
                });

                return Json(lx, JsonRequestBehavior.AllowGet);
            }

            return Json(new List<byte>(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id = 0)
        {
            Instructor instructor = repository.GetByID(id);
            var model = new
            {
                FirstMidName = instructor.FirstMidName,
                FullName = instructor.FullName,
                HireDate = instructor.HireDate,
                LastName = instructor.LastName,
                OfficeAssignment = new { Location = instructor.OfficeAssignment == null ? null : instructor.OfficeAssignment.Location },
                PersonID = instructor.PersonID
            };

            Dictionary<string, object> res = new Dictionary<string, object>
            {
                { "model", model }
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create(Instructor instructor, string[] selectedCourses)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            res["success"] = 1;
            res["message"] = string.Format("{0} has been saved", instructor.FullName);

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AllCourses()
        {
            List<AssignedCourseData> o = GetCourses();
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }

        private List<AssignedCourseData> GetCourses(Instructor instructor = null)
        {
            DbSet<Course> allCourses = repository.Context.Courses;
            HashSet<int> instructorCourses = null;

            if (instructor != null)
                instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));

            List<AssignedCourseData> viewModel = new List<AssignedCourseData>();
            foreach (Course course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructor == null ? false : instructorCourses.Contains(course.CourseID)
                });
            }

            return viewModel;
        }
    }
}
