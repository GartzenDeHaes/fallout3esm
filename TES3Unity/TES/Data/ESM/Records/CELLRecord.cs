﻿using System;
using System.Collections.Generic;

using Fallout.NET;

using Portland;

using UnityEngine;

namespace TES3Unity.ESM.Records
{
	#region SubRecords 

	public class CELLDATASubRecord : SubRecord
	{
		public uint flags;
		public int gridX;
		public int gridY;

		public override void DeserializeData(ITesReader reader, uint dataSize)
		{
			flags = reader.ReadUInt32();
			gridX = reader.ReadInt32();
			gridY = reader.ReadInt32();
		}
	}

	public class RGNNSubRecord : NAMESubRecord { }

	public class NAM0SubRecord : UInt32SubRecord { }

	public class NAM5SubRecord : Int32SubRecord { } // map color (COLORREF)

	/// <summary>Water height</summary>
	public class WHGTSubRecord : FLTVSubRecord { }

	public class AMBISubRecord : SubRecord
	{
		public uint ambientColor;
		public uint sunlightColor;
		public uint fogColor;
		public float fogDensity;

		public override void DeserializeData(ITesReader reader, uint dataSize)
		{
			ambientColor = reader.ReadUInt32();
			sunlightColor = reader.ReadUInt32();
			fogColor = reader.ReadUInt32();
			fogDensity = reader.ReadSingle();
		}
	}

	public class RefObjDataGroup
	{
		public class FRMRSubRecord : UInt32SubRecord { }
		public class XSCLSubRecord : FLTVSubRecord { }
		public class DODTSubRecord : SubRecord
		{
			public Vector3 position;
			public Vector3 eulerAngles;

			public override void DeserializeData(ITesReader reader, uint dataSize)
			{
				position = reader.ReadVector3();
				eulerAngles = reader.ReadVector3();
			}
		}

		public class DNAMSubRecord : NAMESubRecord { }

		public class KNAMSubRecord : NAMESubRecord { }

		public class TNAMSubRecord : NAMESubRecord { }

		public class UNAMSubRecord : ByteSubRecord { }

		public class ANAMSubRecord : NAMESubRecord { }

		public class BNAMSubRecord : NAMESubRecord { }

		public class NAM9SubRecord : UInt32SubRecord { }

		public class XSOLSubRecord : NAMESubRecord { }

		public class DATASubRecord : SubRecord
		{
			public Vector3 position;
			public Vector3 eulerAngles;

			public override void DeserializeData(ITesReader reader, uint dataSize)
			{
				position = reader.ReadVector3();
				eulerAngles = reader.ReadVector3();
			}
		}

		public FRMRSubRecord FRMR;
		public NAMESubRecord NAME;
		public XSCLSubRecord XSCL;
		public DODTSubRecord DODT;
		public DNAMSubRecord DNAM;
		public FLTVSubRecord FLTV;
		public KNAMSubRecord KNAM;
		public TNAMSubRecord TNAM;
		public UNAMSubRecord UNAM;
		public ANAMSubRecord ANAM;
		public BNAMSubRecord BNAM;
		public INTVSubRecord INTV;
		public NAM9SubRecord NAM9;
		public XSOLSubRecord XSOL;
		public DATASubRecord DATA;
	}

	#endregion

	// TODO: add support for strange INTV before object data?
	public class CELLRecord : Record
	{
		private bool _isReadingObjectDataGroups = false;

		public NAMESubRecord NAME;
		public CELLDATASubRecord DATA;
		public RGNNSubRecord RGNN;
		public NAM0SubRecord NAM0;
		
		// Exterior Cells

		public NAM5SubRecord NAM5;

		// Interior Cells

		/// <summary>Water height</summary>
		public WHGTSubRecord WHGT;
		public AMBISubRecord AMBI;
		public List<RefObjDataGroup> refObjDataGroups = new List<RefObjDataGroup>();

		public bool isInterior => Utils.ContainsBitFlags(DATA.flags, 0x01);

		public Vector2i gridCoords => new Vector2i(DATA.gridX, DATA.gridY);

		public CELLRecord()
		: base()
		{
			UseNewTes3SerializeMethod = false;
		}

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			throw new NotImplementedException();
		}

