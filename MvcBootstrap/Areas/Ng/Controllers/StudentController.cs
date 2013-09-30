using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.Helpers;
using MvcBootstrap.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Areas.Ng.Controllers
{
    public class StudentController : Controller
    {
        private IStudentRepository repository;

        public StudentController(IStudentRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            if (searchString != null)
                page = 1;

            var students = repository.GetStudents();

            if (!string.IsNullOrEmpty(keyword))
            {
                students = students.Where(x => x.LastName.ToUpper().Contains(keyword) ||
                    x.FirstMidName.ToUpper().Contains(keyword));
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    students = students.OrderByDescending(x => x.LastName);
                    break;

                case "FirstName":
                    students = students.OrderBy(x => x.FirstMidName);
                    break;

                case "FirstName_desc":
                    students = students.OrderByDescending(x => x.FirstMidName);
                    break;

                case "Date":
                    students = students.OrderBy(x => x.EnrollmentDate);
                    break;

                case "Date_desc":
                    students = students.OrderByDescending(x => x.EnrollmentDate);
                    break;

                default:
                    students = students.OrderBy(x => x.LastName);
                    break;
            }

            int pageSize = Constants.PAGE_SIZE;
            int pageNumber = (page ?? 1);

            var l = students.ToPagedList(pageNumber, pageSize);
            var lx = l.Select(x => new
            {
                EnrollmentDate = x.EnrollmentDate,
                FirstMidName = x.FirstMidName,
                LastName = x.LastName,
                PersonID = x.PersonID
            });
            Pager pager = new Pager(l.TotalItemCount, l.PageNumber, l.PageSize);
            Dictionary<string, object> res = new Dictionary<string, object>
            {
                { "pager", pager },
                { "model", lx }
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id = 0)
        {
            Student student = repository.GetByID(id);
            var enrollments = student.Enrollments.Select(x => new
            {
                Course = new Course { Title = x.Course.Title },
                Grade = x.Grade == null ? Enrollment.NO_GRADE : Enum.GetName(typeof(Grade), x.Grade)
            });

            var model = new
            {
                EnrollmentDate = student.EnrollmentDate,
                FirstMidName = student.FirstMidName,
                FullName = student.FullName,
                LastName = student.LastName,
                PersonID = student.PersonID,
                Enrollments = enrollments
            };

            Dictionary<string, object> res = new Dictionary<string, object>
            {
                { "model", model },
                { "enrollments", enrollments }
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "LastName, FirstMidName, EnrollmentDate")] Student student)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                //throw new Exception("Error" + DateTime.Now);
                if (ModelState.IsValid)
                {
                    repository.Insert(student);
                    repository.Save();
                    res["success"] = 1;
                    res["message"] = string.Format("{0} has been saved", student.FullName);
                }
            }

            catch (Exception ex)
            {
                res["error"] = 1;
                res["message"] = ex.ToString();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Edit(int id = 0)
        {
            Student student = repository.GetByID(id);
            Student o = new Student
            {
                EnrollmentDate = student.EnrollmentDate,
                FirstMidName = student.FirstMidName,
                LastName = student.LastName,
                PersonID = student.PersonID
            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(Student student)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                if (ModelState.IsValid)
                {
                    repository.Update(student);
                    repository.Save();
                    res["success"] = 1;
                    res["message"] = string.Format("{0} has been saved", student.FullName);
                }
            }

            catch (Exception ex)
            {
                res["error"] = 1;
                res["message"] = ex.ToString();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(List<int> ids)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                repository.Delete(ids);
                repository.Save();
                res["success"] = 1;
                res["message"] = string.Format("{0} student(s) has been successfully deleted", ids.Count);
            }

            catch (Exception ex)
            {
                res["error"] = 1;
                res["message"] = ex.ToString();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }
    }
}
