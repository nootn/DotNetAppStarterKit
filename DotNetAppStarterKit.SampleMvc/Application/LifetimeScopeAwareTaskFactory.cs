using System;
using System.Threading.Tasks;
using Autofac;

namespace DotNetAppStarterKit.SampleMvc.Application
{
    public interface ILifetimeScopeAwareTaskFactory<TResult>
    {
        Task<TResult> StartNew(Func<TResult> function);
    }

    public class LifetimeScopeAwareTaskFactory<TResult> : ILifetimeScopeAwareTaskFactory<TResult>
    {
        private readonly ILifetimeScope _lifetimeScope;

        public LifetimeScopeAwareTaskFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        public Task<TResult> StartNew(Func<TResult> function)
        {
            return Task.Run(() =>
            {
                DependencyResolverEventBroker.LifetimeScope = _lifetimeScope;
                var result = function();
                DependencyResolverEventBroker.LifetimeScope = null;
                return result;
            });
        }
    }
}