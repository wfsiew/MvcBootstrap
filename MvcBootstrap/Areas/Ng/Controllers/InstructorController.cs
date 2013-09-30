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
