using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

using Fallout.NET;

using TES3Unity.ESM;
using TES3Unity.ESS.Records;

namespace TES3Unity.ESS
{
	/// <summary>
	/// Save file
	/// </summary>
	public class ESSFile
	{
		public Dictionary<string, List<Record>> m_Records;

		public ESSFile(string filePath)
		{
			m_Records = new Dictionary<string, List<Record>>();

			ReadRecords(filePath);
		}

		public void FindStartLocation(out string cellName, out float[] position, out float[] rotation)
		{
			var TES3 = (TES3Record)m_Records["TES3"][0];
			var REFR = (REFRRecord)m_Records["REFR"][0];

			cellName = TES3.CellName;
			position = REFR.Position;
			rotation = REFR.Rotation;
		}

		private void ReadRecords(string filePath)
		{
			using (var reader = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
			{
				string recordName = string.Empty;

				while (reader.BaseStream.Position < reader.BaseStream.Length)
				{
					var recordStartStreamPosition = reader.BaseStream.Position;

					recordName = reader.ReadASCIIString(4);
					var record = CreateRecordOfType(ref recordName);

					//recordName = recordHeader.name;

						// Read or skip the record.
					if (record == null)
					{
						var dataSize = reader.ReadUInt32();
						reader.BaseStream.Position += dataSize;
						continue;
					}

					var recordDataStreamPosition = reader.BaseStream.Position + 12;

					record.Deserialize(reader, recordName, GameID.Marrowind);

					if (reader.BaseStream.Position != (recordDataStreamPosition + record.Header.DataSize))
					{
						throw new FormatException("Failed reading " + recordName + " record at offset " + recordStartStreamPosition + " in " + filePath);
					}

					if (!m_Records.ContainsKey(recordName))
					{
						m_Records.Add(recordName, new List<Record>());
					}

					m_Records[recordName].Add(record);
				}
			}
		}

		private Record CreateRecordOfType(ref string name)
		{
			if (name == "GAME")
			{
				return new GAMERecord();
			}
			else if (name == "TES3")
			{
				return new TES3Record();
			}
			else if (name == "REFR")
			{
				return new REFRRecord();
			}

			return null;
		}
	}
}
