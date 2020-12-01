﻿using System.Collections;
using System.Collections.Generic;
using Assets.AToonWorld.Scripts.UI;
using Assets.AToonWorld.Scripts.Utils;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

namespace Assets.AToonWorld.Scripts.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private AudioMixer _audioMixer = null;
        
        void Awake()
        {
            float volumePref = PlayerPrefs.GetFloat("Volume", 1);
            _audioMixer.SetFloat("Volume", volumePref);
            InGameUIController.PrefabInstance.FadeInMenu();
        }

        private void Start() 
        {
            Events.InterfaceEvents.CursorChangeRequest.Invoke(CursorController.CursorType.Menu);
        }

        void Update()
        {
            if (InputUtils.EnterButton)
                PlayButton();
        }

        #region Buttons

        public void PlayButton()
        {
            InGameUIController.PrefabInstance.FadeTo(UnityScenes.LevelsMenu);
        }

        public void QuitButton()
        {
            InGameUIController.PrefabInstance.FadeToExit();
        }

        #endregion
    }
}
