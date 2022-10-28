using System;
using System.Reflection;

using CommonInterface;

namespace AppDomainTests
{
  class TelemetryWrapper
  {
    static void Main(string[] args)
    {
      AppDomain appDomain1 = CreateAppDomain("PlugIn1");
      IPlugIn plugin1 = InstantiatePlugin("PlugIn1", appDomain1);

      plugin1.Initialize();
      Console.WriteLine(plugin1.Name+"\r\n");

      UnloadPlugin(appDomain1);

      TestIfUnloaded(plugin1);
    }

    static int ReturnNum(int num1, int num2)
    {
        return num1 + num2;
    }

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

    static IPlugIn InstantiatePlugin(string dllName, AppDomain domain)
    {
      IPlugIn plugIn = domain.CreateInstanceAndUnwrap(dllName, dllName + ".PlugIn") as IPlugIn;

      return plugIn;
    }

    static void UnloadPlugin(AppDomain domain)
    {
      AppDomain.Unload(domain);
    }

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

  }
}