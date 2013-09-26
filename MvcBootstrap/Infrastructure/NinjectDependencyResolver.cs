using MvcBootstrap.App_Start;
using Ninject;
using Ninject.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Dependencies;

namespace MvcBootstrap.Infrastructure
{
    /// <summary>
    /// <remarks>
    /// http://www.peterprovost.org/blog/2012/06/19/adding-ninject-to-web-api/
    /// </remarks>
    /// </summary>
    public class NinjectDependencyScope : IDependencyScope
    {
        private IResolutionRoot resolver;

        public NinjectDependencyScope()
        {
        }

        public NinjectDependencyScope(IResolutionRoot resolver)
        {
            this.resolver = resolver;
        }

        protected void SetResolver(IResolutionRoot resolver)
        {
            this.resolver = resolver;
        }

        public object GetService(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");
            
            return resolver.TryGet(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            if (resolver == null)
                throw new ObjectDisposedException("this", "This scope has been disposed");

            return resolver.GetAll(serviceType);
        }

        public void Dispose()
        {
            IDisposable disposable = resolver as IDisposable;
            if (disposable != null)
                disposable.Dispose();

            resolver = null;
        }
    }

    public class NinjectDependencyResolver : NinjectDependencyScope, IDependencyResolver
    {
        private IKernel kernel;

        public NinjectDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            this.kernel = kernel;
        }

        public IDependencyScope BeginScope()
        {
            return new NinjectDependencyScope(kernel.BeginBlock());
        }
    }

    public class NinjectMvcDependencyResolver : NinjectDependencyScope, System.Web.Mvc.IDependencyResolver
    {
        private IKernel kernel;

        public NinjectMvcDependencyResolver()
        {
            kernel = NinjectWebCommon.CreateMvcKernel();
            SetResolver(kernel);
        }

        public NinjectMvcDependencyResolver(IKernel kernel)
            : base(kernel)
        {
            this.kernel = kernel;
        }
    }
}