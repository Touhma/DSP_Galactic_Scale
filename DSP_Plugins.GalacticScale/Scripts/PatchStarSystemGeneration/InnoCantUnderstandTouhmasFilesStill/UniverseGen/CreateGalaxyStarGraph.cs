using System;
using UnityEngine;
using Patch = GalacticScale.Scripts.PatchStarSystemGeneration.PatchForStarSystemGeneration;
namespace GalacticScale
{
    public static partial class GS2
    {
        //public static void CreateGalaxyStarGraph()
        //{
        //    galaxy.graphNodes = new StarGraphNode[galaxy.starCount];
        //    Patch.Debug("CreateGalaxyStarGraph " + galaxy.starCount + " " + galaxy.graphNodes.Length);
        //    for (int index1 = 0; index1 < galaxy.starCount; ++index1)
        //    {
        //        galaxy.graphNodes[index1] = new StarGraphNode(galaxy.stars[index1]);
        //        StarGraphNode graphNode1 = galaxy.graphNodes[index1];
        //        for (int index2 = 0; index2 < index1; ++index2)
        //        {
        //            StarGraphNode graphNode2 = galaxy.graphNodes[index2];
        //            if ((graphNode1.pos - graphNode2.pos).sqrMagnitude < 64.0)
        //            {
        //                UniverseGen.list_sorted_add(graphNode1.conns, graphNode2);
        //                UniverseGen.list_sorted_add(graphNode2.conns, graphNode1);
        //            }
        //            //Patch.Debug("list_sorted_add " + index2);
        //        }
        //        //Patch.Debug("Finished inner for loop "+index1);
        //        line_arragement_for_add_node(graphNode1);
        //        Patch.Debug("Finished line_arrangement_for_add_node "+index1);
        //    }
        //    Patch.Debug("Finished CreateGalaxyStarGraph");
        //}
        //private static void line_arragement_for_add_node(StarGraphNode node)
        //{
        //    if (UniverseGen.tmp_state == null)
        //        UniverseGen.tmp_state = new int[128];
        //    Array.Clear((Array)UniverseGen.tmp_state, 0, UniverseGen.tmp_state.Length);
        //    Vector3 pos1 = (Vector3)node.pos;
        //    Patch.Debug("LineArrangement node.conns.count " + node.conns.Count);
        //    for (int index1 = 0; index1 < node.conns.Count; ++index1)
        //    {
        //        StarGraphNode conn1 = node.conns[index1];
        //        Vector3 pos2 = (Vector3)conn1.pos;
        //        for (int index2 = index1 + 1; index2 < node.conns.Count; ++index2)
        //        {
        //            StarGraphNode conn2 = node.conns[index2];
        //            Vector3 pos3 = (Vector3)conn2.pos;
        //            bool flag = false;
        //            for (int index3 = 0; index3 < conn1.conns.Count; ++index3)
        //            {
        //                if (conn1.conns[index3] == conn2)
        //                {
        //                    flag = true;
        //                    break;
        //                }
        //            }
        //            if (flag)
        //            {
        //                float num1 = (float)(((double)pos2.x - (double)pos1.x) * ((double)pos2.x - (double)pos1.x) + ((double)pos2.y - (double)pos1.y) * ((double)pos2.y - (double)pos1.y) + ((double)pos2.z - (double)pos1.z) * ((double)pos2.z - (double)pos1.z));
        //                float num2 = (float)(((double)pos3.x - (double)pos1.x) * ((double)pos3.x - (double)pos1.x) + ((double)pos3.y - (double)pos1.y) * ((double)pos3.y - (double)pos1.y) + ((double)pos3.z - (double)pos1.z) * ((double)pos3.z - (double)pos1.z));
        //                float num3 = (float)(((double)pos2.x - (double)pos3.x) * ((double)pos2.x - (double)pos3.x) + ((double)pos2.y - (double)pos3.y) * ((double)pos2.y - (double)pos3.y) + ((double)pos2.z - (double)pos3.z) * ((double)pos2.z - (double)pos3.z));
        //                float num4 = (double)num1 <= (double)num2 ? ((double)num2 <= (double)num3 ? num3 : num2) : ((double)num1 <= (double)num3 ? num3 : num1);
        //                float num5 = (double)num1 >= (double)num2 ? ((double)num2 >= (double)num3 ? num3 : num2) : ((double)num1 >= (double)num3 ? num3 : num1);
        //                float num6 = (float)(((double)num1 + (double)num2 + (double)num3 - (double)num4 - (double)num5) * 1.00100004673004);
        //                float num7 = num5 * 1.01f;
        //                if ((double)num1 <= (double)num6 || (double)num1 <= (double)num7)
        //                {
        //                    if (UniverseGen.tmp_state[index1] == 0)
        //                    {
        //                        UniverseGen.list_sorted_add(node.lines, conn1);
        //                        UniverseGen.list_sorted_add(conn1.lines, node);
        //                        UniverseGen.tmp_state[index1] = 1;
        //                    }
        //                }
        //                else
        //                {
        //                    UniverseGen.tmp_state[index1] = -1;
        //                    node.lines.Remove(conn1);
        //                    conn1.lines.Remove(node);
        //                }
        //                if ((double)num2 <= (double)num6 || (double)num2 <= (double)num7)
        //                {
        //                    if (UniverseGen.tmp_state[index2] == 0)
        //                    {
        //                        UniverseGen.list_sorted_add(node.lines, conn2);
        //                        UniverseGen.list_sorted_add(conn2.lines, node);
        //                        UniverseGen.tmp_state[index2] = 1;
        //                    }
        //                }
        //                else
        //                {
        //                    UniverseGen.tmp_state[index2] = -1;
        //                    node.lines.Remove(conn2);
        //                    conn2.lines.Remove(node);
        //                }
        //                if ((double)num3 > (double)num6 && (double)num3 > (double)num7)
        //                {
        //                    conn1.lines.Remove(conn2);
        //                    conn2.lines.Remove(conn1);
        //                }
        //            }
        //        }
        //        if (UniverseGen.tmp_state[index1] == 0)
        //        {
        //            UniverseGen.list_sorted_add(node.lines, conn1);
        //            UniverseGen.list_sorted_add(conn1.lines, node);
        //            UniverseGen.tmp_state[index1] = 1;
        //        }
        //    }
        //    Array.Clear((Array)UniverseGen.tmp_state, 0, UniverseGen.tmp_state.Length);
        //}
    }
}