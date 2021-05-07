using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public Sound[] sounds;
    private bool muted = false;

    // Start is called before the first frame update
    void Awake()
    {
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
        Play("Music");
    }

	// Find the given audio source and play it
	public void Play(string name)
    {
        if (muted)
            return;

        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Play();
    }

    // Find the given audio source and stop it (for example music)
    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
            s.source.Stop();
    }

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