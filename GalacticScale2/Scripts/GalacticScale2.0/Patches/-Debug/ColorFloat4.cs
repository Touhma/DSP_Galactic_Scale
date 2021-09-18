using UnityEngine;
using System;

namespace GalacticScale
{
    public static class ColorFloat4
    {
        public static operator Color(this Vector4 v4)
        {
            return new Color();
        }
        public static ColorFloat4(){}
    }
}