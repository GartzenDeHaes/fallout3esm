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
		public STRSubRecord FULL_MapNameMarker;
		/// <summary>FormID of a TREE, SOUN, ACTI, DOOR, STAT, FURN, CONT, ARMO, AMMO, LVLN, LVLC, MISC, WEAP, BOOK, KEYM, ALCH, LIGH, GRAS, ASPC, IDLM, ARMA, MSTT, NOTE, PWAT, SCOL, TACT, TERM or TXST record.</summary>
		public uint NAME_BaseFormId;
		public REFR_XLOCSubRecord XLOC_LockData;
		/// <summary>Ownership data. FormID of a FACT, ACHR or NPC_ record.</summary>
		public FormID XOWN_OwnerId;
		/// <summary>FormID of a REFR, ACRE, ACHR, PGRE or PMIS record.</summary>
		public FormID XLKR_LinkedReference;
		public PosAndRotSubRecord DATA_PosRot;
		public FloatSubRecord XSCL_Scale;
		public REFR_XTELSubRecord XTEL_TeleportDestination;
		public REFR_XNDPSubRecord XNDP_NavigationDoorLink;
		/// <summary>FormID of a REFR record.</summary>
		public FormID XLTW_LitWater;
		public FloatSubRecord XRDS_Radius;
		/// <summary>FormID of a LIGH or REGN record.</summary>
		public FormID XEMI_Emittance;
		public List<REFR_XPWRSubRecord> XPWR_WaterReflection;
		public FloatSubRecord XPRD_PatrolDataIdleTime;
		/// <summary>Patrol data. FormID of an IDLE record, or null.</summary>
		public FormID INAM_PatrolIdle;
		public ScriptSubRecordCollection SCHR_PatrolScript;
		//public REFR_TNAMSubRecord TNAM { get; private set; }
		//public FormID TNAM { get; private set; }
		public BytesSubRecord TNAM_MapMarkerData;
		public REFR_XMBOSubRecord XMBO_BoundsHalfExts;
		public REFR_XPRMSubRecord XPRM_Primitive;
		public REFR_XRMRSubRecord XRMR_RomeDataHeader;
		/// <summary>FormID of a REFR record.</summary>
		public FormID XLRM_LinkedRoom;
		public REFR_XACTSubRecord XACT_ActionFlag;
		public BytesSubRecord XRGD_RagdollData;
		public FloatSubRecord XHLP_Health;
		public ByteSubRecord XSED_SpeedtreeSeed;
		/// <summary>formid[]. Array of REFR record FormIDs, or null.</summary>
		public REFR_XPODSubRecord XPOD_PortalRooms;
		public REFR_XRDOSubRecord XRDO_RadioData;
		public ByteSubRecord XAPD_ActivateParentsFlags;
		public List<REFR_XAPRSubRecord> XAPR_ActivateParentRef;
		public REFR_XESPSubRecord XESP_EnableParent;
		public UInt32SubRecord XLCM_LevelModifer;
		public UInt32SubRecord XCNT_Count;
		public REFR_XTRISubRecord XTRI_TriggerType;
		public BytesSubRecord XOCP_OcclusionPlaneData;
		public FormID XAMT_AmmoType;
		public UInt32SubRecord XAMC_AmmoCount;
		public FloatSubRecord XRAD_Radiation;
		/// <summary>FormID of a REFR, ACRE, ACHR, PGRE or PMIS record.</summary>
		public FormID XTRG_Target;
		public BytesSubRecord XORD_LinkedOcclusionPlanes;
		public FormID XMBR_MultiBoundReference;
		public BytesSubRecord XCLP_LinkedReferenceColor;
		public REFR_FNAMSubRecord FNAM_MapMarkerFlags;
		public BytesSubRecord XLOD;
		public BytesSubRecord RCLR_LinkedReferenceColor;
		public BytesSubRecord XRGB_RagdollBiped;
		public FormID XEZN_EncounterZone;
		public UInt32SubRecord XRNK_FactionRank;

		public bool XPPA_PatrolScriptMarker;
		public bool ONAM_OpenByDefault;
		public bool XIBS_IgnoredBySandbox;
		public bool XMBP_MBP_PrimitiveMarker;
		public bool XMRK_MapMarkerMarker;

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
							EDID = STRSubRecord.Read(stream, name);
							break;
						case "NAME":
							NAME_BaseFormId = FormID.Read(stream, name);
							break;
						case "XLOC":
							XLOC_LockData = new REFR_XLOCSubRecord();
							XLOC_LockData.Deserialize(stream, name);
							break;
						case "XOWN":
							XOWN_OwnerId = new FormID();
							XOWN_OwnerId.Deserialize(stream, name);
							break;
						case "XLKR":
							XLKR_LinkedReference = new FormID();
							XLKR_LinkedReference.Deserialize(stream, name);
							break;
						case "DATA":
							DATA_PosRot = new PosAndRotSubRecord();
							DATA_PosRot.Deserialize(stream, name);
							break;
						case "XSCL":
							XSCL_Scale = new FloatSubRecord();
							XSCL_Scale.Deserialize(stream, name);
							break;
						case "XTEL":
							XTEL_TeleportDestination = new REFR_XTELSubRecord();
							XTEL_TeleportDestination.Deserialize(stream, name);
							break;
						case "XNDP":
							XNDP_NavigationDoorLink = new REFR_XNDPSubRecord();
							XNDP_NavigationDoorLink.Deserialize(stream, name);
							break;
						case "XLTW":
							XLTW_LitWater = new FormID();
							XLTW_LitWater.Deserialize(stream, name);
							break;
						case "XRDS":
							XRDS_Radius = new FloatSubRecord();
							XRDS_Radius.Deserialize(stream, name);
							break;
						case "XEMI":
							XEMI_Emittance = new FormID();
							XEMI_Emittance.Deserialize(stream, name);
							break;
						case "XPWR":
							var xpwr = new REFR_XPWRSubRecord();
							xpwr.Deserialize(stream, name);
							XPWR_WaterReflection ??= new();
							XPWR_WaterReflection.Add(xpwr);
							break;
						case "XPRD":
							XPRD_PatrolDataIdleTime = new FloatSubRecord();
							XPRD_PatrolDataIdleTime.Deserialize(stream, name);
							break;
						case "XPPA":
							var xppaSize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(xppaSize == 0);
							XPPA_PatrolScriptMarker = true;
							break;
						case "INAM":
							INAM_PatrolIdle = new FormID();
							INAM_PatrolIdle.Deserialize(stream, name);
							break;
						case "SCHR":
							SCHR_PatrolScript = new ScriptSubRecordCollection();
							SCHR_PatrolScript.Deserialize(stream, name);
							break;
						case "TNAM":
							TNAM_MapMarkerData = new BytesSubRecord();
							TNAM_MapMarkerData.Deserialize(stream, name);
							break;
						case "XMBO":
							XMBO_BoundsHalfExts = new REFR_XMBOSubRecord();
							XMBO_BoundsHalfExts.Deserialize(stream, name);
							break;
						case "XPRM":
							XPRM_Primitive = new REFR_XPRMSubRecord();
							XPRM_Primitive.Deserialize(stream, name);
							break;
						case "XRMR":
							XRMR_RomeDataHeader = new REFR_XRMRSubRecord();
							XRMR_RomeDataHeader.Deserialize(stream, name);
							break;
						case "XLRM":
							XLRM_LinkedRoom = new FormID();
							XLRM_LinkedRoom.Deserialize(stream, name);
							break;
						case "XACT":
							XACT_ActionFlag = new REFR_XACTSubRecord();
							XACT_ActionFlag.Deserialize(stream, name);
							break;
						case "ONAM":
							var onamSize = stream.ReadUInt16();
							var onamData = stream.ReadBytes(Convert.ToInt32(onamSize));
							ONAM_OpenByDefault = true;
							if (onamData.Length > 0)
							{
								break;
							}
							break;
						case "XRGD":
							XRGD_RagdollData = new BytesSubRecord();
							XRGD_RagdollData.Deserialize(stream, name);
							break;
						case "XHLP":
							XHLP_Health = new FloatSubRecord();
							XHLP_Health.Deserialize(stream, name);
							break;
						case "XSED":
							XSED_SpeedtreeSeed = new ByteSubRecord();
							XSED_SpeedtreeSeed.Deserialize(stream, name);
							break;
						case "XPOD":
							UnityEngine.Debug.Assert(XPOD_PortalRooms == null);
							XPOD_PortalRooms = new REFR_XPODSubRecord();
							XPOD_PortalRooms.Deserialize(stream, name);
							break;
						case "XRDO":
							XRDO_RadioData = new REFR_XRDOSubRecord();
							XRDO_RadioData.Deserialize(stream, name);
							break;
						case "XAPD":
							XAPD_ActivateParentsFlags = new ByteSubRecord();
							XAPD_ActivateParentsFlags.Deserialize(stream, name);
							break;
						case "XAPR":
							var xapr = new REFR_XAPRSubRecord();
							xapr.Deserialize(stream, name);
							XAPR_ActivateParentRef ??= new();
							XAPR_ActivateParentRef.Add(xapr);
							break;
						case "XESP":
							XESP_EnableParent = new REFR_XESPSubRecord();
							XESP_EnableParent.Deserialize(stream, name);
							break;
						case "XLCM":
							XLCM_LevelModifer = new UInt32SubRecord();
							XLCM_LevelModifer.Deserialize(stream, name);
							break;
						case "XCNT":
							XCNT_Count = new UInt32SubRecord();
							XCNT_Count.Deserialize(stream, name);
							break;
						case "XTRI":
							XTRI_TriggerType = new REFR_XTRISubRecord();
							XTRI_TriggerType.Deserialize(stream, name);
							break;
						case "XOCP":
							XOCP_OcclusionPlaneData = new BytesSubRecord();
							XOCP_OcclusionPlaneData.Deserialize(stream, name);
							break;
						case "XAMT":
							XAMT_AmmoType = new FormID();
							XAMT_AmmoType.Deserialize(stream, name);
							break;
						case "XAMC":
							XAMC_AmmoCount = new UInt32SubRecord();
							XAMC_AmmoCount.Deserialize(stream, name);
							break;
						case "XRAD":
							XRAD_Radiation = new FloatSubRecord();
							XRAD_Radiation.Deserialize(stream, name);
							break;
						case "XIBS":
							var xibsSize = stream.ReadUInt16();
							var xibsData = stream.ReadBytes(xibsSize);
							XIBS_IgnoredBySandbox = true;
							break;
						case "XTRG":
							XTRG_Target = new FormID();
							XTRG_Target.Deserialize(stream, name);
							break;
						case "XORD":
							XORD_LinkedOcclusionPlanes = new BytesSubRecord();
							XORD_LinkedOcclusionPlanes.Deserialize(stream, name);
							break;
						case "XMBP":
							var xmbpSize = stream.ReadUInt16();
							var xmbpData = stream.ReadBytes(xmbpSize);
							XMBP_MBP_PrimitiveMarker = true;
							break;
						case "XMBR":
							XMBR_MultiBoundReference = new FormID();
							XMBR_MultiBoundReference.Deserialize(stream, name);
							break;
						case "XCLP":
							XCLP_LinkedReferenceColor = new BytesSubRecord();
							XCLP_LinkedReferenceColor.Deserialize(stream, name);
							break;
						case "XMRK":
							var xmrkSize = stream.ReadUInt16();
							var xmrkData = stream.ReadBytes(xmrkSize);
							XMRK_MapMarkerMarker = true;
							break;
						case "FNAM":
							FNAM_MapMarkerFlags = new REFR_FNAMSubRecord();
							FNAM_MapMarkerFlags.Deserialize(stream, name);
							break;
						case "FULL":
							FULL_MapNameMarker = new STRSubRecord();
							FULL_MapNameMarker.Deserialize(stream, name);
							break;
						//case "SCDA":
						//	SCDA = new BytesSubRecord();
						//	SCDA.Deserialize(stream, name);
						//	break;
						//case "SCRO":
						//	SCRO = new BytesSubRecord();
						//	SCRO.Deserialize(stream, name);
						//	break;
						case "XLOD":
							XLOD = new BytesSubRecord();
							XLOD.Deserialize(stream, name);
							break;
						case "RCLR":
							RCLR_LinkedReferenceColor = new BytesSubRecord();
							RCLR_LinkedReferenceColor.Deserialize(stream, name);
							break;
						case "XRGB":
							XRGB_RagdollBiped = new BytesSubRecord();
							XRGB_RagdollBiped.Deserialize(stream, name);
							break;
						case "XEZN":
							XEZN_EncounterZone = new();
							XEZN_EncounterZone.Deserialize(stream, name);
							break;
						case "XRNK":
							XRNK_FactionRank = new();
							XRNK_FactionRank.Deserialize(stream, name);
							break;
						default:
							UnityEngine.Debug.Log($"Unhandled {Type} subrec {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}