using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
namespace RPG.Managers
{
    public class GameEvents : MonoBehaviour
    {
        public static GameEvents current;
        void Awake()
        {
            current = this;
        }

        //TODO: Next iteration add more burden of data handling to Managers/Handlers
        public event Action onCombatTriggerEnter;
        public event Action onCutsceneEnter;
        public event Action onOutofBoundsEnter;
        public event Action deathEvent;
        public event Action onFinalScene;
        public void TriggerCombat()
        {
            onCombatTriggerEnter();
        }
        public void TriggerCutscene() => onCutsceneEnter?.Invoke();
        public void TriggerOutBounds() => onOutofBoundsEnter?.Invoke();
        public void GameOver() => deathEvent?.Invoke();
    }
}