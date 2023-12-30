using Fallout.NET.TES4.Records;

using System;
using System.Collections.Generic;
using System.Text;

namespace Fallout.NET.TES4.SubRecords.INFO
{
	public sealed class INFO_ResponseSubRecord
	{
		public INFO_TRDTSubrecord TRDT;
		public STRSubRecord NAM1_ResponseText;
		public STRSubRecord NAM2_ScriptNotes;
		public STRSubRecord NAM3_Edits;
		public FormID SNAM_SpeakerAnim;
		public FormID LNAM_ListenerAnim;
	}
}
