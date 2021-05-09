using UnityEngine.Audio;
using UnityEngine;

/// <summary>
/// Class describing a sound entity for our audio manager
/// </summary>
[System.Serializable]
public class Sound
{
	/// <summary>
	/// For internal use only
	/// </summary>
	public string name;

	/// <summary>
	/// Actual sound clip to be played
	/// </summary>
	public AudioClip clip;

	// Parameters that will be passed to audio source object when created
	[Range(0f, 1f)]
	public float volume;
	[Range(0.1f, 3f)]
	public float pitch;
	public bool loop;
	
	/// <summary>
	/// AudioSource component that will be instantiated to play this sound
	/// </summary>
	[HideInInspector]
	public AudioSource source;
	[HideInInspector]
	public bool isPlaying;
}
