using System.Collections.Generic;
namespace GalacticScale
{
    public class GSVegeSettings
    {
        public string Algorithm = "Vanilla";
        public bool Empty {  get { return (Group1.Count + Group2.Count + Group3.Count + Group4.Count + Group5.Count + Group6.Count == 0); } }
        public List<string> Group1 = new List<string>();
        public List<string> Group2 = new List<string>();
        public List<string> Group3 = new List<string>();
        public List<string> Group4 = new List<string>();
        public List<string> Group5 = new List<string>();
        public List<string> Group6 = new List<string>();
    
        public static List<string> FromIDArray (int[] a)
        {
            List<string> l = new List<string>();
            //GS2.Log("Working here");
            if (a.Length == 0) return l;
            for (var i = 0; i < a.Length; i++)

            {
                //GS2.Log(i + " " + a.Length + " " + a[i]);
                //GS2.Log(GS2.Utils.ReverseLookup(GS2.VegeTypesDictionary, a[i]));
                l.Add(GS2.Utils.ReverseLookup(GS2.VegeTypesDictionary, a[i]));
            }
            return l;
        }
        public static int[] ToIDArray(List<string> l)
        {
            int[] a = new int[l.Count];
            if (l.Count == 0) return a;
            //GS2.LogJson(l);
            //GS2.Log(l.Count.ToString());
            for (var i = 0; i < l.Count; i++) a[i] = GS2.VegeTypesDictionary[l[i]];
            return a;
        }
        public GSVegeSettings Clone()
        {
            GSVegeSettings clone = (GSVegeSettings)MemberwiseClone();
            clone.Group1 = new List<string>(Group1);
            clone.Group2 = new List<string>(Group2);
            clone.Group3 = new List<string>(Group3);
            clone.Group4 = new List<string>(Group4);
            clone.Group5 = new List<string>(Group5);
            clone.Group6 = new List<string>(Group6);
            return clone;
        }
    }
}