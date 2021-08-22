using System;
using System.IO;
using System.Reflection;

namespace GalacticScale
{
    public static partial class GS2
    {
        public static void LoadPlugins()
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
                    GS2.Warn("Failed to load generator:" +e.Message);
                    GS2.updateMessage +=
                        $"Failed to load external generator:{filePath}\r\nIf you have recently upgraded GS2, Please check to see if an updated build of the generator is available, or downgrade GS2 to continue using it";
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