using System;
using System.Threading.Tasks;
using Autofac;

namespace DotNetAppStarterKit.SampleMvc.Application
{
    public interface ILifetimeScopeAwareTaskFactory
    {
        Task<TResult> StartNew<TResult>(Func<TResult> function);
        Task StartNew(Action action);
    }

    public class LifetimeScopeAwareTaskFactory : ILifetimeScopeAwareTaskFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public LifetimeScopeAwareTaskFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public Task<TResult> StartNew<TResult>(Func<TResult> function)
        {
            return Task.Run(() =>
            {
                try
                {
                    DependencyResolverEventBroker.LifetimeScope = _lifetimeScope;
                    var result = function();
                    return result;
                }
                finally
                {
                    DependencyResolverEventBroker.LifetimeScope = null;
                }
            });
        }

        public Task StartNew(Action action)
        {
            return Task.Run(() =>
            {
                try
                {
                    DependencyResolverEventBroker.LifetimeScope = _lifetimeScope;
                    action();
                }
                finally
                {
                    DependencyResolverEventBroker.LifetimeScope = null;
                }
            });
        }
    }
}