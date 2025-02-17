﻿using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.Level;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.UI
{
    public class InGameUIController : Singleton<InGameUIController>
    {
        #region Fields

        private Canvas _inGameCanvas;
        private SceneFaderController _sceneFaderController;
        private PauseMenuController _pauseMenuController;
        private EndLevelMenuController _endLevelMenuController;
        private CollectiblesMenuController _collectiblesMenuController;
        private WorldToUIEffectsController _worldToUIEffectsController;
        private LevelHandler _levelHandler;
        private const float _defaultSpeed = 1.25f;
        private const int _defaultDelay = 1000;
        private bool _isInGame = false;
        public bool _isEndLevelMenu { get; set; } = false;

        [SerializeField] private GameObject _inkSelector = null;
        [SerializeField] private GameObject _inkWheel = null;
        [SerializeField] private GameObject _pauseMenuUI = null;
        [SerializeField] private GameObject _endLevelMenuUI = null;
        [SerializeField] private GameObject _collectibleMenuUI = null;

        #endregion

        // Initialization only the first time
        protected override void Awake()
        {
            base.Awake();

            _inGameCanvas = GetComponentInChildren<Canvas>();
            _sceneFaderController = _inGameCanvas.GetComponentInChildren<SceneFaderController>();
            _pauseMenuController = GetComponent<PauseMenuController>();
            _endLevelMenuController = GetComponent<EndLevelMenuController>();
            _collectiblesMenuController = GetComponent<CollectiblesMenuController>();
            _worldToUIEffectsController = GetComponentInChildren<WorldToUIEffectsController>();
        }

        # region Faders

        // Awaitable fade in inside a level
        public async UniTask FadeIn(float fadingSpeed = _defaultSpeed)
        {
            _sceneFaderController.gameObject.SetActive(true);
            await _sceneFaderController.FadeIn(fadingSpeed);
            _sceneFaderController.gameObject.SetActive(false);
        }

        // Awaitable fade out inside a level
        public async UniTask FadeOut(float fadingSpeed = _defaultSpeed)
        {
            _sceneFaderController.gameObject.SetActive(true);
            await _sceneFaderController.FadeOut(fadingSpeed);
            _sceneFaderController.gameObject.SetActive(false);
        }

        // Awaitable fade blink inside a level
        public async UniTask FadeOutAndIn(float fadeOutSpeed = _defaultSpeed, int delayInMs = _defaultDelay, float fadeInSpeed = _defaultSpeed)
        {
            _sceneFaderController.gameObject.SetActive(true);
            await _sceneFaderController.FadeOut(fadeOutSpeed);
            await UniTask.Delay(delayInMs);
            await _sceneFaderController.FadeIn(fadeInSpeed);
            _sceneFaderController.gameObject.SetActive(false);
        }

        // Setup the UI for the level and do a fade in
        public void FadeInLevel(float fadingSpeed = _defaultSpeed)
        {
            _pauseMenuUI.SetActive(false);
            _endLevelMenuUI.SetActive(false);
            _inkSelector.SetActive(true);
            _inkWheel.SetActive(false);
            _collectibleMenuUI.SetActive(true);
            RefreshValues();
            _sceneFaderController.FadeIn(fadingSpeed).ContinueWith(() =>
                _sceneFaderController.gameObject.SetActive(false)).Forget();
            _isInGame = true;
            _isEndLevelMenu = false;
        }

        // Setup the UI for the menu and do a fade in
        public void FadeInMenu(float fadingSpeed = _defaultSpeed)
        {
            _pauseMenuUI.SetActive(false);
            _endLevelMenuUI.SetActive(false);
            _inkSelector.SetActive(false);
            _inkWheel.SetActive(false);
            _collectibleMenuUI.SetActive(false);
            _sceneFaderController.FadeIn(fadingSpeed).ContinueWith(() =>
                _inGameCanvas.gameObject.SetActive(false)).Forget();
            _isInGame = false;
            _isEndLevelMenu = false;
        }

        // Do a fade out when changing scene
        public void FadeTo(string scene, float fadingSpeed = _defaultSpeed)
        {
            _inGameCanvas.gameObject.SetActive(true);
            _sceneFaderController.gameObject.SetActive(true);
            _sceneFaderController.FadeTo(scene, fadingSpeed).Forget();
        }

        // Do a fade out when exiting the game
        public void FadeToExit(float fadingSpeed = _defaultSpeed)
        {
            _inGameCanvas.gameObject.SetActive(true);
            _sceneFaderController.gameObject.SetActive(true);
            _sceneFaderController.FadeExit(fadingSpeed).Forget();
        }

        #endregion
        
        // Refresh values that depend on a level when a new level is loaded
        private void RefreshValues()
        {
            _levelHandler = FindObjectOfType<LevelHandler>();
            _pauseMenuController.RefreshValues();
            _endLevelMenuController.RefreshValues(_levelHandler);
        }

        public InkWheelController inkWheelController => _inkWheel.GetComponent<InkWheelController>();
        public InkSelectorController InkSelector => _inkSelector.GetComponent<InkSelectorController>();
        public CollectiblesMenuController CollectibleMenu => _collectiblesMenuController;
        public WorldToUIEffectsController WorldToUIEffects => _worldToUIEffectsController;

        public bool CanPause => _isInGame && !_isEndLevelMenu && !_levelHandler.RespawningPlayer;
    }
}
