using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.Helpers;
using MvcBootstrap.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Controllers
{
    public class CourseController : Controller
    {
        public const string MENU = "Course";
        private ICourseRepository repository;

        public CourseController(ICourseRepository repository)
        {
            this.repository = repository;
        }

        //
        // GET: /Course/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.menu = MENU;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.TitleSortParm = string.IsNullOrEmpty(sortOrder) ? "Title_desc" : "";
            ViewBag.DeptSortParm = sortOrder == "Dept" ? "Dept_desc" : "Dept";
            ViewBag.CourseIDSortParm = sortOrder == "CourseID" ? "CourseID_desc" : "CourseID";
            ViewBag.CreditsSortParm = sortOrder == "Credits" ? "Credits_desc" : "Credits";

            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            if (searchString != null)
                page = 1;

            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

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

            return View(courses.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Course/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.menu = MENU;
            Course course = repository.GetByID(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        //
        // GET: /Course/Create

        public ActionResult Create()
        {
            ViewBag.menu = MENU;
            PopulateDepartmentsDropDownList();
            return View();
        }

        //
        // POST: /Course/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CourseID,Title,Credits,DepartmentID")] Course course)
        {
            ViewBag.menu = MENU;
            try
            {
                if (ModelState.IsValid)
                {
                    repository.Insert(course);
                    repository.Save();
                    TempData["message"] = string.Format("{0} has been saved", course.Title);
                    return RedirectToAction("Index");
                }
            }

            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
            }

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        //
        // GET: /Course/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.menu = MENU;
            Course course = repository.GetByID(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        //
        // POST: /Course/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Course course)
        {
            ViewBag.menu = MENU;
            if (ModelState.IsValid)
            {
                repository.Update(course);
                repository.Save();
                TempData["message"] = string.Format("{0} has been saved", course.Title);
                return RedirectToAction("Index");
            }

            PopulateDepartmentsDropDownList(course.DepartmentID);
            return View(course);
        }

        //
        // GET: /Course/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ViewBag.menu = MENU;
            Course course = repository.GetByID(id);
            if (course == null)
            {
                return HttpNotFound();
            }

            return View(course);
        }

        //
        // POST: /Course/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.menu = MENU;
            repository.Delete(id);
            repository.Save();
            TempData["message"] = "Course was deleted";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            IOrderedQueryable<Department> departmentsQuery = repository.Context.Departments.OrderBy(x => x.Name);
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }
    }
}