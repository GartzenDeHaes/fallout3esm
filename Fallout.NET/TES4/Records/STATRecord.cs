using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.STAT;
using System;

namespace Fallout.NET.TES4.Records
{
	public class STATRecord : Record
	{
        public STRSubRecord EDID { get; private set; }
        public OBNDSubRecord OBND { get; private set; }

        // Model collection
        public STRSubRecord MODL { get; private set; }
        public BytesSubRecord MODB { get; private set; }
        public BytesSubRecord MODT { get; private set; }
        public MODSSubRecord MODS { get; private set; }
        public MODDSubRecord MODD { get; private set; }

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
                        case "OBND":
                            OBND = new OBNDSubRecord();
                            OBND.Deserialize(stream, name);
                            break;
                        case "MODL":
                            MODL = new STRSubRecord();
                            MODL.Deserialize(stream, name);
                            break;
                        case "MODB":
                            MODB = new BytesSubRecord();
                            MODB.Deserialize(stream, name);
                            break;
                        case "MODT":
                            MODT = new BytesSubRecord();
                            MODT.Deserialize(stream, name);
                            break;
                        case "MODS":
                            MODS = new MODSSubRecord();
                            MODS.Deserialize(stream, name);
                            break;
                        case "MODD":
                            MODD = new MODDSubRecord();
                            MODD.Deserialize(stream, name);
                            break;
                        default:
                            var rest = stream.ReadUInt16();
                            stream.ReadBytes(rest);
                            break;
                    }
                }
            }
        }

        public override string ToString()
        {
            return EDID.ToString();
        }
    }
}