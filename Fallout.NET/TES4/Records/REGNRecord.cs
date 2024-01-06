using System;
using System.Buffers;
using System.Collections.Generic;
using System.Xml.Linq;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Portland;

using UnityEngine;
namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Region. Region is a polygonal sub-area of a WRLD
	/// https://tes5edit.github.io/fopdoc/Fallout3/Records/REGN.html
	/// </summary>
	public sealed class REGNRecord : Record
	{
		public STRSubRecord ICON = new();
		public STRSubRecord MICO = new();
		public Color RCLR_MapColor;
		public FormID WNAM_WRLD = new();
		public List<RegionAreaSubRecordCollection> RPLI_Boundries = new();
		public List<RegionDataEntrySubRecordCollection> RDAT_Data = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;
			RegionAreaSubRecordCollection currentArea = null;
			ushort datalen;
			RegionDataEntrySubRecordCollection currentData = null;

			using (var stream = new BetterMemoryReader(bytes))
			{
				while (stream.Position < ssize)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "ICON":
							ICON.Deserialize(stream, name);
							break;
						case "MICO":
							MICO.Deserialize(stream, name);
							break;
						case "RCLR":
							RCLR_MapColor = ColorSubRecord.Read(stream, name);
							break;
						case "WNAM":
							WNAM_WRLD.Deserialize(stream, name);
							break;
						case "RPLI":
							currentArea = new();
							currentArea.EdgeFallOff.Deserialize(stream, name);
							RPLI_Boundries.Add(currentArea);
							break;
						case "RPLD":
							datalen = stream.ReadUInt16();
							for (int i = 0; i < datalen / 8; ++i)
							{
								var v = new Vector2(stream.ReadSingle(), stream.ReadSingle());
								currentArea.RPLD_RegionPointList.Add(v);
							}
							break;
						case "RDAT":
							currentData = new();
							RDAT_Data.Add(currentData);
							datalen = stream.ReadUInt16();
							currentData.Header.Type = (RegionDataType)stream.ReadUInt32();
							currentData.Header.Flags = (RegionDataFlags)stream.ReadByte();
							stream.ReadByte();
							stream.ReadByte();
							stream.ReadByte();
							break;
						case "RDOT":
							datalen = stream.ReadUInt16();
							for (int i = 0; i < datalen / 52; ++i)
							{
								var obj = new RDOT_Objects();
								obj.ObjectFormId = stream.ReadUInt32();
								obj.ParentIndex = stream.ReadUInt16();
								stream.ReadByte();
								stream.ReadByte();
								obj.Density = stream.ReadSingle();
								obj.Clustering = stream.ReadByte();
								obj.MinSlope = stream.ReadByte();
								obj.MaxSlope = stream.ReadByte();
								obj.Flags = (ObjectFlags)stream.ReadByte();
								obj.RadiusWithRespectToParent = stream.ReadUInt16();
								obj.Radius = stream.ReadUInt16();
								stream.ReadUInt32();
								obj.MaxHeight = stream.ReadSingle();
								obj.Sink = stream.ReadSingle();
								obj.SinkVariance = stream.ReadSingle();
								obj.SizeVariance = stream.ReadSingle();
								obj.XAngleVariance = stream.ReadUInt16();
								obj.YAngleVariance = stream.ReadUInt16();
								obj.ZAngleVariance = stream.ReadUInt16();
								stream.ReadUInt16();
								stream.ReadUInt32();

								currentData.ObjectParams.Add(obj);
							}
							break;
						case "RDMP":
							currentData.RDMP_MapName.Deserialize(stream, name);
							break;
						case "RDGS":
							datalen = stream.ReadUInt16();
							currentData.RDGS_GrassFormId_Unused = stream.ReadUInt32();
							stream.ReadUInt32();
							break;
						case "RDMD":
							datalen = stream.ReadUInt16();
							currentData.RDMD_MusicType = (MusicType)stream.ReadUInt32();
							break;
						case "RDMO":
							currentData.RDMO_MusicFormId_Unused.Deserialize(stream, name);
							break;
						case "RDSD":
							currentData.RDSD_Sound = new();
							datalen = stream.ReadUInt16();
							for (int i = 0; i < datalen / 12; ++i)
							{
								var sound = new RDSD_SubRecord();
								sound.SoundFormId = stream.ReadUInt32();
								sound.Flags = (SoundFlags)stream.ReadUInt32();
								sound.Chance = stream.ReadUInt32();
								currentData.RDSD_Sound.Add(sound);
							}
							break;
						case "RDWT":
							currentData.RDWT_Weather = new();
							datalen = stream.ReadUInt16();
							for (int i = 0; i < datalen / 12; ++i)
							{
								var weather = new RDWT_SubRecord();
								weather.WeatherFormId = stream.ReadUInt32();
								weather.Chance = stream.ReadUInt32();
								weather.GlobFormId = stream.ReadUInt32();
								currentData.RDWT_Weather.Add(weather);
							}
							break;
						default:
							UnityEngine.Debug.Log($"Unknown REGN sub record {name}");
							if (Char.IsLetter(name[0]) && Char.IsLetter(name[1]) && Char.IsLetter(name[2]) && Char.IsLetter(name[3]))
							{
								var rest = stream.ReadUInt16();
								stream.ReadBytes(rest);
							}
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}

	public sealed class RegionAreaSubRecordCollection
	{
		public UInt32SubRecord EdgeFallOff = new();
		public List<Vector2> RPLD_RegionPointList = new(); 
	}

	public sealed class RegionDataEntrySubRecordCollection
	{
		public RDAT_Header Header = new();
		public List<RDOT_Objects> ObjectParams = new();
		public STRSubRecord RDMP_MapName = new();
		public uint RDGS_GrassFormId_Unused;
		public MusicType RDMD_MusicType;
		public FormID RDMO_MusicFormId_Unused = new();
		public List<RDSD_SubRecord> RDSD_Sound;
		public List<RDWT_SubRecord> RDWT_Weather;
	}

	public enum RegionDataType
	{
		Unknown1 = 0,
		Unknown2 = 1,
		Objects = 2,
		Weather = 3,
		Map = 4,
		Land = 5,
		Grass = 6,
		Sound = 7,
		Unknown3 = 8,
		Unknown4 = 9,
	}

	[Flags]
	public enum RegionDataFlags
	{
		None = 0x0,
		Override = 0x1,
	}

	public struct RDAT_Header
	{
		public RegionDataType Type;
		public RegionDataFlags Flags;
	}

	[Flags]
	public enum ObjectFlags
	{
		None = 0x0,
		ConformToSlope = 0x01,
		PaintVertices	=	0x02,
		SizeVariance =	0x04, // +/-
		X 				=	0x08, // +/-
		Y 				=	0x10, // +/-
		Z 				=	0x20, // +/-
		Tree			=		0x40,
		HugeRock			=	0x80,
	}

	public struct RDOT_Objects
	{
		public uint ObjectFormId;  // FormID of a TREE, STAT or LTEX record.
		public ushort ParentIndex;
		//Unused   byte[2]
		public float Density;
		public byte Clustering;
		public byte MinSlope;
		public byte MaxSlope;
		public ObjectFlags Flags;
		public ushort RadiusWithRespectToParent;
		public ushort Radius;
		//Unknown  byte[4]
		public float MaxHeight;
		public float Sink;
		public float SinkVariance;
		public float SizeVariance;
		public ushort XAngleVariance;
		public ushort YAngleVariance;
		public ushort ZAngleVariance;
		//public Unknown  byte[6]
	}

	public enum MusicType
	{
		Default = 0,
		Public = 1,
		Dungeon = 2
	}

	[Flags]
	public enum RegionSoundFlags
	{
		None = 0x0,
		Pleasant = 0x1,
		Cloudy = 0x2,
		Rainy = 0x4,
		Snowy = 0x8,
	}

	public struct RDSD_SubRecord
	{
		public uint SoundFormId;
		public SoundFlags Flags;
		public uint Chance;
	}

	public struct RDWT_SubRecord
	{
		public uint WeatherFormId;
		public uint Chance;
		public uint GlobFormId;
	}
}
