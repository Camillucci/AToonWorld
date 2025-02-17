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
    public class PlayerFeet : MonoBehaviour
    {
        private readonly ColliderTaggedEvents<Collider2D> _colliderTrigger = new ColliderTaggedEvents<Collider2D>();
        public IColliderTaggedEvents<Collider2D> ColliderTrigger => _colliderTrigger;


        private void OnTriggerEnter2D(Collider2D collider)
            => _colliderTrigger.NotifyEnter(collider.gameObject.tag, collider);

        private void OnTriggerExit2D(Collider2D collider)
            => _colliderTrigger.NotifyEnter(collider.gameObject.tag, collider);
    }
}
