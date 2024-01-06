using System;
using System.Collections.Generic;
using System.IO;

using Fallout.NET;
using Fallout.NET.TES4;

using Portland;

using TES4Unity;

using UnityEngine;

namespace TES3Unity.ESM
{
	/// <summary>
	/// A record file is a game data file used in the Gamebryo and Creation engines. Record files contain information about in-game items, characters, world spaces, and settings. Game data such as textures, models, sounds, and other assets are found in BSA files rather than record files. Record files come in two forms: master (ESM) files and plugin (ESP) files. Both master and plugin files are found in the Data subfolder of any parent game directory (i.e. where the game's .exe file is found). 
	/// </summary>
	public class Record
	{
		private static List<string> MissingRecordLogs = new List<string>();

		#region Deprecated

		protected bool UseNewTes3SerializeMethod = true;

		public RecordHeader Header;

		public virtual SubRecord CreateUninitializedSubRecord(string subRecordName, uint dataSize)
		{
			return null;
		}

		public void Deserialize(ITesReader reader, in AsciiId4 recordTypeName, GameID gameID)
		{
			Header.Deserialize(reader, recordTypeName);

			if (Header.Compressed)
			{
				var decompSize = (int)reader.ReadUInt32();
				var compressedData = reader.ReadBytes((int)Header.DataSize - 4);

				//Utils.LogBuffer("\t\tCompressed Data {0}", Header.Type);

				var decompressedData = Decompress(compressedData);

				Debug.Assert(decompressedData.Length == decompSize);

				using (var betterReader = new MemoryStream(decompressedData))
				{
					using (var r = new UnityBinaryReader(betterReader))
					{
						DeserializeSubRecords(r, gameID);
					}
				}
			}

			if (gameID != GameID.Marrowind || UseNewTes3SerializeMethod)
			{
				DeserializeSubRecords(reader, gameID);
				return;
			}

			var dataEndPos = reader.BaseStream.Position + Header.DataSize;

			while (reader.BaseStream.Position < dataEndPos)
			{
				var subRecordStartStreamPosition = reader.BaseStream.Position;

				var subRecordHeader = new SubRecordHeader();
				subRecordHeader.Deserialize(reader);

				var subRecord = CreateUninitializedSubRecord(subRecordHeader.name, subRecordHeader.dataSize);

				// Read or skip the record.
				if (subRecord != null)
				{
					subRecord.header = subRecordHeader;

					var subRecordDataStreamPosition = reader.BaseStream.Position;
					subRecord.DeserializeData(reader, subRecordHeader.dataSize);

					if (reader.BaseStream.Position != (subRecordDataStreamPosition + subRecord.header.dataSize))
					{
						throw new FormatException("Failed reading " + subRecord.header.name + " subrecord at offset " + subRecordStartStreamPosition);
					}
				}
				else
				{
					reader.BaseStream.Position += subRecordHeader.dataSize;
				}
			}
		}

		#endregion

		#region New API to deserialize SubRecords

		protected virtual void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			reader.ReadBytes((int)dataSize);
		}

		public void ReadMissingSubRecord(ITesReader reader, string subRecordName, uint dataSize)
		{
			reader.BaseStream.Position += dataSize;

			var log = $"{Header.RecType} have missing subRecord: {subRecordName}";

			if (!MissingRecordLogs.Contains(log))
			{
				MissingRecordLogs.Add(log);

				if (TES3Engine.LogEnabled)
				{
					Debug.Log(log);
				}
			}
		}

		void DeserializeSubRecords(ITesReader reader, GameID gameID)
		{
			var dataEndPos = reader.BaseStream.Position + Header.DataSize;

			while (reader.BaseStream.Position < dataEndPos)
			{
				var subRecordName = reader.ReadASCIIString(4);
				var dataSize = reader.ReadUInt32();

				DeserializeSubRecord(reader, gameID, subRecordName, dataSize);
			}
		}

		#endregion

		static byte[] Decompress(byte[] data)
		{
			using (var outMemoryStream = new MemoryStream())
			using (var outZStream = new ComponentAce.Compression.Libs.zlib.ZOutputStream(outMemoryStream))
			using (var inMemoryStream = new MemoryStream(data))
			{
				CopyStream(inMemoryStream, outZStream);
				outZStream.finish();
				return outMemoryStream.ToArray();
			}
		}

		static void CopyStream(Stream input, Stream output)
		{
			byte[] buffer = new byte[2000];
			int len;
			while ((len = input.Read(buffer, 0, 2000)) > 0)
			{
				output.Write(buffer, 0, len);
			}
			output.Flush();
		}
	}
}