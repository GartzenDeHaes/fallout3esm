using Fallout.NET.TES4.Records;

using System;
using System.Collections.Generic;
using System.Text;

namespace Fallout.NET.TES4.SubRecords.INFO
{
	public sealed class INFO_ResponseSubRecord
	{
		public INFO_TRDTSubrecord TRDT = new();
		public STRSubRecord NAM1_ResponseText = new();
		public STRSubRecord NAM2_ScriptNotes = new();
		public STRSubRecord NAM3_Edits = new();
		public FormID SNAM_SpeakerAnim = new();
		public FormID LNAM_ListenerAnim = new();
	}
}
