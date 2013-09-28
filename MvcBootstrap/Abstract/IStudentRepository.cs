using MvcBootstrap.Context;
using MvcBootstrap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBootstrap.Abstract
{
    public interface IStudentRepository : IDisposable
    {
        SchoolContext Context { get; }
        IQueryable<Student> GetStudents();
        Student GetByID(int id);
        void Insert(Student student);
        void Delete(int id);
        void Delete(List<int> ids);
        void Update(Student student);
        void Save();
    }
}