		public override SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
		{
			if (!_isReadingObjectDataGroups && subRecordName == "FRMR")
			{
				_isReadingObjectDataGroups = true;
			}

			if (!_isReadingObjectDataGroups)
			{
				switch (subRecordName)
				{
					case "NAME":
						NAME = new NAMESubRecord();
						return NAME;
					case "DATA":
						DATA = new CELLDATASubRecord();
						return DATA;
					case "RGNN":
						RGNN = new RGNNSubRecord();
						return RGNN;
					case "NAM0":
						NAM0 = new NAM0SubRecord();
						return NAM0;
					case "NAM5":
						NAM5 = new NAM5SubRecord();
						return NAM5;
					case "WHGT":
						WHGT = new WHGTSubRecord();
						return WHGT;
					case "AMBI":
						AMBI = new AMBISubRecord();
						return AMBI;
					default:
						return null;
				}
			}
			else
			{
				switch (subRecordName)
				{
					// RefObjDataGroup sub-records
					case "FRMR":
						refObjDataGroups.Add(new RefObjDataGroup());
						ArrayUtils.Last(refObjDataGroups).FRMR = new RefObjDataGroup.FRMRSubRecord();
						return ArrayUtils.Last(refObjDataGroups).FRMR;
					case "NAME":
						ArrayUtils.Last(refObjDataGroups).NAME = new NAMESubRecord();
						return ArrayUtils.Last(refObjDataGroups).NAME;
					case "XSCL":
						ArrayUtils.Last(refObjDataGroups).XSCL = new RefObjDataGroup.XSCLSubRecord();
						return ArrayUtils.Last(refObjDataGroups).XSCL;
					case "DODT":
						ArrayUtils.Last(refObjDataGroups).DODT = new RefObjDataGroup.DODTSubRecord();
						return ArrayUtils.Last(refObjDataGroups).DODT;
					case "DNAM":
						ArrayUtils.Last(refObjDataGroups).DNAM = new RefObjDataGroup.DNAMSubRecord();
						return ArrayUtils.Last(refObjDataGroups).DNAM;
					case "FLTV":
						ArrayUtils.Last(refObjDataGroups).FLTV = new FLTVSubRecord();
						return ArrayUtils.Last(refObjDataGroups).FLTV;
					case "KNAM":
						ArrayUtils.Last(refObjDataGroups).KNAM = new RefObjDataGroup.KNAMSubRecord();
						return ArrayUtils.Last(refObjDataGroups).KNAM;
					case "TNAM":
						ArrayUtils.Last(refObjDataGroups).TNAM = new RefObjDataGroup.TNAMSubRecord();
						return ArrayUtils.Last(refObjDataGroups).TNAM;
					case "UNAM":
						ArrayUtils.Last(refObjDataGroups).UNAM = new RefObjDataGroup.UNAMSubRecord();
						return ArrayUtils.Last(refObjDataGroups).UNAM;
					case "ANAM":
						ArrayUtils.Last(refObjDataGroups).ANAM = new RefObjDataGroup.ANAMSubRecord();
						return ArrayUtils.Last(refObjDataGroups).ANAM;
					case "BNAM":
						ArrayUtils.Last(refObjDataGroups).BNAM = new RefObjDataGroup.BNAMSubRecord();
						return ArrayUtils.Last(refObjDataGroups).BNAM;
					case "INTV":
						ArrayUtils.Last(refObjDataGroups).INTV = new INTVSubRecord();
						return ArrayUtils.Last(refObjDataGroups).INTV;
					case "NAM9":
						ArrayUtils.Last(refObjDataGroups).NAM9 = new RefObjDataGroup.NAM9SubRecord();
						return ArrayUtils.Last(refObjDataGroups).NAM9;
					case "XSOL":
						ArrayUtils.Last(refObjDataGroups).XSOL = new RefObjDataGroup.XSOLSubRecord();
						return ArrayUtils.Last(refObjDataGroups).XSOL;
					case "DATA":
						ArrayUtils.Last(refObjDataGroups).DATA = new RefObjDataGroup.DATASubRecord();
						return ArrayUtils.Last(refObjDataGroups).DATA;
					default:
						return null;
				}
			}
		}
	}
}
