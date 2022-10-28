using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainApp;


namespace NuGetGallery
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("IN DEFAULT APP DOMAIN");
            TelemetryWrapper.PrintLoadedAssemblies();
            
            Console.WriteLine("CREATE APP DOMAIN NOW");

            try
            {
                TelemetryWrapper.AppDomainStartup();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


            TelemetryWrapper.PrintLoadedAssemblies();


            // AppDomain appDomain1 = CreateAppDomain("PlugIn1");
            // IPlugIn plugin1 = InstantiatePlugin("PlugIn1", appDomain1);

            // plugin1.Initialize();
            // Console.WriteLine(plugin1.Name+"\r\n");

            // UnloadPlugin(appDomain1);

            // TestIfUnloaded(plugin1);
            Console.WriteLine(TelemetryWrapper.ReturnNum(2, 7));

            TelemetryWrapper.AppDomainTakedown();

            Console.WriteLine("AFTER SECONDARY APP DOMAIN TAKEDOWN");
            TelemetryWrapper.PrintLoadedAssemblies();
        }

        // equivalent to method in SearchController class:
        static void SearchModule(string pkgName, string pkgVersion)
        {
            long success = 1;
            bool statusSuccess = true;
            int statusCode = 200;
            string exceptionType = "";
            long duration = 5;

            // TelemetryWrapper.TelemetryProcess(); // TODO: to use TelemetryOperation enum create project reference to SharedInterface
            TelemetryWrapper.TelemetryProcessSearchModule(success, pkgName, pkgVersion, statusSuccess, statusCode, exceptionType, duration);
        }

        static void SearchScript(string pkgName, string pkgVersion)
        {
            long success = 1;
            bool statusSuccess = true;
            int statusCode = 200;
            string exceptionType = "";
            long duration = 5;

            // TelemetryWrapper.TelemetryProcess(); // TODO: to use TelemetryOperation enum create project reference to SharedInterface
            TelemetryWrapper.TelemetryProcessSearchScript(success, pkgName, pkgVersion, statusSuccess, statusCode, exceptionType, duration);
        }

        static void Download(string pkgName, string pkgVersion)
        {
            long success = 1;
            bool statusSuccess = true;
            int statusCode = 200;
            string exceptionType = "";
            long duration = 5;

            // TelemetryWrapper.TelemetryProcess(); // TODO: to use TelemetryOperation enum create project reference to SharedInterface
            TelemetryWrapper.TelemetryProcessDownloadResource(success, pkgName, pkgVersion, statusSuccess, statusCode, exceptionType, duration);
        }
    }
}
