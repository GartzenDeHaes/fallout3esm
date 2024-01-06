using System;
using UnityEngine;

/// <summary>
/// Pulse-code modulation (uncompressed samples) audio buffer
/// </summary>
public struct PCMAudioBuffer
{
    public int channelCount;
    public int bitDepth; // bits per sample
    public int samplingRate; // sample frames per second
    public byte[] data; // sample data (channels are interleaved)

    public int BytesPerSample => bitDepth / 8;
    public int BytesPerSampleFrame => channelCount * BytesPerSample;
    public int SampleFrameCount => SampleCount / channelCount;
    public int SampleCount => data.Length / BytesPerSample;

    public PCMAudioBuffer(int channelCount, int bitDepth, int samplingRate, int sampleFrameCount)
    {
        this.channelCount = channelCount;
        this.bitDepth = bitDepth;
        this.samplingRate = samplingRate;
        data = null; // Finish assigning values to all members so that properties can be used.

        data = new byte[sampleFrameCount * BytesPerSampleFrame];
    }

    public PCMAudioBuffer(int channelCount, int bitDepth, int samplingRate, byte[] data)
    {
        this.channelCount = channelCount;
        this.bitDepth = bitDepth;
        this.samplingRate = samplingRate;
        this.data = data;

        Debug.Assert((data.Length % BytesPerSampleFrame) == 0);
    }

    public float[] ToFloatArray()
    {
        float[] floatArray = new float[SampleCount];
        ToFloatArray(floatArray, 0, SampleFrameCount);

        return floatArray;
    }

    // TODO: assert numSampleFrames valid
    public void ToFloatArray(float[] floatArray, int offsetInSampleFrames, int numSampleFrames)
    {
        int offsetInSamples = offsetInSampleFrames * channelCount;
        int numSamples = numSampleFrames * channelCount;

        switch (bitDepth)
        {
            case 8:
                for (int i = 0; i < numSamples; i++)
                {
                    floatArray[offsetInSamples + i] = (float)(unchecked((sbyte)data[i])) / sbyte.MaxValue;
                }

                break;
            case 16:
                for (int i = 0; i < numSamples; i++)
                {
                    floatArray[offsetInSamples + i] = (float)BitConverter.ToInt16(data, 2 * i) / short.MaxValue;
                }

                break;
            case 32:
                for (int i = 0; i < numSamples; i++)
                {
                    floatArray[offsetInSamples + i] = BitConverter.ToSingle(data, 4 * i);
                }

                break;
            case 64:
                for (int i = 0; i < numSamples; i++)
                {
                    floatArray[offsetInSamples + i] = (float)BitConverter.ToDouble(data, 8 * i);
                }

                break;
            default:
                throw new NotImplementedException("Tried to convert a PCMAudioBuffer with an unsupported bit depth (" + bitDepth.ToString() + ") to a float array.");
        }
    }
}
