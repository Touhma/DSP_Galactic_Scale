using NebulaAPI;
using System.IO;

namespace GalacticScale
{
    [RegisterPacketProcessor]
    public class LobbyRequestUpdateSolarSystemsProcessor: BasePacketProcessor<LobbyRequestUpdateSolarSystems>
    {
        public override void ProcessPacket(LobbyRequestUpdateSolarSystems packet, INebulaConnection conn)
        {
            if (IsClient)
            {
                return;
            }

            using (MemoryStream ms = new MemoryStream())
            {
                using (BinaryWriter w = new BinaryWriter(ms))
                {
                    var data = GSSettings.Serialize();
                    w.Write(data);
                    w.Close();
                    byte[] output = ms.ToArray();
                    conn.SendPacket(new LobbyResponseUpdateSolarSystems(output));
                }
            }
        }
    }
}
