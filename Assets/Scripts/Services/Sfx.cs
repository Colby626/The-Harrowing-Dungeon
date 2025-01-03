using UnityEngine;

public class Sfx : MonoBehaviour
{
    private AudioSource mAudioSource = null;

    void Awake()
    {
        Services.Sfx = this;
    }

    void Start()
    {
        GameObject go = new GameObject();
        go.transform.SetParent(transform, false);
        go.name = "SfxPlayer";
        mAudioSource = go.AddComponent<AudioSource>();

        // TODO, load these preferences, can store with PlayerPrefs
        mAudioSource.volume = 0.5f;
    }

    public void Play(AudioClip clip)
    {
        if (mAudioSource.mute == false)
        {
            mAudioSource.PlayOneShot(clip);
        }
    }

    public void Mute()
    {
        mAudioSource.mute = true;
    }

    public void Unmute()
    {
        mAudioSource.mute = false;
    }
}
