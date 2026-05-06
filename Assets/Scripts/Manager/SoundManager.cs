using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using TigerForge;

public class SoundManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource bgMusic;

    [SerializeField]
    private AudioSource sound;

    [SerializeField]
    private AudioSource sound_medium;

    private string bgMusicName;

    private Dictionary<int, float> _startTimes;

    private const float MIN_INTERVAL = 0.1f;

    private bool fadeInning;

    private bool fadeOuting;

    private float fadeInOutTimeMax = 1f;

    private float fadeInOutTime;

    public static SoundManager instance;

    private IEnumerator orderBgm;

    private AudioClipsHolder audioClipHolder;

    private float volume;

    public static void InitInstance(Transform parent)
    {
        if (instance == null)
        {
            GameObject gameObject = GameManager.Instance.InstantiatePrefab("SoundManager");
            instance = gameObject.GetComponent<SoundManager>();
            instance.transform.parent = parent;
        }
    }

    private AudioClipsHolder LoadAudioHolder(string name)
    {
        GameObject gameObject = GameManager.Instance.InstantiatePrefab(name);
        gameObject.transform.SetParent(base.transform);
        return gameObject.GetComponent<AudioClipsHolder>();
    }

    private void Start()
    {
        _startTimes = new Dictionary<int, float>();

        // registry event change ON/OFF music
        EventManager.StartListening(EventVariables.ChangeMusic, this.OnMusicChanged);

        audioClipHolder = LoadAudioHolder("MainSound");
        volume = 0.2f;
    }

    private void Update()
    {
        if (this.fadeOuting)
        {
            this.fadeInning = false;
            this.fadeInOutTime += Time.deltaTime;
            if (this.fadeInOutTime >= this.fadeInOutTimeMax)
            {
                this.fadeOuting = false;
                this.bgMusic.volume = 0f;
                this.bgMusic.Stop();
            }
            else
            {
                this.bgMusic.volume = volume - this.fadeInOutTime / this.fadeInOutTimeMax * volume;
            }
        }
        if (this.fadeInning)
        {
            this.fadeInOutTime += Time.deltaTime;
            if (this.fadeInOutTime >= this.fadeInOutTimeMax)
            {
                this.fadeInning = false;
                this.bgMusic.volume = volume;
            }
            else
            {
                this.bgMusic.volume = this.fadeInOutTime / this.fadeInOutTimeMax * volume;
            }
        }
    }

    private void OnDestroy()
    {
        // Cancel registry event change ON/OFF music
        EventManager.StopListening(EventVariables.ChangeMusic, this.OnMusicChanged);
    }

    private void OnMusicChanged()
    {
        var changeValue = EventManager.GetBool(EventVariables.ChangeMusic);
        if ((bool)changeValue)
        {
            this.ResumeBGMusic();
        }
        else
        {
            this.StopBGMusic();
        }
    }

    public void PlayBGMusic(string name, bool loop = true, bool overrideMode = true)
    {
        if (overrideMode)
        {
            this.StopOrderedBgm();
        }
        bool flag = true;
        if (this.bgMusicName != name)
        {
            this.bgMusic.clip = this.LoadMusicClip(name);
        }
        else if (this.bgMusic.isPlaying)
        {
            flag = false;
        }
        if (this.fadeOuting)
        {
            this.fadeOuting = false;
            this.fadeInOutTime = 0f;
            flag = true;
        }
        this.bgMusicName = name;
        this.bgMusic.loop = loop;
        if (flag && UserConfig.Instance.Music)
        {
            this.bgMusic.Play();
            this.fadeInning = true;
            this.fadeInOutTime = 0f;
            this.bgMusic.volume = 0f;
        }
    }

    private AudioClip LoadMusicClip(string namePath)
    {
        return (AudioClip)Resources.Load("Musics/" + namePath, typeof(AudioClip));
    }

    public void StopBGMusic()
    {
        if (!this.fadeOuting && this.bgMusic.isPlaying)
        {
            this.fadeOuting = true;
            this.fadeInOutTime = 0f;
            this.bgMusic.volume = volume;
        }
    }

    public void ResumeBGMusic()
    {
        this.fadeOuting = false;
        if (this.bgMusicName != null)
        {
            this.PlayBGMusic(this.bgMusicName, this.bgMusic.loop, true);
        }
    }

    public void SetBGVol(float vol)
    {
        if (this.bgMusic != null)
        {
            this.bgMusic.volume = vol;
        }
    }

    public float GetBGVol()
    {
        return (!(this.bgMusic == null)) ? this.bgMusic.volume : 0f;
    }

    public void PlayOrderedBgm(params string[] music)
    {
        this.StopOrderedBgm();
        this.orderBgm = this.DoPlayOrderedBgm(music);
        base.StartCoroutine(this.orderBgm);
    }

    private void StopOrderedBgm()
    {
        if (this.orderBgm != null)
        {
            base.StopCoroutine(this.orderBgm);
            this.orderBgm = null;
        }
    }

    private IEnumerator DoPlayOrderedBgm(params string[] music)
    {
        for (int i = 0; i < music.Length; i++)
        {
            if (i == music.Length - 1)
            {
                PlayBGMusic(music[i], loop: true, overrideMode: false);
                continue;
            }
            PlayBGMusic(music[i], loop: false, overrideMode: false);
            yield return new WaitForSeconds(bgMusic.clip.length);
        }
    }

    public AudioClip GetCatchedAudioClip(string name)
    {
        AudioClip audioClip = this.audioClipHolder.TryGetAudioClip(name);
        if (audioClip == null)
        {
            audioClip = this.audioClipHolder.TryGetAudioClip(name);
        }
        return audioClip;
    }

    public void PauseSoundMedium()
    {
        this.sound_medium.Pause();
    }

    public void UnpauseSoundMedium()
    {
        this.sound_medium.UnPause();
    }

    public void StopSoundMedium()
    {
        this.sound_medium.loop = false;
        this.sound_medium.clip = null;
        this.sound_medium.Stop();
    }

    public void PlaySoundMedium(string name, bool loop = false, float volumeScale = 1f)
    {
        AudioClip audioClip = this.audioClipHolder.TryGetAudioClip(name);
        if (audioClip != null)
        {
            if (UserConfig.Instance.Sound)
            {
                if (!(audioClip == null))
                {
                    this.sound_medium.loop = loop;
                    if (!loop)
                    {
                        this.sound_medium.PlayOneShot(audioClip, volumeScale);
                    }
                    else
                    {
                        this.sound_medium.volume = volumeScale;
                        this.sound_medium.clip = audioClip;
                        this.sound_medium.Play();
                    }
                }
            }
        }
    }

    public void PlaySound(string name, bool loop = false, float delay = 0f, float volumeScale = 1f)
    {
        AudioClip audioClip = this.audioClipHolder.TryGetAudioClip(name);
        if (audioClip == null)
        {
            audioClip = this.audioClipHolder.TryGetAudioClip(name);
        }
        if (audioClip != null)
        {
            this.PlaySound(audioClip, loop, delay, volumeScale);
        }
    }

    void PlaySound(AudioClip clip, bool loop = false, float delay = 0f, float volumeScale = 1f)
    {
        if (UserConfig.Instance.Sound)
        {
            if (!(clip == null))
            {
                this.sound.loop = loop;
                if (delay > 0f)
                {
                    StartCoroutine(PlaySoundDelay(clip, delay, volumeScale));
                }
                else if (this.SetStartTime(clip.GetInstanceID()))
                {
                    if (!loop)
                    {
                        this.sound.PlayOneShot(clip, volumeScale);
                    }
                    else
                    {
                        this.sound.clip = clip;
                        this.sound.Play();
                    }
                }
            }
        }
    }

    IEnumerator PlaySoundDelay(AudioClip clip, float delay = 0f, float volumeScale = 1f)
    {
        yield return new WaitForSecondsRealtime(delay);
        this.PlaySound(clip, false, 0f, volumeScale);
    }

    private bool SetStartTime(int audio)
    {
        if (!this._startTimes.ContainsKey(audio))
        {
            this._startTimes[audio] = 0f;
        }
        if (Time.time - this._startTimes[audio] > 0.1f)
        {
            this._startTimes[audio] = Time.time;
            return true;
        }
        return false;
    }
}
