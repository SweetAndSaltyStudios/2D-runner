using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class Sound
{
    protected AudioSource audioSource;
    public string clipName;
    public AudioClip audioClip;
    public AudioMixerGroup audioMixerGroup;

    [Range(0, 1)]
    public float volume;
    [Range(0.9f, 1.1f)]
    public float pitch;

    protected bool playOnAwake = false;

    public virtual void SetAudioSource(AudioSource audioSource)
    {
        this.audioSource = audioSource;
        audioSource.clip = audioClip;
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        audioSource.volume = volume;
        audioSource.pitch = pitch;
        audioSource.playOnAwake = playOnAwake;
    }
}

[Serializable]
public class MusicTrack : Sound
{
    public bool loop = false;

    public override void SetAudioSource(AudioSource audioSource)
    {
        base.SetAudioSource(audioSource);
        audioSource.loop = loop;
    }

    public void PlayTrack()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }

    public void StopTrack()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}

[Serializable]
public class Sfx : Sound
{
    public override void SetAudioSource(AudioSource audioSource)
    {
        base.SetAudioSource(audioSource);
    }

    public void PlayUISfx()
    {
        if (!audioSource.isPlaying)
            audioSource.Play();
    }
}

public class MusicPlayer : Singelton<MusicPlayer>
{
    public MusicTrack[] MusicTracks;

    private void Start()
    {
        CreateMusicTrackAudioSources();
    }

    private void CreateMusicTrackAudioSources()
    {
        for (int i = 0; i < MusicTracks.Length; i++)
        {
            GameObject go = new GameObject("Music_" + i + "_" + MusicTracks[i].clipName);
            go.transform.SetParent(transform);
            MusicTracks[i].SetAudioSource(go.AddComponent<AudioSource>());
        }
    }

    public void PlayMusicTrack(string trackName)
    {
        for (int i = 0; i < MusicTracks.Length; i++)
        {
            if (MusicTracks[i].clipName == trackName)
            {
                MusicTracks[i].PlayTrack();
                return;
            }
        }
    }

    public void StopMusicTrack(string trackName)
    {
        for (int i = 0; i < MusicTracks.Length; i++)
        {
            if (MusicTracks[i].clipName == trackName)
            {
                MusicTracks[i].StopTrack();
                return;
            }
        }
    }

    public void PlaySfx(AudioSource audioSource, float minRandomPitch = 1, float maxRandomPitch = 1)
    {
        if (!audioSource.isPlaying)
        {
            float randomPitch = UnityEngine.Random.Range(minRandomPitch, maxRandomPitch);

            audioSource.pitch = randomPitch;
            audioSource.PlayOneShot(audioSource.clip);
        }
    }

    public void PlaySfx(string uiSoundName)
    {
        for (int i = 0; i < SoundLibrary.Instance.UiSoundEffects.Length; i++)
        {
            if (SoundLibrary.Instance.UiSoundEffects[i].clipName == uiSoundName)
            {
                SoundLibrary.Instance.UiSoundEffects[i].PlayUISfx();
                return;
            }
        }
    }
}

public class SoundLibrary : Singelton<SoundLibrary>
{
    public Sfx[] SoundEffects;
    public Sfx[] UiSoundEffects;

    private void Start()
    {
        CreateUIEffectAudioSources();
    }

    private void CreateUIEffectAudioSources()
    {
        for (int i = 0; i < UiSoundEffects.Length; i++)
        {
            GameObject go = new GameObject("Sfx_" + i + "_" + UiSoundEffects[i].clipName);
            go.transform.SetParent(transform);
            UiSoundEffects[i].SetAudioSource(go.AddComponent<AudioSource>());
        }
    }

    public void GetAudioSourceValues(AudioSource audioSource, string clipName)
    {
        foreach (Sfx sfx in SoundEffects)
        {
            if (sfx.clipName == clipName)
            {
                sfx.SetAudioSource(audioSource);
            }
        }
    }
}

public class AudioManager : Singelton<AudioManager>
{
    #region VARIABLES

    public AudioMixer AudioMixer;

    #endregion VARIABLES

    #region PROPERTIES

    public AudioMixerUpdateMode AudioMixerUpdateMode
    {
        get;
        private set;
    }
    public AudioMixerGroup[] AudioMixerGroups
    {
        get;
        private set;
    }

    public bool IsFading
    {
        get;
        private set;
    }

    #endregion PROPERTIES

    #region UNITY_FUNCTIONS

    private void Awake()
    {
        Initialize();
    }

    #endregion UNITY_FUNCTIONS

    #region CUSTOM_FUNCTIONS

    private void Initialize()
    {
        AudioMixerUpdateMode = AudioMixerUpdateMode.UnscaledTime;
        AudioMixerGroups = AudioMixer.FindMatchingGroups(string.Empty);
    }

    private AudioMixerGroup GetChannelOutput(string outputName)
    {
        foreach (AudioMixerGroup output in AudioMixerGroups)
        {
            if (output.name == outputName)
            {
                return output;
            }
        }

        return null;
    }

    private float DecibelToLinearValue(float decibelValue)
    {
        return Mathf.Pow(10.0f, decibelValue / 20.0f);
    }

    private float LinearToDecibelValue(float linearValue)
    {
        return linearValue != 0 ? 20.0f * Mathf.Log10(linearValue) : -80f;
    }

    public float GetChannelValue(string channelName)
    {
        AudioMixer.GetFloat(channelName, out float value);
        return value;
    }

    public void SetLowPassValue(float newValueInHertz)
    {
        AudioMixer.SetFloat("LowPassValue", newValueInHertz);
    }

    public void SetAudioMixerChannelValue(string channelParameterName, float value)
    {
        AudioMixer.SetFloat(channelParameterName, LinearToDecibelValue(value));
    }

    public void FadeChannelVolume(string channelParameterName, float targetVolume, float fadeTime)
    {      
        StartCoroutine(IFadeVolume(channelParameterName, targetVolume, fadeTime));
    }

    #endregion CUSTOM_FUNCTIONS  

    #region COROUTINES

    private IEnumerator IFadeVolume(string channelParameterName, float targetVolume, float fadeDuration)
    {
        yield return new WaitUntil(() => IsFading == false);
        float startChannelVolume = GetChannelValue(channelParameterName);

        float startLerpTime = Time.unscaledTime;
        float timeSinceStarted = Time.unscaledTime - startLerpTime;
        float percentToComplete = timeSinceStarted / fadeDuration;

        targetVolume = LinearToDecibelValue(targetVolume);

        if (targetVolume != startChannelVolume)
        {
            IsFading = true;

            while (true)
            {
                timeSinceStarted = Time.unscaledTime - startLerpTime;
                percentToComplete = timeSinceStarted / fadeDuration;

                float currentVolume = Mathf.Lerp(startChannelVolume, targetVolume, percentToComplete);
                AudioMixer.SetFloat(channelParameterName, currentVolume);

                if (percentToComplete > 1f)
                {
                    IsFading = false;
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    #endregion COROUTINES
}
