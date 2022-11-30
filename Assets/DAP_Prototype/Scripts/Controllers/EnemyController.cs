using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Movement;
using RPG.Attributes;
using RPG.Abilities;
using RPG.Managers;

namespace RPG.Controllers
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private float lineOfSight = 5f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float wayPointTolerance = 2f;

        private Navigation enemyMovement;
        private CoreAttributes core;
        private AbilityHandler abilityHandler;
        public LayerMask enemyLayer;
        public LayerMask allyLayer;
        public Transform searchPoint;
        private Collider2D playerData;
        private GameObject searchTarget;
        private Collider2D[] detected;
        private Vector2 nextPosition;

        public float searchArea;
        public float aggroRange;
        private float pathDistance;
        public bool isPatrol;
        public bool facingRight;
        private int currentWaypointIndex = 0;

        void Awake()
        {
            enemyMovement = GetComponent<Navigation>();
            core = GetComponent<CoreAttributes>();
            abilityHandler = GetComponent<AbilityHandler>();
        }

        void Update()
        {
            if(core.charHealth.IfDead()) { return; }
            SearchPlayer();
            InteractWithMovement();
            InteractWithCombat();
        }

        void FixedUpdate()
        {
            PatrolBehaviour();
        }

        private void InteractWithMovement()
        {
            CheckDirection();
            if((Vector2)transform.position != enemyMovement.pathTarget)
            {
                enemyMovement.isMoving = true;
                enemyMovement.isInRange = false;
                abilityHandler.StopAttack();
                if(!isPatrol) { enemyMovement.GetNavAgent().SetDestination(enemyMovement.pathTarget); }
            }
            if(AtSpawn())
            {
                enemyMovement.isMoving = false;
                enemyMovement.animator.SetBool("isMoving", false);
            }
            if(enemyMovement.GetNavAgent().velocity.x > 0 && !facingRight)
            {
                Flip();
            }
            else if(enemyMovement.GetNavAgent().velocity.x < 0 && facingRight)
            {
                Flip();
            }
        }

        private void InteractWithCombat()
        {
            CheckDirection();
            if(searchTarget == null) { return; }
            if(searchTarget.GetComponent<Health>().IfDead()) 
            { 
                abilityHandler.StopAttack();
                abilityHandler.StopAnimation();
            }
            if(searchTarget.tag == "Player" && pathDistance <= aggroRange)
            {
                isPatrol = false;
                enemyMovement.pathTarget = searchTarget.transform.position;
                enemyMovement.GetNavAgent().SetDestination(enemyMovement.pathTarget);
                abilityHandler.Attack(searchTarget);
                core.charHealth.isInCombat = true;
                GameEvents.current.TriggerCombat();
                if (abilityHandler.GetTargetDistance() <= abilityHandler.attackRange)
                {
                    abilityHandler.UseAbility("0");
                    enemyMovement.isInRange = true;
                    enemyMovement.isMoving = false;
                    enemyMovement.animator.SetBool("isMoving", false);
                    enemyMovement.Cancel();
                }
                if(abilityHandler.GetTargetDistance() > abilityHandler.attackRange)
                {
                    abilityHandler.StopAnimation();
                }
            }
        }
        private void PatrolBehaviour()
        {
            if(core.charHealth.isInCombat) { return; }
            if(isPatrol == false) { return; }
            if(patrolPath != null)
            {
                isPatrol = true;
                if(AtWaypoint())
                {
                    CycleWaypoint();
                }
                nextPosition = GetCurrentWayPoint();
            }
            enemyMovement.GetNavAgent().SetDestination(nextPosition);
        }

        private Vector2 GetCurrentWayPoint()
        {
            return patrolPath.GetWaypoint(currentWaypointIndex);
        }

        private void CycleWaypoint()
        {
            currentWaypointIndex = patrolPath.GetNextIndex(currentWaypointIndex);
        }

        private bool AtWaypoint()
        {
            float distanceToWaypoint = Vector2.Distance(transform.position, GetCurrentWayPoint());
            return distanceToWaypoint < wayPointTolerance;
        }
        private bool AtSpawn()
        {
            float distanceToSpawn = Vector2.Distance(transform.position, enemyMovement.pathTarget);
            return distanceToSpawn < wayPointTolerance;
        }
        private void SearchPlayer()
        {
            SearchInteract();
            foreach (Collider2D player in detected)
            {
                searchTarget = player.transform.gameObject;
                //Debug.Log("Targeting: " + searchTarget);
            }
        }
        private void SearchInteract()
        {
            detected = Physics2D.OverlapCircleAll(searchPoint.position, searchArea, enemyLayer);
            if (detected != null)
            {
                return;
            }
        }
        public CoreAttributes GetCoreAttributes()
        {
            return core;
        }
        void OnDrawGizmosSelected()
        {
            if (searchPoint == null) { return; };
            Gizmos.DrawWireSphere(searchPoint.position, searchArea);
        }
        private void CheckDirection()
        {
            if (searchTarget == null) { return; }
            if (searchTarget.transform.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            if (searchTarget.transform.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
        }
        private void GetPathingDistance()
        {
            pathDistance = Vector2.Distance(transform.position, searchTarget.transform.position);
        }
        private void Flip()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = !facingRight;
        }
    }
}
