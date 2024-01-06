using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// TODO: Handle long audio files.
public static class AudioUtils
{
    public static AudioClip CreateAudioClip(string name, PCMAudioBuffer audioBuffer)
    {
        var sampleData = audioBuffer.ToFloatArray();

        var audioClip = AudioClip.Create(name, sampleData.Length, audioBuffer.channelCount, audioBuffer.samplingRate, false);
        audioClip.SetData(sampleData, 0);

        return audioClip;
    }

    public static int SampleFramesToBytes(int sampleFrameCount, int channelCount, int bitDepth)
    {
        return sampleFrameCount * channelCount * (bitDepth / 8);
    }
    public static int BytesToSampleFrames(int byteCount, int channelCount, int bitDepth)
    {
        return byteCount / (bitDepth / 8) / channelCount;
    }
    public static GameObject Play2DAudioStream(MP3StreamReader audioStream)
    {
        GameObject gameObject = new GameObject("tmp2DAudioStream");

        var audioSource = gameObject.AddComponent<AudioSource>();
        //audioSource.clip = CreateStreamingAudioClip("tmpAudioClip", audioStream);
        audioSource.loop = true;

        var audioStreamComponent = gameObject.AddComponent<OneShotAudioStreamComponent>();
        audioStreamComponent.audioStream = audioStream;

        return gameObject;
    }

    public static PCMAudioBuffer ReadAudioFile(string filePath)
    {
        string fileExtension = Path.GetExtension(filePath).ToLower();

        switch (fileExtension)
        {
            case ".wav":
                return ReadWAV(filePath);
            case ".mp3":
                return ReadMP3(filePath);
            default:
                throw new ArgumentOutOfRangeException("filePath", "Tried to read an unsupported audio file format.");
        }
    }

