using System;
using System.Reflection;

namespace GalacticScale
{
    public static class NebulaCompatibility
    {
        //public struct Double3 
        //{
        //    public Double3(double x, double y, double z)
        //    {
        //        this.x = x;
        //        this.y = y;
        //        this.z = z;
        //    }
        //    public override string ToString()
        //    {
        //        return string.Format("x: {0}, y: {1}, z: {2}", this.x, this.y, this.z);
        //    }

        //    public double x;

        //    public double y;

        //    public double z;
        //}
        public static bool Initialized
        {
            get
            {
                if (N_SimulatedWorld == null)
                {
                    return false;
                }

                return (bool)N_SimulatedWorld.GetProperty("Initialized").GetValue(null);
            }
        }
        public static bool IsMasterClient
        {
            get
            {
                //GS2.Log("Trying to get masterClient " + (N_LocalPlayer == null));
                if (N_LocalPlayer == null)
                {
                    return true;
                }

               bool val = (bool)N_LocalPlayer.GetProperty("IsMasterClient").GetValue(null);
                //GS2.Warn(val.ToString());
                return val;
            }
        }
        //public static Double3 LocalPlayerDataUPosition
        //{
        //    get
        //    {
        //        if (N_LocalPlayer == null)
        //        {
        //            GS2.Error("N_LocalPlayer null");
        //            return new Double3();
        //        }

        //        var x = N_LocalPlayer.GetProperty("Data").GetValue(null);
        //        var uPos = (Double3)N_PlayerData.GetProperty("UPosition").GetValue(x);
        //        GS2.Warn(uPos.ToString());
        //        return uPos;
        //    }
        //}
        public static Type N_LocalPlayer = null;
        public static Type N_SimulatedWorld = null;
        //public static Type N_Double3 = null;
        //public static Type N_PlayerData = null;
        public static bool Nebula = false;

        public static void Init()
        {
            Assembly[] asms = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly asm in asms)
            {
                //GS2.Warn(" "+asm.GetName().Name);
                if (asm.GetName().Name == "NebulaWorld")
                {
                    Nebula = true;
                    foreach (Type t in asm.GetTypes())
                    {
                        //GS2.Log(t.Name);
                        if (t.Name == "LocalPlayer")
                        {
                            //GS2.Warn("FOUND LOCALPLAYER");
                            N_LocalPlayer = t;
                        }

                        if (t.Name == "SimulatedWorld")
                        {
                            N_SimulatedWorld = t;
                            
                        }
                    }
                }
                //if (asm.GetName().Name == "NebulaModel")
                //{
                //    foreach(Type t in asm.GetTypes())
                //    {
                //        if (t.Name == "Double3") N_Double3 = t;
                //        if (t.Name == "PlayerData") N_PlayerData = t;
                //    }
                //}
            }
        }
    }
}