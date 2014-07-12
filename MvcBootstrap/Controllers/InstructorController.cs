using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.Helpers;
using MvcBootstrap.Models;
using MvcBootstrap.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DoddleReport;
using DoddleReport.Web;

namespace MvcBootstrap.Controllers
{
    public class InstructorController : Controller
    {
        public const string MENU = "Instructor";
        private IInstructorRepository repository;

        public InstructorController(IInstructorRepository repository)
        {
            this.repository = repository;
        }

        //
        // GET: /Instructor/

        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page, int? id, int? courseID)
        {
            ViewBag.menu = MENU;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewBag.LocationSortParm = sortOrder == "Loc" ? "Loc_desc" : "Loc";

            InstructorIndexData viewModel = new InstructorIndexData();

            if (searchString != null)
                page = 1;

            else
                searchString = currentFilter;

            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            ViewBag.CurrentFilter = searchString;

            var instructors = repository.GetInstructors()
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(x => x.Department));

            if (!string.IsNullOrEmpty(keyword))
            {
                instructors = instructors.Where(x => x.LastName.ToUpper().Contains(keyword) ||
                    x.FirstMidName.ToUpper().Contains(keyword));
            }

            if (id != null)
            {
                ViewBag.PersonID = id.Value;
                viewModel.Courses = instructors.Where(i => i.PersonID == id.Value).Single().Courses;
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                viewModel.Enrollments = viewModel.Courses.Where(x => x.CourseID == courseID).Single().Enrollments;
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

            viewModel.Instructors = instructors.ToPagedList(pageNumber, pageSize);

            return View(viewModel);
        }

        //
        // GET: /Instructor/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.menu = MENU;
            Instructor instructor = repository.GetByID(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }

            return View(instructor);
        }

        //
        // GET: /Instructor/Create

        public ActionResult Create()
        {
            ViewBag.menu = MENU;
            PopulateAssignedCourseData();
            return View();
        }

        //
        // POST: /Instructor/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Instructor instructor, FormCollection fc, string[] selectedCourses)
        {
            ViewBag.menu = MENU;

            if (TryUpdateModel(instructor, "",
                new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                repository.Insert(instructor, selectedCourses);
                repository.Save();
                TempData["message"] = string.Format("{0} has been saved", instructor.FullName);

                return RedirectToAction("Index");
            }

            PopulateAssignedCourseData();
            return View(instructor);
        }

        //
        // GET: /Instructor/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.menu = MENU;
            Instructor instructor = repository.GetInstructors()
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.PersonID == id)
                .Single();
            if (instructor == null)
            {
                return HttpNotFound();
            }

            PopulateAssignedCourseData(instructor);
            return View(instructor);
        }

        //
        // POST: /Instructor/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, FormCollection fc, string[] selectedCourses)
        {
            ViewBag.menu = MENU;
            Instructor instructorToUpdate = repository.GetInstructors()
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.PersonID == id)
                .Single();

            if (TryUpdateModel(instructorToUpdate, "",
                new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    repository.Update(instructorToUpdate, selectedCourses);
                    repository.Save();
                    TempData["message"] = string.Format("{0} has been saved", instructorToUpdate.FullName);

                    return RedirectToAction("Index");
                }

                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateInstructorsDropDownList(instructorToUpdate.PersonID);
            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        //
        // GET: /Instructor/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ViewBag.menu = MENU;
            Instructor instructor = repository.GetByID(id);
            if (instructor == null)
            {
                return HttpNotFound();
            }

            return View(instructor);
        }

        //
        // POST: /Instructor/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ViewBag.menu = MENU;
            repository.Delete(id);
            repository.Save();
            TempData["message"] = "Instructor was deleted";
            return RedirectToAction("Index");
        }

        public ActionResult HtmlReport(string sortOrder, string currentFilter, string searchString)
        {
            Report report = GetReport(sortOrder, currentFilter, searchString, true);
            return new ReportResult(report);
        }

        public ActionResult PdfReport(string sortOrder, string currentFilter, string searchString)
        {
            Report report = GetReport(sortOrder, currentFilter, searchString);
            return new ReportResult(report, new DoddleReport.iTextSharp.PdfReportWriter(), "application/pdf");
        }

        public ActionResult ExcelReport(string sortOrder, string currentFilter, string searchString)
        {
            var report = GetReport(sortOrder, currentFilter, searchString);
            return new ReportResult(report, new DoddleReport.OpenXml.ExcelReportWriter(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public ActionResult CsvReport(string sortOrder, string currentFilter, string searchString)
        {
            var report = GetReport(sortOrder, currentFilter, searchString);
            return new ReportResult(report, new DoddleReport.Writers.DelimitedTextReportWriter(), "text/plain;charset=UTF-8");
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }

        private void PopulateInstructorsDropDownList(object selectedInstructor = null)
        {
            ViewBag.PersonID_ = new SelectList(repository.Context.OfficeAssignments, "PersonID", "Location", selectedInstructor);
        }

        private void PopulateAssignedCourseData(Instructor instructor = null)
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

            ViewBag.Courses = viewModel;
        }

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            HashSet<string> selectedCoursesHS = new HashSet<string>(selectedCourses);
            HashSet<int> instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.CourseID));
            foreach (Course course in repository.Context.Courses)
            {
                if (selectedCoursesHS.Contains(course.CourseID.ToString()))
                {
                    if (!instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Add(course);
                    }
                }

                else
                {
                    if (instructorCourses.Contains(course.CourseID))
                    {
                        instructorToUpdate.Courses.Remove(course);
                    }
                }
            }
        }

        private Report GetReport(string sortOrder, string currentFilter, string searchString, bool ishtml = false)
        {
            ViewBag.menu = MENU;
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = string.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            ViewBag.DateSortParm = sortOrder == "Date" ? "Date_desc" : "Date";
            ViewBag.LocationSortParm = sortOrder == "Loc" ? "Loc_desc" : "Loc";

            InstructorIndexData viewModel = new InstructorIndexData();

            if (searchString == null)
                searchString = currentFilter;

            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            ViewBag.CurrentFilter = searchString;

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

            var lr = instructors.Select(x => new
            {
                LastName = x.LastName,
                FirstName = x.FirstMidName,
                HireDate = x.HireDate,
                Office = x.OfficeAssignment,
                Courses = x.GetCourses(ishtml)
            });
            Report report = new Report(lr.ToReportSource());
            report.TextFields.Title = "Instructors Report";
            report.TextFields.Header = string.Format(@"Report Generated: {0} Total Instructors: {1}", DateTime.Now, instructors.Count());

            report.DataFields["HireDate"].DataFormatString = "{0:yyyy-MM-dd}";

            report.RenderHints.FreezeRows = 4;

            return report;
        }
    }
}