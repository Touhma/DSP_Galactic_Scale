using System;
using System.Reflection;
using HarmonyLib;
using Steamworks;

namespace GalacticScale
{
    public static class Methods
    {
        public static MethodInfo GetRadiusFromAstroId(this CodeMatcher matcher) => AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromAstroId)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
        public static MethodInfo GetRadiusFromLocalPlanet(this CodeMatcher matcher) => AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromLocalPlanet)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
        public static MethodInfo GetRadiusFromMecha(this CodeMatcher matcher) => AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromMecha)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
        public static MethodInfo GetRadiusFromEnemyData(this CodeMatcher matcher) => AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromEnemyData)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
        
        public static MethodInfo GetRadiusFromFactory(this CodeMatcher matcher) => AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromFactory)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
        public static MethodInfo GetRadiusFromAltitude(this CodeMatcher matcher) => AccessTools.Method(typeof(Utils), nameof(Utils.GetRadiusFromAltitude)).MakeGenericMethod(matcher.Operand?.GetType() ?? typeof(float));
    }
    public static partial class Utils
    {
        public static double ModifyRadius(double vanilla, double realRadius)
        {
            var negative = false;
            if (realRadius < 0)
            {
                negative = true;
                realRadius *= -1;
            }
            var diff = vanilla - 200.0;
            realRadius += diff;
            if (negative) realRadius *= -1;
            return realRadius;
        }
        public static T GetRadiusFromFactory<T>(T t, PlanetFactory factory)
        {
            var realRadius = ModifyRadius(Convert.ToDouble(t), factory?.planet?.realRadius ?? 200.0);
            if (VFInput.alt) GS3.Log($"GetRadiusFromFactory Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{Convert.ToDouble(t)} returning {realRadius}");
            return (T)Convert.ChangeType(realRadius, typeof(T));
        }
        public static T GetRadiusFromAstroId<T>(T t, int id)
        {
            var realRadius = ModifyRadius(Convert.ToDouble(t), GameMain.data.galaxy?.astrosFactory[id]?.planet?.realRadius ?? 200.0);
            if (VFInput.alt) GS3.Log($"GetRadiusFromAstroId Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} returning {realRadius}");
            return (T)Convert.ChangeType(realRadius, typeof(T));
        }

        public static T GetRadiusFromLocalPlanet<T>(T t)
        {
            var realRadius = ModifyRadius(Convert.ToDouble(t), GameMain.localPlanet?.realRadius ?? 200.0);
            if (VFInput.alt) GS3.Log($"GetRadius Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{Convert.ToDouble(t)} returning {realRadius}");
            return (T)Convert.ChangeType(realRadius, typeof(T));
        }
        public static T GetRadiusFromEnemyData<T>(T t, ref EnemyData enemyData)
        {
            var realRadius = ModifyRadius(Convert.ToDouble(t), GameMain.galaxy.PlanetById(enemyData.astroId)?.realRadius??200.0);
            if (VFInput.alt) GS3.Log($"GetRadiusFromFactory Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{Convert.ToDouble(t)} returning {realRadius}");
            return (T)Convert.ChangeType(realRadius, typeof(T));
        }
        public static T GetRadiusFromAltitude<T>(T t, float alt) //Original / Altitude
        {
            var realRadius = ModifyRadius(Convert.ToDouble(t), Convert.ToDouble(alt));
            return (T)Convert.ChangeType(realRadius, typeof(T));
        }
        public static T GetRadiusFromMecha<T>(T t, Mecha mecha)
        {
            // var num = mecha?.player?.planetData?.realRadius ?? 200f;
            var realRadius = ModifyRadius(Convert.ToDouble(t), mecha?.player?.planetData?.realRadius ?? 200.0);
            // float orig = Convert.ToSingle(t);
            // var diff = orig - 200f;
            // num += diff;
            // if (VFInput.alt) GS3.Log($"GetRadiusFromMecha Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{orig} returning {num}");
            return (T)Convert.ChangeType(realRadius, typeof(T));
        }
        
        // public static T GetRadiusFromEnemyData<T>(T t, ref EnemyData enemyData)
        // {
        //     float orig = Convert.ToSingle(t);
        //     var num = 200f;
        //     GS3.Log(enemyData.astroId.ToString());
        //     var planet = GameMain.galaxy.PlanetById(enemyData.astroId);
        //     if (planet != null)
        //     {
        //         var diff = orig - 200f;
        //         num += diff;
        //     }
        //     if (VFInput.alt) GS3.Log($"GetRadiusFromFactory Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{orig} returning {num}");
        //     
        //     
        //     
        //     return (T)Convert.ChangeType(num, typeof(T));
        // }
        
        // public static T GetRadiusFromAstroId<T>(T t, int id)
        // {
        //     var negative = false;
        //     var num = GameMain.data.galaxy?.astrosFactory[id]?.planet?.realRadius ?? 200f;
        //     float orig = Convert.ToSingle(t);
        //     if (num < 0)
        //     {
        //         negative = true;
        //         num *= -1;
        //     }
        //     var diff = orig - 200f;
        //     num += diff;
        //     if (negative) num *= -1;
        //     if (VFInput.alt) GS3.Log($"GetRadiusFromAstroId Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} returning {num}");
        //     return (T)Convert.ChangeType(num, typeof(T));
        // }
        // public static T GetRadiusFromLocalPlanet<T>(T t)
        // {
        //     var negative = false;
        //     var num = GameMain.localPlanet?.realRadius ?? 200f;
        //     float orig = Convert.ToSingle(t);
        //     if (num < 0)
        //     {
        //         negative = true;
        //         num *= -1;
        //     }
        //     var diff = orig - 200f;
        //     num += diff;
        //     if (negative) num *= -1;
        //     if (VFInput.alt)
        //         GS3.Log(
        //             $"GetRadius Called By {GS3.GetCaller(0)} {GS3.GetCaller(1)} {GS3.GetCaller(2)} orig:{orig} returning {num}");
        //     return (T)Convert.ChangeType(num, typeof(T));
        // }
    }
}