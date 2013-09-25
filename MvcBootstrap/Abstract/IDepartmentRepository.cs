using MvcBootstrap.Context;
using MvcBootstrap.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MvcBootstrap.Abstract
{
    public interface IDepartmentRepository : IDisposable
    {
        SchoolContext Context { get; }
        IQueryable<Department> GetDepartments();
        Department GetByID(int id);
        void Insert(Department department);
        void Delete(Department department);
        void Update(Department department);
        void Save();
    }
}
