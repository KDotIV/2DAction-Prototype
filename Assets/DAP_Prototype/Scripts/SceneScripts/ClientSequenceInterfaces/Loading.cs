namespace RPG.SceneScripts.ClientSequenceInterfaces
{
    using RPG.Managers.PersistentManagers;

    public class Loading : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private UnityEngine.UI.Slider progressSlider = null;

        private Managers.PersistentManagers.ClientSequences.LoadingSequence sequence = null;

        private void StartInterop()
        {
            if (sequence != null) return;
            // @neuro: here we specifically return null from a getter when we mean
            // to, and if that's a null-like Unity object, we want this to error out.
#pragma warning disable UNT0008 // Null propagation on Unity objects
            sequence = ClientSequenceManager.Instance?.LoadingSequence;
#pragma warning restore UNT0008 // Null propagation on Unity objects
            if (sequence == null) return;
        }

        private void StopInterop()
        {
            if (sequence == null) return;
            sequence = null;
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

        private void Update()
        {
            if (progressSlider != null && sequence != null)
            {
                progressSlider.value = sequence.NextSequenceProgress;
            }
        }
    }
}
