using System.Collections.Generic;

using UnityEngine;

namespace TES3Unity
{
	/// <summary>
	/// Manages loading and instantiation of Morrowind textures.
	/// </summary>
	public class TextureManager
	{
		private static Dictionary<string, Texture2D> TextureStore = new Dictionary<string, Texture2D>();
		private static Dictionary<Color, Texture2D> MaskTextureStore = new Dictionary<Color, Texture2D>();
		private static Dictionary<Texture2D, Texture2D> NormalMapsStore = new Dictionary<Texture2D, Texture2D>();

		private ITESDataReader _dataReader;

		public TextureManager(ITESDataReader reader)
		{
			_dataReader = reader;
		}

		/// <summary>
		/// Create a mast texture used by HDRP material.
		/// </summary>
		/// <param name="r">Metallic</param>
		/// <param name="g">Occlusion</param>
		/// <param name="b">Detail Mask</param>
		/// <param name="a">Smoothness</param>
		/// <returns>A mask texture.</returns>
		public static Texture2D CreateMaskTexture(float r, float g, float b, float a)
		{
			var color = new Color(r, g, b, a);

			if (MaskTextureStore.ContainsKey(color))
			{
				return MaskTextureStore[color];
			}

			var texture = new Texture2D(1, 1);
			texture.SetPixels(new Color[1] { color });
			texture.Apply();

			MaskTextureStore.Add(color, texture);

			return texture;
		}

		public static Texture2D CreateNormalMapTexture(Texture2D source)
		{
			return GenerateNormalMap(source, TES3Engine.NormalMapGeneratorIntensity);
		}

		// https://gamedev.stackexchange.com/questions/106703/create-a-normal-map-using-a-script-unity
		public static Texture2D GenerateNormalMap(Texture2D source, float strength)
		{
			if (NormalMapsStore.ContainsKey(source))
			{
				return NormalMapsStore[source];
			}

			strength = Mathf.Clamp(strength, 0.0F, 100.0f);

			float xLeft;
			float xRight;
			float yUp;
			float yDown;
			float yDelta;
			float xDelta;

			var normalTexture = new Texture2D(source.width, source.height, TextureFormat.RGB24, true);

			for (int y = 0; y < normalTexture.height; y++)
			{
				for (int x = 0; x < normalTexture.width; x++)
				{
					xLeft = source.GetPixel(x - 1, y).grayscale * strength;
					xRight = source.GetPixel(x + 1, y).grayscale * strength;
					yUp = source.GetPixel(x, y - 1).grayscale * strength;
					yDown = source.GetPixel(x, y + 1).grayscale * strength;
					xDelta = ((xLeft - xRight) + 1) * 0.5f;
					yDelta = ((yUp - yDown) + 1) * 0.5f;
					normalTexture.SetPixel(x, y, new Color(xDelta, yDelta, 1.0f, yDelta));
				}
			}

			normalTexture.Apply();

			return normalTexture;
		}

		public Texture2D LoadTexture(string texturePath, bool flipVertically = false)
		{
			if (TextureStore.ContainsKey(texturePath))
			{
				return TextureStore[texturePath];
			}

			var textureInfo = _dataReader.LoadTexture(texturePath);
			var texture = (textureInfo != null) ? textureInfo.ToTexture2D() : new Texture2D(1, 1);

			if (flipVertically)
			{
				TextureUtils.FlipTexture2DVertically(texture);
			}

			TextureStore.Add(texturePath, texture);

			return texture;
		}
	}
}
