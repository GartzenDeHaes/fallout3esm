using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.CELL;

namespace Fallout.NET.TES4.Records
{
	[Flags]
	public enum CoordFlags
	{
		None = 0,
		ForceHideLandQuad1 = 0x1,
		ForceHideLandQuad2 = 0x2,
		ForceHideLandQuad3 = 0x4,
		ForceHideLandQuad4 = 0x8,
	}

	public class CELLRecord : Record
	{
		public STRSubRecord FULL;
		public CELL_DATASubRecord DATA_Flags;
		public BytesSubRecord XCLL_Lighting;
		public ByteSubRecord XCMT_Unused;
		/// <summary>Regions | formid[] | Array of REGN record FormIDs.</summary>
		public List<uint> XCLR_Regions = new();
		/// <summary>Image Space | formid | FormID of an IMGS record.</summary>
		public FormID XCIM_ImageSpace;
		/// <summary>Encounter Zone | formid | FormID of an ECZN record.</summary>
		public FormID XEZN_EncounterZone;
		/// <summary>Climate | formid | FormID of a CLMT record.</summary>
		public FormID XCCM_Climate;
		/// <summary>Water | formid | FormID of a WATR record.</summary>
		public FormID XCWT_Water;
		/// <summary>Water Noise Texture | cstring</summary>
		public STRSubRecord XNAM_WaterNoiseTexture;
		/// <summary>Ownership data (interior cells). FormID of a FACT, ACHR or NPC_ record.</summary>
		public FormID XOWN_Owner;
		/// <summary>Acoustic Space | formid | FormID of an ASPC record.</summary>
		public FormID XCAS_AcousticSpace;
		/// <summary>Light Template | formid | Light template. FormID of an LGTM record, or null.
		/// </summary>
		//public FormID LTMP_LightTemplate;
		//public uint LNAM_LightFlags;
		/// <summary>Music Type | formid | FormID of a MUSC record.</summary>
		public FormID XCMO_MusicType;
		/// <summary>Faction rank | int32 | Ownership data </summary>
		public UInt32SubRecord XRNK_FactionRank;
		/// <summary>Water Height | float32</summary>
		public FloatSubRecord XCLW_WaterHeight;
		/// <summary>(X, Y) grid (only used for exterior cells)</summary>
		public Vector2iSubRecord XCLC_GridCoord;
		public CoordFlags XCLC_GridCoordFlags;
		public LightTemplate LightTempate;
		public List<Group> SubGroups;
		
		public override void NoteSubGroup(Group subGroup)
		{
			SubGroups ??= new List<Group>();
			SubGroups.Add(subGroup);
		}

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			//var bytes = reader.ReadBytes((int)size);
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

			using (var stream = new BetterMemoryReader(bytes))
			{
				//var end = stream.Length;

				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;

						case "FULL":
							FULL = new STRSubRecord();
							FULL.Deserialize(stream, name);
							break;

						case "DATA":
							DATA_Flags = new CELL_DATASubRecord();
							DATA_Flags.Deserialize(stream, name);
							break;

						case "XCLL":
							XCLL_Lighting = new BytesSubRecord();
							XCLL_Lighting.Deserialize(stream, name);
							break;

						case "XCMT":
							XCMT_Unused = new ByteSubRecord();
							XCMT_Unused.Deserialize(stream, name);
							break;

						//case "LNAM":
						//	LNAM_LightFlags = stream.ReadUInt32();
						//	break;

						case "XRNK":
							XRNK_FactionRank = new UInt32SubRecord();
							XRNK_FactionRank.Deserialize(stream, name);
							break;

						case "XNAM":
							XNAM_WaterNoiseTexture = new STRSubRecord();
							XNAM_WaterNoiseTexture.Deserialize(stream, name);
							break;

						case "XCLW":
							XCLW_WaterHeight = new FloatSubRecord();
							XCLW_WaterHeight.Deserialize(stream, name);
							break;

						case "XCLC":
							XCLC_GridCoord = new Vector2iSubRecord();
							XCLC_GridCoord.Deserialize(stream, name);
							if (XCLC_GridCoord.Size > 8)
							{
								// Docs say there should be a uint32 flags here for FO3 (not Oblivion)
								XCLC_GridCoordFlags = (CoordFlags)stream.ReadUInt32();
							}
							break;
						case "XCLR":
							// Docs say could be an array
							UnityEngine.Debug.Assert(XCLR_Regions.Count == 0);
							var datasize = stream.ReadUInt16();
							for (int i = 0; i < datasize / 4; ++i)
							{
								XCLR_Regions.Add(stream.ReadUInt32());
							}
							break;
						case "XCIM":
							XCIM_ImageSpace = new FormID();
							XCIM_ImageSpace.Deserialize(stream, name);
							break;
						case "XEZN":
							XEZN_EncounterZone = new FormID();
							XEZN_EncounterZone.Deserialize(stream, name);
							break;
						case "XCCM":
							XCCM_Climate = new FormID();
							XCCM_Climate.Deserialize(stream, name);
							break;
						case "XCWT":
							XCWT_Water = new FormID();
							XCWT_Water.Deserialize(stream, name);
							break;
						case "XOWN":
							XOWN_Owner = new FormID();
							XOWN_Owner.Deserialize(stream, name);
							break;
						case "XCAS":
							XCAS_AcousticSpace = new FormID();
							XCAS_AcousticSpace.Deserialize(stream, name);
							break;
						case "XCMO":
							XCMO_MusicType = new FormID();
							XCMO_MusicType.Deserialize(stream, name);
							break;
						case "LTMP":
							LightTempate = new LightTemplate();
							LightTempate.LGTM_FormId.Deserialize(stream, name);
							break;
						case "LNAM":
							datasize = stream.ReadUInt16();
							LightTempate.Flags = (LightTemplateInheritFlags)stream.ReadUInt32();
							break;
						case "XCET":
							// unknown
							datasize = stream.ReadUInt16();
							stream.ReadByte();
							break;
						default:
							UnityEngine.Debug.Log($"{Type} unknown {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}

		public override string ToString()
		{
			return EDID.ToString();
		}
	}

	[Flags]
	public enum LightTemplateInheritFlags
	{
		None = 0,
		AmbientColor = 0x00000001,
		DirectionalColor = 0x00000002,
		FogColor = 0x00000004,
		FogNear = 0x00000008,
		FogFar = 0x00000010,
		DirectionalRotation = 0x00000020,
		DirectionalFade = 0x00000040,
		FogClipDistance = 0x00000080,
		FogPower = 0x00000100,
	}

	public class LightTemplate
	{
		public FormID LGTM_FormId = new();
		public LightTemplateInheritFlags Flags;
	}
}
