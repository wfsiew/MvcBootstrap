using MvcBootstrap.Abstract;
using MvcBootstrap.Concrete;
using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MvcBootstrap.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            return controllerType == null ? null : (IController)ninjectKernel.Get(controllerType);
        }

        private void AddBindings()
        {
            ninjectKernel.Bind<ICourseRepository>().To<CourseRepository>();
            ninjectKernel.Bind<IDepartmentRepository>().To<DepartmentRepository>();
            ninjectKernel.Bind<IInstructorRepository>().To<InstructorRepository>();
            ninjectKernel.Bind<IStudentRepository>().To<StudentRepository>();
        }
    }
}