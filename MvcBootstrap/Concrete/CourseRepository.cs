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
    public class CourseRepository : ICourseRepository, IDisposable
    {
        private bool disposed = false;
        private SchoolContext context;

        public CourseRepository()
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

        public IQueryable<Course> GetCourses()
        {
            return context.Courses;
        }

        public Course GetByID(int id)
        {
            return context.Courses.Find(id);
        }

        public void Insert(Course course)
        {
            context.Courses.Add(course);
        }

        public void Delete(int id)
        {
            Course course = new Course { CourseID = id };
            context.Entry(course).State = EntityState.Deleted;
        }

        public void Update(Course course)
        {
            context.Entry(course).State = EntityState.Modified;
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