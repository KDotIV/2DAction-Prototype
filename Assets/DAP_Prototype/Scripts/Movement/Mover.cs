using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour
    {
        public Vector2 movement;
        public Animator animator;
        [SerializeField] private Transform pathTarget;
        public Vector3 target;
        private CoreAttributes core;
        private float cooldown = Mathf.Infinity;
        private float timeBetween = 3f;
        [SerializeField] private float moveSpeed = 10f;
        [SerializeField] private Rigidbody2D rb;
        public bool isMoving = false;
        public bool canClick = true;

        void Awake()
        {
            core = GetComponent<CoreAttributes>();
            animator = GetComponent<Animator>();
        }
        void Update()
        {
            cooldown += Time.deltaTime;
            LockZPosition();
            UpdateMovement(target);
        }

        public void UpdateMovement(Vector2 destination)
        {
            if(isMoving == false) { return; }
            rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
        }
        public void Cancel()
        {
            rb.velocity = Vector2.zero;
            isMoving = false;
            animator.SetFloat("Speed", 0);
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
    }
}