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

        public ActionResult Index(string sortOrder, string searchString, int? page, int? id, int? courseID)
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

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }

        public JsonResult Instructors()
        {
            object o = GetInstructors();
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        private object GetInstructors()
        {
            List<OfficeAssignment> l = repository.Context.OfficeAssignments.ToList();
            var o = l.Select(x => new { PersonID = x.PersonID, Location = x.Location });
            return o;
        }
    }
}
