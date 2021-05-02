using System.IO;
using HarmonyLib;

namespace GalacticScale
{
	public static class PatchOnGameDescImport
	{
		[HarmonyPatch(typeof(GameDesc))]
		[HarmonyPrefix, HarmonyPatch("Import")]
		public static bool Import(BinaryReader r, ref GameDesc __instance)
		{
			if (!DSPGame.IsMenuDemo)
			{
				GS2.Import(r);
				return true;
			}
			GSSettings.Instance.imported = false;
			return true;
		}
	}
}