    // TODO: Endianness?
    public static PCMAudioBuffer ReadWAV(string filePath)
    {
        using (var reader = new UnityBinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read)))
        {
            var chunkID = reader.ReadBytes(4);
            if (!StringUtils.Equals(chunkID, "RIFF"))
            {
                throw new Exception("Invalid chunk ID.");
            }

            var chunkSize = reader.ReadUInt32(); // Size of the rest of the chunk after this number.

            var format = reader.ReadBytes(4);
            if (!StringUtils.Equals(format, "WAVE"))
            {
                throw new Exception("Invalid chunk format.");
            }

            var subchunk1ID = reader.ReadBytes(4);
            if (!StringUtils.Equals(subchunk1ID, "fmt "))
            {
                throw new Exception("Invalid subchunk ID.");
            }

            var subchunk1Size = reader.ReadUInt32(); // Size of rest of subchunk.

            var audioFormat = reader.ReadUInt16();
            if (audioFormat != 1) // 1 = PCM
            {
                throw new NotImplementedException("Unsupported audio format.");
            }

            var numChannels = reader.ReadUInt16();
            var samplingRate = reader.ReadUInt32(); // # of samples per second (not including all channels).
            var byteRate = reader.ReadUInt32(); // # of bytes per second (including all channels).
            var blockAlign = reader.ReadUInt16(); // # of bytes for one sample (including all channels).
            var bitsPerSample = reader.ReadUInt16(); // # of bits per sample (not including all channels).

            if (subchunk1Size == 18)
            {
                // Read any extra values.
                var subchunk1ExtraSize = reader.ReadUInt16();
                reader.ReadBytes(subchunk1ExtraSize);
            }

            var subchunk2ID = reader.ReadBytes(4); // "data"
            if (!StringUtils.Equals(subchunk2ID, "data"))
            {
                throw new Exception("Invalid subchunk ID.");
            }

            var subchunk2Size = reader.ReadUInt32(); // Size of rest of subchunk.
            byte[] audioData = reader.ReadBytes((int)subchunk2Size);

            return new PCMAudioBuffer(numChannels, bitsPerSample, (int)samplingRate, audioData);
        }
    }

    // TODO: Handle exceptions
    public static PCMAudioBuffer ReadMP3(string filePath)
    {
        using (MP3StreamReader audioStream = new MP3StreamReader(filePath))
        {
            var audioData = new List<byte>(2 * (int)audioStream.compressedStreamLengthInBytes); // Allocate enough space for a 50% compression ratio.

            int streamBufferSizeInSampleFrames = 16384;
            var streamBuffer = new byte[SampleFramesToBytes(streamBufferSizeInSampleFrames, audioStream.channelCount, audioStream.bitDepth)];

            do
            {
                int sampleFramesRead = audioStream.ReadSampleFrames(streamBuffer, 0, streamBufferSizeInSampleFrames);

                if (sampleFramesRead > 0)
                {
                    int bytesRead = SampleFramesToBytes(sampleFramesRead, audioStream.channelCount, audioStream.bitDepth);

                    audioData.AddRange(new ArrayRange<byte>(streamBuffer, 0, bytesRead));
                }
            } while (!audioStream.isDoneStreaming);

            return new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, audioData.ToArray());
        }
    }

    /// <summary>
    /// Streams audio into a floating point sample buffer.
    /// </summary>
    /// <param name="unityBuffer"></param>
    /// <param name="intermediateBuffer">A PCM sample buffer to act as an intermediary between the raw audio stream and Unity.</param>
    /// <param name="audioStream"></param>
    /// <returns>Returns the number of samples that were read from the stream.</returns>
    public static int FillUnityStreamBuffer(float[] unityBuffer, PCMAudioBuffer intermediateBuffer, MP3StreamReader audioStream)
    {
        if (audioStream.isDoneStreaming)
        {
            // Fill the Unity sample buffer with zeros.
            Array.Clear(unityBuffer, 0, unityBuffer.Length);

            return 0;
        }

        int totalSampleFramesToRead = unityBuffer.Length / audioStream.channelCount;
        int sampleFramesRead = 0;

        while (sampleFramesRead < totalSampleFramesToRead)
        {
            // Read some sample frames.
            int sampleFramesLeftToRead = totalSampleFramesToRead - sampleFramesRead;
            int sampleFramesReturned = audioStream.ReadSampleFrames(intermediateBuffer.data, 0, Math.Min(sampleFramesLeftToRead, intermediateBuffer.SampleFrameCount));

            if (sampleFramesReturned > 0)
            {
                // Convert the read samples to floats copy them to the output buffer.
                intermediateBuffer.ToFloatArray(unityBuffer, sampleFramesRead, sampleFramesReturned);

                sampleFramesRead += sampleFramesReturned;
            }
            else
            {
                // Fill the rest of the Unity sample buffer with zeros.
                int samplesRead = sampleFramesRead * audioStream.channelCount;
                Array.Clear(unityBuffer, samplesRead, unityBuffer.Length - samplesRead);

                break;
            }
        }

        return sampleFramesRead * audioStream.channelCount;
    }

    // Quick hack until Unity bugs are fixed.
    public static void ResampleHack(float[] srcSamples, float[] dstSamples)
    {
        var srcSampleFrameCount = srcSamples.Length / 2;
        var dstSampleFrameCount = dstSamples.Length / 2;

        var lastSrcSampleFrameIndex = srcSampleFrameCount - 1;
        var lastDstSampleFrameIndex = dstSampleFrameCount - 1;

        for (int channelIndex = 0; channelIndex < 2; channelIndex++)
        {
            for (int dstSampleFrameIndex = 0; dstSampleFrameIndex <= lastDstSampleFrameIndex; dstSampleFrameIndex++)
            {
                int dstSmpI = channelIndex + (2 * dstSampleFrameIndex);

                float sample;

                if (dstSampleFrameIndex == 0)
                {
                    sample = srcSamples[channelIndex];
                }
                else if (dstSampleFrameIndex == lastDstSampleFrameIndex)
                {
                    sample = srcSamples[channelIndex + (2 * lastSrcSampleFrameIndex)];
                }
                else
                {
                    float iPercent = (float)dstSampleFrameIndex / lastDstSampleFrameIndex;

                    var srcSampleFrameIF = iPercent * lastSrcSampleFrameIndex;
                    int LSrcSampleFrameI = (int)Math.Floor(srcSampleFrameIF);
                    int RSrcSampleFrameI = LSrcSampleFrameI + 1;
                    float t = srcSampleFrameIF - LSrcSampleFrameI;

                    int srcSmp0I = channelIndex + (2 * LSrcSampleFrameI);
                    int srcSmp1I = channelIndex + (2 * RSrcSampleFrameI);

                    sample = Mathf.Lerp(srcSamples[srcSmp0I], srcSamples[srcSmp1I], t);
                }

                dstSamples[dstSmpI] = sample;
            }
        }
    }

    public static void LowPassHack(float[] samples)
    {
        var sampleFrameCount = samples.Length / 2;

        for (int channelI = 0; channelI < 2; channelI++)
        {
            for (int sampleFrameI = 0; sampleFrameI < sampleFrameCount - 1; sampleFrameI++)
            {
                int sample0I = channelI + (2 * sampleFrameI);
                int sample1I = sample0I + 2;

                samples[sample0I] = (samples[sample0I] + samples[sample1I]) / 2;
            }
        }
    }

    /// <summary>
    /// Create a Unity audio clip for an audio stream.
    /// </summary>
    private static AudioClip CreateStreamingAudioClip(string name, MP3StreamReader audioStream)
    {
        PCMAudioBuffer streamBuffer = new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, 8192);
        int bufferAudioClipSampleFrameCount = audioStream.samplingRate;

        return AudioClip.Create(name, bufferAudioClipSampleFrameCount, audioStream.channelCount, audioStream.samplingRate, true, delegate (float[] samples)
        {
            int samplesReturned = FillUnityStreamBuffer(samples, streamBuffer, audioStream);

            if (audioStream.isOpen && audioStream.isDoneStreaming)
            {
                audioStream.Close();
            }
        });
    }
}