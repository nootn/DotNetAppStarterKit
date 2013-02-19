// /*
// Copyright (c) 2013 Andrew Newton (http://www.nootn.com.au)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// */

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Builder;
using Autofac.Integration.Mvc;
using DotNetAppStarterKit.Core.Command;
using DotNetAppStarterKit.Core.Event;
using DotNetAppStarterKit.Core.Query;
using DotNetAppStarterKit.Mapping;
using DotNetAppStarterKit.SampleMvc.DataProject.Context;
using DotNetAppStarterKit.Web.Caching;
using DotNetAppStarterKit.Web.Security;

namespace DotNetAppStarterKit.SampleMvc
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication
    {
        public static AutofacDependencyResolver GlobalResolver { get; private set; }

        protected void Application_Start()
        {
            //General MVC setup
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //Configure EF
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DummyDataContext>());

            //IOC Setup - example using AutoFac
            ConfigureIoc();
        }

        private static void ConfigureIoc()
        {
            var builder = new ContainerBuilder();

            //Web components
            var webAssembly = typeof (MvcApplication).Assembly;
            builder.RegisterControllers(webAssembly);
            builder.RegisterFilterProvider();
            builder.RegisterModule(new AutofacWebTypesModule());

            //Components defined within this website
            builder.RegisterType<DummyDataContext>().AsImplementedInterfaces().InstancePerHttpRequest();

            //DotNetAppStarterKit components
            builder.RegisterType<User>().AsImplementedInterfaces().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof (WebCacheProvider<>)).AsImplementedInterfaces().InstancePerHttpRequest();
            RegisterGenericTypes(builder, webAssembly, typeof (ICommand<,>), true)
                .ForEach(_ => _.InstancePerHttpRequest());
            RegisterGenericTypes(builder, webAssembly, typeof (ICommand<>), true)
                .ForEach(_ => _.InstancePerHttpRequest());
            RegisterGenericTypes(builder, webAssembly, typeof (IQuery<,>), true)
                .ForEach(_ => _.InstancePerHttpRequest());
            RegisterGenericTypes(builder, webAssembly, typeof (IQuery<>), true)
                .ForEach(_ => _.InstancePerHttpRequest());
            RegisterGenericTypes(builder, webAssembly, typeof (MapperBase<,>), false)
                .ForEach(_ => _.InstancePerHttpRequest());
            builder.RegisterGeneric(typeof (EventPublisher<>)).AsImplementedInterfaces().InstancePerHttpRequest();
            builder.RegisterGeneric(typeof (EventSubscribersProvider<>)).AsImplementedInterfaces().InstancePerHttpRequest();
            RegisterGenericTypes(builder, webAssembly, typeof (IEventSubscriber<>), true)
                .ForEach(_ => _.InstancePerHttpRequest());

            //Build and set resolver
            try
            {
                var container = builder.Build();

                GlobalResolver = new AutofacDependencyResolver(container);

                DependencyResolver.SetResolver(GlobalResolver);
            }
            catch (Exception ex)
            {
                if (ex is ReflectionTypeLoadException)
                {
                    var typeLoadException = ex as ReflectionTypeLoadException;
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    throw new AggregateException(typeLoadException.Message, loaderExceptions);
                }
                throw;
            }
        }

        private static bool IsAssignableToGenericType(Type givenType, Type genericType)
        {
            var interfaceTypes = givenType.GetInterfaces();

            if (interfaceTypes.Where(it => it.IsGenericType).Any(it => it.GetGenericTypeDefinition() == genericType))
                return true;

            var baseType = givenType.BaseType;
            if (baseType == null) return false;

            return baseType.IsGenericType &&
                   baseType.GetGenericTypeDefinition() == genericType ||
                   IsAssignableToGenericType(baseType, genericType);
        }

        private static List<IRegistrationBuilder<object, object, object>> RegisterGenericTypes(ContainerBuilder builder,
                                                                                               Assembly parentAssembly,
                                                                                               Type
                                                                                                   genericRepositoryType,
                                                                                               bool asInterfaces)
        {
            var types = parentAssembly.GetExportedTypes()
                                      .Where(t => !t.IsInterface && !t.IsAbstract)
                                      .Where(t => IsAssignableToGenericType(t, genericRepositoryType))
                                      .ToArray();

            var res = new List<IRegistrationBuilder<object, object, object>>();
            foreach (var type in types)
            {
                if (type.IsGenericType)
                    res.Add(asInterfaces
                                ? builder.RegisterGeneric(type).AsImplementedInterfaces()
                                : builder.RegisterGeneric(type).AsSelf());
                else
                    res.Add(asInterfaces
                                ? builder.RegisterType(type).AsImplementedInterfaces()
                                : builder.RegisterType(type).AsSelf());
            }
            return res;
        }
    }
}