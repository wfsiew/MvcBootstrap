﻿using MvcBootstrap.Abstract;
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
    public class HomeController : Controller
    {
        public const string MENU = "Home";
        private IStudentRepository repository;

        public HomeController(IStudentRepository repository)
        {
            this.repository = repository;
        }

        //
        // GET: /Home/

        public ActionResult Index()
        {
            ViewBag.menu = MENU;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.menu = "About";
            IQueryable<EnrollmentDateGroup> data = repository.GetStudents().GroupBy(x => x.EnrollmentDate, 
                (k, v) => new EnrollmentDateGroup { EnrollmentDate = k, StudentCount = v.Count() });
            
            return View(data);
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }
    }
}
