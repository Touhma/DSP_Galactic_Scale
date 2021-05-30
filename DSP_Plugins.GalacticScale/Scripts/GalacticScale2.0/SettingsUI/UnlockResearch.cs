using System.Linq;

namespace GalacticScale
{
    public static partial class GS2
    {        
        public static GSUI UnlockTechOption;
        // All credit to Windows10CE
        public static void UnlockTechOptionCallback(object o)
        {
            GS2.Log("Unlocking Tech");
            foreach (TechProto tech in LDB.techs.dataArray.Where(x => x.Published))
                {
                    if (!GameMain.history.TechUnlocked(tech.ID))
                        UnlockTechRecursive(tech.ID, GameMain.history);
                }
        }

        private static void UnlockTechRecursive(int techId, GameHistoryData history)
            {
                TechState state = history.TechState(techId);
                TechProto proto = LDB.techs.Select(techId);

                foreach (var techReq in proto.PreTechs)
                {
                    if (!history.TechState(techReq).unlocked)
                        UnlockTechRecursive(techReq, history);
                }
                foreach (var techReq in proto.PreTechsImplicit)
                {
                    if (!history.TechState(techReq).unlocked)
                        UnlockTechRecursive(techReq, history);
                }
                foreach (var itemReq in proto.itemArray)
                {
                    if (itemReq.preTech != null && !history.TechState(itemReq.preTech.ID).unlocked)
                        UnlockTechRecursive(itemReq.preTech.ID, history);
                }

                int current = state.curLevel;
                for (; current < state.maxLevel; current++)
                    for (int j = 0; j < proto.UnlockFunctions.Length; j++)
                        history.UnlockTechFunction(proto.UnlockFunctions[j], proto.UnlockValues[j], current);

                history.UnlockTech(techId);
            }
    }
}