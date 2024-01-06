using System.Xml.Linq;
using System;

using Fallout.NET.Core;
using System.Buffers;
using Fallout.NET.TES4.SubRecords;
using System.Collections.Generic;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Script
	/// </summary>
	public class SCPTRecord : Record
	{
		public SCHRSubRecord SCHR_BasicScriptData = new();
		public BytesSubRecord SCDA_CompiledScript = new();
		public STRSubRecord SCTX_Source = new();
		public List<LocalVariable> LocalVariables = new();
		/// <summary>A local variable reference, or FormID of a ACTI, DOOR, STAT, FURN, CREA, SPEL, NPC_, CONT, ARMO, AMMO, MISC, WEAP, IMAD, BOOK, KEYM, ALCH, LIGH, QUST, PLYR, PACK, LVLI, ECZN, EXPL, FLST, IDLM, PMIS, FACT, ACHR, REFR, ACRE, GLOB, DIAL, CELL, SOUN, MGEF, WTHR, CLAS, EFSH, RACE, LVLC, CSTY, WRLD, SCPT, IMGS, MESG, MSTT, MUSC, NOTE, PERK, PGRE, PROJ, LVLN, WATR, ENCH, TREE, TERM, HAIR, EYES, ADDN or NULL record.</summary>
		public List<uint> References = new();

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = ArrayPool<byte>.Shared.Rent((int)size);
			var ssize = reader.ReadBytes(new Span<byte>(bytes, 0, (int)size));
			string name;
			LocalVariable currentLocalVar = null;

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
						case "SCHR":
							SCHR_BasicScriptData.Deserialize(stream, name);
							break;
						case "SCDA":
							SCDA_CompiledScript.Deserialize(stream, name);
							break;
						case "SCTX":
							SCTX_Source.Deserialize(stream, name);
							break;
						case "SLSD":
							var datasize = stream.ReadUInt16();
							currentLocalVar = new();
							currentLocalVar.SLSD_Index = stream.ReadUInt32();
							stream.ReadBytes(12);
							currentLocalVar.SLSD_Flags = (LocalVariableFlags)stream.ReadByte();
							stream.ReadBytes(7);
							break;
						case "SCVR":
							datasize = stream.ReadUInt16();
							currentLocalVar.SCVR_LocalVariableName.Deserialize(stream, name);
							break;
						case "SCRO":
							datasize = stream.ReadUInt16();
							References.Add(stream.ReadUInt32());
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

	public class LocalVariable
	{
		public uint SLSD_Index;
		public LocalVariableFlags SLSD_Flags;
		public STRSubRecord SCVR_LocalVariableName = new();
	}

	[Flags]
	public enum LocalVariableFlags
	{
		None = 0,
		IsLongOrShort = 0x01,
	}
}