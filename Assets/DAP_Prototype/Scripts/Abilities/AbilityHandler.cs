using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using RPG.Attributes;
using RPG.Movement;

namespace RPG.Abilities
{
    public class AbilityHandler : MonoBehaviour
    {
        public Animator animator;
        public Transform attackPoint;
        public Transform vfxPoint;
        public AbilityData[] abilities;
        private AbilityData[] equippedAbilities;
        private CoreAttributes coreAttributes;
        private Health currentHealth;
        public LayerMask enemyLayers;
        protected Stats coreStats;
        private Mover movement;
        private Navigation navigation;
        private CoreAttributes target;
        private GameObject attackTarget;

        private float targetDistance;
        public float attackRange = 5f;
        public float attackSpeed = 1f;
        public float nextAttack = 1f;
        public float globalCoodDown = 1f;
        public float vfxRadius = 0.5f;
        public int statCheck;
        public bool isCasting;
        public bool isInStance = false;
        private int currentAbility;
        private int currentStance;

        void Awake() 
        {
            coreStats = GetComponent<Stats>();
            coreAttributes = GetComponent<CoreAttributes>();
            currentHealth = GetComponent<Health>();
            movement = GetComponent<Mover>();
            if(this.gameObject.tag == "Enemy")
            {
                navigation = GetComponent<Navigation>();
            }
            LoadAbilities(abilities);
        }

        void Update()
        {
            AbilityCycle();
            if (attackTarget == null) return;
            if (target == null) return;
            if (target.charHealth.IfDead()) return;
            IfInRange();
        }

