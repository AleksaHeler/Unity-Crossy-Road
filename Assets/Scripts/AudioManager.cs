using UnityEngine.Audio;
using System;
using UnityEngine;

/// <summary>
/// This class is responsible for all audio data and sources in whole game
/// </summary>
public class AudioManager : MonoBehaviour
{
    // Singleton pattern
    private static AudioManager _instance;
    public static AudioManager Instance { get { return _instance; } }

    [Tooltip("Sounds that will be accessed in game to be played")]
    public Sound[] sounds;

    // Keep track if the audio is muted
    private bool muted = false;

    // Start is called before the first frame update
    void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        // Mark this object as dont destroy on load so it persists trough scenes
        DontDestroyOnLoad(gameObject);

        // Copy info from inspector edit list to actual game components (audio sources)
        foreach (Sound s in sounds)
		{
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

	private void Start()
	{
        // Always play music
        Play("Music");
    }

    /// <summary>
    /// Set volume for given audio source. Doesnt matter if it is playing or not
    /// </summary>
    /// <param name="name">Clip name, internal use (not filename)</param>
    /// <param name="volume">Float between 0 and 1</param>
    public void SetVolume(string name, float volume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.volume = volume;
    }

    /// <summary>
    /// Find the given audio source and play it
    /// </summary>
    /// <param name="name">Clip name, internal use (not filename)</param>
    public void Play(string name)
    {
        // Dont play sounds if muted
        if (muted)
            return;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Play();
    }

    /// <summary>
    /// Find the given audio source and stop it (for example music)
    /// </summary>
    /// <param name="name">Clip name, internal use (not filename)</param>
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Stop();
    }

    /// <summary>
    /// Find the given audio source and pause it (for example car sounds)
    /// </summary>
    /// <param name="name">Clip name, internal use (not filename)</param>
    public void Pause(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Pause();
    }

    /// <summary>
    /// Mutes or unmutes the audio manager and all sounds
    /// </summary>
    /// <param name="enabled">Wether the audio is enabled or not</param>
    public void ToggleSound(bool enabled)
	{
        muted = !enabled;

        if (enabled)
        {
            // If the sound was being played continue it
            foreach (Sound s in sounds)
            {
                if(s.isPlaying)
                    s.source.Play();
            }
        }
		else
        {
            // Stop all sounds and keep track if they were being played
            foreach (Sound s in sounds)
            {
                s.isPlaying = s.source.isPlaying;
                s.source.Pause();
            }
        }
    }
}
