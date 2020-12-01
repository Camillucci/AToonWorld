﻿using Assets.AToonWorld.Scripts.Utils;
using Assets.AToonWorld.Scripts.Utils.Events.TaggedEvents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Player
{
    public class PlayerBody : MonoBehaviour
    {
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;

        private Collider2D _collider;

        void Awake()
        {
            this._collider = GetComponent<Collider2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
            => _colliderTrigger.NotifyEnter(collision.gameObject.tag, collision);

        private void OnTriggerExit2D(Collider2D collision)
            => _colliderTrigger.NotifyExit(collision.gameObject.tag, collision);

        public void EnableCollider() => _collider.enabled = true;

        public void DisableCollider() => _collider.enabled = false;
    }
}
