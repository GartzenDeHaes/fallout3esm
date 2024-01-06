using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords;
using Fallout.NET.TES4.SubRecords.REFR;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Placed NPC
	/// </summary>
	public sealed class ACHRRecord : Record
	{
		public FormID NAME_BaseNpcFormId;
		public FormID XEZN_EncounterZone;
		public float XPRD_IdleTime;
		public FormID INAM_IdleFormId;
		public ScriptSubRecordCollection PatrolScript;
		public FormID TNAM_TopicDIAL_FormID;
		public int XLCM_LevelModifier;
		public FormID XMRC_MerchanContainer_RefrFormId;
		//public int XCNT_Count;
		public float XRDS_Radius;
		//public float XHLP_Health;
		/// <summary>FormID of a REFR, ACRE, ACHR, PGRE or PMIS record.</summary>
		public FormID XLKR_LinkedReference;
		/// <summary>FormID of a LIGH or REGN record.</summary>
		//public FormID XEMI_Emittance;
		/// <summary>FormID of a REFR record.</summary>
		//public FormID XMBR_MultiBoundRecord;
		public REFR_XESPSubRecord XESP_EnableParent;
		public float XSCL_Scale;
		public PosAndRotSubRecord DATA_PosRot;
		public bool XIBS_IgnoredBySandbox;

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
							NAME_BaseNpcFormId = new FormID();
							NAME_BaseNpcFormId.Deserialize(stream, name);
							break;
						case "XEZN":
							XEZN_EncounterZone = new FormID();
							XEZN_EncounterZone.Deserialize(stream, name);
							break;
						case "XPRD":
							var datasize = stream.ReadUInt16();
							XPRD_IdleTime = stream.ReadSingle();
							break;
						case "INAM":
							INAM_IdleFormId = new FormID();
							INAM_IdleFormId.Deserialize(stream, name);
							break;
						case "SCHR":
							PatrolScript = new();
							PatrolScript.SCHR_ScriptData.Deserialize(stream, name);
							break;
						case "TNAM":
							TNAM_TopicDIAL_FormID = new FormID();
							TNAM_TopicDIAL_FormID.Deserialize(stream, name);
							break;
						case "XMRC":
							XMRC_MerchanContainer_RefrFormId = new FormID();
							XMRC_MerchanContainer_RefrFormId.Deserialize(stream, name);
							break;
						case "XLCM":
							datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 4);
							XLCM_LevelModifier = stream.ReadInt32();
							break;
						//case "XCNT":
						//	datasize = stream.ReadUInt16();
						//	UnityEngine.Debug.Assert(datasize == 4);
						//	XCNT_Count = stream.ReadInt32();
						//	break;
						case "XRDS":
							datasize = stream.ReadUInt16();
							UnityEngine.Debug.Assert(datasize == 4);
							XRDS_Radius = stream.ReadInt32();
							break;
						//case "XHLP":
						//	datasize = stream.ReadUInt16();
						//	UnityEngine.Debug.Assert(datasize == 4);
						//	XHLP_Health = stream.ReadSingle();
						//	break;
						case "XLKR":
							XLKR_LinkedReference = new FormID();
							XLKR_LinkedReference.Deserialize(stream, name);
							break;
						//case "XEMI":
						//	XEMI_Emittance = new FormID();
						//	XEMI_Emittance.Deserialize(stream, name);
						//	break;
						//case "XMBR":
						//	XMBR_MultiBoundRecord = new FormID();
						//	XMBR_MultiBoundRecord.Deserialize(stream, name);
						//	break;
						case "DATA":
							DATA_PosRot = new PosAndRotSubRecord();
							DATA_PosRot.Deserialize(stream, name);
							break;
						case "XESP":
							XESP_EnableParent = new();
							XESP_EnableParent.Deserialize(stream, name);
							break;
						case "XSCL":
							datasize = stream.ReadUInt16();
							XSCL_Scale = stream.ReadSingle();
							break;
						case "XIBS":
							datasize = stream.ReadUInt16();
							stream.ReadBytes(datasize);
							XIBS_IgnoredBySandbox = true;
							break;
						case "XRGD": // ragdoll
						case "XRGB": // ragdoll biped data
						case "XPPA": // patrol script marker (null)
							stream.ReadBytes(stream.ReadUInt16());
							break;
						default:
							UnityEngine.Debug.Log($"Unknown {Type} sub record {name}");
							var rest = stream.ReadUInt16();
							stream.ReadBytes(rest);
							break;
					}
				}
			}
		}
	}
}