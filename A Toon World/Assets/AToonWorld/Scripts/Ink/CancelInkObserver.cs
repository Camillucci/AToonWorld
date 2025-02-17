using Assets.AToonWorld.Scripts.Audio;
using Assets.AToonWorld.Scripts.Extensions;
using Assets.AToonWorld.Scripts.Level;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CancelInkObserver : MonoBehaviour
{
    // Editor Fields
    [SerializeField] private GameObject _cancelEffectPrefab = null;

    private List<GameObject> _inkToDelete;
    private Dictionary<GameObject, LineRenderer> _renderersCache;
    private Dictionary<GameObject, float> _alphaCache;

    // Initialization
    void Start()
    {
        _inkToDelete = new List<GameObject>();
        _renderersCache = new Dictionary<GameObject, LineRenderer>();
        _alphaCache = new Dictionary<GameObject, float>();
       ObjectPoolingManager<string>.Instance.CreatePool(nameof(_cancelEffectPrefab), _cancelEffectPrefab, 10, 25, true, true);
    }

    public void NotifyDelete(GameObject inkToDelete, Vector2 effectLocation, Vector2 direction)
    {
        if(!_inkToDelete.Contains(inkToDelete))
        {
            inkToDelete.PlaySound(SoundEffects.DrawingSounds.RandomOrDefault());
            _inkToDelete.Add(inkToDelete);
            ApplyEffect(inkToDelete, effectLocation, direction);
        }
    }

    private void ApplyEffect(GameObject inkToDelete, Vector2 effectLocation, Vector2 direction)
    {
        //Particle effect
        GameObject particleEffect = ObjectPoolingManager<string>.Instance.GetObject(nameof(_cancelEffectPrefab));
        particleEffect.transform.position = effectLocation;
        particleEffect.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        //Try to fade
        if(inkToDelete.GetComponent<LineRenderer>() is LineRenderer inkLineRenderer)
        {
            _renderersCache.Add(inkToDelete, inkLineRenderer);
            _alphaCache.Add(inkToDelete, inkLineRenderer.startColor.a);
            Color newColor = new Color(inkLineRenderer.startColor.r,
                                                inkLineRenderer.startColor.g,
                                                inkLineRenderer.startColor.b,
                                                inkLineRenderer.startColor.a * 0.5f);
            inkLineRenderer.startColor = newColor;
            inkLineRenderer.endColor = newColor;
        }
    }

    public void Commit()
    {
        _inkToDelete.ForEach(obj => {

            //Reset fade
            if(_renderersCache.ContainsKey(obj))
            {
                Color oldColor = new Color(_renderersCache[obj].startColor.r,
                                            _renderersCache[obj].startColor.g,
                                            _renderersCache[obj].startColor.b,
                                            _alphaCache[obj]);
                _renderersCache[obj].startColor = oldColor;
                _renderersCache[obj].endColor = oldColor;
            }
            
            //Release pooled object
            obj.SetActive(false);
        });
        _renderersCache.Clear();
        _alphaCache.Clear();
        _inkToDelete.Clear();
    }
}
