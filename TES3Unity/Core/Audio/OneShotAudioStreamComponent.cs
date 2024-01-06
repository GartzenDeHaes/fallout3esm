using UnityEngine;

public class OneShotAudioStreamComponent : MonoBehaviour
{
    public MP3StreamReader audioStream;
    private PCMAudioBuffer streamBuffer;
    private int UnitySampleRate = -1;

    private void Start()
    {
        streamBuffer = new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, 8192);
        UnitySampleRate = AudioSettings.outputSampleRate;
    }

    private void Update()
    {
        if (audioStream.isDoneStreaming)
        {
            Destroy(gameObject);
            enabled = false;
        }
    }

    private void OnAudioFilterRead(float[] samples, int channelCount)
    {
        if (UnitySampleRate > 0)
        {
            int lowSRSampleCount = (int)((44100.0f / UnitySampleRate) * samples.Length);
            var lowSRSamples = new float[lowSRSampleCount];

            int samplesReturned = AudioUtils.FillUnityStreamBuffer(lowSRSamples, streamBuffer, audioStream);
            AudioUtils.ResampleHack(lowSRSamples, samples);
            //AudioUtils.LowPassHack(samples);

            if (audioStream.isOpen && audioStream.isDoneStreaming)
            {
                audioStream.Close();
            }
        }
    }
}