using System.Collections.Generic;
using UnityEngine;

// shuffles and repeats
public class MusicPlayer
{
    private List<string> songFilePaths = new List<string>();
    private int currentSongIndex = -1;
    private GameObject currentAudioSourceObj;

    public void AddSong(string songFilePath) => songFilePaths.Add(songFilePath);
    public void AddSongs(string[] songsFilePaths) => songFilePaths.AddRange(songsFilePaths);

    public void Play()
    {
        if (songFilePaths.Count == 0)
        {
            return;
        }

        currentSongIndex = GetNextSongIndex();

        if (currentSongIndex >= 0)
        {
            var audioStream = new MP3StreamReader(songFilePaths[currentSongIndex]);
            currentAudioSourceObj = AudioUtils.Play2DAudioStream(audioStream);
        }
    }

    public void Update()
    {
        if (songFilePaths.Count == 0)
        {
            return;
        }

        if (currentAudioSourceObj == null)
        {
            currentSongIndex = GetNextSongIndex();

            if (currentSongIndex >= 0)
            {
                var audioStream = new MP3StreamReader(songFilePaths[currentSongIndex]);
                currentAudioSourceObj = AudioUtils.Play2DAudioStream(audioStream);
            }
        }
    }

    private int GetNextSongIndex()
    {
        if (songFilePaths.Count == 0)
        {
            return -1;
        }
        else if (songFilePaths.Count == 1)
        {
            return 0;
        }
        else if (currentSongIndex < 0)
        {
            return Random.Range(0, songFilePaths.Count);
        }
        else
        {
            return (currentSongIndex + Random.Range(1, songFilePaths.Count - 1)) % songFilePaths.Count;
        }
    }
}