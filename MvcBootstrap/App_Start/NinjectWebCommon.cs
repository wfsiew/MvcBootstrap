[assembly: WebActivator.PreApplicationStartMethod(typeof(MvcBootstrap.App_Start.NinjectWebCommon), "Start")]
[assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof(MvcBootstrap.App_Start.NinjectWebCommon), "Stop")]

namespace MvcBootstrap.App_Start
{
    using Microsoft.Web.Infrastructure.DynamicModuleHelper;
    using MvcBootstrap.Abstract;
    using MvcBootstrap.Concrete;
    using MvcBootstrap.Infrastructure;
    using Ninject;
    using Ninject.Web.Common;
    using System;
    using System.Web;
    using System.Web.Http;

    public static class NinjectWebCommon 
    {
        private static readonly Bootstrapper bootstrapper = new Bootstrapper();

        /// <summary>
        /// Starts the application
        /// </summary>
        public static void Start() 
        {
            DynamicModuleUtility.RegisterModule(typeof(OnePerRequestHttpModule));
            DynamicModuleUtility.RegisterModule(typeof(NinjectHttpModule));
            bootstrapper.Initialize(CreateKernel);
        }
        
        /// <summary>
        /// Stops the application.
        /// </summary>
        public static void Stop()
        {
            bootstrapper.ShutDown();
        }

        public static IKernel CreateMvcKernel()
        {
            StandardKernel kernel = new StandardKernel();
            RegisterServices(kernel);
            return kernel;
        }
        
        /// <summary>
        /// Creates the kernel that will manage your application.
        /// </summary>
        /// <returns>The created kernel.</returns>
        private static IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            kernel.Bind<Func<IKernel>>().ToMethod(ctx => () => new Bootstrapper().Kernel);
            kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>();
            
            RegisterServices(kernel);

            GlobalConfiguration.Configuration.DependencyResolver = new NinjectDependencyResolver(kernel);

            return kernel;
        }

        /// <summary>
        /// Load your modules or register your services here!
        /// </summary>
        /// <param name="kernel">The kernel.</param>
        private static void RegisterServices(IKernel kernel)
        {
            kernel.Bind<ICourseRepository>().To<CourseRepository>();
            kernel.Bind<IDepartmentRepository>().To<DepartmentRepository>();
            kernel.Bind<IInstructorRepository>().To<InstructorRepository>();
            kernel.Bind<IStudentRepository>().To<StudentRepository>();
        }        
    }
}
