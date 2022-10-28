using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CommonInterface;

namespace MainApp
{
    public class TelemetryWrapper
    {
        private static AppDomain _domain;
        private static IPlugIn _plugin;

        public static void AppDomainStartup()
        {
            // _domain = CreateAppDomain("PlugIn1");
            Console.WriteLine("in method");

            var assemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "..\\..\\..\\", "TelemetryAppDomain\\bin\\Debug");
            var resolvedPath = Path.GetFullPath(assemblyPath);
            Console.WriteLine(assemblyPath);
            Console.WriteLine(resolvedPath);
            
            string dllName = "PlugIn1";
            AppDomainSetup setup = new AppDomainSetup()
            {
                ApplicationName = dllName,
                ConfigurationFile = dllName + ".dll.config",
                ApplicationBase = resolvedPath,
                DynamicBase = resolvedPath
            };


            
           
            _domain = AppDomain.CreateDomain(
              setup.ApplicationName,
              AppDomain.CurrentDomain.Evidence,
              setup);

            // Assembly.GetEntryAssembly().GetName().Nam

            //                       "C:\Users\annavied\Documents\TestTelemetryAppDomain\src\TelemetryAppDomain\bin\Debug\"

            var dllPath = Path.Combine(resolvedPath, "TelemetryAppDomain.dll");
            var dllResolvedPath = Path.GetFullPath(dllPath);
            //string dllPath = @"file://C:\Users\annavied\Documents\TestTelemetryAppDomain\src\TelemetryAppDomain\bin\Debug\TelemetryAppDomain.dll";
            // _plugin = InstantiatePlugin("PlugIn1", _domain);
            _plugin = _domain.CreateInstanceFromAndUnwrap(dllResolvedPath, "PlugIn1.PlugIn") as IPlugIn;

            PrintGetAssemblies();
        }

        public static void AppDomainTakedown()
        {
            // UnloadPlugin(_domain);
            AppDomain.Unload(_domain);
        }

        public static void TelemetryStartUp()
        {
            // TODO: need to add error handling if it is null
            if (_plugin != null)
            {
                _plugin.OtelStartUp();
            }
        }

        public static void TelemetryTakeDown()
        {
            if (_plugin != null)
            {
                _plugin.OtelTakedown();
            }
        }

        public static void TelemetryProcess(TelemetryOperation operation, long success, string packageName, string packageVersion, bool statusSuccess, int statusCode, string exceptionType, long duration)
        {
            _plugin.OtelProcess(operation, success, packageName, packageVersion, statusSuccess, statusCode, exceptionType, duration);
        }

        /** If we decide MainApp should not take a dependency/project reference to SharedInterface */
        public static void TelemetryProcessSearchModule(long success, string packageName, string packageVersion, bool statusSuccess, int statusCode, string exceptionType, long duration)
        {
            _plugin.OtelProcess(TelemetryOperation.SearchModule, success, packageName, packageVersion, statusSuccess, statusCode, exceptionType, duration); 
        }

        public static void TelemetryProcessSearchScript(long success, string packageName, string packageVersion, bool statusSuccess, int statusCode, string exceptionType, long duration)
        {
            _plugin.OtelProcess(TelemetryOperation.SearchScript, success, packageName, packageVersion, statusSuccess, statusCode, exceptionType, duration);
        }

        public static void TelemetryProcessDownloadResource(long success, string packageName, string packageVersion, bool statusSuccess, int statusCode, string exceptionType, long duration)
        {
            _plugin.OtelProcess(TelemetryOperation.Download, success, packageName, packageVersion, statusSuccess, statusCode, exceptionType, duration);
        }

        /** End of "if we decide..." */

        public static int ReturnNum(int num1, int num2)
        {
            return _plugin.Multiply2Nums(num1, num2);
        }

        /**
        static AppDomain CreateAppDomain(string dllName)
        {
            AppDomainSetup setup = new AppDomainSetup()
            {
                ApplicationName = dllName,
                ConfigurationFile = dllName + ".dll.config",
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };
            AppDomain appDomain = AppDomain.CreateDomain(
              setup.ApplicationName,
              AppDomain.CurrentDomain.Evidence,
              setup);

            return appDomain;
        }
        */

        /**
        static IPlugIn InstantiatePlugin(string dllName, AppDomain domain)
        {
            IPlugIn plugIn = domain.CreateInstanceAndUnwrap(dllName, dllName + ".PlugIn") as IPlugIn;

            return plugIn;
        }
        */

        /**
        static void UnloadPlugin(AppDomain domain)
        {
            AppDomain.Unload(domain);
        }
        */

        /**
        static void TestIfUnloaded(IPlugIn plugin)
        {
            bool unloaded = false;

            try
            {
                Console.WriteLine(plugin.Name);
            }
            catch (AppDomainUnloadedException)
            {
                unloaded = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (!unloaded)
            {
                Console.WriteLine("It does not appear that the app domain successfully unloaded.");
            }
        }
        */

        public static void PrintLoadedAssemblies()
        {
            Assembly[] assys = AppDomain.CurrentDomain.GetAssemblies();
            Console.WriteLine("----------------------------------");

            foreach (Assembly assy in assys)
            {
                Console.WriteLine(assy.FullName);
            }

            Console.WriteLine("----------------------------------");
        }

        public static void PrintGetAssemblies()
        {
            Assembly[] assys = _domain.GetAssemblies();
            Console.WriteLine("----------------------------------");

            foreach (Assembly assy in assys)
            {
                Console.WriteLine(assy.FullName);
            }

            Console.WriteLine("----------------------------------");
        }
    }
}
