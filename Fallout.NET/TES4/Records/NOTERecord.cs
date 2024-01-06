using System;
using System.Buffers;
using System.Collections.Generic;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	public enum NoteType
	{
		Sound = 0,
 		Text = 1,
	 	Image = 2,
		Voice = 3,
	}

	/// <summary>
	/// Note
	/// </summary>
	public sealed class NOTERecord : Record
	{
		public OBNDSubRecord OBND = new();
		public STRSubRecord FULL = new();
		public MODLSubRecord MODL = new();
		public MODLSubRecord MODB;
		public STRSubRecord ICON = new();
		public STRSubRecord MICO = new();
		public FormID YNAM_SoundPickup = new();
		public FormID ZNAM_SoundDrop = new();
		public NoteType DATA_Type;
		public List<uint> ONAM_Quests = new();
		public STRSubRecord XNAM_Texture = new();
		public string TNAM_Text;
		public uint TNAM_DIAL_Topic;
		public FormID SNAM_SoundOrNpcId = new();

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
						case "FULL":
							FULL.Deserialize(stream, name);
							break;
						case "MODL":
							MODL.Deserialize(stream, name);
							break;
						case "MODB":
							MODB = new();
							MODB.Deserialize(stream, name);
							break;
						case "ICON":
							ICON.Deserialize(stream, name);
							break;
						case "MICO":
							MICO.Deserialize(stream, name);
							break;
						case "YNAM":
							YNAM_SoundPickup.Deserialize(stream, name);
							break;
						case "ZNAM":
							ZNAM_SoundDrop.Deserialize(stream, name);
							break;
						case "DATA":
							var datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 1);
							DATA_Type = (NoteType)stream.ReadByte();
							break;
						case "ONAM":
							datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 4);
							ONAM_Quests.Add(stream.ReadUInt32());
							break;
						case "XNAM":
							XNAM_Texture.Deserialize(stream, name);
							break;
						case "TNAM":
							datasize = stream.ReadUInt16();
							if (datasize == 4)
							{
								TNAM_DIAL_Topic = stream.ReadUInt32();
							}
							else
							{
								TNAM_Text = stream.ReadString(datasize);
							}
							break;
						case "SNAM":
							SNAM_SoundOrNpcId.Deserialize(stream, name);
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