﻿using HarmonyLib;

namespace GalacticScale {
    public partial class PatchOnUniverseGen {
        [HarmonyPrefix, HarmonyPatch(typeof(UniverseGen), "CreateGalaxyStarGraph")]
        public static bool CreateGalaxyStarGraph(GalaxyData galaxy) {
            int starCount = galaxy.starCount;
            //if (starCount > 64 ) starCount = 64;
            galaxy.graphNodes = new StarGraphNode[starCount];
            UniverseGen.tmp_state = new int[1024]; // increased from 128 to allow denser galaxies
            for (int index1 = 0; index1 < starCount; ++index1) {
                galaxy.graphNodes[index1] = new StarGraphNode(galaxy.stars[index1]);
                StarGraphNode graphNode1 = galaxy.graphNodes[index1];
                for (int index2 = 0; index2 < System.Math.Min(GSSettings.GalaxyParams.graphMaxStars, index1); ++index2) {
                    StarGraphNode graphNode2 = galaxy.graphNodes[index2];
                    if ((graphNode1.pos - graphNode2.pos).sqrMagnitude < GSSettings.GalaxyParams.graphDistance) {
                        UniverseGen.list_sorted_add(graphNode1.conns, graphNode2);
                        UniverseGen.list_sorted_add(graphNode2.conns, graphNode1);
                    }
                }
                UniverseGen.line_arragement_for_add_node(graphNode1);
            }
            return false;
        }
    }
}
