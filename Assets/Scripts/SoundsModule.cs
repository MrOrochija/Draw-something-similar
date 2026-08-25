using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundsModule : MonoBehaviour
{
    public AudioClip[] sounds;

    private AudioSource _audioSrc;
    private AudioSource audioSrc 
    {
        get 
        {
            if (_audioSrc == null) _audioSrc = GetComponent<AudioSource>();
            return _audioSrc;
        }
    }

    private float nextSoundTime = 0f;

    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float p1 = 1f, float p2 = 1f)
    {
        if (clip == null) return; 
        audioSrc.pitch = UnityEngine.Random.Range(p1, p2);
        audioSrc.PlayOneShot(clip, volume);
    }

    public void PlaySequentialSound(AudioClip clip, float volume = 1f, float p1 = 0.9f, float p2 = 1.1f)
    {
        if (clip == null) return;
        
        if (Time.time < nextSoundTime && audioSrc.clip == clip) return;

        float randomPitch = UnityEngine.Random.Range(p1, p2);
        audioSrc.pitch = randomPitch;
        audioSrc.clip = clip;
        audioSrc.loop = false;
        audioSrc.volume = volume;
        audioSrc.Play();

        nextSoundTime = Time.time + (clip.length / randomPitch);
    }

    public void PlayLoopSound(AudioClip clip, float volume = 1f, float p1 = 1f, float p2 = 1f)
    {
        if (clip == null) return;
        if (audioSrc.isPlaying && audioSrc.clip == clip && audioSrc.loop) return;

        audioSrc.pitch = UnityEngine.Random.Range(p1, p2);
        audioSrc.clip = clip;
        audioSrc.loop = true;
        audioSrc.volume = volume;
        audioSrc.Play();
    }

    public void StopSound()
    {
        if (audioSrc != null)
        {
            audioSrc.Stop();
            audioSrc.loop = false;
            audioSrc.clip = null;
        }
        
        nextSoundTime = 0f; 
    }
}