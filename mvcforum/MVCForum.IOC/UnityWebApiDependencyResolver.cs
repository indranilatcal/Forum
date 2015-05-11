using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Web.Http.Dependencies;

namespace MVCForum.IOC
{
	public class UnityWebApiDependencyResolver : IDependencyResolver
	{
		protected IUnityContainer container;
		public UnityWebApiDependencyResolver(IUnityContainer container)
		{
			if (container == null)
			{
				throw new ArgumentNullException("container");
			}
			this.container = container;
		}

		public object GetService(Type serviceType)
		{
			try
			{
				return container.Resolve(serviceType);
			}
			catch (ResolutionFailedException)
			{
				return null;
			}
		}

		public IEnumerable<object> GetServices(Type serviceType)
		{
			try
			{
				return container.ResolveAll(serviceType);
			}
			catch (ResolutionFailedException)
			{
				return new List<object>();
			}
		}

		public IDependencyScope BeginScope()
		{
			var child = container.CreateChildContainer();
			return new UnityWebApiDependencyResolver(child);
		}

		public void Dispose()
		{
			container.Dispose();
		}

	}
}
