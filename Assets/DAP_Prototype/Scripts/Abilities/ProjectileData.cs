using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Abilities
{
    public class ProjectileData : MonoBehaviour
    {
        [SerializeField] private float travelSpeed = 10f;
        public AbilityData abilityData;
        private AbilityData loadedData;
        public GameObject target;
        private Rigidbody2D rb;
        public LayerMask enemyLayers;
        private Animator animator;

        public float range;
        private float curTime = 0;
        private float nextDamage = 1;

        private void Awake()
        {
            loadedData = abilityData;
            animator = GetComponent<Animator>();
        }
        
        private void Update()
        {
            ProjectTravel();
        }
        public float GetTravelSpeed()
        {
            return travelSpeed;
        }
        private void ProjectTravel()
        {
            curTime -= Time.deltaTime;
            if (target != null)
            {
                float travelDistance = GetTravelDistance(target);
                if (travelDistance > range)
                {
                    transform.position = Vector2.MoveTowards(transform.position,
                    target.transform.position,
                    travelSpeed * Time.deltaTime);
                }
                if (travelDistance <= range)
                {
                    animator.SetTrigger("Impact");
                    Destroy(this.gameObject, 0.4f);
                    Collider();
                }
            }
        }
        private float GetTravelDistance(GameObject target)
        {
            return Vector2.Distance(transform.position, target.transform.position);
        }
        private void Collider()
        {
            if(target != null)
            {
                if (loadedData.abilityType == AbilityType.BigCast)
                {
                    Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, loadedData.hitRadius, enemyLayers);
                    foreach (Collider2D enemy in hits)
                    {
                        if(curTime <= 0)
                        {
                            target.GetComponent<Health>().TakeDamage(loadedData.damage);
                            Debug.Log("We hit: " + target.gameObject + " for " + loadedData.damage);
                            curTime = nextDamage;
                        }
                    }
                }
            }
        }
    }
}
