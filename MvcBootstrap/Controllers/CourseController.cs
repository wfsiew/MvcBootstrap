using MvcBootstrap.Context;
using MvcBootstrap.Models;
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
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Course/

        public ActionResult Index()
        {
            ViewBag.menu = MENU;
            var courses = db.Courses.Include(c => c.Department);
            return View(courses.ToList());
        }

        //
        // GET: /Course/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.menu = MENU;
            Course course = db.Courses.Find(id);
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
                    db.Courses.Add(course);
                    db.SaveChanges();
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
            Course course = db.Courses.Find(id);
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
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                TempData["message"] = string.Format("{0} has been saved", course.Title);
                return RedirectToAction("Index");
            }

            ViewBag.DepartmentID = new SelectList(db.Departments, "DepartmentID", "Name", course.DepartmentID);
            return View(course);
        }

        //
        // GET: /Course/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ViewBag.menu = MENU;
            Course course = db.Courses.Find(id);
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
            Course course = new Course { CourseID = id };
            db.Entry(course).State = EntityState.Deleted;
            db.SaveChanges();
            TempData["message"] = "Course was deleted";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            IOrderedQueryable<Department> departmentsQuery = db.Departments.OrderBy(x => x.Name);
            ViewBag.DepartmentID = new SelectList(departmentsQuery, "DepartmentID", "Name", selectedDepartment);
        }
    }
}