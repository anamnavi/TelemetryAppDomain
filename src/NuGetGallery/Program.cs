using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NuGetGallery
{
    public static class Program
    {
        public static void Main()
        {
            Console.WriteLine("IN DEFAULT APP DOMAIN");
            TelWrapper.PrintLoadedAssemblies();
            
            Console.WriteLine("CREATE APP DOMAIN NOW");

            try
            {
                TelWrapper.AppDomainStartup();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }


            TelWrapper.PrintLoadedAssemblies();


            // AppDomain appDomain1 = CreateAppDomain("PlugIn1");
            // IPlugIn plugin1 = InstantiatePlugin("PlugIn1", appDomain1);

            // plugin1.Initialize();
            // Console.WriteLine(plugin1.Name+"\r\n");

            // UnloadPlugin(appDomain1);

            // TestIfUnloaded(plugin1);
            Console.WriteLine(TelWrapper.ReturnNum(2, 7));

            TelWrapper.AppDomainTakedown();

            Console.WriteLine("AFTER SECONDARY APP DOMAIN TAKEDOWN");
            TelWrapper.PrintLoadedAssemblies();
        }

        // equivalent to method in SearchController class:
        static void SearchModule(string pkgName, string pkgVersion)
        {
            long success = 1;
            bool statusSuccess = true;
            int statusCode = 200;
            string exceptionType = "";
            long duration = 5;

            // TelWrapper.TelemetryProcess(); // TODO: to use TelemetryOperation enum create project reference to SharedInterface
            TelWrapper.TelemetryProcessSearchModule(success, pkgName, pkgVersion, statusSuccess, statusCode, exceptionType, duration);
        }

        static void SearchScript(string pkgName, string pkgVersion)
        {
            long success = 1;
            bool statusSuccess = true;
            int statusCode = 200;
            string exceptionType = "";
            long duration = 5;

            // TelWrapper.TelemetryProcess(); // TODO: to use TelemetryOperation enum create project reference to SharedInterface
            TelWrapper.TelemetryProcessSearchScript(success, pkgName, pkgVersion, statusSuccess, statusCode, exceptionType, duration);
        }

        static void Download(string pkgName, string pkgVersion)
        {
            long success = 1;
            bool statusSuccess = true;
            int statusCode = 200;
            string exceptionType = "";
            long duration = 5;

            // TelWrapper.TelemetryProcess(); // TODO: to use TelemetryOperation enum create project reference to SharedInterface
            TelWrapper.TelemetryProcessDownloadResource(success, pkgName, pkgVersion, statusSuccess, statusCode, exceptionType, duration);
        }
    }
}
