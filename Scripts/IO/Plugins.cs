using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static List<iConfigurablePlugin> Plugins = new();

        public static void LoadPlugins()
        {
            Log("Start");
            if (!Directory.Exists(Path.Combine(DataDir, "Plugins")))
                Directory.CreateDirectory(Path.Combine(DataDir, "Plugins"));

            foreach (var filePath in Directory.GetFiles(Path.Combine(DataDir, "Plugins")))
            {
                Log(filePath);
                Assembly a;
                try
                {
                    a = Assembly.LoadFrom(filePath);
                    foreach (var type in a.GetTypes())
                    foreach (var t in type.GetInterfaces())
                        if (t.Name == "iConfigurablePlugin" && !type.IsAbstract && !type.IsInterface)
                        {
                            Warn($"Loading Plugin: {a.GetName().Name}");
                            var plugin = (iConfigurablePlugin)Activator.CreateInstance(type);
                            Plugins.Add(plugin);
                            plugin.Init();
                        }
                }
                catch (Exception e)
                {
                    Warn("Failed to load plugin:" + e.Message);
                    updateMessage += $"Failed to load plugin:{filePath}\r\nIf you have recently upgraded GS2, Please check to see if an updated build of the plugin is available, or downgrade GS2 to continue using it";
                }
            }

            foreach (var g in Generators)
            {
                Log("GalacticScale2|LoadPlugins|Loading Generator:" + g.Name);
                g.Init();
            }

            Log("End");
        }


        public static void LoadGenerators()
        {
            Log("Start");
            if (!Directory.Exists(Path.Combine(DataDir, "Generators")))
                Directory.CreateDirectory(Path.Combine(DataDir, "Generators"));

            foreach (var filePath in Directory.GetFiles(Path.Combine(DataDir, "Generators")))
            {
                Log(filePath);
                Assembly a;
                try
                {
                    a = Assembly.LoadFrom(filePath);
                    foreach (var type in a.GetTypes())
                    foreach (var t in type.GetInterfaces())
                        if (t.Name == "iGenerator" && !type.IsAbstract && !type.IsInterface)
                            Generators.Add((iGenerator)Activator.CreateInstance(type));
                }
                catch (Exception e)
                {
                    Warn("Failed to load generator:" + e.Message);
                    updateMessage += $"Failed to load external generator:{filePath}\r\nIf you have recently upgraded GS2, Please check to see if an updated build of the generator is available, or downgrade GS2 to continue using it";
                }
            }

            foreach (var g in Generators)
            {
                Log("GalacticScale2|LoadPlugins|Loading Generator:" + g.Name);
                g.Init();
            }

            Log("End");
        }
    }
}