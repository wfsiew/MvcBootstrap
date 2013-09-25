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
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Controllers
{
    public class DepartmentController : Controller
    {
        public const string MENU = "Department";
        private IDepartmentRepository repository;

        public DepartmentController(IDepartmentRepository repository)
        {
            this.repository = repository;
        }

        //
        // GET: /Department/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.menu = MENU;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.BudgetSortParm = sortOrder == "Budget" ? "Budget_desc" : "Budget";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewBag.AdminSortParm = sortOrder == "Admin" ? "Admin_desc" : "Admin";

            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            if (searchString != null)
                page = 1;

            else
                searchString = currentFilter;

            ViewBag.CurrentFilter = searchString;

            var departments = repository.GetDepartments().Include(d => d.Administrator);

            if (!string.IsNullOrEmpty(keyword))
            {
                departments = departments.Where(x => x.Name.Contains(keyword) ||
                    x.Administrator.FirstMidName.Contains(keyword) ||
                    x.Administrator.LastName.Contains(keyword));
            }

            switch (sortOrder)
            {
                case "Name_desc":
                    departments = departments.OrderByDescending(x => x.Name);
                    break;

                case "Budget":
                    departments = departments.OrderBy(x => x.Budget);
                    break;

                case "Budget_desc":
                    departments = departments.OrderByDescending(x => x.Budget);
                    break;

                case "Date":
                    departments = departments.OrderBy(x => x.StartDate);
                    break;

                case "Date_desc":
                    departments = departments.OrderByDescending(x => x.StartDate);
                    break;

                case "Admin":
                    departments = departments.OrderBy(x => x.Administrator.LastName);
                    break;

                case "Admin_desc":
                    departments = departments.OrderByDescending(x => x.Administrator.LastName);
                    break;

                default:
                    departments = departments.OrderBy(x => x.Name);
                    break;
            }

            int pageSize = Constants.PAGE_SIZE;
            int pageNumber = (page ?? 1);

            return View(departments.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /Department/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.menu = MENU;
            Department department = repository.GetByID(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            return View(department);
        }

        //
        // GET: /Department/Create

        public ActionResult Create()
        {
            ViewBag.menu = MENU;
            PopulateInstructorsDropDownList();
            return View();
        }

        //
        // POST: /Department/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Department department)
        {
            ViewBag.menu = MENU;
            if (ModelState.IsValid)
            {
                repository.Insert(department);
                repository.Save();
                TempData["message"] = string.Format("{0} has been saved", department.Name);
                return RedirectToAction("Index");
            }

            PopulateInstructorsDropDownList(department.PersonID);
            return View(department);
        }

        //
        // GET: /Department/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.menu = MENU;
            Department department = repository.GetByID(id);
            if (department == null)
            {
                return HttpNotFound();
            }

            PopulateInstructorsDropDownList(department.PersonID);
            return View(department);
        }

        //
        // POST: /Department/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentID, Name, Budget, StartDate, RowVersion, PersonID")] Department department)
        {
            ViewBag.menu = MENU;
            try
            {
                if (ModelState.IsValid)
                {
                    repository.Update(department);
                    repository.Save();
                    TempData["message"] = string.Format("{0} has been saved", department.Name);
                    return RedirectToAction("Index");
                }
            }

            catch (DbUpdateConcurrencyException ex)
            {
                DbEntityEntry entry = ex.Entries.Single();
                Department clientValues = (Department)entry.Entity;
                Department databaseValues = (Department)entry.GetDatabaseValues().ToObject();

                if (databaseValues.Name != clientValues.Name)
                    ModelState.AddModelError("Name", "Current value: "
                        + databaseValues.Name);
                if (databaseValues.Budget != clientValues.Budget)
                    ModelState.AddModelError("Budget", "Current value: "
                        + String.Format("{0:c}", databaseValues.Budget));
                if (databaseValues.StartDate != clientValues.StartDate)
                    ModelState.AddModelError("StartDate", "Current value: "
                        + String.Format("{0:d}", databaseValues.StartDate));
                if (databaseValues.PersonID != clientValues.PersonID)
                    ModelState.AddModelError("PersonID", "Current value: "
                        + repository.Context.Instructors.Find(databaseValues.PersonID).FullName);
                ModelState.AddModelError(string.Empty, "The record you attempted to edit "
                    + "was modified by another user after you got the original value. The "
                    + "edit operation was canceled and the current values in the database "
                    + "have been displayed. If you still want to edit this record, click "
                    + "the Save button again. Otherwise click the Back to List hyperlink.");
                department.RowVersion = databaseValues.RowVersion;
            }

            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to save changes. Try again, and if the problem persists contact your system administrator.");
            }

            PopulateInstructorsDropDownList(department.PersonID);
            return View(department);
        }

        //
        // GET: /Department/Delete/5

        public ActionResult Delete(int id, bool? concurrencyError)
        {
            ViewBag.menu = MENU;
            Department department = repository.GetByID(id);
            if (concurrencyError.GetValueOrDefault())
            {
                if (department == null)
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                        + "was deleted by another user after you got the original values. "
                        + "Click the Back to List hyperlink.";
                }

                else
                {
                    ViewBag.ConcurrencyErrorMessage = "The record you attempted to delete "
                        + "was modified by another user after you got the original values. "
                        + "The delete operation was canceled and the current values in the "
                        + "database have been displayed. If you still want to delete this "
                        + "record, click the Delete button again. Otherwise "
                        + "click the Back to List hyperlink.";
                }
            }

            return View(department);
        }

        //
        // POST: /Department/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(Department department)
        {
            ViewBag.menu = MENU;
            try
            {
                repository.Delete(department);
                repository.Save();
                TempData["message"] = "Department was deleted";
                return RedirectToAction("Index");
            }

            catch (DbUpdateConcurrencyException)
            {
                return RedirectToAction("Delete", new { concurrencyError = true });
            }

            catch (DataException)
            {
                ModelState.AddModelError(string.Empty, "Unable to delete. Try again, and if the problem persists contact your system administrator.");
                return View(department);
            }
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }

        private void PopulateInstructorsDropDownList(object selectedInstructor = null)
        {
            IOrderedQueryable<Instructor> instructorsQuery = repository.Context.Instructors.OrderBy(x => x.LastName);
            ViewBag.PersonID = new SelectList(instructorsQuery, "PersonID", "FullName", selectedInstructor);
        }
    }
}