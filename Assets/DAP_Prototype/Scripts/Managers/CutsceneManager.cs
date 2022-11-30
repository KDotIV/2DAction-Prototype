using UnityEngine;
using System;
using RPG.DiagSystem;
using RPG.Controllers;

namespace RPG.Managers
{
    public class CutsceneManager : MonoBehaviour
    {
        public static CutsceneManager m_CutsceneManager;
        private PlayerConvo _playerConvo;
        public Dialogue dialogue;
        public GameObject speaker;

        //BROADCAST
        public event Action npcMovePath;

        //LISTENERS
        private void Start()
        {
            GameEvents.current.onCutsceneEnter += PlayCutscene;
            _playerConvo = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerConvo>();
        }
        private void Awake()
        {
            m_CutsceneManager = this;
        }
        private void OnDisable()
        {
            GameEvents.current.onCutsceneEnter -= PlayCutscene;
        }
        private void PlayCutscene()
        {
            Debug.Log("Cutscene was called...");
            if(dialogue != null)
            {
                if(speaker != null) { SpeakerHandler.CreateSpeakerLog(speaker); }
                _playerConvo.StartDialogue(dialogue);
            }
            TriggerPathMovement();
        }

        //BROADCASTERS
        public void TriggerPathMovement() => npcMovePath?.Invoke();
    }
}
