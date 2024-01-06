using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fallout.NET.Core;
using Fallout.NET.TES4.Records;

using Portland;

namespace Fallout.NET.TES4.SubRecords
{
	/// <summary>
	/// https://tes5edit.github.io/fopdoc/Fallout3/Records/Subrecords/Script.html
	/// </summary>
	public class ScriptSubRecordCollection
	{
		public SCHRSubRecord SCHR_ScriptData = new();
		public BytesSubRecord SCDA_CompiledScript = new();
		public STRSubRecord SCTX_ScriptSrc = new();
		public List<LocalVariable> LocalVariables = new();
		public List<uint> SCRO_GlobalVars = new();

		public void Deserialize(BetterMemoryReader stream, string name)
		{
			UnityEngine.Debug.Assert(name == "SCHR");
			SCHR_ScriptData.Deserialize(stream, name);

			if (stream.PeekAscii4() == "SCDA")
			{
				name = stream.ReadString(4);
				SCDA_CompiledScript.Deserialize(stream, name);								
			}
			if (stream.PeekAscii4() == "SCTX")
			{
				name = stream.ReadString(4);
				SCTX_ScriptSrc.Deserialize(stream, name);
			}
			while (stream.PeekAscii4() == "SLSD")
			{
				name = stream.ReadString(4);
				var datasize = stream.ReadUInt16();

				LocalVariable localVar = new LocalVariable();
				localVar.SLSD_Index = stream.ReadUInt32();
				stream.ReadBytes(12);
				localVar.SLSD_Flags = (LocalVariableFlags)stream.ReadByte();
				stream.ReadBytes(7);

				name = stream.ReadString(4);
				UnityEngine.Debug.Assert(name == "SCVR");
				datasize = stream.ReadUInt16();
				localVar.SCVR_LocalVariableName.Deserialize(stream, name);

				LocalVariables.Add(localVar);
			}
			while (stream.PeekAscii4() == "SCRO")
			{
				name = stream.ReadString(4);
				var datasize = stream.ReadUInt16();
				SCRO_GlobalVars.Add(stream.ReadUInt32());
			}
		}
	}
}
