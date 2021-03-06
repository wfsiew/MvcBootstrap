﻿using MvcBootstrap.Abstract;
using MvcBootstrap.Context;
using MvcBootstrap.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MvcBootstrap.Concrete
{
    public class StudentRepository : IStudentRepository, IDisposable
    {
        private bool disposed = false;
        private SchoolContext context;

        public StudentRepository()
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

        public IQueryable<Student> GetStudents()
        {
            return context.Students;
        }

        public Student GetByID(int id)
        {
            return context.Students.Find(id);
        }

        public void Insert(Student student)
        {
            context.Students.Add(student);
        }

        public void Delete(int studentID)
        {
            Student student = new Student { PersonID = studentID };
            context.Entry(student).State = EntityState.Deleted;
        }

        public void Delete(List<int> ids)
        {
            foreach (int studentID in ids)
            {
                Student student = new Student { PersonID = studentID };
                context.Entry(student).State = EntityState.Deleted;
            }
        }

        public void Update(Student student)
        {
            context.Entry(student).State = EntityState.Modified;
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