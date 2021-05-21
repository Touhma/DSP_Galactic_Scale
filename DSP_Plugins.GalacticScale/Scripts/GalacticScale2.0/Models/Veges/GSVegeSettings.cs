using System.Collections.Generic;
namespace GalacticScale
{
    public class GSVegeSettings
    {
        public string Algorithm = "Vanilla";
        public List<string> Group1 = new List<string>();
        public List<string> Group2 = new List<string>();
        public List<string> Group3 = new List<string>();
        public List<string> Group4 = new List<string>();
        public List<string> Group5 = new List<string>();
        public List<string> Group6 = new List<string>();
    
        public static List<string> FromIDArray (int[] a)
        {
            List<string> l = new List<string>();
            for (var i = 0; i < a.Length; i++) l.Add(GS2.Utils.ReverseLookup(GS2.VegeTypesDictionary, a[i]));
            return l;
        }
        public static int[] ToIDArray(List<string> l)
        {
            int[] a = new int[l.Count];
            for (var i = 0; i < l.Count; i++) a[i] = GS2.VegeTypesDictionary[l[i]];
            return a;
        }
    }
}