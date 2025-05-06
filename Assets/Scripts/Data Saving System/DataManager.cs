using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Managers
{
    public class DataManager : MonoBehaviour
    {
        #region Variables
        [SerializeField] private Data _data;
        [SerializeField] private string _fileName = "data.dat";
        [SerializeField] private string _dataPath;

        private static DataManager _instance;
        public static DataManager Instance => _instance;
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

            _dataPath = GetDataPath();

            // LoadSettingsData();
        }

        private void Start()
        {
        }
        #endregion

        #region Methods
        /// <summary>
        /// Saves game data persistently as PlayerPrefs
        /// </summary>
        [ContextMenu("Save file")]
        public void SaveGameData()
        {
            _dataPath = GetDataPath();

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            FileStream file = File.Create(_dataPath);

            binaryFormatter.Serialize(file, _data);

            file.Close();

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads data stored in PlayerPrefs
        /// </summary>
        [ContextMenu("Load save file")]
        public void LoadGameData()
        {
            _dataPath = GetDataPath();

            if (!File.Exists(_dataPath)) return;

            BinaryFormatter binaryFormatter = new BinaryFormatter();

            FileStream file = File.Open(_dataPath, FileMode.Open);

            _data = (Data)binaryFormatter.Deserialize(file);

            file.Close();
        }

        /// <summary>
        /// Deletes the save file
        /// </summary>
        [ContextMenu("Delete save file")]
        public void DeleteGameData()
        {
            _dataPath = GetDataPath();

            File.Delete(_dataPath);
        }


        private string GetDataPath()
        {
            return $"{Application.persistentDataPath}/{_fileName}";
        }

        // public Stat GetStatistic(string key)
        // {
        //     return _data.statistics.FirstOrDefault(stat => stat.key == key);
        // }

        // public Achievement[] GetLockedAchievementsWithStat(Stat statToCheck)
        // {
        //     return _data.achievements.Where(achievement => !achievement.isUnlocked && achievement.statKey == statToCheck.key).ToArray();
        // }

        /// <summary>
        /// Saves options data
        /// </summary>
        public void SaveSettingsPrefs()
        {
            PlayerPrefs.SetFloat("masterVolume", AudioManager.Instance.MasterVolume);
            PlayerPrefs.SetFloat("musicVolume", AudioManager.Instance.MusicVolume);
            PlayerPrefs.SetFloat("sfxVolume", AudioManager.Instance.SFXVolume);

            PlayerPrefs.Save();
        }

        public void SavePrefsField(string key, float value)
        {
            PlayerPrefs.SetFloat(key, value);

            PlayerPrefs.Save();
        }

        /// <summary>
        /// Loads options data
        /// </summary>
        public void LoadSettingsData()
        {
            // AudioManager.Instance.SetMasterVolume(HasKey("masterVolume") ? GetFloat("masterVolume") : 1);
            // AudioManager.Instance.SetMusicVolume(HasKey("musicVolume") ? GetFloat("musicVolume") : 1);
            // AudioManager.Instance.SetEffectsVolume(HasKey("sfxVolume") ? GetFloat("sfxVolume") : 1);
            // AudioManager.Instance.SetUIVolume(HasKey("uiVolume") ? GetFloat("uiVolume") : 1);
        }

        /// <summary>
        /// Returns the float from the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public float GetFloat(string key)
        {
            if (HasKey(key))
            {
                return PlayerPrefs.GetFloat(key);
            }
            else
            {
                Debug.LogWarning($"No audio key \"{key}\" found");

                return 0;
            }
        }

        /// <summary>
        /// Returns the float from the given key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool TryGetFloat(string key, out float value)
        {
            if (HasKey(key))
            {
                value = PlayerPrefs.GetFloat(key);

                return true;
            }
            else
            {
                Debug.LogWarning($"No audio key \"{key}\" found");

                value = 0;

                return false;
            }
        }

        /// <summary>
        /// Returns bool from the int saved in given key (0 = false, 1 = true)
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBool(string key)
        {
            return PlayerPrefs.GetInt(key) > 0;
        }

        /// <summary>
        /// Stores the given bool as an int (false = 0, true = 1)
        /// </summary>
        /// <param name="key"></param>
        /// <param name="state"></param>
        public void SetBool(string key, bool state)
        {
            PlayerPrefs.SetInt(key, state ? 1 : 0);
        }

        /// <summary>
        /// Checks if the key is stored in the player prefs
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(key);
        }

        /// <summary>
        /// Erases all data stored
        /// </summary>
        [ContextMenu("Clear All Data")]
        public void ClearAllData()
        {
            PlayerPrefs.DeleteAll();
        }
    }
    #endregion
}
