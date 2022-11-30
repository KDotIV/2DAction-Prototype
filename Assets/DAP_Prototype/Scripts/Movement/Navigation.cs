using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Navigation : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private Rigidbody2D rb;

        public Vector2 movement;
        public Animator animator;
        private NavMeshAgent agent;
        public Vector2 pathTarget;
        public Vector2 spawnPoint;
        private CoreAttributes core;

        private float cooldown = Mathf.Infinity;
        private float timeBetween = 3f;
        public bool isMoving = false;
        public bool isInRange;
        public bool inDialogue;

        private void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            pathTarget = transform.position;
            agent.SetDestination(pathTarget);
            LockZPosition();
        }

        void Awake()
        {
            core = GetComponent<CoreAttributes>();
            animator = GetComponent<Animator>();
            spawnPoint = transform.position;
        }
        void Update()
        {
            cooldown += Time.deltaTime;
            if(inDialogue == true) { return; }
            UpdateMovement();
            UpdateAnimator();
        }

        public void UpdateMovement()
        {
            if (isMoving)
            {
                animator.SetBool("isMoving", true);
                rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
                agent.speed = moveSpeed;
            }
        }

        private void UpdateAnimator()
        {
            Vector2 velocity = agent.velocity;
            Vector2 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.x;
            animator.SetFloat("Speed", speed);
            animator.SetFloat("Horizontal", localVelocity.x);
            animator.SetFloat("Vertical", localVelocity.y);


        }
        public void Cancel()
        {
            agent.isStopped = true; 
            agent.ResetPath();
            animator.SetBool("isMoving", false);
            isMoving = false;
        }

        public void HandleDodge()
        {
            if (cooldown > timeBetween)
            {
                animator.SetTrigger("Dodge");
                float _dodgeDistance = 2f;
                rb.position += movement * _dodgeDistance;
                cooldown = 0;
            }
            else { Debug.Log("Dodge is on Cooldown"); }
        }
        private void LockZPosition()
        {
            Vector3 pos = transform.position;
            pos.z = 0;
            transform.position = pos;
        }
        public float PropsMoveSpeed
        {
            get { return moveSpeed; }
            set { moveSpeed = value; }
        }

        public Rigidbody2D GetRigidbody()
        {
            return rb;
        }
        public NavMeshAgent GetNavAgent()
        {
            return agent;
        }
    }
}
