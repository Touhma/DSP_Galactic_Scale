namespace GalacticScale
{
    //    public partial class PatchOnGameSave
    //    {
    //     [HarmonyPatch(typeof(GameSave),"ReadHeaderAndDescAndProperty")]
    //     [HarmonyPrefix]
    //        	public static bool ReadHeaderAndDescAndProperty(string saveName, bool readImage, out GameSaveHeader header, out GameDesc desc, out ClusterPropertyData property)
    //            {
    //             GS2.Warn("ReadHeaderAndDescAndProperty");
    // 	header = null;
    // 	desc = null;
    // 	property = null;
    // 	if (saveName == null)
    // 	{
    // 		return false;
    // 	}
    // 	saveName = saveName.ValidFileName();
    // 	string path = GameConfig.gameSaveFolder + saveName + GameSave.saveExt;
    // 	if (!File.Exists(path))
    // 	{
    // 		return false;
    // 	}
    //
    // 	GS2.Warn("Trying");
    // 	try
    // 	{
    // 		header = new GameSaveHeader();
    // 		using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
    // 		{
    // 			using (BinaryReader binaryReader = new BinaryReader(fileStream))
    // 			{
    // 				GS2.Warn("Using");
    // 				if (!true || binaryReader.ReadChar() != 'V' || binaryReader.ReadChar() != 'F' || binaryReader.ReadChar() != 'S' || binaryReader.ReadChar() != 'A' || binaryReader.ReadChar() != 'V' || binaryReader.ReadChar() != 'E')
    // 				{
    // 					GS2.Warn("False");
    // 					header = null;
    // 				}
    // 				else
    // 				{
    // 					GS2.Warn("Else");
    // 					long length = fileStream.Length;
    // 					header.fileSize = binaryReader.ReadInt64();
    // 					if (length != header.fileSize)
    // 					{
    // 						GS2.Warn("Header wrong size");
    // 						header = null;
    // 					}
    // 					else
    // 					{
    // 						GS2.Warn("Else2");
    // 						header.headerVersion = binaryReader.ReadInt32();
    // 						if (header.headerVersion < 1)
    // 						{
    // 							GS2.Warn("Version mismatch");
    // 							header = null;
    // 						}
    // 						else
    // 						{
    // 							GS2.Warn($"Else3 {header.headerVersion}");
    // 							header.lastSaveVersion.Major = binaryReader.ReadInt32();
    // 							header.lastSaveVersion.Minor = binaryReader.ReadInt32();
    // 							header.lastSaveVersion.Release = binaryReader.ReadInt32();
    // 							if (header.headerVersion >= 6)
    // 							{
    // 								GS2.Warn(">=6");
    // 								header.lastSaveVersion.Build = binaryReader.ReadInt32();
    // 							}
    // 							header.gameTick = binaryReader.ReadInt64();
    // 							long value = binaryReader.ReadInt64();
    // 							header.saveTime = default(DateTime).AddTicks(value);
    // 							if (header.headerVersion >= 6)
    // 							{
    // 								GS2.Warn(">=6 2");
    // 								header.saveTime = DateTime.SpecifyKind(header.saveTime, DateTimeKind.Utc);
    // 							}
    // 							else
    // 							{
    // 								GS2.Warn(">=6 else");
    // 								header.saveTime = DateTime.SpecifyKind(header.saveTime, DateTimeKind.Local);
    // 							}
    // 							GS2.Warn("ReadingImage");
    // 							if (readImage)
    // 							{
    // 								int count = binaryReader.ReadInt32();
    // 								header.themeImage = binaryReader.ReadBytes(count);
    // 							}
    // 							if (header.headerVersion >= 5)
    // 							{
    // 								GS2.Warn(">=5");
    // 								header.accountData.Import(binaryReader);
    // 								header.clusterGeneration = binaryReader.ReadUInt64();
    // 							}
    // 							else
    // 							{
    // 								GS2.Warn(">=5 else");
    // 								header.accountData = AccountData.NULL;
    // 								header.clusterGeneration = 0UL;
    // 							}
    // 							long position = binaryReader.BaseStream.Position;
    // 							GS2.Log("Importing GameDesc");
    // 							desc = GameData.ImportGameDesc(binaryReader);
    // 							
    // 							binaryReader.BaseStream.Position = position;
    // 							GS2.Log("Importing GamePropertyData");
    // 							property = GameData.ImportGamePropertyData(binaryReader);
    // 							GS2.Warn("Completed");
    // 						}
    // 					}
    // 				}
    // 			}
    // 		}
    // 	}
    // 	catch (Exception e)
    // 	{
    // 		GS2.Warn("Exception");
    // 		GS2.Warn(e.Message);
    // 		GS2.Warn(e.StackTrace);
    // 		header = null;
    // 		desc = null;
    // 		property = null;
    // 	}
    //
    // 	return false;
    // }
    //    }
}