using System;
using System.Buffers;

using Assets.Fallout.NET.TES4.SubRecords;

using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	[Flags]
	public enum ClassFlags
	{
		None = 0,
		Playable = 0x01,
		Guard = 0x02,
	}

	public sealed class CLASRecord : Record
	{
		public STRSubRecord FULL_Name = new();
		public STRSubRecord DESC = new();
		public STRSubRecord ICON = new();
		public STRSubRecord MICO = new();
		public ActorValue TagSkill1;
		public ActorValue TagSkill2;
		public ActorValue TagSkill3;
		public ActorValue TagSkill4;
		public ClassFlags Flags;
		public MerchantServiceFlags MerchantFlags;
		public SkillType TeachesSkill;
		public byte MaxTrainingLevel;
		public byte ATTR_Strength;
		public byte ATTR_Perception;
		public byte ATTR_Endurance;
		public byte ATTR_Charisma;
		public byte ATTR_Intelligence;
		public byte ATTR_Agility;
		public byte ATTR_Luck;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			//reader.ReadBytes((int)size);

			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));

			//var bytes = reader.ReadBytes((int)size);
			var name = string.Empty;

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
							FULL_Name.Deserialize(stream, name);
							break;
						case "DESC":
							DESC.Deserialize(stream, name);
							break;
						case "ICON":
							ICON.Deserialize(stream, name);
							break;
						case "MICO":
							MICO.Deserialize(stream, name);
							break;
						case "DATA":
							var datalen = stream.ReadUInt16();
							TagSkill1 = (ActorValue)stream.ReadInt32();
							TagSkill2 = (ActorValue)stream.ReadInt32();
							TagSkill3 = (ActorValue)stream.ReadInt32();
							TagSkill4 = (ActorValue)stream.ReadInt32();
							Flags = (ClassFlags)stream.ReadUInt32();
							MerchantFlags = (MerchantServiceFlags)stream.ReadUInt32();
							TeachesSkill = (SkillType)(sbyte)stream.ReadByte();
							MaxTrainingLevel = stream.ReadByte();
							stream.ReadByte();
							stream.ReadByte();
							break;
						case "ATTR":
							datalen = stream.ReadUInt16();

							UnityEngine.Debug.Assert(datalen == 7);
							ATTR_Strength = stream.ReadByte();							
							ATTR_Perception = stream.ReadByte();
							ATTR_Endurance = stream.ReadByte();
							ATTR_Charisma = stream.ReadByte();
							ATTR_Intelligence = stream.ReadByte();
							ATTR_Agility = stream.ReadByte();
							ATTR_Luck = stream.ReadByte();
							break;
						default:
							UnityEngine.Debug.Log($"Unknown CLAS subrecord {name}");
							var rest2 = stream.ReadUInt16();
							stream.ReadBytes(rest2);
							break;
					}
				}
			}

			ArrayPool<byte>.Shared.Return(bytes);
		}
	}
}