namespace RPG.Components.LevelDesign
{
    public class Destination : UnityEngine.MonoBehaviour
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
            if (destination != null)
                campaignSequence.RegisterDestination(destination, this.gameObject);
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

#if UNITY_EDITOR
        void OnValidate()
        {
            if (destination != null)
            {
                var _destScenePath = destination.areaScene.ScenePath;
                if (
                    gameObject.scene != null
                    && gameObject.scene.path != null
                    && _destScenePath != gameObject.scene.path
                )
                {
                    destination = null;
                    throw new System.Exception(
                        "Cannot assign destination for scene '" + _destScenePath + "'" +
                        " onto an object from scene '" + gameObject.scene.path + "'"
                    );
                }
            }
        }
#endif
    }

}