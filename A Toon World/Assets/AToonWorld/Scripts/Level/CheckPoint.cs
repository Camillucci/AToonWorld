﻿using Assets.AToonWorld.Scripts.Utils;
using Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.AToonWorld.Scripts.Level
{
    public class CheckPoint : MonoBehaviour
    {
        // Editor Fields
        [SerializeField] private int _checkPointNumber = -1;
        [SerializeField] private bool _isStart = false;


        
        // Private fields
        private bool _hit;



        // Initialization
        private void Awake()
        {
            _transform = transform;
            _triggerEnter.SubscribeWithTag(UnityTag.Player, OnPlayerHit);
        }



        // Events
        public event Action<CheckPoint> PlayerHit;
        [SerializeField] private UnityEvent _checkpointTaken = null;
        [SerializeField] private UnityEvent _playerSpawning = null;
        [SerializeField] private UnityEvent _playerSpawned = null;



        // Private Fields
        private readonly TaggedEvent<string, Collider2D> _triggerEnter = new TaggedEvent<string, Collider2D>();
        private Transform _transform;




        // Public Properties
        public Vector3 Position => _transform.position;
        public int CheckPointNumber => _checkPointNumber;
        public ITaggedEvent<string, Collider2D> TriggerEnter => _triggerEnter;

        public bool Hit => _hit || IsStart;
        public bool IsStart => _isStart;




        // Unity Events
        private void OnTriggerEnter2D(Collider2D collision) => _triggerEnter.InvokeWithTag(collision.gameObject.tag, collision);

        // CheckPoint events        
        private void OnPlayerHit(Collider2D collision)
        {
            if (Hit)
                return;

            _hit = true;
            
            //Events
            Events.AnaliticsEvents.Checkpoint.Invoke(new Analitic(_checkPointNumber));
            LevelEvents.CheckpointReached.Invoke();
            _checkpointTaken?.Invoke();
            PlayerHit?.Invoke(this);
        }

        public void OnPlayerRespawnStart()
        {
            _playerSpawning?.Invoke();
        }

        public void OnPlayerRespawnEnd()
        {
            _playerSpawned?.Invoke();
        }
    }
}
