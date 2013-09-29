﻿using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using MvcBootstrap.Context;
using MvcBootstrap.Helpers;
using MvcBootstrap.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MvcBootstrap.Controllers
{
    public class NgdepartmentController : Controller
    {
        private IDepartmentRepository repository;

        public NgdepartmentController(IDepartmentRepository repository)
        {
            this.repository = repository;
        }

        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            string keyword = string.IsNullOrEmpty(searchString) ? null : searchString.ToUpper();

            if (searchString != null)
                page = 1;

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

            var l = departments.ToPagedList(pageNumber, pageSize);
            var lx = l.Select(x => new
            {
                Administrator = new { FullName = x.Administrator == null ? null : x.Administrator.FullName },
                Budget = x.Budget,
                DepartmentID = x.DepartmentID,
                Name = x.Name,
                PersonID = x.PersonID,
                StartDate = x.StartDate
            });
            Pager pager = new Pager(l.TotalItemCount, l.PageNumber, l.PageSize);
            Dictionary<string, object> res = new Dictionary<string, object>
            {
                { "pager", pager },
                { "model", lx }
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(int id = 0)
        {
            Department department = repository.GetByID(id);
            var model = new
            {
                Budget = department.Budget,
                DepartmentID = department.DepartmentID,
                Name = department.Name,
                PersonID = department.PersonID,
                StartDate = department.StartDate,
                Administrator = new { FullName = department.Administrator == null ? null : department.Administrator.FullName }
            };

            Dictionary<string, object> res = new Dictionary<string, object>
            {
                { "model", model }
            };

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Create(Department department)
        {
            Dictionary<string, object> res = new Dictionary<string, object>();

            try
            {
                //throw new Exception("Error" + DateTime.Now);
                if (ModelState.IsValid)
                {
                    repository.Insert(department);
                    repository.Save();
                    res["success"] = 1;
                    res["message"] = string.Format("{0} has been saved", department.Name);
                }
            }

            catch (Exception ex)
            {
                res["error"] = 1;
                res["message"] = ex.ToString();
            }

            return Json(res, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Instructors()
        {
            IOrderedQueryable<Instructor> instructorsQuery = repository.Context.Instructors.OrderBy(x => x.LastName);
            List<Instructor> l = instructorsQuery.ToList();
            var o = l.Select(x => new { PersonID = x.PersonID, FullName = x.FullName });
            return Json(o, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            repository.Dispose();
            base.Dispose(disposing);
        }
    }
}