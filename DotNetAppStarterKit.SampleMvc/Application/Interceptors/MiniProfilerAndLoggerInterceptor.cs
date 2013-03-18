using Castle.DynamicProxy;
using DotNetAppStarterKit.Core.Logging;
using StackExchange.Profiling;

namespace DotNetAppStarterKit.SampleMvc.Application.Interceptors
{
    public class MiniProfilerAndLoggerInterceptor : IInterceptor
    {
        private readonly ILogWithCallerInfo<MiniProfilerAndLoggerInterceptor> _log;

        public MiniProfilerAndLoggerInterceptor(ILogWithCallerInfo<MiniProfilerAndLoggerInterceptor> log)
        {
            _log = log;
        }

        public void Intercept(IInvocation invocation)
        {
            var typeName = invocation.TargetType.Name;
            var methodName = invocation.Method.Name;
            var args = string.Join(", ", invocation.Arguments);

            var callInfo = string.Format("{0}.{1}({2})", typeName, methodName, args);

            _log.Debug("[ThreadId: {1}] Starting call: {0}", callInfo, System.Threading.Thread.CurrentThread.ManagedThreadId);
            try
            {
                using (MiniProfiler.Current.Step(callInfo))
                {
                    invocation.Proceed();
                }
            }
            finally
            {
                _log.Debug("[ThreadId: {1}] Finished call: {0}", callInfo, System.Threading.Thread.CurrentThread.ManagedThreadId);
            }
        }
    }
}