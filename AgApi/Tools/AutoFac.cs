using MediatR;
using System.Reflection;
using Base.Controllers;
using Autofac.Integration.WebApi;

namespace Autofac
{
    public static class AutoFacConfig
    {
        public static IContainer RegisterComponents()
        {




            //Log.Logger = new LoggerConfiguration()
            //    //.Enrich.WithUserName()
            //    .Enrich.WithCorrelationId()
            //    .MinimumLevel.ControlledBy(GlobalVariables.levelSwitch)
            //    .WriteTo.File(
            //    formatter: new CompactJsonFormatter(),//new RenderedCompactJsonFormatter(), //new JsonFormatter(renderMessage: true), //new CompactJsonFormatter() // necessario instalacao de pacote outra opcao de formatador
            //    path: AppDomain.CurrentDomain.GetData("DataDirectory").ToString() + "\\SysLogs\\Log.txt",
            //    fileSizeLimitBytes: (4 * 1024) * 1024, // 4mb de limite no tamanho do arquivo de log
            //    rollingInterval: RollingInterval.Day,
            //    rollOnFileSizeLimit: true,
            //    encoding: Encoding.GetEncoding("ISO-8859-1")
            //    )
            //    .CreateLogger();





            //builder.RegisterLogger(Log.Logger);

            //// uncomment to enable polymorphic dispatching of requests, but note that
            //// this will conflict with generic pipeline behaviors
            //// builder.RegisterSource(new ContravariantRegistrationSource());
            //// mediator itself
            //builder
            //  .RegisterType<Mediator>()
            //  .As<IMediator>()
            //  .InstancePerLifetimeScope();

            // request & notification handlers
            var builder = new ContainerBuilder();
            builder.Register<ServiceFactory>(context =>
            {
                var c = context.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            // Registrar todos os controllers - se eles estiverem dentro no mesmo assembly
            // Necessário! para fazer a injecao de classes no contructor do controller

            builder.RegisterApiControllers(typeof(BaseController).Assembly);


            // finally register our custom code (individually, or via assembly scanning)
            // - requests & handlers as transient, i.e. InstancePerDependency()
            // - pre/post-processors as scoped/per-request, i.e. InstancePerLifetimeScope()
            // - behaviors as transient, i.e. InstancePerDependency() //Where(t => t.Name.EndsWith("Request"))
            var ass = Assembly.GetExecutingAssembly();
            builder.RegisterAssemblyTypes(ass).AsImplementedInterfaces(); // via assembly scan
                                                                          //builder.RegisterAssemblyTypes(ass).AsImplementedInterfaces(); // via assembly scan
                                                                          //  builder.RegisterAssemblyTypes(typeof(MvcApplication).Assembly).Where(t => t.Name.EndsWith("Response")).AsImplementedInterfaces(); // via assembly scan
                                                                          //builder.RegisterType<MyHandler>().AsImplementedInterfaces().InstancePerDependency();          // or individually            


            //--- fim do exemplo de configuracao

            // outro execmplo de configuracao
            // assim adicionamos um comportamento somente para um tipo de handler
            // builder.RegisterType(typeof(RequestLoggerBehavior<CQRS.Requests.UserInfoRequest, CQRS.Requests.UserInfoResponse>)).As(typeof(IPipelineBehavior<CQRS.Requests.UserInfoRequest, CQRS.Requests.UserInfoResponse>));

            // Assim adicionamos um comportamento para todos os Handlers, os comportamentos são adicionados na ordem de criacao
            //  builder.RegisterGeneric(typeof(RequestLoggerBehavior<,>)).As(typeof(IPipelineBehavior<,>));


            //builder.RegisterGeneric(typeof(RequestPostProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(RequestPreProcessorBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(GenericRequestPreProcessor<>)).As(typeof(IRequestPreProcessor<>));
            //builder.RegisterGeneric(typeof(GenericRequestPostProcessor<,>)).As(typeof(IRequestPostProcessor<,>));
            //builder.RegisterGeneric(typeof(GenericPipelineBehavior<,>)).As(typeof(IPipelineBehavior<,>));
            //builder.RegisterGeneric(typeof(ConstrainedRequestPostProcessor<,>)).As(typeof(IRequestPostProcessor<,>));
            //builder.RegisterGeneric(typeof(ConstrainedPingedHandler<>)).As(typeof(INotificationHandler<>));

            return builder.Build();
        }
    }
}