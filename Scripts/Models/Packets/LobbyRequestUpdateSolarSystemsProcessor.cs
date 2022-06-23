using System.Collections.Generic;
using System.IO;
using GalacticScale;
using NebulaAPI;

namespace NebulaCompatibility
{
    [RegisterPacketProcessor]
    public class LobbyRequestUpdateSolarSystemsProcessor : BasePacketProcessor<LobbyRequestUpdateSolarSystems>
    {
        public override void ProcessPacket(LobbyRequestUpdateSolarSystems packet, INebulaConnection conn)
        {
            if (IsClient) return;

            var names = new List<string>();
            var starIds = new List<int>();
            var planetIds = new List<int>();
            if (GameMain.galaxy != null)
                foreach (var s in GameMain.galaxy.stars)
                {
                    if (!string.IsNullOrEmpty(s.overrideName))
                    {
                        names.Add(s.overrideName);
                        starIds.Add(s.id);
                        planetIds.Add(NebulaModAPI.PLANET_NONE);
                    }

                    foreach (var p in s.planets)
                        if (!string.IsNullOrEmpty(p.overrideName))
                        {
                            names.Add(p.overrideName);
                            starIds.Add(NebulaModAPI.STAR_NONE);
                            planetIds.Add(p.id);
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