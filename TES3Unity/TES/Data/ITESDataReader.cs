using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TES3Unity.NIF;

namespace TES3Unity
{
	public interface ITESDataReader
	{
		string GetSound(string soundId);
		NiFile LoadNif(string filePath);
		Texture2DInfo LoadTexture(string texturePath);
	}
}
