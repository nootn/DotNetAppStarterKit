using System.Collections.Generic;
using System.Web.Mvc;
using Autofac;
using Autofac.Integration.Mvc;
using DotNetAppStarterKit.Core.EventBroker;

namespace DotNetAppStarterKit.SampleMvc.Application
{
    public class DependencyResolverEventBroker : IEventBroker
    {
        private readonly ILifetimeScope _lifetimeScope;

        public DependencyResolverEventBroker(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public void Raise<TDomainEvent>(TDomainEvent domainEvent) where TDomainEvent : IDomainEvent
        {
            var dependencyResolver = DependencyResolver.Current;
            if (dependencyResolver is AutofacDependencyResolver)
            {
                var handlers = dependencyResolver.GetServices<IHandle<TDomainEvent>>();
                DispatchToHanders(domainEvent, handlers);
                return;
            }

            using (var scope = _lifetimeScope.BeginLifetimeScope())
            {
                var handlers = scope.Resolve<IEnumerable<IHandle<TDomainEvent>>>();
                DispatchToHanders(domainEvent, handlers);
            }
        }

        private static void DispatchToHanders<TDomainEvent>(TDomainEvent domainEvent,
            IEnumerable<IHandle<TDomainEvent>> handlers)
            where TDomainEvent : IDomainEvent
        {
            foreach (var handler in handlers) handler.Handle(domainEvent);
        }
    }
}