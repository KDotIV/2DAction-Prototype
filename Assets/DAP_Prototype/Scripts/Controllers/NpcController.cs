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
    public class NpcController : MonoBehaviour
    {
        [SerializeField] private float lineOfSight = 5f;
        [SerializeField] private PatrolPath patrolPath;
        [SerializeField] private float wayPointTolerance = 2f;

        private Navigation npcMovement;
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
        private bool isOnPath = false;
        private int currentWaypointIndex = 0;

        void Start()
        {
            CutsceneManager.m_CutsceneManager.npcMovePath += MoveOnPath;
        }
        void Awake()
        {
            npcMovement = GetComponent<Navigation>();
            core = GetComponent<CoreAttributes>();
            abilityHandler = GetComponent<AbilityHandler>();
        }
        void Update()
        {
            if(npcMovement.inDialogue == true) { return; }
            SearchPlayer();
            InteractWithMovement();
        }

        void FixedUpdate()
        {
            PatrolBehaviour();
            OnPath();
        }

        //Handles NPC movement and it's animations
        private void InteractWithMovement()
        {
            CheckDirection();
            if(isOnPath) { return; }
            if((Vector2)transform.position != npcMovement.pathTarget)
            {
                npcMovement.isMoving = true;
                npcMovement.isInRange = false;
                abilityHandler.StopAttack();
                if(!isPatrol) { npcMovement.GetNavAgent().SetDestination(npcMovement.pathTarget); }
            }
            if(AtSpawn())
            {
                npcMovement.isMoving = false;
                npcMovement.animator.SetBool("isMoving", false);
            }
            if(npcMovement.GetNavAgent().velocity.x > 0 && !facingRight)
            {
                Flip();
            }
            else if(npcMovement.GetNavAgent().velocity.x < 0 && facingRight)
            {
                Flip();
            }
        }
        private void SearchPlayer()
        {
            SearchInteract();
            foreach (Collider2D player in detected)
            {
                searchTarget = player.transform.gameObject;
            }
        }
        //Places any objects within it's search radius into an array
        private void SearchInteract()
        {
            detected = Physics2D.OverlapCircleAll(searchPoint.position, searchArea, enemyLayer);
            if (detected != null)
            {
                return;
            }
        }
        //Needs a PatrolPath object w/ PatrolPath script attached to
        //When the paths iterate, the SetDestination() is set for new waypoint
        private void PatrolBehaviour()
        {
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
            npcMovement.GetNavAgent().SetDestination(nextPosition);
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
            float distanceToSpawn = Vector2.Distance(transform.position, npcMovement.pathTarget);
            return distanceToSpawn < wayPointTolerance;
        }
        public CoreAttributes GetCoreAttributes()
        {
            return core;
        }
        void OnDrawGizmos()
        {
            if (searchPoint == null) { return; };
            Gizmos.DrawWireSphere(searchPoint.position, searchArea);
        }
        //This checks which direction the sprite is facing and flips it's renderer so the sprite faces the right way
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
        public void StartDialogue()
        {
            npcMovement.Cancel();
            abilityHandler.StopAnimation();
            npcMovement.pathTarget = this.transform.position;
            npcMovement.isMoving = false;
            isPatrol = false;
            npcMovement.inDialogue = true;
        }
        public bool CheckOnPath()
        {
            return isOnPath;
        }
        private void OnPath()
        {
            if(isOnPath == false ) { return; }
            if(patrolPath != null)
            {
                Debug.Log("Moving in Scene....");
                if(AtWaypoint())
                {
                    Debug.Log("At Waypoint");
                    npcMovement.GetNavAgent().isStopped = true;
                    npcMovement.isMoving = false;
                    npcMovement.animator.SetBool("isMoving", true);
                    isOnPath = false;
                }
                nextPosition = GetCurrentWayPoint();
            }
            npcMovement.isMoving = true;
            npcMovement.animator.SetBool("isMoving", true);
            npcMovement.GetNavAgent().SetDestination(nextPosition);
        }
        private void MoveOnPath()
        {
            isOnPath = true;
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
