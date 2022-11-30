namespace RPG.Components.LevelDesign
{
    public class Portal : UnityEngine.MonoBehaviour
    {
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

        private void OnTriggerEnter2D(UnityEngine.Collider2D other)
        {
            var _go = other.gameObject;
            if (_go.CompareTag("Player")) {
                campaignSequence.TeleportViaCurtain(destination, _go);
            }
        }
    }

}