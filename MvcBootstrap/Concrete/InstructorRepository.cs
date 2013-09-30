using MvcBootstrap.Abstract;
using MvcBootstrap.Context;
using MvcBootstrap.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MvcBootstrap.Concrete
{
    public class InstructorRepository : IInstructorRepository, IDisposable
    {
        private bool disposed = false;
        private SchoolContext context;

        public InstructorRepository()
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

        public IQueryable<Instructor> GetInstructors()
        {
            return context.Instructors;
        }

        public Instructor GetByID(int id)
        {
            return context.Instructors.Find(id);
        }

        public void Insert(Instructor instructor, string[] selectedCourses)
        {
            UpdateInstructorCourses(selectedCourses, instructor);
            context.Instructors.Add(instructor);
        }

        public void Delete(int id)
        {
            Instructor instructor = context.Instructors
                .Include(i => i.OfficeAssignment)
                .Where(i => i.PersonID == id)
                .Single();

            instructor.OfficeAssignment = null;
            context.Instructors.Remove(instructor);
        }

        public void Delete(List<int> ids)
        {
            IQueryable<Instructor> instructors = context.Instructors
                .Include(i => i.OfficeAssignment)
                .Where(i => ids.Contains(i.PersonID));

            foreach (Instructor instructor in instructors)
            {
                instructor.OfficeAssignment = null;
                context.Instructors.Remove(instructor);
            }
        }

        public void Update(Instructor instructor, string[] selectedCourses)
        {
            if (string.IsNullOrWhiteSpace(instructor.OfficeAssignment.Location))
                instructor.OfficeAssignment = null;

            UpdateInstructorCourses(selectedCourses, instructor);

            context.Entry(instructor).State = EntityState.Modified;
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

        private void UpdateInstructorCourses(string[] selectedCourses, Instructor instructorToUpdate)
        {
            if (selectedCourses == null)
            {
                instructorToUpdate.Courses = new List<Course>();
                return;
            }

            if (instructorToUpdate.Courses == null)
                instructorToUpdate.Courses = new List<Course>();

            HashSet<string> selectedCoursesHS = new HashSet<string>(selectedCourses);
            HashSet<int> instructorCourses = new HashSet<int>
                (instructorToUpdate.Courses.Select(c => c.CourseID));
            foreach (Course course in context.Courses)
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