﻿using Assets.AToonWorld.Scripts.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public class DeathObserver : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _maxFallDistanceBeforeDeath = 3;


        // Private Fields
        private PlayerMovementController _playerMovementController;
        private Transform _playerTransform;
        private Vector3 _previousGroundedPosition;
        private MapBorders _mapBorders;
        private bool _isImmortal;


        // Initialization
        private void Awake()
        {
            _playerMovementController = FindObjectOfType<PlayerMovementController>();
            _mapBorders = FindObjectOfType<MapBorders>();
            InitializeMapBorders();
        }

        private void InitializeMapBorders()
        {
            _mapBorders.TriggerEnter.SubscribeWithTag(UnityTag.Player, OnPlayerOutOfMapBorders);
        }



        private void Start()
        {
            IsImmortal = false;
            _playerTransform = _playerMovementController.transform;
            ResetStatus();

            SubscribeToFallDeathEvents();
        }


        // Public Events
        public event Action PlayerDead;



        // Public Properties        
        public bool IsImmortal
        {
            get => _isImmortal;
            set
            {
                if (value == _isImmortal)
                    return;
                _isImmortal = value;
                if (!value)
                    ResetStatus();
            }
        }


        // DeathObserver Events
        private void OnPlayerOutOfMapBorders(Collider2D collision)
        {
            InvokeDeathEvent();
        }





        // Private Methods
        private void ResetStatus()
        {
            _previousGroundedPosition = _playerTransform.position;
        }

        private void SubscribeToFallDeathEvents()
        {            
            var fallDeathTagsToCheck = new string[] { UnityTag.ClimbingWall, UnityTag.Drawing, UnityTag.Ground };

            foreach(var tag in fallDeathTagsToCheck)
            {
                _playerMovementController.PlayerFeet.TriggerEnter.SubscribeWithTag(tag, CheckFallDeath);
                _playerMovementController.PlayerFeet.TriggerExit.SubscribeWithTag(tag, CheckFallDeath);
            }
        }

      
        private void CheckFallDeath(Collider2D collision)
        {
            var (previousPos, currentPos) = (_previousGroundedPosition, _playerTransform.position);
            _previousGroundedPosition = currentPos;
            if (IsFallDeath(previousPos, currentPos))
                InvokeDeathEvent();
            _previousGroundedPosition = currentPos;
        }


        private bool IsFallDeath(Vector3 start, Vector3 end) 
            => start.y - end.y > _maxFallDistanceBeforeDeath;


        private void InvokeDeathEvent()
        {
            if (!IsImmortal)
                PlayerDead?.Invoke();
        }



    }
}
