using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;

namespace Fallout.NET.TES4.Records
{
	public sealed class ACHRRecord : Record
	{
		public STRSubRecord EDID;
		public FormID NAME;
		public FormID XEZN;
		public FormID INAM;
		public FormID TNAM;
		public FormID XMRC;
		public FormID XLKR;
		public FormID XEMI;
		public FormID XMBR;
		public PosAndRotSubRecord DATA;

		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			var bytes = reader.ReadBytes((int)size);
			var name = string.Empty;

			using (var stream = new BetterMemoryReader(bytes))
			{
				var end = stream.Length;

				while (stream.Position < end)
				{
					name = stream.ReadString(4);

					switch (name)
					{
						case "EDID":
							EDID = new STRSubRecord();
							EDID.Deserialize(stream, name);
							break;
						case "NAME":
							NAME = new FormID();
							NAME.Deserialize(stream, name);
							break;
						case "XEZN":
							XEZN = new FormID();
							XEZN.Deserialize(stream, name);
							break;
						case "INAM":
							INAM = new FormID();
							INAM.Deserialize(stream, name);
							break;
						case "TNAM":
							TNAM = new FormID();
							TNAM.Deserialize(stream, name);
							break;
						case "XMRC":
							XMRC = new FormID();
							XMRC.Deserialize(stream, name);
							break;
						case "XLKR":
							XLKR = new FormID();
							XLKR.Deserialize(stream, name);
							break;
						case "XEMI":
							XEMI = new FormID();
							XEMI.Deserialize(stream, name);
							break;
						case "XMBR":
							XMBR = new FormID();
							XMBR.Deserialize(stream, name);
							break;
						case "DATA":
							DATA = new PosAndRotSubRecord();
							DATA.Deserialize(stream, name);
							break;
						default:
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}