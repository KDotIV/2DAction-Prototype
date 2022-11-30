namespace RPG.SceneScripts.ClientSequenceInterfaces
{
    using RPG.Managers.PersistentManagers;

    public class Intro : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private UnityEngine.GameObject skipPrompt = null;
        private Managers.PersistentManagers.ClientSequences.IntroSequence sequence = null;

        private void StartInterop()
        {
            if (sequence != null) return;
            // @neuro: here we specifically return null from a getter when we mean
            // to, and if that's a null-like Unity object, we want this to error out.
#pragma warning disable UNT0008 // Null propagation on Unity objects
            sequence = ClientSequenceManager.Instance?.IntroSequence;
#pragma warning restore UNT0008 // Null propagation on Unity objects
            if (sequence == null) return;
            sequence.NextSequenceReady += AttemptSkipPromptActivation;
        }

        private void StopInterop()
        {
            if (sequence == null) return;
            sequence.NextSequenceReady -= AttemptSkipPromptActivation;
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
            if (skipPrompt != null && skipPrompt.activeSelf && UnityEngine.Input.anyKeyDown)
                IntroDone();
        }

        public void IntroDone()
        {
            sequence?.OnIntroDone(this);
        }

        private void AttemptSkipPromptActivation(object sender, System.EventArgs _)
        {
            if (sequence == null) return;
            sequence.NextSequenceReady -= AttemptSkipPromptActivation;
            if (!sequence.IsIntroDone && skipPrompt != null && ! skipPrompt.activeSelf)
            {
                skipPrompt.SetActive(true);
            }
        }
    }
}
