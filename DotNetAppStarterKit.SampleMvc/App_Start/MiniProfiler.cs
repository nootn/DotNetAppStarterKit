using System.Web;
using System.Web.Mvc;
using System.Linq;
using StackExchange.Profiling;
using StackExchange.Profiling.MVCHelpers;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;

[assembly: WebActivator.PreApplicationStartMethod(
	typeof(DotNetAppStarterKit.SampleMvc.App_Start.MiniProfilerPackage), "PreStart")]

[assembly: WebActivator.PostApplicationStartMethod(
	typeof(DotNetAppStarterKit.SampleMvc.App_Start.MiniProfilerPackage), "PostStart")]


namespace DotNetAppStarterKit.SampleMvc.App_Start 
{
    public static class MiniProfilerPackage
    {
        public static void PreStart()
        {
            MiniProfiler.Settings.SqlFormatter = new StackExchange.Profiling.SqlFormatters.SqlServerFormatter();
			MiniProfilerEF.Initialize();
            DynamicModuleUtility.RegisterModule(typeof(MiniProfilerStartupModule));
            GlobalFilters.Filters.Add(new ProfilingActionFilter());
        }

        public static void PostStart()
        {
            var copy = ViewEngines.Engines.ToList();
            ViewEngines.Engines.Clear();
            foreach (var item in copy)
            {
                ViewEngines.Engines.Add(new ProfilingViewEngine(item));
            }
        }
    }

    public class MiniProfilerStartupModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += (sender, e) => MiniProfiler.Start();
            context.EndRequest += (sender, e) => MiniProfiler.Stop();
        }

        public void Dispose() { }
    }
}

