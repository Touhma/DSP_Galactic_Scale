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
            foreach (string filePath in Directory.GetFiles(Path.Combine(DataDir, "Generators")))
            {
                Log(filePath);
                foreach (Type type in Assembly.LoadFrom(filePath).GetTypes())
                    foreach (Type t in type.GetInterfaces())
                        if (t.Name == "iGenerator" && !type.IsAbstract && !type.IsInterface)
                            generators.Add((iGenerator)Activator.CreateInstance(type));
            }
            foreach (iGenerator g in generators)
            {
                Log("GalacticScale2|LoadPlugins|Loading Generator:" + g.Name);
                g.Init();
            }
            Log("End");
        }
    }
}