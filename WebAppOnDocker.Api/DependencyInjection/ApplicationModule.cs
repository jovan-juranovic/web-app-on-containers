using System.Reflection;
using Autofac;
using WebAppOnDocker.Api.Application.IntegrationEvents.EventHandling;
using WebAppOnDocker.Shared.EventBus.Abstractions;

namespace WebAppOnDocker.Api.DependencyInjection
{
    public class ApplicationModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(DummyIntegrationEventHandler).GetTypeInfo().Assembly)
                   .AsClosedTypesOf(typeof(IIntegrationEventHandler<>));
        }
    }
}