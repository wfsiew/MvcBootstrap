using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.Helpers;
using MvcBootstrap.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Controllers
{
    public class StudentController : Controller
    {
        public const string MENU = "Student";
        private IStudentRepository repository;

        public StudentController(IStudentRepository repository)
        {
            this.repository = repository;
        }

        //
        // GET: /Student/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.menu = MENU;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";

            if (searchString != null)
                page = 1;

            else
                searchString = currentFilter;

            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            ViewBag.CurrentFilter = searchString;

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

            return View(students.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Student/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.menu = MENU;
            Student student = repository.GetByID(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {
            ViewBag.menu = MENU;
            return View();
        }

        //
        // POST: /Student/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="LastName, FirstMidName, EnrollmentDate")] Student student)
        {
            ViewBag.menu = MENU;
            try
            {
                if (ModelState.IsValid)
                {
                    repository.Insert(student);
                    repository.Save();
                    TempData["message"] = string.Format("{0} has been saved", student.FullName);
                    return RedirectToAction("Index");
                }
            }

            catch (DataException)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
            }

            return View(student);
        }

        //
        // GET: /Student/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.menu = MENU;
            Student student = repository.GetByID(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        //
        // POST: /Student/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Student student)
        {
            ViewBag.menu = MENU;
            if (ModelState.IsValid)
            {
                repository.Update(student);
                repository.Save();
                TempData["message"] = string.Format("{0} has been saved", student.FullName);
                return RedirectToAction("Index");
            }

            return View(student);
        }

        //
        // GET: /Student/Delete/5

        public ActionResult Delete(bool? saveChangesError = false, int id = 0)
        {
            ViewBag.menu = MENU;
            if (saveChangesError.GetValueOrDefault())
                ViewBag.ErrorMessage = "Delete failed. Try again, and if the problem persists see your system administrator.";

            Student student = repository.GetByID(id);
            if (student == null)
            {
                return HttpNotFound();
            }

            return View(student);
        }

        //
        // POST: /Student/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            ViewBag.menu = MENU;
            try
            {
                repository.Delete(id);
                repository.Save();
                TempData["message"] = "Student was deleted";
            }

            catch (DataException)
            {
                return RedirectToAction("Delete", new { id = id, saveChangesError = true });
            }

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }
    }
}