using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;

namespace RPG.Managers
{
    public class GameOver : MonoBehaviour
    {
        public GameObject _player;
        public AudioSource _audioCue;
        public AudioClip _audioClip;
        private string scenePath;
        [UnityEngine.SerializeField] private ScriptableObjects.Destination destination = null;

        private Managers.PersistentManagers.ClientSequences.CampaignSequence campaignSequence = null;
        private void StartInterop()
        {
            if (campaignSequence != null) return;
            // @neuro: here we specifically return null from a getter when we mean
            // to, and if that's a null-like Unity object, we want this to error out.
    #pragma warning disable UNT0008 // Null propagation on Unity objects
            campaignSequence = Managers.PersistentManagers.ClientSequenceManager.Instance?.CampaignSequence;
    #pragma warning restore UNT0008 // Null propagation on Unity objects
            if (campaignSequence == null) return;
        }

        private void StopInterop()
        {
            if (campaignSequence == null) return;
            campaignSequence = null;
        }

        private void OnEnable()
        {
            StartInterop();
        }
        private void Start()
        {
            GameEvents.current.deathEvent += LoadGameOver;
        }
        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            StartInterop();
        }
        private void OnDisable()
        {
            GameEvents.current.deathEvent -= LoadGameOver;
            StopInterop();
        }

        private void OnDestroy()
        {
            GameEvents.current.deathEvent -= LoadGameOver;
            StopInterop();
        }
        private void LoadGameOver()
        {
            StartCoroutine(ChangeAudioClip());
            campaignSequence.TeleportViaCurtain(destination, _player);
        }
        private IEnumerator ChangeAudioClip()
        {
            yield return new WaitForSeconds(1f);
            _audioCue.clip = _audioClip;
            _audioCue.Play();
        }
    }
}