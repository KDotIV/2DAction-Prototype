using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Managers;
using System.Linq;

namespace RPG.Combat
{
    public class CombatHandler : MonoBehaviour
    {
        private float round;
        Queue<CoreAttributes> roundQueue;
        private CoreAttributes combatant;
        private List<CoreAttributes> allCombatants;
        private bool inCombat = false;
        public bool forceCombat;

        void Start()
        {
            GameEvents.current.onCombatTriggerEnter += BeginCombat;
        }
        
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                if(forceCombat == true && inCombat == false)
                {
                    Debug.Log("Initiating Combat...");
                    InitiateTurnOrder();
                    inCombat = true;
                    BeginCombat();
                }
            }
            CheckDead();
        }

        private void BeginCombat()
        {
            if(inCombat) { return; }
            Debug.Log("Initiating Combat...");
            inCombat = true;
            InitiateTurnOrder();
            //StartCoroutine(BeginQueue());
        }

        private void ExitCombat()
        {
            inCombat = false;
            allCombatants = CoreAttributes.GetEnemies(CoreAttributes.AllChars);
            foreach (CoreAttributes found in allCombatants)
            {
                Debug.Log(found + " is out of Combat");
                found.SurpriseRound(false);
                found.charHealth.isInCombat = false;
            }
        }

        private void PauseCombat()
        {
            Time.timeScale = 0;
        }

        private void ResumeCombat()
        {
            Time.timeScale = 1;
        }
        private void InitiateTurnOrder()
        {
            IEnumerable<CoreAttributes> findEnemies = CoreAttributes.GetEnemies(CoreAttributes.AllChars)
            .OrderByDescending(CoreAttributes => CoreAttributes.PropsInitiative);
            roundQueue = new Queue<CoreAttributes>();
            foreach (CoreAttributes initOrder in findEnemies)
            {
                if(initOrder.charHealth.isInCombat == true)
                {
                    combatant = initOrder.GetComponent<CoreAttributes>();
                    combatant.SurpriseRound(true);
                    Debug.Log(combatant + " is surprised");
                    roundQueue.Enqueue(combatant);
                    Debug.Log(combatant.GetName() + " was added to TurnOrder");
                }
            }
        }

        private IEnumerator BeginQueue()
        {
            int _canAttack;
            for(_canAttack = roundQueue.Count; _canAttack > 0; _canAttack--)
            {
                if(combatant != null)
                {
                    combatant = roundQueue.Peek().GetComponent<CoreAttributes>();
                }
                combatant.SurpriseRound(false);
                Debug.Log(combatant + " is not surprised");
                roundQueue.Dequeue();
                yield return new WaitForSeconds(0.1f);
                Debug.Log("Queue: " + roundQueue.Count);
            }
        }

        private void CreateListener()
        {
            GameEvents.current.onCombatTriggerEnter += BeginCombat;
        }
        private void DestroyListener()
        {
            GameEvents.current.onCombatTriggerEnter -= BeginCombat;
        }
        private void OnDestroy()
        {
            GameEvents.current.onCombatTriggerEnter -= BeginCombat;
        }
        private void CheckDead()
        {
            if(!inCombat) { return; }
            allCombatants = CoreAttributes.GetEnemies(CoreAttributes.AllChars);
            foreach (CoreAttributes found in allCombatants)
            {
                if(found.tag != "Player" && found.charHealth.IfDead() == true)
                {
                    Debug.Log("Deleted enemy... ");
                    Destroy(found.gameObject, 3);
                }
                if(found.tag == "Player" && found.charHealth.lastHit >= 8)
                {
                    Debug.Log("WE EXITED COMBAT....");
                    ExitCombat();
                }
                if(found.tag == "Player" && found.charHealth.IfDead() == true)
                {
                    Debug.Log("GameOver... YOU SUCK");
                }
            }
        }
    }
}
