using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using RPG.Managers;
using RPG.UI;

namespace RPG.Attributes
{
    public class Health : MonoBehaviour
    {
        private Stats stats;
        private Animator animator;
        public HealthBarUI healthBar;
        [SerializeField] private float healthPoints;
        [SerializeField] private int defensePoints;
        [SerializeField] private float maxHealthPoints;
        [SerializeField] private float regenerationPercentage;
        [SerializeField] private float weakened;
        public bool isBlocking;
        public bool isInCombat = false;
        private bool isDead = false;
        public float lastHit;
        private void Awake()
        {
            stats = GetComponent<Stats>();
            healthPoints = GetComponent<Stats>().currentCon * 5;
            defensePoints = GetComponent<Stats>().currentDef;
            animator = GetComponent<Animator>();
            if(healthBar != null)
            {
                healthBar.GetComponent<HealthBarUI>();
                healthBar.SetMaxHealth(healthPoints);
            }
        }
        private void Update()
        {
            CheckHealthPoints();
        }
        private void FixedUpdate()
        {
            BaseRegenerateHealth();
        }
        public bool IfDead()
        {
            return isDead;
        }
        public void TakeDamage(int damage)
        {
            healthPoints = Mathf.Max(healthPoints - (defensePoints - damage * -1), 0);
            Debug.Log(this.gameObject + " has " + healthPoints);
            healthBar.SetHealth(healthPoints);
            Debug.Log("Healthbar: " + healthBar.slider.value);
            lastHit = 0;
            if(isInCombat == false)
            { 
                isInCombat = true;
                GameEvents.current.TriggerCombat();
            }
            if(healthPoints == 0)
            {
                Die();
            }
        }
        public void HealDamage(float damage)
        {
            if(healthPoints < maxHealthPoints)
            {
                healthPoints = Mathf.Max(healthPoints + damage, 0);
                Debug.Log(this.gameObject + " has " + healthPoints);
                healthBar.SetHealth(healthPoints);
                Debug.Log("Healthbar: " + healthBar.slider.value);
            }
        }

        public void BlockDamage(int blockDamage)
        {
            if(isBlocking == true)
            {
                defensePoints += blockDamage;
            }
        }
        public void ResetDefensives()
        {
            if(isBlocking == false)
            {
                defensePoints = stats.currentDef;
            }
        }
        private void CheckHealthPoints()
        {
            lastHit += Time.deltaTime;
            if(healthPoints <= weakened)
            {
                animator.SetBool("Exhausted", true);
                return;
            }

        }
        private void BaseRegenerateHealth()
        {
            if(!isInCombat)
            {
                HealDamage(0.2f);
            }
        }
        public void Die()
        {
            if(isDead) return;

            isDead = true;
            GetComponent<Animator>().SetTrigger("Die");
            if(this.tag == "Player")
            {
                GameEvents.current.GameOver();
            }
        }
        public float GetHealthPoints()
        {
            return healthPoints;
        }
        public float GetMaxHealtPoints()
        {
            return maxHealthPoints;
        }
        public float GetHealthPercent()
        {
            return 100 * GetFraction();
        }
        public float GetFraction()
        {
            return healthPoints / GetComponent<Stats>().currentCon;
        }
    }
}
