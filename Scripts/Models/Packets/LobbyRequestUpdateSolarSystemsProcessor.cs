using System.IO;
using NebulaAPI;
using GalacticScale;
using System.Collections.Generic;

namespace NebulaCompatibility
{
    [RegisterPacketProcessor]
    public class LobbyRequestUpdateSolarSystemsProcessor : BasePacketProcessor<LobbyRequestUpdateSolarSystems>
    {
        public override void ProcessPacket(LobbyRequestUpdateSolarSystems packet, INebulaConnection conn)
        {
            if (IsClient) return;
                        
            List<string> names = new List<string>();
            List<int> starIds = new List<int>();
            List<int> planetIds = new List<int>();
            if (GameMain.galaxy != null)
            {
                foreach (StarData s in GameMain.galaxy.stars)
                {
                    if (!string.IsNullOrEmpty(s.overrideName))
                    {
                        names.Add(s.overrideName);
                        starIds.Add(s.id);
                        planetIds.Add(NebulaModAPI.PLANET_NONE);
                    }
                    foreach (PlanetData p in s.planets)
                    {
                        if (!string.IsNullOrEmpty(p.overrideName))
                        {
                            names.Add(p.overrideName);
                            starIds.Add(NebulaModAPI.STAR_NONE);
                            planetIds.Add(p.id);
                        }
                    }
                }
            }

            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms))
                {
                    var data = GSSettings.Serialize();
                    w.Write(data);
                    w.Close();
                    var output = ms.ToArray();
                    conn.SendPacket(new LobbyResponseUpdateSolarSystems(output, names.ToArray(), starIds.ToArray(), planetIds.ToArray()));
                }
            }
        }
    }
}