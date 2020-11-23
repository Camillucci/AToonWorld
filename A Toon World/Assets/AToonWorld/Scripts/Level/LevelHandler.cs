﻿using Assets.AToonWorld.Scripts.Camera;
using Assets.AToonWorld.Scripts.Player;
using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Level
{
    public class LevelHandler : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _respawnSpeed;


        // Editor Fields
        private CheckPointsManager _checkPointsManager;
        private PlayerController _playerController;
        private CameraMovementController _cameraMovementController;
        private DeathObserver _deathObserver;
        private MapBorders _mapBorders;


        // Public Properties
        public bool RespawningPlayer { get; private set; }



        // Initialization
        private void Awake()
        {
            _checkPointsManager = GetComponentInChildren<CheckPointsManager>();
            _playerController = FindObjectOfType<PlayerController>();
            _cameraMovementController = FindObjectOfType<CameraMovementController>();
            _deathObserver = FindObjectOfType<DeathObserver>();
            _mapBorders = FindObjectOfType<MapBorders>();
            Events.PlayerEvents.Death.AddListener(OnPlayerDead);
        }      


        // Public Methods
        public async Task SpawnFromLastCheckpoint()
        {
            RespawningPlayer = true;

            var lastCheckPoint = _checkPointsManager.LastCheckPoint;
            _playerController.DisablePlayer();
            lastCheckPoint.OnPlayerRespawnStart();      
            await _playerController.MoveToPosition(lastCheckPoint.Position, _cameraMovementController.CameraSpeed);
            _playerController.EnablePlayer();
            lastCheckPoint.OnPlayerRespawnEnd();  
            RespawningPlayer = false;
        }
      

        // Unity events
        private void Update()
        {
            if (InputUtils.KillPlayer && !RespawningPlayer)
                OnPlayerDead();
        }


        // Level Events
        private async void OnPlayerDead()
        {
            if (!_playerController.IsImmortal)
            {
                _playerController.IsImmortal = true;
                await SpawnFromLastCheckpoint();
                _deathObserver.ResetStatus();
                _playerController.IsImmortal = false;
            }
        }
    }
}
