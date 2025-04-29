using System;
using UnityEngine;
using static PlayerInputs;

namespace Managers
{
    public class InputController : MonoBehaviour
    {
        [Header("Components")]
        [SerializeField] private GameObject _playerActionsGO;
        [SerializeField] private GameObject _uiActionsGO;
        // [SerializeField] private GameObject _gameActionsGO;
    
        private PlayerInputs _inputs;
        private IPlayerActions _playerActions;
        private IUIActions _uiActions;
        // private IGameActions _gameActions;
    
        private static InputController _instance;
        public static InputController Instance => _instance;
    
        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(this);
            }
    
            _inputs = new PlayerInputs();
        }
    
        private void OnEnable()
        {
            // GameManager.OnGamePaused += PausePlayerInputs;
            // GameManager.OnGameOver += () => PausePlayerInputs(true);
        }
    
        private void OnDisable()
        {
            // GameManager.OnGamePaused -= PausePlayerInputs;
            // GameManager.OnGameOver -= () => PausePlayerInputs(true);
        }
    
        private void Start()
        {
            InitializeInputs();
    
            EnablePlayerInputs(true);
    
            EnableUIInputs(false);
        }
    
        /// <summary>
        /// Initializes and connects the inputs to the scripts that use the inputs interfaces
        /// </summary>
        private void InitializeInputs()
        {
            if (_playerActionsGO != null) _playerActions = _playerActionsGO.GetComponent<IPlayerActions>();
            if (_uiActionsGO != null) _uiActions = _uiActionsGO.GetComponent<IUIActions>();
            // if (_gameActionsGO != null) _gameActions = _gameActionsGO.GetComponent<IGameActions>();
    
            if (_playerActions != null)
            {
                _inputs.Player.AddCallbacks(_playerActions);
            }
    
            if (_uiActions != null)
            {
                _inputs.UI.AddCallbacks(_uiActions);
            }
    
            // if (_gameActions != null)
            // {
                // _inputs.Game.AddCallbacks(_gameActions);
            // }
        }
    
        /// <summary>
        /// Handles the Player input state
        /// </summary>
        /// <param name="state"></param>
        public void EnablePlayerInputs(bool state)
        {
            if (state)
            {
                _inputs.Player.Enable();
            }
            else
            {
                _inputs.Player.Disable();
            }
        }
    
        /// <summary>
        /// Handles the UI input state
        /// </summary>
        /// <param name="state"></param>
        public void EnableUIInputs(bool state)
        {
            if (state)
            {
                _inputs.UI.Enable();
            }
            else
            {
                _inputs.UI.Disable();
            }
        }
    
        /// <summary>
        /// Habdles the Mouse state
        /// </summary>
        /// <param name="state"></param>
        public void EnableMouse(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = state;
        }
    
        /// <summary>
        /// Handles the Player's inputs when pausing the game
        /// </summary>
        /// <param name="obj"></param>
        private void PausePlayerInputs(bool obj)
        {
            EnablePlayerInputs(!obj);
    
            EnableUIInputs(obj);
        }
    }
}
