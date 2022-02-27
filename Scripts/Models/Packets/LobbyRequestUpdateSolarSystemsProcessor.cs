using System.IO;
using NebulaAPI;
using GalacticScale;

namespace NebulaCompatibility
{
    [RegisterPacketProcessor]
    public class LobbyRequestUpdateSolarSystemsProcessor : BasePacketProcessor<LobbyRequestUpdateSolarSystems>
    {
        public override void ProcessPacket(LobbyRequestUpdateSolarSystems packet, INebulaConnection conn)
        {
            if (IsClient) return;

            using (var ms = new MemoryStream())
            {
                using (var w = new BinaryWriter(ms))
                {
                    var data = GSSettings.Serialize();
                    w.Write(data);
                    w.Close();
                    var output = ms.ToArray();
                    conn.SendPacket(new LobbyResponseUpdateSolarSystems(output));
                }
            }
        }
    }
}