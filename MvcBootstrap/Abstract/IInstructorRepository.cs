using MvcBootstrap.Context;
using MvcBootstrap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBootstrap.Abstract
{
    public interface IInstructorRepository : IDisposable
    {
        SchoolContext Context { get; }
        IQueryable<Instructor> GetInstructors();
        Instructor GetByID(int id);
        void Insert(Instructor instructor, string[] selectedCourses);
        void Delete(int id);
        void Delete(List<int> ids);
        void Update(Instructor instructor, string[] selectedCourses);
        void Save();
    }
}
