﻿using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.PathFinding;
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Seeker
{
    public class SeekerMovementController : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private float _speed = 5f;
        

        // Private Fields
        private SeekerBody _seekerBody;
        private SeekerTargetAreaController _targetAreaController;
        private GridController _gridController;
        private Transform _seekerTransform;
        private Transform _playerTransform;
        private bool _isPlayerInside;
        private bool _canFollow;
        private Vector3 _startPosition;
        private UniTask _currentMovementTask = UniTask.CompletedTask;



        // Initialization
        private void Awake()
        {
            _seekerBody = GetComponentInChildren<SeekerBody>();
            _seekerTransform = _seekerBody.transform;
            _targetAreaController = GetComponentInChildren<SeekerTargetAreaController>();
            _gridController = GetComponentInChildren<GridController>();
            _startPosition = _seekerTransform.position;
            InitializeAreaController();
        }

        private void InitializeAreaController()
        {
            _targetAreaController.TriggerEnter.SubscribeWithTag(UnityTag.Player, OnPlayerEnter);
            _targetAreaController.TriggerExit.SubscribeWithTag(UnityTag.Player, OnPlayerExit);
        }


        // Public Methods
        public UniTask TranslateTo(Vector3 position) => _seekerTransform.MoveToAnimated(position, _speed, false);



        // Seeker Events
        private void OnPlayerEnter(Collider2D collision)
        {
            _playerTransform = collision.gameObject.transform;
            _isPlayerInside = true;
            FollowPlayer().Forget();
        }

        private async void OnPlayerExit(Collider2D collision)
        {           
            _isPlayerInside = false;
            await GoBackToStart();
        }




        // Private Methods
        private async UniTask GoBackToStart()
        {
            async UniTask GoBackToStartTask()
            {
                var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, _startPosition);
                foreach (var position in path)
                    if (_canFollow)
                        await TranslateTo(position);
                    else
                        return;
            }

            await CancelFollowTask();
            _currentMovementTask = GoBackToStartTask();
        }

        private async UniTask FollowPlayer()
        {            
            async UniTask FollowTask()
            {
                while(_canFollow && _isPlayerInside)
                {
                    if (!IsSeekerNearToPlayer)
                    {
                        var playerPosition = _playerTransform.position;
                        var path = _targetAreaController.MinimumPathTo(_seekerTransform.position, playerPosition);
                        var nextPositions = from pos in path where Vector2.Distance(_seekerTransform.position, pos) > _gridController.NodeRadius select pos;
                        if (nextPositions.Any())
                            await TranslateTo(nextPositions.First());
                    }
                    await UniTask.WaitForEndOfFrame();
                }
            }

            await CancelFollowTask();
            _currentMovementTask = FollowTask();
        }

        private async UniTask CancelFollowTask()
        {
            _canFollow = false;
            await _currentMovementTask;
            _canFollow = true;
        }

        private bool IsSeekerNearToPlayer => Vector2.Distance(_seekerTransform.position, _playerTransform.position) < _gridController.NodeRadius;
    }
}
