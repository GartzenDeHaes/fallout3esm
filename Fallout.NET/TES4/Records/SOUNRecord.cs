using System.Xml.Linq;
using System;

using Fallout.NET.Core;
using System.Buffers;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	[Flags]
	public enum SoundFlags
	{
		RandomFrequencyShift = 0x00000001,
		PlayAtRandom = 0x00000002,
		EnvironmentIgnored = 0x00000004,
		RandomLocation = 0x00000008,
		Loop = 0x00000010,
		MenuSound = 0x00000020,
		Is2D = 0x00000040,
		Is360LFE = 0x00000080,
		DialogueSound = 0x00000100,
		EnvelopeFast = 0x00000200,
		EnvelopeSlow = 0x00000400,
		Radius2D = 0x00000800,
		MuteWhenSubmerged = 0x00001000,
	}

	public class SOUNRecord : Record
	{
		public OBNDSubRecord OBND = new();
		public STRSubRecord FNAM_FileName = new();
		public byte SNDX_MinAttenuationDistance; // mutiplied by 5
		public byte SNDX_MaxAttenuationDistance; // multipled by 100
		public sbyte SNDX_FrequenceAdjustPercentage;
		public SoundFlags SNDX_Flags;
		public short SNDX_StaticAttenuationCdB;
		public byte SNDX_StopTime;
		public byte SNDX_StartTime;
		public short SNDD_AttenuationPoint1;
		public short SNDD_AttenuationPoint2;
		public short SNDD_AttenuationPoint3;
		public short SNDD_AttenuationPoint4;
		public short SNDD_AttenuationPoint5;
		public short SNDD_ReverbAttenuationControl;
		public int SNDD_Priority;
		public short[] ANAM_AttenuationPoints;
		public short GNAM_ReverbAttenuationControl;
		public int HNAM_Priority;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;

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
						case "OBND":
							OBND.Deserialize(stream, name); 
							break;
						case "FNAM":
							FNAM_FileName.Deserialize(stream, name); 
							break;
						case "SNDD":
						case "SNDX":
							var datasize = stream.ReadUInt16();
							SNDX_MinAttenuationDistance = stream.ReadByte();
							SNDX_MaxAttenuationDistance = stream.ReadByte();
							SNDX_FrequenceAdjustPercentage = (sbyte)stream.ReadByte();
							stream.ReadByte();
							SNDX_Flags = (SoundFlags)stream.ReadUInt32();
							SNDX_StaticAttenuationCdB = stream.ReadInt16();
							SNDX_StopTime = stream.ReadByte();
							SNDX_StartTime = stream.ReadByte();
							if (name == "SNDX")
							{
								break;
							}
							SNDD_AttenuationPoint1 = stream.ReadInt16();
							SNDD_AttenuationPoint2 = stream.ReadInt16();
							SNDD_AttenuationPoint3 = stream.ReadInt16();
							SNDD_AttenuationPoint4 = stream.ReadInt16();
							SNDD_AttenuationPoint5 = stream.ReadInt16();
							SNDD_ReverbAttenuationControl = stream.ReadInt16();
							SNDD_Priority = stream.ReadInt32();
							stream.ReadUInt32();
							stream.ReadUInt32();
							break;
						case "ANAM":
							datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 10);
							ANAM_AttenuationPoints = new short[5];
							ANAM_AttenuationPoints[0] = stream.ReadInt16();
							ANAM_AttenuationPoints[1] = stream.ReadInt16();
							ANAM_AttenuationPoints[2] = stream.ReadInt16();
							ANAM_AttenuationPoints[3] = stream.ReadInt16();
							ANAM_AttenuationPoints[4] = stream.ReadInt16();
							break;
						case "GNAM":
							datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 2);
							GNAM_ReverbAttenuationControl = stream.ReadInt16();
							break;
						case "HNAM":
							datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 4);
							HNAM_Priority = stream.ReadInt32();
							break;
						default:
							UnityEngine.Debug.Log($"Unknown {Type} sub record {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}
}