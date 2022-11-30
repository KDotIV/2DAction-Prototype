using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.ScriptableObjects;

namespace RPG.Managers
{
    public class Continue : MonoBehaviour
    {
        public GameObject _player;
        private string scenePath;
        [SerializeField] private ScriptableObjects.Destination exploreDest = null;
        [SerializeField] private ScriptableObjects.Destination digSiteDest = null;
        [SerializeField] private ScriptableObjects.Destination dungeonDest = null;
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

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");
            StartInterop();
        }
        private void Update() 
        {
            switch (Input.inputString)
            {
                case "y":
                    Debug.Log("Loaded Explorer Scene");
                    GetContinue(exploreDest);
                break;
                case "t":
                    GetContinue(digSiteDest);
                break;
                case "m":
                    GetContinue(dungeonDest);
                break;
            }
        }
        private void OnDisable()
        {
            StopInterop();
        }

        private void OnDestroy()
        {
            StopInterop();
        }
    private void GetContinue(Destination destination)
    {

        campaignSequence.TeleportViaCurtain(destination, _player);
    }
    }
}