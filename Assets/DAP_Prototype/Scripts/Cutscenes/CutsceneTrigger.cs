using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using RPG.DiagSystem;
using RPG.Managers;

namespace RPG.Cutscenes
{
   public class CutsceneTrigger : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            GameEvents.current.TriggerCutscene();
            Destroy(this, 1.0f);
        }
    }
}
