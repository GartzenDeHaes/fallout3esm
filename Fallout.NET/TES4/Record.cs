using Fallout.NET.Core;
using Fallout.NET.TES4.Records;

using Portland;

using System.IO;

using UnityEngine;

namespace Fallout.NET.TES4
{
	public enum RecordFlags : uint
	{
		ThePluginIsaMasterFile = 0x00000001,
		Unknown1 = 0x00000002,
		Unknown2 = 0x00000004,
		Unknown3 = 0x00000008,
		Unknown4 = 0x00000010,
		Deleted = 0x00000020,
		BorderRegion_HasTreeLOD_Constant_HiddenFromLocalMap = 0x00000040,
		TurnOffFire = 0x00000080,
		Inaccessible = 0x00000100,
		CastsShadows_OnLocalMap_MotionBlur = 0x00000200,
		QuestItem_PersistentReference = 0x00000400,
		InitiallyDisabled = 0x00000800,
		Ignored = 0x00001000,
		NoVoiceFilter = 0x00002000,
		Unknown5 = 0x00004000,
		VisibleWhenDistant = 0x00008000,
		RandomAnimStart_HighPriorityLOD = 0x00010000,
		Dangerous_OfflimitsInteriorCell_RadioStationTalkingActivator = 0x00020000,
		Compressed = 0x00040000,
		CantWait_PlatformSpecificTexture = 0x00080000,
		Unknown6 = 0x00100000,
		Unknown7 = 0x00200000,
		Unknown8 = 0x00400000,
		Unknown9 = 0x00800000,
		Unknown10 = 0x01000000,
		Obstacle_NoAIAcquire = 0x02000000,
		NavMeshGenerationFilter = 0x04000000,
		NavMeshGenerationBoundingBox = 0x08000000,
		NonPipboy_ReflectedByAutoWater = 0x10000000,
		ChildCanUse_RefractedByAutoWater = 0x20000000,
		NavMeshGenerationGround = 0x40000000,
		Unknow11 = 0x80000000,
	}

	public class Record
	{
		public AsciiId4 Type;
		uint dataSize;
		protected RecordFlags flags;
		public uint FormId;
		//uint revision;
		//uint version;
		//uint unknow;

		protected byte[] data;

		public string EDID { get; protected set; }

		public bool Compressed
		{
			get { return ((uint)flags & 0x00040000) != 0; }
		}

		public bool Deleted
		{
			get { return ((uint)flags & 0x20) != 0; }
		}

		public bool Ignored
		{
			get { return ((uint)flags & 0x1000) != 0; }
		}

		public void Deserialize(BetterReader reader, in AsciiId4 name, GameID gameID)
		{
			Type = name;
			dataSize = reader.ReadUInt32();
			flags = (RecordFlags)reader.ReadUInt32();
			FormId = reader.ReadUInt32();
			var revision = reader.ReadUInt32();

			if (gameID != GameID.Oblivion)
			{
				var version = reader.ReadUInt16();
				var unknow = reader.ReadUInt16();
			}

			if (Deleted)
			{
				Debug.Log($"{Type} DELETED REC size={dataSize}");
				reader.ReadBytes((int)dataSize);
				return;
			}

			if (Compressed)
			{
				var decompSize = (int)reader.ReadUInt32();
				//var compressedData = reader.ReadBytes((int)dataSize - 4);
				data = reader.ReadBytes((int)dataSize - 4);
				//Utils.LogBuffer("\t\tCompressed Data {0}", Type);

				//var decompressedData = Decompress(compressedData);
				//using (var betterReader = new BetterMemoryReader(decompressedData))
				//	ExtractSubRecords(betterReader, gameID, (uint)betterReader.Length);
			}
			else
			{
				ExtractSubRecords(reader, gameID, dataSize);
			}
		}

		public void DecompressSubRecords(in GameID gameId)
		{
			if (data != null && Compressed)
			{
				var decompSize = data.Length;

				var decompressedData = Decompress(data);
				data = null;
				using (var betterReader = new BetterMemoryReader(decompressedData))
				{
					ExtractSubRecords(betterReader, gameId, (uint)betterReader.Length);
				}
				flags = (RecordFlags)((uint)flags & ~0x00040000);
				UnityEngine.Debug.Assert(!Compressed);
			}
		}

		protected virtual void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}

		public virtual void NoteSubGroup(Group subGroup)
		{
			UnityEngine.Debug.Log($"Unhandled subgroup for {Type}");
		}

		private static byte[] Decompress(byte[] data)
		{
			using (var outMemoryStream = new MemoryStream())
			using (var outZStream = new ComponentAce.Compression.Libs.zlib.ZOutputStream(outMemoryStream))
			using (var inMemoryStream = new MemoryStream(data))
			{
				CopyStream(inMemoryStream, outZStream);
				outZStream.finish();
				return outMemoryStream.ToArray();
			}
		}

