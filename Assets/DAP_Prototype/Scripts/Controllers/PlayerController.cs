using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using RPG.Movement;
using RPG.Attributes;
using RPG.Abilities;
using RPG.DiagSystem;

namespace RPG.Controllers
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float raycastRadius = 1f;
        [SerializeField] GameObject interactPrefab;
        
        private Mover playermovement;
        private CoreAttributes core;
        private Animator animator;
        private AbilityHandler abilityHandler;
        private GameObject combatTarget;
        public Transform searchPoint;
        public LayerMask interactLayer;
        private Collider2D[] detected;
        private Camera cam;
        private Vector2 point;
        private PlayerConvo playerConvo;

        private float searchArea = 3f;
        private bool facingRight = true;
        private bool isTalking;

        void Awake()
        {
            playermovement = GetComponent<Mover>();
            core = GetComponent<CoreAttributes>();
            animator = GetComponent<Animator>();
            abilityHandler = GetComponent<AbilityHandler>();
            interactPrefab.SetActive(false);
            playerConvo = GetComponent<PlayerConvo>();
        }

        private void OnEnable()
        {
            StartInterop();
        }

        private void Start()
        {
            StartInterop();
        }

        private void StartInterop()
        {
            foreach (var _go in gameObject.scene.GetRootGameObjects())
            {
                if (_go.tag != "MainCamera") continue;
                var _cam = _go.GetComponent<Camera>();
                if (_cam == null) continue;
                cam = _cam;
                break;
            }
        }

        void Update()
        {
            InteractWithMovement();
            InteractWithCombat();
        }

        void FixedUpdate()
        {
            interactPrefab.SetActive(!SearchInteract());
            InteractWithInteractbles();
        }
        private void InteractWithMovement()
        {
            SetMouseRay();
            if(isTalking) { playermovement.Cancel(); return; }
            playermovement.movement.x = Input.GetAxisRaw("Horizontal");
            playermovement.movement.y = Input.GetAxisRaw("Vertical");
            if(playermovement.movement.x != 0 || playermovement.movement.y != 0)
            {
                playermovement.canClick = false;
                playermovement.isMoving = true;
                combatTarget = null;
                abilityHandler.StopAnimation();
            }
            playermovement.animator.SetFloat("Horizontal", playermovement.movement.x);
            playermovement.animator.SetFloat("Vertical", playermovement.movement.y);
            playermovement.animator.SetFloat("Speed", playermovement.movement.sqrMagnitude);

            if(Input.GetAxisRaw("Horizontal") > 0 && !facingRight)
            {
                Flip();
            } 
            else if(Input.GetAxisRaw("Horizontal") < 0 && facingRight) 
            {
                Flip();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                playermovement.HandleDodge();
            }
        }

        private void InteractWithCombat()
        {
            CheckDirection();
            if(core.charHealth.IfDead() == true) { playermovement.Cancel(); return;}
            if(Input.GetMouseButtonUp(1))
            {
                core.charHealth.isBlocking = false;
                core.charHealth.ResetDefensives();
                animator.SetBool("Block", false);
            }
            if(Input.GetMouseButtonDown(0))
            {
                detected = RaycastOnMouse();
                playermovement.canClick = true;
                abilityHandler.isCasting = false;
                if(CheckEnemy() == true) { abilityHandler.UseAbility("0"); }
            }
            if(Input.GetMouseButtonDown(1))
            {
                detected = RayCastOnSelf();
                playermovement.canClick = true;
                abilityHandler.isCasting = false;
                abilityHandler.UseAbility("d");
            }
            switch (Input.inputString)
            {
                case "q":
                    detected = RaycastOnMouse();
                    if (CheckEnemy() == true) { abilityHandler.UseAbility("1"); }
                    Debug.Log("First Ability");
                    break;
                case "e":
                    detected = RaycastOnMouse();
                    if (CheckEnemy() == true) { abilityHandler.UseAbility("2"); }
                    Debug.Log("Second Ability");
                    break;
                case "f":
                    detected = RaycastOnMouse();
                    if (CheckEnemy() == true) { abilityHandler.UseAbility("3"); }
                    Debug.Log("Third Ability");
                    break;
                case "r":
                    detected = RaycastOnMouse();
                    if (CheckEnemy() == true) { abilityHandler.UseAbility("4"); }
                    Debug.Log("Ultimate");
                    break;
            }
        }
        private void InteractWithInteractbles()
        {
            if(core.charHealth.isInCombat == true) return;
            isTalking = playerConvo.CheckDialogueActive();
            if (SearchInteract())
            {
                foreach (Collider2D interactable in detected)
                {
                    if(interactable.tag == "NPC")
                    {
                        var _detectedDialogue = interactable.GetComponent<AIConvo>();
                        Debug.Log("Found: " + _detectedDialogue);
                        interactPrefab.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            if(interactable.GetComponent<NpcController>() != null)
                            {
                                interactable.GetComponent<NpcController>().StartDialogue();
                            }
                            if(!SpeakerHandler.CreateSpeakerLog(interactable.gameObject))
                            {
                                GetComponent<PlayerConvo>().StartDialogue(_detectedDialogue.GetCommonDialogue());
                            } else
                            {
                                GetComponent<PlayerConvo>().StartDialogue(_detectedDialogue.GetDialogue());
                            }
                        }
                    }
                }
            }
            if(!SearchInteract())
            {
                Debug.Log("Player left Search Area: ");
                interactPrefab.SetActive(false);
                GetComponent<PlayerConvo>().Quit();
            }
        }

        private bool SearchInteract()
        {
            detected = Physics2D.OverlapCircleAll(searchPoint.position, searchArea, interactLayer);
            if (detected != null)
            {
                return true;
            }
            else { return false; }
        }

        private Collider2D[] RayCastOnSelf()
        {
            return Physics2D.OverlapCircleAll(searchPoint.position, searchArea, interactLayer);
        }
        private Collider2D[] RaycastOnMouse()
        {
            return Physics2D.OverlapCircleAll(point, raycastRadius, interactLayer);
        }
        private bool CheckEnemy()
        {
            foreach (Collider2D hit in detected)
            {
                if(hit.tag == "Enemy")
                {
                    combatTarget = hit.transform.gameObject;
                    abilityHandler.Attack(combatTarget);
                    Debug.Log(hit + " is our target");
                    return true;
                }
                if(hit.tag == "NPC")
                { 
                    Debug.Log("You can't attack " + hit.transform.gameObject); return false; 
                }
            }
            return false;
        }
        private void CheckDirection()
        {
            if(combatTarget == null) { return; }
            if (combatTarget.transform.position.x > transform.position.x && !facingRight)
            {
                Flip();
            }
            if (combatTarget.transform.position.x < transform.position.x && facingRight)
            {
                Flip();
            }
        }
        private void Flip()
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            facingRight = !facingRight;
        }
        public void OnCollisionEnter2D(UnityEngine.Collision2D collision)
        {
            if (collision.collider is UnityEngine.Tilemaps.TilemapCollider2D)
            {
                foreach (var (_tilemap, _location) in Utils.Physics.GetColliderTiles(collision))
                {
                    var _tile = _tilemap.GetTile(_location);
                    if (_tile is ScriptableObjects.Tiles.Door)
                        ((ScriptableObjects.Tiles.Door)_tile).Open(_location, _tilemap);
                }
            }
        }
        public void SetMouseRay()
        {
            if (cam == null) return;
            point = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        void OnDrawGizmosSelected()
        {
            if (searchPoint == null) { return; };

            Gizmos.DrawWireSphere(searchPoint.position, searchArea);
        }
        public static bool IsNullOrFakeNull(GameObject aObj)
        {
            return aObj == null || aObj.Equals(null);
        }
    }
}
