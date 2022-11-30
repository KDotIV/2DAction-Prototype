using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using RPG.ScriptableObjects;

namespace RPG.Managers
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader current;
        public GameObject _player;
        public float loadRange;
        public AudioSource _audioCue;
        public AudioClip _audioClip;
        private bool isPlaying;
        private bool isLoaded;
        private bool shouldLoad;
        [SerializeField] private Destination finalDest = null;


        private PersistentManagers.ClientSequences.CampaignSequence campaignSequence = null;
        private void Awake()
        {
            current = this;
        }
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
        private void OnDisable()
        {
            StopInterop();
        }

        private void OnDestroy()
        {
            StopInterop();
        }
        private IEnumerator ChangeAudioClip()
        {
            yield return new WaitForSeconds(1f);
            _audioCue.clip = _audioClip;
            _audioCue.Play();
        }
        // private IEnumerator LoadScene()
        // {
        //     yield return new WaitForSeconds(1f);
        //     SceneManager.LoadScene("GameOver", LoadSceneMode.Additive);
        // }
        public void LoadFinalScene()
        {
            campaignSequence.TeleportViaCurtain(finalDest, _player);
        }
    }
}
