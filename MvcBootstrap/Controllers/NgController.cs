using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Controllers
{
    public class NgController : Controller
    {
        private IStudentRepository repository;

        public NgController(IStudentRepository repository)
        {
            this.repository = repository;
        }

        //
        // GET: /Ng/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            IQueryable<EnrollmentDateGroup> data = repository.GetStudents().GroupBy(x => x.EnrollmentDate,
                (k, v) => new EnrollmentDateGroup { EnrollmentDate = k, StudentCount = v.Count() });
            List<EnrollmentDateGroup> l = data.ToList();

            return Json(l, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }
    }
}
