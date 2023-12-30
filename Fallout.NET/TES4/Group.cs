using Fallout.NET.Core;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Fallout.NET.TES4
{
	/// <summary>
	/// https://github.com/TES5Edit/fopdoc/blob/master/Fallout3/Groups.md
	/// </summary>
	public enum GroupType
	{											//		Label Type
		Top = 0,								//	A record type
		WorldChildren = 1,				// A WRLD record FormID
		InteriorCellBlock = 2,			// A cell block number
		InteriorCellSubBlock = 3,		// A cell sub-block number
		ExteriorCellBlock = 4,        // Cell block grid (Y, X) coordinates, stored as int8 values.
		ExteriorCellSubBlock = 5,     // Cell sub-block grid (Y, X) coordinates, stored as int8 values.
		CellChildren = 6,					// A CELL record FormID
		TopicChildren = 7,				// A DIAL record FormID
		CellPersistentChildren = 8,	// A CELL record FormID
		CellTemporaryChildren = 9,		// A CELL record FormID
		CellVisibleDistantChildren = 10// A CELL record FormID
	}

	public sealed class Group
	{
		uint _parentId;
		string _type;
		uint _groupSize;
		byte[] _label;
		GroupType _groupType;
		ushort _stamp;
		ushort _unknow;
		ushort _version;
		ushort _unknow2;
		Dictionary<GroupType, List<Group>> _subGroups;
		Dictionary<string, List<Record>> _records;

		public uint ParentID => _parentId;

		public string Label
		{
			get; private set;
		}

		public Dictionary<GroupType, List<Group>> SubGroups => _subGroups;

		public Dictionary<string, List<Record>> Records => _records;

		public GroupType Type => _groupType;

		public void Deserialize(BetterReader reader, string name, GameID gameID)
		{
			_type = name;
			_subGroups = new Dictionary<GroupType, List<Group>>();
			_records = new Dictionary<string, List<Record>>();

			var headerSize = 24;

			if (gameID == GameID.Oblivion)
				headerSize = 20;

			_groupSize = reader.ReadUInt32();
			_label = reader.ReadBytes(4);
			_groupType = (GroupType)reader.ReadInt32();

			switch (_groupType)
			{
				case GroupType.Top:
					//if (label[0] >= 32)
					Label = Utils.ToString(_label);
					//UnityEngine.Debug.Log($"Loading {name} {Label}");
					break;
				case GroupType.WorldChildren:
				case GroupType.CellChildren:
				case GroupType.TopicChildren:
				case GroupType.CellPersistentChildren:
				case GroupType.CellTemporaryChildren:
				case GroupType.CellVisibleDistantChildren:
					// Not used by FO3
					_parentId = BitConverter.ToUInt32(_label, 0);
					break;
			}

			_stamp = reader.ReadUInt16();
			_unknow = reader.ReadUInt16();

			if (gameID != GameID.Oblivion)
			{
				_version = reader.ReadUInt16();
				_unknow2 = reader.ReadUInt16();
			}

			if (Label != null)
			{
				Utils.LogBuffer("{0} > {1}", _type, Label);
			}

			var endRead = reader.Position + (_groupSize - headerSize);
			var fname = string.Empty;

			Group group = null;
			Record record = null;

			// Only used for debug helping.
			var logGroupDebugDico = new List<GroupType>();
			var logRecordDebugDico = new List<string>();

			while (reader.Position < endRead)
			{
				fname = reader.ReadString(4);

				if (fname == "GRUP")
				{
					group = new Group();
					group.Deserialize(reader, fname, gameID);

					if (!_subGroups.ContainsKey(group.Type))
					{
						var list = new List<Group>();
						list.Add(group);
						_subGroups.Add(group.Type, list);
					}
					else
						_subGroups[group.Type].Add(group);

					if (!logGroupDebugDico.Contains(group._groupType))
					{
						logGroupDebugDico.Add(group._groupType);
						Utils.LogBuffer("\t# SubGroup: {0}", (GroupType)group._groupType);
					}
				}
				else
				{
					record = Record.GetRecord(fname);
					record.Deserialize(reader, fname, gameID);

					if (!_records.ContainsKey(record.Type))
					{
						var list = new List<Record>();
						list.Add(record);
						_records.Add(record.Type, list);
					}
					else
						_records[record.Type].Add(record);

					if (!logRecordDebugDico.Contains(record.Type))
					{
						logRecordDebugDico.Add(record.Type);
						Utils.LogBuffer("\t- Record: {0}", record.Type);
					}
				}
			}
		}
	}
}