        public void UseAbility(string input)
        {
            if(globalCoodDown <= 2) { 
                Debug.Log("GCD"); return; 
            } else { isCasting = false; }
            {
                switch (input)
                {
                    //Main Attack bound to Mouse 0
                    case "0":
                    if (coreStats.currentStr >= statCheck)
                    {
                        if (target != null)
                        {
                            if (target.charHealth.IfDead()) { Debug.Log("Target is Dead..."); break; }
                        }
                        if (nextAttack >= attackSpeed)
                        {
                            currentAbility = 0;
                            animator.SetTrigger(equippedAbilities[0].animTrigger);
                            animator.ResetTrigger("StopAttack");
                            nextAttack = 0;
                        }
                        break;
                    }
                    else { Debug.Log("Failed Check"); }
                    break;

                    //Defensive Ability Bound to Mouse 1
                    case "d":
                        if (coreStats.currentStr >= statCheck)
                        {
                            currentHealth.isBlocking = true;
                            currentAbility = 5;
                            animator.SetBool(equippedAbilities[5].animTrigger, true);
                            animator.ResetTrigger("StopAttack");
                            nextAttack = 0;
                            break;
                        }
                        else { Debug.Log("Failed Check"); }
                        break;

                    case "1":
                    if(coreStats.currentStr >= statCheck)
                    {
                        if(target != null)
                        {
                            if (target.charHealth.IfDead()) { Debug.Log("Target is Dead..."); break; }
                        }
                        currentAbility = 1;
                        if(equippedAbilities[1].coolDown >= equippedAbilities[1].nextCoolDown)
                        {
                            isCasting = true;
                            animator.SetTrigger(equippedAbilities[1].animTrigger);
                            equippedAbilities[1].ResetCooldown();
                            if(attackTarget != null) { globalCoodDown = 0; }
                            break;
                        } else { Debug.Log("Ability on CoolDown"); }
                    } else { Debug.Log("Failed Check"); }
                    break;
                    
                    case "2":
                    if (coreStats.currentStr >= statCheck)
                    {
                        if (target != null)
                        {
                            if (target.charHealth.IfDead()) { Debug.Log("Target is Dead..."); break; }
                        }
                        currentAbility = 2;
                        if (equippedAbilities[2].coolDown >= equippedAbilities[2].nextCoolDown)
                        {
                            isCasting = true;
                            animator.SetTrigger(equippedAbilities[2].animTrigger);
                            equippedAbilities[2].ResetCooldown();
                            globalCoodDown = 0;
                            break;
                        }
                        else { Debug.Log("Ability on CoolDown"); }
                    }
                    else { Debug.Log("Failed Check"); }
                    break;

                    case "3":
                    if (coreStats.currentStr >= statCheck)
                    {
                        if (target != null)
                        {
                            if (target.charHealth.IfDead()) { Debug.Log("Target is Dead..."); break; }
                        }
                        currentAbility = 3;
                        if (equippedAbilities[3].coolDown >= equippedAbilities[3].nextCoolDown)
                        {
                            isCasting = true;
                            animator.SetTrigger(equippedAbilities[3].animTrigger);
                            equippedAbilities[3].ResetCooldown();
                            globalCoodDown = 0;
                            break;
                        }
                        else { Debug.Log("Ability on CoolDown"); }
                    }
                    else { Debug.Log("Failed Check"); }
                    break;
                    
                    case "4":
                    if (coreStats.currentStr >= statCheck)
                    {
                        if (target != null)
                        {
                            if (target.charHealth.IfDead()) { Debug.Log("Target is Dead..."); break; }
                        }
                        if(target.charHealth.IfDead()) {Debug.Log("Target is Dead..."); break; }
                        currentAbility = 4;
                        if (equippedAbilities[4].coolDown >= equippedAbilities[4].nextCoolDown)
                        {
                            isCasting = true;
                            animator.SetTrigger(equippedAbilities[4].animTrigger);
                            equippedAbilities[4].ResetCooldown();
                            globalCoodDown = 0;
                            break;
                        }
                        else { Debug.Log("Ability on CoolDown"); }
                    }
                    else { Debug.Log("Failed Check"); }
                    break;
                }
            }
        }
        private void LoadAbilities(AbilityData[] abilityData)
        {
            equippedAbilities = new AbilityData[7];
            for (int i = 0; i < abilities.Length; i++)
            {
                equippedAbilities[i] = abilities[i];
                equippedAbilities[i].ResetCooldown();
                Debug.Log("You have loaded: " + equippedAbilities);
            }
        }
        private void AbilityCollider()
        {
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, equippedAbilities[currentAbility].hitRadius, enemyLayers);
            if(hitEnemies == null) { return; };
            foreach (Collider2D enemy in hitEnemies)
            {
                enemy.GetComponent<Health>().TakeDamage(equippedAbilities[currentAbility].damage);
                Debug.Log("We hit: " + enemy.gameObject + " for " + equippedAbilities[currentAbility].damage);
            }
        }
        private void GetTargetRange(Transform targetTransform)
        {
            if(targetTransform == null) { return; }
            targetDistance = Vector2.Distance(transform.position, targetTransform.transform.position);
        }
        public void Attack(GameObject combatTarget)
        {
            attackTarget = combatTarget;
            target = combatTarget.GetComponent<CoreAttributes>();
            GetTargetRange(attackTarget.transform);
        }
        private void IfInRange()
        {
            if(isCasting) { return; }
            if(attackTarget != null)
            {
                GetTargetRange(attackTarget.transform);
                if (targetDistance > attackRange)
                {
                    Debug.Log("Out Of Range: ");
                }
                if (targetDistance <= attackRange)
                {
                    if(coreAttributes.surprised == false)
                    {
                        AutoAttack();
                    }
                }
            } else { Debug.Log("Target was null"); }
        }
        private void AutoAttack()
        {
            if(target.charHealth.IfDead())
            {
                StopAttack();
                Debug.Log("Target Died");
                
            }
            if(!isCasting)
            {
                if (nextAttack >= attackSpeed)
                {
                    currentAbility = 0;
                    animator.SetTrigger(equippedAbilities[0].animTrigger);
                    animator.ResetTrigger("StopAttack");
                    nextAttack = 0;
                }
            }
        }
        private void AbilityCycle()
        {
            for(int i= 0; i < equippedAbilities.Length; i++)
            {
                if(equippedAbilities[i] != null)
                {
                    equippedAbilities[i].coolDown += Time.deltaTime;
                }
            }
            nextAttack += Time.deltaTime;
            globalCoodDown += Time.deltaTime;
        }
#region Animation Events
        //Melee Event
        public void Hit()
        {
            Debug.Log("Hit Called....");
            if(equippedAbilities[currentAbility].abilityType == AbilityType.Stance)
            {
                currentAbility = currentStance;
                isInStance = true;
            }
            AbilityCollider();
            coreAttributes.charHealth.isInCombat = true;
        }
        //Range Event
        public void ProjectileHit()
        {
            if (equippedAbilities[currentAbility].animationPrefab.Length > 0)
            {
                StartCoroutine(CycleProjectiles());
                coreAttributes.charHealth.isInCombat = true;
            }
        }
        //Block Event
        public void BlockHit()
        {
            if(currentHealth.isBlocking == true)
            {
                currentHealth.BlockDamage(equippedAbilities[currentAbility].damage); 
                Debug.Log("We blocked for " + equippedAbilities[currentAbility].damage);
            }
        }
#endregion

        private IEnumerator CycleProjectiles()
        {
            if (equippedAbilities[currentAbility].animationPrefab.Length > 0)
            {
                Debug.Log("Launched...");

                for (int i = 0; i < equippedAbilities[currentAbility].animationPrefab.Length; i++)
                {
                    GameObject clone = Instantiate(equippedAbilities[currentAbility].animationPrefab[i], transform.position, Quaternion.identity);
                    clone.GetComponent<ProjectileData>().target = attackTarget;
                    yield return new WaitForSeconds(0.3f);
                }
            }
        }
        //Cancels
        public void Cancel()
        {
            movement.Cancel();
        }
        public void StopAttack()
        {
            attackTarget = null;
        }
        public void StopAnimation()
        {
            animator.ResetTrigger("PrimeAttack");
            animator.SetTrigger("StopAttack");
        }

        //Getters
        public CoreAttributes GetTarget()
        {
            return target;
        }
        public int GetCurrentAbility()
        {
            return currentAbility;
        }
        public AbilityData[] GetAbilities()
        {
            return equippedAbilities;
        }
        public float GetTargetDistance()
        {
            return targetDistance;
        }
        void OnDrawGizmosSelected() 
        {
            if(attackPoint == null) { return; };
            if(vfxPoint == null) { return; };

            Gizmos.DrawWireSphere(attackPoint.position, attackRange);
            Gizmos.DrawWireSphere(vfxPoint.position, vfxRadius);
        }
    }
}
