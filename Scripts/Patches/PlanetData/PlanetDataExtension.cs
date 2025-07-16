using System;

namespace GalacticScale
{
    public static class PlanetDataExtension
    {
        public static float GetScaleFactored(this PlanetData planet)
        {
            if (planet == null)
            {
                GS2.Error("Trying to get factored scale while planet is null");
                return 1f;
            }

            if (planet.type == EPlanetType.Gas) return planet.radius / 80;

            return planet.radius / 200;
        }

        // Deep copy method for safe copying of all relevant fields
        public static void DeepCopyScannedDataFrom(this PlanetData target, PlanetData copy)
        {
            // Copy the same fields as CopyScannedDataFrom
            target.landPercent = copy.landPercent;
            target.veinBiasVector = copy.veinBiasVector;
            target.birthPoint = copy.birthPoint;
            target.birthResourcePoint0 = copy.birthResourcePoint0;
            target.birthResourcePoint1 = copy.birthResourcePoint1;
            if (copy.veinGroups != null)
            {
                target.veinGroups = new VeinGroup[copy.veinGroups.Length];
                if (target.veinGroups.Length > 0)
                    target.veinGroups[0].SetNull();
                for (int i = 1; i < copy.veinGroups.Length; i++)
                {
                    target.veinGroups[i] = copy.veinGroups[i];
                }
            }
            else
            {
                target.veinGroups = null;
            }

            // Deep copy modData
            if (copy.modData != null)
                target.modData = (byte[])copy.modData.Clone();
            else
                target.modData = null;

            // Deep copy data (PlanetRawData)
            if (copy.data != null)
                target.data = DeepCopyPlanetRawData(copy.data);
            else
                target.data = null;
        }

        // Helper for deep copying PlanetRawData
        public static PlanetRawData DeepCopyPlanetRawData(PlanetRawData src)
        {
            if (src == null) return null;
            var copy = new PlanetRawData(src.precision);
            // Copy arrays
            if (src.heightData != null) copy.heightData = (ushort[])src.heightData.Clone();
            if (src.modData != null) copy.modData = (byte[])src.modData.Clone();
            if (src.vegeIds != null) copy.vegeIds = (ushort[])src.vegeIds.Clone();
            if (src.biomoData != null) copy.biomoData = (byte[])src.biomoData.Clone();
            if (src.temprData != null) copy.temprData = (short[])src.temprData.Clone();
            if (src.vertices != null) copy.vertices = (UnityEngine.Vector3[])src.vertices.Clone();
            if (src.normals != null) copy.normals = (UnityEngine.Vector3[])src.normals.Clone();
            if (src.indexMap != null) copy.indexMap = (int[])src.indexMap.Clone();
            // Copy primitive fields
            copy.indexMapPrecision = src.indexMapPrecision;
            copy.indexMapDataLength = src.indexMapDataLength;
            copy.indexMapFaceStride = src.indexMapFaceStride;
            copy.indexMapCornerStride = src.indexMapCornerStride;
            // Copy veinPool and vegePool (shallow, as in vanilla)
            if (src.veinPool != null) copy.veinPool = (VeinData[])src.veinPool.Clone();
            copy.veinCursor = src.veinCursor;
            if (src.vegePool != null) copy.vegePool = (VegeData[])src.vegePool.Clone();
            copy.vegeCursor = src.vegeCursor;
            // Return the deep-copied object
            return copy;
        }
    }
}