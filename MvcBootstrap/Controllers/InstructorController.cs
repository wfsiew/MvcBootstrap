using MvcBootstrap.Context;
using MvcBootstrap.Models;
using MvcBootstrap.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Controllers
{
    public class InstructorController : Controller
    {
        public const string MENU = "Instructor";
        private SchoolContext db = new SchoolContext();

        //
        // GET: /Instructor/

        public ActionResult Index(int? id, int? courseID)
        {
            ViewBag.menu = MENU;
            InstructorIndexData viewModel = new InstructorIndexData();
            viewModel.Instructors = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses.Select(x => x.Department))
                .OrderBy(i => i.LastName);

            if (id != null)
            {
                ViewBag.InstructorID = id.Value;
                viewModel.Courses = viewModel.Instructors.Where(i => i.InstructorID == id.Value).Single().Courses;
            }

            if (courseID != null)
            {
                ViewBag.CourseID = courseID.Value;
                viewModel.Enrollments = viewModel.Courses.Where(x => x.CourseID == courseID).Single().Enrollments;
            }

            return View(viewModel);
        }

        //
        // GET: /Instructor/Details/5

        public ActionResult Details(int id = 0)
        {
            ViewBag.menu = MENU;
            Instructor instructor = db.Instructors.Find(id);
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
            ViewBag.InstructorID = new SelectList(db.OfficeAssignments, "InstructorID", "Location");
            return View();
        }

        //
        // POST: /Instructor/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Instructor instructor)
        {
            ViewBag.menu = MENU;
            if (ModelState.IsValid)
            {
                db.Instructors.Add(instructor);
                db.SaveChanges();
                TempData["message"] = string.Format("{0} has been saved", instructor.FullName);
                return RedirectToAction("Index");
            }

            ViewBag.InstructorID = new SelectList(db.OfficeAssignments, "InstructorID", "Location", instructor.InstructorID);
            return View(instructor);
        }

        //
        // GET: /Instructor/Edit/5

        public ActionResult Edit(int id = 0)
        {
            ViewBag.menu = MENU;
            Instructor instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.InstructorID == id)
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
            Instructor instructorToUpdate = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Include(i => i.Courses)
                .Where(i => i.InstructorID == id)
                .Single();

            if (TryUpdateModel(instructorToUpdate, "",
                new string[] { "LastName", "FirstMidName", "HireDate", "OfficeAssignment" }))
            {
                try
                {
                    if (String.IsNullOrWhiteSpace(instructorToUpdate.OfficeAssignment.Location))
                        instructorToUpdate.OfficeAssignment = null;

                    UpdateInstructorCourses(selectedCourses, instructorToUpdate);

                    db.Entry(instructorToUpdate).State = EntityState.Modified;
                    db.SaveChanges();
                    TempData["message"] = string.Format("{0} has been saved", instructorToUpdate.FullName);

                    return RedirectToAction("Index");
                }

                catch (DataException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }

            PopulateAssignedCourseData(instructorToUpdate);
            return View(instructorToUpdate);
        }

        //
        // GET: /Instructor/Delete/5

        public ActionResult Delete(int id = 0)
        {
            ViewBag.menu = MENU;
            Instructor instructor = db.Instructors.Find(id);
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
            Instructor instructor = db.Instructors
                .Include(i => i.OfficeAssignment)
                .Where(i => i.InstructorID == id)
                .Single();

            instructor.OfficeAssignment = null;
            db.Instructors.Remove(instructor);
            db.SaveChanges();
            TempData["message"] = "Instructor was deleted";
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private void PopulateAssignedCourseData(Instructor instructor)
        {
            DbSet<Course> allCourses = db.Courses;
            HashSet<int> instructorCourses = new HashSet<int>(instructor.Courses.Select(c => c.CourseID));
            List<AssignedCourseData> viewModel = new List<AssignedCourseData>();
            foreach (Course course in allCourses)
            {
                viewModel.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
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
            foreach (Course course in db.Courses)
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
    }
}