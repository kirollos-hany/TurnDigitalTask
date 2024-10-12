using Autofac;
using TurnDigital.Domain.DataAccess.Interfaces;
using TurnDigital.Domain.IO.Interfaces;
using TurnDigital.Infrastructure.DataAccess;
using TurnDigital.Infrastructure.DataAccess.Interceptors;
using TurnDigital.Infrastructure.IO;
using Module = Autofac.Module;
namespace TurnDigital.Infrastructure;

public class InfrastructureModule : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder
            .RegisterType(typeof(ReadRepository))
            .As(typeof(IReadRepository))
            .InstancePerLifetimeScope();
        
        builder
            .RegisterType<Repository>()
            .As<IRepository>()
            .InstancePerLifetimeScope();

        builder
          .RegisterType<FileExtensionContentTypeProvider>()
          .As<IContentTypeProvider>()
          .InstancePerDependency();

        builder
          .RegisterType<FileManager>()
          .As<IFileManager>()
          .InstancePerDependency();

        builder
            .RegisterType<ReadRepository>()
            .As<IReadRepository>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<ActionAuditInterceptor>()
            .InstancePerLifetimeScope();

        builder
            .RegisterType<FireEventInterceptor>()
            .InstancePerLifetimeScope();
    }
}