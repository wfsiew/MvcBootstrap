using MvcBootstrap.Context;
using MvcBootstrap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBootstrap.Abstract
{
    public interface ICourseRepository : IDisposable
    {
        SchoolContext Context { get; }
        IQueryable<Course> GetCourses();
        Course GetByID(int id);
        void Insert(Course course);
        void Delete(int id);
        void Update(Course course);
        void Save();
    }
}