		public static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[2000];
			int len;
			while ((len = input.Read(buffer, 0, 2000)) > 0)
			{
				output.Write(buffer, 0, len);
			}
			output.Flush();
		}

		public static Record GetRecord(string name)
		{
			if (name == "ACHR")
				return new ACHRRecord();
			else if (name == "ACRE")
				return new ACRERecord();
			else if (name == "ACTI")
				return new ACTIRecord();
			else if (name == "ADDN")
				return new ADDNRecord();
			else if (name == "ALCH")
				return new ALCHRecord();
			else if (name == "AMMO")
				return new AMMORecord();
			else if (name == "ANIO")
				return new ANIORecord();
			else if (name == "ARMA")
				return new ARMAARecord();
			else if (name == "ASPC")
				return new ASPCRecord();
			else if (name == "ARMO")
				return new ARMORecord();
			else if (name == "AVIF")
				return new AVIFRecord();
			else if (name == "BOOK")
				return new BOOKRecord();
			//else if (name == "BSGN")
			//	return new BSGNRecord();
			else if (name == "CELL")
				return new CELLRecord();
			else if (name == "CLAS")
				return new CLASRecord();
			else if (name == "CLMT")
				return new CLMTRecord();
			else if (name == "CLOT")
				return new CLOTRecord();
			else if (name == "CONT")
				return new CONTRecord();
			else if (name == "CREA")
				return new CREARecord();
			else if (name == "CSTY")
				return new CSTYRecord();
			else if (name == "DEBR")
				return new DEBRRecord();
			else if (name == "DIAL")
				return new DIALRecord();
			else if (name == "DOBJ")
				return new DOBJRecord();
			else if (name == "DOOR")
				return new DOORRecord();
			else if (name == "EFSH")
				return new EFSHRecord();
			else if (name == "ENCH")
				return new ENCHRecord();
			else if (name == "ECZN")
				return new ECZNRecord();
			else if (name == "EXPL")
				return new EXPLRecord();
			else if (name == "EYES")
				return new EYESRecord();
			else if (name == "FACT")
				return new FACTRecord();
			//else if (name == "FLOR")
			//	return new FLORRecord();
			else if (name == "FLST")
				return new FLSTRecord();
			else if (name == "FURN")
				return new FURNRecord();
			else if (name == "GLOB")
				return new GLOBRecord();
			else if (name == "GMST")
				return new GMSTRecord();
			else if (name == "GRAS")
				return new GRASRecord();
			else if (name == "HAIR")
				return new HAIRRecord();
			else if (name == "IMAD")
				return new IMADRecord();
			else if (name == "IDLM")
				return new IDLMRecord();
			else if (name == "IDLE")
				return new IDLERecord();
			else if (name == "IMGS")
				return new IMGSRecord();
			else if (name == "INFO")
				return new INFORecord();
			else if (name == "INGR")
				return new INGRRecord();
			else if (name == "IPCT")
				return new IPCTRecord();
			else if (name == "IPDS")
				return new IPDSRecord();
			else if (name == "KEYM")
				return new KEYMRecord();
			else if (name == "LAND")
				return new LANDRecord();
			else if (name == "LIGH")
				return new LIGHRecord();
			else if (name == "LSCR")
				return new LSCRRecord();
			else if (name == "LTEX")
				return new LTEXRecord();
			else if (name == "LVLC")
				return new LVLCRecord();
			else if (name == "LVLI")
				return new LVLIRecord();
			else if (name == "LVLN")
				return new LVLNRecord();
			//else if (name == "LVSP")
			//	return new LVSPRecord();
			else if (name == "MGEF")
				return new MGEFRecord();
			else if (name == "MICN")
				return new MICNRecord();
			else if (name == "MISC")
				return new MISCRecord();
			else if (name == "MSTT")
				return new MSTTRecord();
			else if (name == "MUSC")
				return new MUSCRecord();
			else if (name == "NOTE")
				return new NOTERecord();
			else if (name == "NPC_")
				return new NPC_Record();
			else if (name == "PACK")
				return new PACKRecord();
			else if (name == "PERK")
				return new PERKRecord();
			//else if (name == "PGRD")
			//	return new PGRDRecord();
			else if (name == "PGRE")
				return new PGRERecord();
			else if (name == "PROJ")
				return new PROJRecord();
			else if (name == "PWAT")
				return new PWATRecord();
			else if (name == "QUST")
				return new QUSTRecord();
			else if (name == "RADS")
				return new RADSRecord();
			else if (name == "RACE")
				return new RACERecord();
			else if (name == "REFR")
				return new REFRRecord();
			else if (name == "REGN")
				return new REGNRecord();
			//else if (name == "ROAD")
			//	return new ROADRecord();
			//else if (name == "SBSP")
			//	return new SBSPRecord();
			else if (name == "SCOL")
				return new SCOLRecord();
			else if (name == "SCPT")
				return new SCPTRecord();
			//else if (name == "SGST")
			//	return new SGSTRecord();
			//else if (name == "SKIL")
			//	return new SKILRecord();
			//else if (name == "SLGM")
			//	return new SLGMRecord();
			else if (name == "SOUN")
				return new SOUNRecord();
			else if (name == "SPEL")
				return new SPELRecord();
			else if (name == "STAT")
				return new STATRecord();
			else if (name == "TACT")
				return new TACTRecord();
			else if (name == "TERM")
				return new TERMRecord();
			else if (name == "TES4")
				return new TES4Record();
			else if (name == "TREE")
				return new TREERecord();
			else if (name == "TXST")
				return new TXSTRecord();
			else if (name == "VTYP")
				return new VTYPRecord();
			else if (name == "WATR")
				return new WATRRecord();
			else if (name == "WEAP")
				return new WEAPRecord();
			else if (name == "WRLD")
				return new WRLDRecord();
			else if (name == "WTHR")
				return new WTHRRecord();

			// Nav Mesh, (tutorial) Message, Head Part, Body Part Data, Camera Shot, Camera Path, Lighting Template, Body Part Data
			if 
			(
				name != "RGDL" && 
				name != "NAVM" && 
				name != "MESG" && 
				name != "HDPT" && 
				name != "BPDT" && 
				name != "CAMS" && 
				name != "CPTH" && 
				name != "LGTM" && 
				name != "BPTD" &&
				name != "NAVI"
			)
			{ 
				Debug.Log($"Unknow recored {name}"); 
			}

			return new Record();
		}
	}
}
