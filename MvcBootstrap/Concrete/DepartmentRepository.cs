using MvcBootstrap.Abstract;
using MvcBootstrap.Context;
using MvcBootstrap.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MvcBootstrap.Concrete
{
    public class DepartmentRepository : IDepartmentRepository, IDisposable
    {
        private bool disposed = false;
        private SchoolContext context;

        public DepartmentRepository()
        {
            this.context = new SchoolContext();
        }

        public SchoolContext Context
        {
            get
            {
                return context;
            }
        }

        public IQueryable<Department> GetDepartments()
        {
            return context.Departments;
        }

        public Department GetByID(int id)
        {
            return context.Departments.Find(id);
        }

        public void Insert(Department department)
        {
            context.Departments.Add(department);
        }

        public void Delete(Department department)
        {
            context.Entry(department).State = EntityState.Deleted;
        }

        public void Update(Department department)
        {
            context.Entry(department).State = EntityState.Modified;
        }

        public void Save()
        {
            context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}