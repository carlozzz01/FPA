using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        #region Variables

        [Header("Audio Mixer")]
        [SerializeField] private AudioMixer _audioMixer;

        [Header("Music")]
        [SerializeField] private GameAudio[] _musicList;
        [SerializeField] private AudioSource _musicAudioSource;

        [Header("SFX")]
        [SerializeField] private GameAudio[] _sfxList;
        [SerializeField] private AudioSource _sfxAudioSource;

        [Header("UI")]
        [SerializeField] private GameAudio[] _uiList;
        [SerializeField] private AudioSource _uiAudioSource;


        [Header("Audio Fade Configuration")]
        [SerializeField] private float _fadeTime = 2f;
        [SerializeField] private float _pitchTime = 1f;
        [SerializeField] private float _pitchSlow = 0.06f;

        [Header("Configuration")]
        [SerializeField] private string _masterVolumeKey = "masterVolume";
        [SerializeField] private float _masterVolume = 1;
        [Space(5)]
        [SerializeField] private string _musicVolumeKey = "musicVolume";
        [SerializeField] private float _musicVolume = 1;
        [Space(5)]
        [SerializeField] private string _effectsVolumeKey = "effectsVolume";
        [SerializeField] private float _effectsVolume = 1;
        [Space(5)]
        [SerializeField] private string _uiVolumeKey = "uiVolume";
        [SerializeField] private float _uiVolume = 1;

        private GameAudio _currentMusicClip;
        private Coroutine _fadeCoroutine;
        private Coroutine _pitchCoroutine;
        private static AudioManager _instance;
        public static AudioManager Instance => _instance;
        public float MasterVolume => _masterVolume;
        public float SFXVolume => _effectsVolume;
        public float MusicVolume => _musicVolume;

        #endregion

        #region Unity Events

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;

                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Sets saved volume on mixer. Uses 1 as default volume if no data is saved
        private void Start()
        {
            // DataManager.Instance.LoadSettingsData();

            bool masterVolumeSaved = DataManager.Instance.TryGetFloat(_masterVolumeKey, out float masterVolume);
            SetMasterVolume(masterVolumeSaved ? masterVolume : 1);

            bool musicVolumeSaved = DataManager.Instance.TryGetFloat(_musicVolumeKey, out float musicVolume);
            SetMasterVolume(musicVolumeSaved ? musicVolume : 1);

            bool effectsVolumeSaved = DataManager.Instance.TryGetFloat(_effectsVolumeKey, out float effectsVolume);
            SetMasterVolume(effectsVolumeSaved ? effectsVolume : 1);

            bool uiVolumeSaved = DataManager.Instance.TryGetFloat(_uiVolumeKey, out float uiVolume);
            SetMasterVolume(uiVolumeSaved ? uiVolume : 1);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Plays SFX with given index in the SFX List.
        /// </summary>
        /// <param name="index"></param>
        public void PlaySFX(int index)
        {
            if (index < 0 || index >= _sfxList.Length)
            {
                Debug.LogWarning("SFX index out of bounds");

                return;
            }

            GameAudio sfxAudio = _sfxList[index];

            // PlayOneShot() allows a single AudioSource to play multiple sound effects at the same time
            _sfxAudioSource.PlayOneShot(sfxAudio.clip, sfxAudio.volume * _effectsVolume);
        }

        /// <summary>
        /// Plays SFX with given id in the SFX List.
        /// </summary>
        /// <param name="id"></param>
        public void PlaySFX(string id)
        {
            for (int i = 0; i < _sfxList.Length; i++)
            {
                if (_sfxList[i].id == id)
                {
                    PlaySFX(i);
                    return;
                }
            }

            Debug.LogWarning($"No SFX found with id \"{id}\"");
        }

        /// <summary>
        /// Plays SFX with given id in the SFX List.
        /// </summary>
        /// <param name="id"></param>
        public void PlaySFX(GameAudio sfxAudio)
        {
            _sfxAudioSource.PlayOneShot(sfxAudio.clip, sfxAudio.volume * _effectsVolume);
        }

        /// <summary>
        /// Plays UI with given index in the UI List.
        /// </summary>
        /// <param name="index"></param>
        public void PlayUI(int index)
        {
            if (index < 0 || index >= _uiList.Length)
            {
                Debug.LogWarning("UI index out of bounds");

                return;
            }

            GameAudio uiAudio = _uiList[index];

            // PlayOneShot() allows a single AudioSource to play multiple sound effects at the same time
            _uiAudioSource.PlayOneShot(uiAudio.clip, uiAudio.volume * _uiVolume);
        }

        /// <summary>
        /// Plays UI sound effect with given id in the UI List.
        /// </summary>
        /// <param name="id"></param>
        public void PlayUI(string id)
        {
            for (int i = 0; i < _uiList.Length; i++)
            {
                if (_uiList[i].id == id)
                {
                    PlayUI(i);
                    return;
                }
            }

            Debug.LogWarning($"No UI found with id \"{id}\"");
        }

        /// <summary>
        /// Plays Music with given index in the Music list. Fades in and out of track.
        /// </summary>
        /// <param name="index"></param>
        public void PlayMusic(int index)
        {
            if (index < 0 || index >= _musicList.Length)
            {
                Debug.LogWarning("Music index out of bounds");

                return;
            }

            GameAudio musicAudio = _musicList[index];

            _currentMusicClip = musicAudio;

            if (_musicAudioSource.clip == musicAudio.clip) return;

            if (_musicAudioSource.isPlaying)
            {
                Debug.Log("Switching clips");

                if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

                _fadeCoroutine = StartCoroutine(FadeAndChangeClip(musicAudio));
            }
            else
            {
                Debug.Log("Playing new clip");

                if (_fadeCoroutine != null) StopCoroutine(_fadeCoroutine);

                _fadeCoroutine = StartCoroutine(FadeClip(musicAudio));
            }

        }

        /// <summary>
        /// Plays Music with given id in the Music list.
        /// </summary>
        /// <param name="id"></param>
        public void PlayMusic(string id)
        {
            for (int i = 0; i < _musicList.Length; i++)
            {
                if (_musicList[i].id == id)
                {
                    PlayMusic(i);
                    return;
                }
            }

            Debug.LogWarning($"No Music found with id \"{id}\"");
        }

        /// <summary>
        /// Changes to given clip, fading the volume to smoothly transition between tracks.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        private IEnumerator FadeClip(GameAudio audio)
        {
            // fadeTime is halved. First half is to fade out, second half is to fade in.
            float time = _fadeTime;

            float timer = time;

            _musicAudioSource.clip = audio.clip;

            _musicAudioSource.Play();

            _musicAudioSource.volume = 0f;

            // fade to goal volume
            while (timer > 0)
            {
                // Frame 1: volume = 1 - (2 / 2);       volume = 1 - 1;     volume = 0
                // Frame 2: volume = 1 - (1.98 / 2);    volume = 1 - 0.98;  volume = 0.02;
                _musicAudioSource.volume = (1 - (timer / time)) * audio.volume;

                timer -= Time.deltaTime;

                yield return null;
            }

            // ensure goal volume
            _musicAudioSource.volume = audio.volume;
        }


        /// <summary>
        /// Changes to given clip, fading the volume to smoothly transition between tracks.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns></returns>
        private IEnumerator FadeAndChangeClip(GameAudio audio)
        {
            // fadeTime is halved. First half is to fade out, second half is to fade in.
            float time = _fadeTime / 2f;

            float timer = time;

            // fade current track to silence
            while (timer > 0.0f)
            {
                // Frame 1: volume = (2 / 2);       volume = 1 * goalVolume;
                // Frame 2: volume = (1.98 / 2);    volume = 0.98 * goalVolume;
                _musicAudioSource.volume = (timer / time) * _currentMusicClip.volume;

                timer -= Time.unscaledDeltaTime;

                yield return null;
            }

            // ensure 0 volume
            _musicAudioSource.volume = 0;

            // change clip after silence
            _musicAudioSource.clip = audio.clip;

            // play again. changing clip stops the audiosource.
            _musicAudioSource.Play();

            // reset timer
            timer = time;

            // fade to goal volume
            while (timer > 0)
            {
                // Frame 1: volume = 1 - (2 / 2);       volume = 1 - 1;     volume = 0
                // Frame 2: volume = 1 - (1.98 / 2);    volume = 1 - 0.98;  volume = 0.02;
                _musicAudioSource.volume = (1 - (timer / time)) * audio.volume;

                timer -= Time.unscaledDeltaTime;

                yield return null;
            }

            // ensure goal volume
            _musicAudioSource.volume = audio.volume;
        }

        /// <summary>
        /// Reduces the pitch, slowing down the audio source
        /// </summary>
        public void SlowMusicPitch()
        {
            if (_pitchCoroutine != null) StopCoroutine(_pitchCoroutine);

            _pitchCoroutine = StartCoroutine(SlowMo(true));
        }

        /// <summary>
        /// Restores the pitch, turning the audio source back to normal
        /// </summary>
        public void RestoreMusicPitch()
        {
            if (_pitchCoroutine != null) StopCoroutine(_pitchCoroutine);

            _pitchCoroutine = StartCoroutine(SlowMo(false));
        }

        /// <summary>
        /// Para reducir el pitch de reproducción
        /// </summary>
        /// <param name="slow"></param>
        /// <returns></returns>
        private IEnumerator SlowMo(bool slow)
        {
            float target = slow ? _pitchSlow : 1;
            float initial = _musicAudioSource.pitch;
            float count = 0f;

            while (count < _pitchTime)
            {
                _musicAudioSource.pitch = Mathf.Lerp(initial, target, count / _pitchTime);
                count += Time.deltaTime;
                yield return null;
            }
        }

        public void SetMasterVolume(float volume)
        {
            _masterVolume = volume;

            _audioMixer.SetFloat(_masterVolumeKey, Mathf.Log10(volume) * 20);

            DataManager.Instance.SavePrefsField(_masterVolumeKey, _masterVolume);
        }

        /// <summary>
        /// Sets the Music volume
        /// </summary>
        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;

            _audioMixer.SetFloat(_musicVolumeKey, Mathf.Log10(volume) * 20);

            DataManager.Instance.SavePrefsField(_musicVolumeKey, _musicVolume);
        }

        /// <summary>
        /// Sets the Effects volume
        /// </summary>
        public void SetEffectsVolume(float volume)
        {
            _effectsVolume = volume;

            _audioMixer.SetFloat(_effectsVolumeKey, Mathf.Log10(volume) * 20);

            DataManager.Instance.SavePrefsField(_effectsVolumeKey, _effectsVolume);
        }

        /// <summary>
        /// Sets the Effects volume
        /// </summary>
        public void SetUIVolume(float volume)
        {
            _uiVolume = volume;

            _audioMixer.SetFloat(_uiVolumeKey, Mathf.Log10(volume) * 20);

            DataManager.Instance.SavePrefsField(_uiVolumeKey, _uiVolume);
        }

        #endregion
    }
}

[System.Serializable]
public struct GameAudio
{
    public string id;
    public AudioClip clip;
    [Range(0f, 1f)] public float volume;
}