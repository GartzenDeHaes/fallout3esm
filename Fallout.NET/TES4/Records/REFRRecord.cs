using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.REFR;

using System;
using System.Collections.Generic;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Placed Object
	/// </summary>
	public sealed class REFRRecord : Record
	{
		public STRSubRecord EDID;
		public FormID NAME;
		public REFR_XLOCSubRecord XLOC;
		public FormID XOWN;
		public FormID XLKR;
		public PosAndRotSubRecord DATA;
		public FloatSubRecord XSCL;
		public REFR_XTELSubRecord XTEL;
		public REFR_XNDPSubRecord XNDP;
		public FormID XLTW;
		public FloatSubRecord XRDS;
		public FormID XEMI;
		public List<REFR_XPWRSubRecord> XPWR = new List<REFR_XPWRSubRecord>();
		public FloatSubRecord XPRD;
		public FormID INAM;
		public SCHRSubRecord SCHR;
		//public REFR_TNAMSubRecord TNAM { get; private set; }
		//public FormID TNAM { get; private set; }
		public BytesSubRecord TNAM;
		public REFR_XMBOSubRecord XMBO;
		public REFR_XPRMSubRecord XPRM;
		public REFR_XRMRSubRecord XRMR;
		public FormID XLRM;
		public REFR_XACTSubRecord XACT;
		public BytesSubRecord XRGD;
		public FloatSubRecord XHLP;
		public ByteSubRecord XSED;
		public REFR_XPODSubRecord XPOD;
		public REFR_XRDOSubRecord XRDO;
		public ByteSubRecord XAPD;
		public List<REFR_XAPRSubRecord> XAPR = new List<REFR_XAPRSubRecord>();
		public REFR_XESPSubRecord XESP;
		public UInt32SubRecord XLCM;
		public UInt32SubRecord XCNT;
		public REFR_XTRISubRecord XTRI;
		public BytesSubRecord XOCP;
		public FormID XAMT;
		public UInt32SubRecord XAMC;
		public FloatSubRecord XRAD;
		public FormID XTRG;
		public BytesSubRecord XORD;
		public FormID XMBR;
		public BytesSubRecord XCLP;
		public REFR_FNAMSubRecord FNAM;
		public STRSubRecord FULL;
		public BytesSubRecord SCDA;
		public BytesSubRecord SCRO;
		public BytesSubRecord XLOD;
		public BytesSubRecord RCLR;
		public BytesSubRecord XRGB;

		public bool XPPA;
		public bool ONAM;
		public bool XIBS;
		public bool XMBP;
		public bool XMRK;

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
						case "XLOC":
							XLOC = new REFR_XLOCSubRecord();
							XLOC.Deserialize(stream, name);
							break;
						case "XOWN":
							XOWN = new FormID();
							XOWN.Deserialize(stream, name);
							break;
						case "XLKR":
							XLKR = new FormID();
							XLKR.Deserialize(stream, name);
							break;
						case "DATA":
							DATA = new PosAndRotSubRecord();
							DATA.Deserialize(stream, name);
							break;
						case "XSCL":
							XSCL = new FloatSubRecord();
							XSCL.Deserialize(stream, name);
							break;
						case "XTEL":
							XTEL = new REFR_XTELSubRecord();
							XTEL.Deserialize(stream, name);
							break;
						case "XNDP":
							XNDP = new REFR_XNDPSubRecord();
							XNDP.Deserialize(stream, name);
							break;
						case "XLTW":
							XLTW = new FormID();
							XLTW.Deserialize(stream, name);
							break;
						case "XRDS":
							XRDS = new FloatSubRecord();
							XRDS.Deserialize(stream, name);
							break;
						case "XEMI":
							XEMI = new FormID();
							XEMI.Deserialize(stream, name);
							break;
						case "XPWR":
							var xpwr = new REFR_XPWRSubRecord();
							xpwr.Deserialize(stream, name);
							XPWR.Add(xpwr);
							break;
						case "XPRD":
							XPRD = new FloatSubRecord();
							XPRD.Deserialize(stream, name);
							break;
						case "XPPA":
							var xppaSize = stream.ReadUInt16();
							var xppaData = stream.ReadBytes(Convert.ToInt32(xppaSize));
							XPPA = true;
							if (xppaData.Length > 0)
							{
								break;
							}
							break;
						case "INAM":
							INAM = new FormID();
							INAM.Deserialize(stream, name);
							break;
						case "SCHR":
							SCHR = new SCHRSubRecord();
							SCHR.Deserialize(stream, name);
							break;
						case "TNAM":
							TNAM = new BytesSubRecord();
							TNAM.Deserialize(stream, name);
							break;
						case "XMBO":
							XMBO = new REFR_XMBOSubRecord();
							XMBO.Deserialize(stream, name);
							break;
						case "XPRM":
							XPRM = new REFR_XPRMSubRecord();
							XPRM.Deserialize(stream, name);
							break;
						case "XRMR":
							XRMR = new REFR_XRMRSubRecord();
							XRMR.Deserialize(stream, name);
							break;
						case "XLRM":
							XLRM = new FormID();
							XLRM.Deserialize(stream, name);
							break;
						case "XACT":
							XACT = new REFR_XACTSubRecord();
							XACT.Deserialize(stream, name);
							break;
						case "ONAM":
							var onamSize = stream.ReadUInt16();
							var onamData = stream.ReadBytes(Convert.ToInt32(onamSize));
							ONAM = true;
							if (onamData.Length > 0)
							{
								break;
							}
							break;
						case "XRGD":
							XRGD = new BytesSubRecord();
							XRGD.Deserialize(stream, name);
							break;
						case "XHLP":
							XHLP = new FloatSubRecord();
							XHLP.Deserialize(stream, name);
							break;
						case "XSED":
							XSED = new ByteSubRecord();
							XSED.Deserialize(stream, name);
							break;
						case "XPOD":
							XPOD = new REFR_XPODSubRecord();
							XPOD.Deserialize(stream, name);
							break;
						case "XRDO":
							XRDO = new REFR_XRDOSubRecord();
							XRDO.Deserialize(stream, name);
							break;
						case "XAPD":
							XAPD = new ByteSubRecord();
							XAPD.Deserialize(stream, name);
							break;
						case "XAPR":
							var xapr = new REFR_XAPRSubRecord();
							xapr.Deserialize(stream, name);
							XAPR.Add(xapr);

							if (XAPR.Count > 1)
							{
								break;
							}
							break;
						case "XESP":
							XESP = new REFR_XESPSubRecord();
							XESP.Deserialize(stream, name);
							break;
						case "XLCM":
							XLCM = new UInt32SubRecord();
							XLCM.Deserialize(stream, name);
							break;
						case "XCNT":
							XCNT = new UInt32SubRecord();
							XCNT.Deserialize(stream, name);
							break;
						case "XTRI":
							XTRI = new REFR_XTRISubRecord();
							XTRI.Deserialize(stream, name);
							break;
						case "XOCP":
							XOCP = new BytesSubRecord();
							XOCP.Deserialize(stream, name);
							break;
						case "XAMT":
							XAMT = new FormID();
							XAMT.Deserialize(stream, name);
							break;
						case "XAMC":
							XAMC = new UInt32SubRecord();
							XAMC.Deserialize(stream, name);
							break;
						case "XRAD":
							XRAD = new FloatSubRecord();
							XRAD.Deserialize(stream, name);
							break;
						case "XIBS":
							var xibsSize = stream.ReadUInt16();
							var xibsData = stream.ReadBytes(xibsSize);
							XIBS = true;
							break;
						case "XTRG":
							XTRG = new FormID();
							XTRG.Deserialize(stream, name);
							break;
						case "XORD":
							XORD = new BytesSubRecord();
							XORD.Deserialize(stream, name);
							break;
						case "XMBP":
							var xmbpSize = stream.ReadUInt16();
							var xmbpData = stream.ReadBytes(xmbpSize);
							XMBP = true;
							break;
						case "XMBR":
							XMBR = new FormID();
							XMBR.Deserialize(stream, name);
							break;
						case "XCLP":
							XCLP = new BytesSubRecord();
							XCLP.Deserialize(stream, name);
							break;
						case "XMRK":
							var xmrkSize = stream.ReadUInt16();
							var xmrkData = stream.ReadBytes(xmrkSize);
							XMRK = true;
							break;
						case "FNAM":
							FNAM = new REFR_FNAMSubRecord();
							FNAM.Deserialize(stream, name);
							break;
						case "FULL":
							FULL = new STRSubRecord();
							FULL.Deserialize(stream, name);
							break;
						case "SCDA":
							SCDA = new BytesSubRecord();
							SCDA.Deserialize(stream, name);
							break;
						case "SCRO":
							SCRO = new BytesSubRecord();
							SCRO.Deserialize(stream, name);
							break;
						case "XLOD":
							XLOD = new BytesSubRecord();
							XLOD.Deserialize(stream, name);
							break;
						case "RCLR":
							RCLR = new BytesSubRecord();
							RCLR.Deserialize(stream, name);
							break;
						case "XRGB":
							XRGB = new BytesSubRecord();
							XRGB.Deserialize(stream, name);
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