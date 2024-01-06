using MP3Sharp;

using System;

using UnityEngine;

// TODO: Handle exceptions
// TODO: Change MP3 libraries to properly handle mono/stereo.
public class MP3StreamReader : IDisposable
{
	public readonly int channelCount = 2;
	public readonly int bitDepth = 16; // bits per sample
	public readonly int samplingRate; // sample frames per second
	public readonly long compressedStreamLengthInBytes;

	public bool isDoneStreaming
	{
		get
		{
			return !isOpen || audioStream.IsEOF;
		}
	}
	public bool isOpen
	{
		get
		{
			return audioStream != null;
		}
	}

	public MP3StreamReader(string filePath)
	{
		audioStream = new MP3Stream(filePath);
		samplingRate = audioStream.Frequency;
		compressedStreamLengthInBytes = audioStream.Length;
	}

	public void Close()
	{
		Debug.Assert(isOpen);

		audioStream.Close();
		audioStream = null;
	}
	public void Dispose()
	{
		if (isOpen)
		{
			Close();
		}
	}

	// Returns how many sample frames were actually read.
	public int ReadSampleFrames(byte[] buffer, int offsetInSampleFrames, int sampleFrameCount)
	{
		Debug.Assert(isOpen);

		int offsetInBytes = AudioUtils.SampleFramesToBytes(offsetInSampleFrames, channelCount, bitDepth);
		int requestedByteCount = AudioUtils.SampleFramesToBytes(sampleFrameCount, channelCount, bitDepth);

		int bytesRead = 0;
		int bytesReturned;

		do
		{
			bytesReturned = audioStream.Read(buffer, offsetInBytes + bytesRead, requestedByteCount - bytesRead);
			bytesRead += bytesReturned;
		} while (bytesReturned > 0);

		Debug.Assert((bytesRead % AudioUtils.SampleFramesToBytes(1, channelCount, bitDepth)) == 0);

		//// Stereoize audio and fix MP3Sharp's strange behavior.
		//if (audioStream.ChannelCount == 1)
		//{
		//	int iEnd = offsetInBytes + bytesRead;

		//	for (int i = offsetInBytes; i < iEnd; i += 4)
		//	{
		//		buffer[i + 2] = buffer[i];
		//		buffer[i + 3] = buffer[i + 1];
		//	}
		//}

		int sampleFramesRead = AudioUtils.BytesToSampleFrames(bytesRead, channelCount, bitDepth);
		return sampleFramesRead;
	}

	private MP3Stream audioStream;
}
