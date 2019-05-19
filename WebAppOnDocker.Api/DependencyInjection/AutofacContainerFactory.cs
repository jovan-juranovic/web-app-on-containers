using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace WebAppOnDocker.Api.DependencyInjection
{
    public static class AutofacContainerFactory
    {
        public static IContainer Create(IServiceCollection services)
        {
            var container = new ContainerBuilder();
            container.Populate(services);

            container.RegisterModule(new ApplicationModule());

            return container.Build();
        }
    }
}