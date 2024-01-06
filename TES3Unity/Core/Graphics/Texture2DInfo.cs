using UnityEngine;

/// <summary>
/// Stores information about a 2D texture.
/// </summary>
public sealed class Texture2DInfo
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public TextureFormat Format { get; private set; }
    public bool HasMipmaps { get; private set; }
    public byte[] RawData { get; private set; }

    public Texture2DInfo(int width, int height, TextureFormat format, bool hasMipmaps, byte[] rawData)
    {
        Width = width;
        Height = height;
        Format = format;
        HasMipmaps = hasMipmaps;
        RawData = rawData;
    }

    /// <summary>
    /// Creates a Unity Texture2D from this Texture2DInfo.
    /// </summary>
    public Texture2D ToTexture2D()
    {
        var texture = new Texture2D(Width, Height, Format, HasMipmaps);

        if (RawData != null)
        {
            texture.LoadRawTextureData(RawData);
            texture.Apply();
        }

        return texture;
    }
}