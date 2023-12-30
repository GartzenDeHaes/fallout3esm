using System;
using System.Collections.Generic;
using System.Text;

namespace Fallout.NET.TES4.SubRecords.INFO
{
	[Flags]
	public enum INFOFlags
	{
		Goodbye = 0x0001,
		Random = 0x0002,
		SayOnce = 0x0004,
		RunImmediately = 0x0008,
		InfoRefusal = 0x0010,
		RandomEnd = 0x0020,
		RunForRumors = 0x0040,
		SpeechChallenge = 0x0080,
		SayOnceADay = 0x0100,
		AlwaysDarken = 0x0200
	}
}
