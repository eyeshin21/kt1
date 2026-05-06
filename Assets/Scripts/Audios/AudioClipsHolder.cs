using System.Collections.Generic;
using UnityEngine;

public class AudioClipsHolder : MonoBehaviour
{
	[HideInInspector]
	public Dictionary<string, AudioClip> audioPool;

	public List<AudioClip> soundList = new List<AudioClip>();

	private void Start()
	{
		audioPool = new Dictionary<string, AudioClip>();
		for (int i = 0; i < soundList.Count; i++)
		{
			if (soundList[i] != null)
			{
				audioPool.Add(soundList[i].name, soundList[i]);
			}
		}
	}

	public AudioClip TryGetAudioClip(string name)
	{
		if (audioPool.TryGetValue(name, out AudioClip value))
		{
			return value;
		}
		return null;
	}
}
