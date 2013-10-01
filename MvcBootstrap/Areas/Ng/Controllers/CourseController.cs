using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.Helpers;
using MvcBootstrap.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Areas.Ng.Controllers
{
    public class CourseController : Controller
    {
        private ICourseRepository repository;

        public CourseController(ICourseRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            if (searchString != null)
                page = 1;

            var courses = repository.GetCourses().Include(c => c.Department);

            if (!string.IsNullOrEmpty(keyword))
            {
                courses = courses.Where(x => x.Title.Contains(keyword) ||
                    x.Department.Name.Contains(keyword));
            }

            switch (sortOrder)
            {
                case "Title_desc":
                    courses = courses.OrderByDescending(x => x.Title);
                    break;

                case "Dept":
                    courses = courses.OrderBy(x => x.Department.Name);
                    break;

                case "Dept_desc":
                    courses = courses.OrderByDescending(x => x.Department.Name);
                    break;

                case "CourseID":
                    courses = courses.OrderBy(x => x.CourseID);
                    break;

                case "CourseID_desc":
                    courses = courses.OrderByDescending(x => x.CourseID);
                    break;

                case "Credits":
                    courses = courses.OrderBy(x => x.Credits);
                    break;

                case "Credits_desc":
                    courses = courses.OrderByDescending(x => x.Credits);
                    break;

                default:
                    courses = courses.OrderBy(x => x.Title);
                    break;
            }

            int pageSize = Constants.PAGE_SIZE;
            int pageNumber = (page ?? 1);

            var l = courses.ToPagedList(pageNumber, pageSize);
            var lx = l.Select(x => new
            {
                CourseID = x.CourseID,
                Credits = x.Credits,
                Department = new { Name = x.Department == null ? null : x.Department.Name },
                Title = x.Title
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
            Course course = repository.GetByID(id);
            var model = new
            {
                CourseID = course.CourseID,
                Credits = course.Credits,
                Department = new { Name = course.Department == null ? null : course.Department.Name },
                Title = course.Title
            };

            Dictionary<string, object> res = new Dictionary<string, object>
            {
                { "model", model }
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")] Course course)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                //throw new Exception("Error" + DateTime.Now);
                if (ModelState.IsValid)
                {
                    repository.Insert(course);
                    repository.Save();
                    res["success"] = 1;
                    res["message"] = string.Format("{0} has been saved", course.Title);
                }

                else
                {
                    return Json(ModelState, JsonRequestBehavior.AllowGet);
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
            Course course = repository.GetByID(id);
            var o = new
            {
                CourseID = course.CourseID,
                Credits = course.Credits,
                DepartmentID = course.DepartmentID,
                Title = course.Title,
                DepartmentIDList = GetDepartments()
            };
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Edit(Course course)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                if (ModelState.IsValid)
                {
                    repository.Update(course);
                    repository.Save();
                    res["success"] = 1;
                    res["message"] = string.Format("{0} has been saved", course.Title);
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
                res["message"] = string.Format("{0} course(s) has been successfully deleted", ids.Count);
            }

            catch (Exception ex)
            {
                res["error"] = 1;
                res["message"] = ex.ToString();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Departments()
        {
            object o = GetDepartments();
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }

        private object GetDepartments()
        {
            IOrderedQueryable<Department> departmentsQuery = repository.Context.Departments.OrderBy(x => x.Name);
            List<Department> l = departmentsQuery.ToList();
            var o = l.Select(x => new { DepartmentID = x.DepartmentID, Name = x.Name });
            return o;
        }
    }
}
