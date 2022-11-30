using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Managers
{
    public class OutOfBounds : MonoBehaviour
{
    private Health _player;
    private void Start()
    {
        GameEvents.current.onOutofBoundsEnter += KillPlayer;
    }
    private void OnDisable()
    {
        GameEvents.current.onOutofBoundsEnter -= KillPlayer;
    }
    private void OnDestroy() 
    {
       GameEvents.current.onOutofBoundsEnter -= KillPlayer; 
    }
    private void KillPlayer()
    {
        Debug.Log("Player went out of bounds");
        _player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        if(_player != null)
        {
            _player.Die();
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        GameEvents.current.TriggerOutBounds();
    }
}
}
