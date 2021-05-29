using System.IO;
using HarmonyLib;

namespace GalacticScale
{
	public static class PatchOnGameDescExport
	{
		[HarmonyPatch(typeof(GameDesc))]
		[HarmonyPostfix, HarmonyPatch("Export")]
		public static void Export(BinaryWriter w)
		{
			if (GS2.IsMenuDemo) return;
			GS2.Export(w);
			return;
		}
	}
}