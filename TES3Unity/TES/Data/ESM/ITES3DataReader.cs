using System;

using TES3Unity.ESM;
using TES3Unity.ESM.Records;
using TES3Unity.NIF;

namespace TES3Unity
{
	public interface ITES3DataReader : ITESDataReader, IDisposable
	{
		string DataPath { get; }
		string FolderPath { get; }

		void Close();
		CELLRecord FindExteriorCellRecord(Vector2i cellIndices);
		CELLRecord FindInteriorCellRecord(string cellName);
		CELLRecord FindInteriorCellRecord(Vector2i gridCoords);
		LANDRecord FindLANDRecord(Vector2i cellIndices);
		LTEXRecord FindLTEXRecord(int index);
		bool TryGetRegion(string id4, out REGNRecord region);
		bool TryGetObjectById(string id, out Record rec);
		SCPTRecord FindScript(string name);
		//string GetSound(string soundId);
		void Load();
		//NiFile LoadNif(string filePath);
		//Texture2DInfo LoadTexture(string texturePath);

		bool TryDumpScripts(out SCPTRecord[] records);
	}
}
