using System.Collections.Generic;
using System.IO;

using UnityEngine;

namespace TES3Unity
{
	/// <summary>
	/// A proof of concept sound manager.
	/// </summary>
	public sealed class SoundManager : MonoBehaviour
	{
		private MusicPlayer m_MusicPlayer = null;

		private static Dictionary<string, AudioClip> AudioClipStore = new Dictionary<string, AudioClip>();

		private void Start()
		{
			m_MusicPlayer = new MusicPlayer();
		}

		private void Update()
		{
			m_MusicPlayer.Update();
		}

		public void Initialize(string dataPath)
		{
			if (!GameSettings.Get().MusicEnabled)
			{
				return;
			}

			var songs = Directory.GetFiles(dataPath + "/Music/Explore");
			if (songs.Length > 0)
			{
				foreach (var songFilePath in songs)
				{
					if (!songFilePath.Contains("Morrowind Title"))
					{
						m_MusicPlayer.AddSong(songFilePath);
					}
				}

				m_MusicPlayer.Play();
			}
		}

		public static AudioClip GetAudioClip(string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				return null;
			}

			if (AudioClipStore.ContainsKey(id))
			{
				return AudioClipStore[id];
			}

			var path = TES3Engine.Inst.DataReader.GetSound(id);
			if (!File.Exists(path))
			{
				return null;
			}

			var pcm = AudioUtils.ReadWAV(path);
			var clip = AudioUtils.CreateAudioClip(id, pcm);

			AudioClipStore.Add(id, clip);

			return clip;
		}
	}
}
