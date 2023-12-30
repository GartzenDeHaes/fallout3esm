using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.WRLD;
using System;

namespace Fallout.NET.TES4.Records
{
    public class WRLDRecord : Record
    {
        public STRSubRecord EDID { get; private set; }
        public STRSubRecord FULL { get; private set; }
        public FormID WNAM { get; private set; }
        public STRSubRecord SNAM { get; private set; }
        public STRSubRecord ICON { get; private set; }
        public FormID CNAM { get; private set; }
        public FormID NAM2 { get; private set; }
        public FormID NAM3 { get; private set; }
        public FloatSubRecord NAM4 { get; private set; }
        public WRLD_DNAMSubRecord DNAM { get; private set; }
        public WRLD_MNAMSubRecord MNAM { get; private set; }
        public WRLD_ONAMSubRecord ONAM { get; private set; }
        public FormID INAM { get; private set; }
        public WRLD_DATASubRecord DATA { get; private set; }
        public Vector2fSubRecord NAM0 { get; private set; }
        public Vector2fSubRecord NAM9 { get; private set; }
        public STRSubRecord NNAM { get; private set; }
        public STRSubRecord XNAM { get; private set; }
        public UInt32SubRecord XXXX { get; private set; }
        public FormID XEZN { get; private set; }
        public WRLD_PNAMSubRecord PNAM { get; private set; }
        public FormID ZNAM { get; private set; }

        public uint Id => id;

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
                        case "FULL":
                            FULL = new STRSubRecord();
                            FULL.Deserialize(stream, name);
                            break;
                        case "WNAM":
                            WNAM = new FormID();
                            WNAM.Deserialize(stream, name);
                            break;
                        case "CNAM":
                            CNAM = new FormID();
                            CNAM.Deserialize(stream, name);
                            break;
                        case "NAM2":
                            NAM2 = new FormID();
                            NAM2.Deserialize(stream, name);
                            break;
                        case "NAM3":
                            NAM3 = new FormID();
                            NAM3.Deserialize(stream, name);
                            break;
                        case "NAM4":
                            NAM4 = new FloatSubRecord();
                            NAM4.Deserialize(stream, name);
                            break;
                        case "DNAM":
                            DNAM = new WRLD_DNAMSubRecord();
                            DNAM.Deserialize(stream, name);
                            break;
                        case "ICON":
                            ICON = new STRSubRecord();
                            ICON.Deserialize(stream, name);
                            break;
                        case "MNAM":
                            MNAM = new WRLD_MNAMSubRecord();
                            MNAM.Deserialize(stream, name);
                            break;
                        case "ONAM":
                            ONAM = new WRLD_ONAMSubRecord();
                            ONAM.Deserialize(stream, name);
                            break;
                        case "INAM":
                            INAM = new FormID();
                            INAM.Deserialize(stream, name);
                            break;
                        case "DATA":
                            DATA = new WRLD_DATASubRecord();
                            DATA.Deserialize(stream, name);
                            break;
                        case "NAM0":
                            NAM0 = new Vector2fSubRecord();
                            NAM0.Deserialize(stream, name);
                            break;
                        case "NAM9":
                            NAM9 = new Vector2fSubRecord();
                            NAM9.Deserialize(stream, name);
                            break;
                        case "NNAM":
                            NNAM = new STRSubRecord();
                            NNAM.Deserialize(stream, name);
                            break;
                        case "XNAM":
                            XNAM = new STRSubRecord();
                            XNAM.Deserialize(stream, name);
                            break;
                        case "XXXX":
                            //var xxxxSize = stream.ReadUInt16();
                            //var xxxxData = stream.ReadBytes(xxxxSize);
                            //var xxxxDataStr = System.Text.Encoding.ASCII.GetString(xxxxData);
                            XXXX = new UInt32SubRecord();
                            XXXX.Deserialize(stream, name);
                            break;
                        case "OFST":
                            var ofstSize = Convert.ToInt32(stream.ReadUInt16());
                            if (ofstSize == 0)
                            {
                                ofstSize = Convert.ToInt32(XXXX.Value);
                            }
                            var ofstData = stream.ReadBytes(ofstSize);
                            break;
                        case "XEZN":
                            XEZN = new FormID();
                            XEZN.Deserialize(stream, name);
                            break;
                        case "PNAM":
                            PNAM = new WRLD_PNAMSubRecord();
                            PNAM.Deserialize(stream, name);
                            break;
                        case "ZNAM":
                            ZNAM = new FormID();
                            ZNAM.Deserialize(stream, name);
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
