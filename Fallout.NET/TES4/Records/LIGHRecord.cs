using System;
using System.Buffers;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	[Flags]
	public enum LightFlags
	{
		Dynamic = 0x00000001,
		CanbeCarried = 0x00000002,
		Negative = 0x00000004,
		Flicker = 0x00000008,
		Unused = 0x00000010,
		OffByDefault = 0x00000020,
		FlickerSlow = 0x00000040,
		Pulse = 0x00000080,
		PulseSlow = 0x00000100,
		SpotLight = 0x00000200,
		SpotShadow = 0x00000400,
	}

	/// <summary>
	/// Light
	/// </summary>
	public class LIGHRecord : Record
	{
		public OBNDSubRecord OBND = new();
		public MODLSubRecord MODL = new();
		public MODLSubRecord MODT;
		public FormID SCRI_ScriptFormId = new();
		public STRSubRecord FULL = new();
		public STRSubRecord ICON = new();
		public STRSubRecord MICO = new();
		public int DATA_Time;
		public uint DATA_Radius;
		public uint DATA_ColorRGBA;
		public LightFlags DATA_Flags;
		public float DATA_FalloutExponent;
		public float DATA_FOV;
		public uint DATA_Value;
		public float DATA_Weight;
		public FloatSubRecord FNAM_VadeValue = new();
		public FormID SNAM_Sound = new();

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
						case "MODL":
							MODL.Deserialize(stream, name);
							break;
						case "MODT":
							MODT = new();
							MODT.Deserialize(stream, name);
							break;
						case "SCRI":
							SCRI_ScriptFormId.Deserialize(stream, name); 
							break;
						case "FULL":
							FULL.Deserialize(stream, name);
							break;
						case "ICON":
							ICON.Deserialize(stream, name);
							break;
						case "MICO":
							MICO.Deserialize(stream, name);
							break;
						case "DATA":
							var datasize = stream.ReadUInt16();
							DATA_Time = stream.ReadInt32();
							DATA_Radius = stream.ReadUInt32();
							DATA_ColorRGBA = stream.ReadUInt32();
							DATA_Flags = (LightFlags)stream.ReadUInt32();
							DATA_FalloutExponent = stream.ReadSingle();
							DATA_FOV = stream.ReadSingle();
							DATA_Value = stream.ReadUInt32();
							DATA_Weight = stream.ReadSingle();
							break;
						case "FNAM":
							FNAM_VadeValue.Deserialize(stream, name);
							break;
						case "SNAM":
							SNAM_Sound.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unknown {Type} sub record {name}");
							var rest = stream.ReadUInt16();
							stream.SkipBytes(rest);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}
}