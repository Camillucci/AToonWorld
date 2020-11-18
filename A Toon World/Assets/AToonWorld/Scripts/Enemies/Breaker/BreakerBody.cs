﻿using Assets.AToonWorld.Scripts.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.AToonWorld.Scripts.Enemies.Breaker
{
    public class BreakerBody : MonoBehaviour
    {
        // Private Fields
        private readonly TaggedEvent<string, Collider2D> _triggerEnter = new TaggedEvent<string, Collider2D>();
        private readonly TaggedEvent<string, Collider2D> _triggerExit = new TaggedEvent<string, Collider2D>();


        // Initialization
        private void Awake()
        {
            TriggerEnter.SubscribeWithTag(UnityTag.Drawing, OnDrawingEnter);
            TriggerExit.SubscribeWithTag(UnityTag.Drawing, OnDrawingExit);
        }

        
        // Public Properties
        public ITaggedEvent<string, Collider2D> TriggerEnter => _triggerEnter;
        public ITaggedEvent<string, Collider2D> TriggerExit => _triggerExit;



        // Unity Events
        private void OnTriggerEnter2D(Collider2D collision)
        {
            _triggerEnter.InvokeWithTag(collision.gameObject.tag, collision);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            _triggerExit.InvokeWithTag(collision.gameObject.tag, collision);
        }



        // Breaker Events
        private void OnDrawingEnter(Collider2D collision)
        {
            collision.gameObject.SetActive(false);
        }

        private void OnDrawingExit(Collider2D collision)
        {
            
        }
    }
}
