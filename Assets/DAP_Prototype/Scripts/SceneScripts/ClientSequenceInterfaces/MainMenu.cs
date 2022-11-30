namespace RPG.SceneScripts.ClientSequenceInterfaces
{
    using RPG.Managers.PersistentManagers;

    public class MainMenu : UnityEngine.MonoBehaviour
    {
        [UnityEngine.SerializeField] private UnityEngine.AudioSource backgroundMusic = null;
        private Managers.PersistentManagers.ClientSequences.MainMenuSequence sequence = null;
        private IAudioSourceManager audioSourceManager = null;

        private void StartInterop()
        {
            if (sequence != null) return;
            // @neuro: here we specifically return null from a getter when we mean
            // to, and if that's a null-like Unity object, we want this to error out.
#pragma warning disable UNT0008 // Null propagation on Unity objects
            sequence = ClientSequenceManager.Instance?.MainMenuSequence;
            audioSourceManager = AudioSourceManager.Instance;
#pragma warning restore UNT0008 // Null propagation on Unity objects
        }

        private void StopInterop()
        {
            sequence = null;
            audioSourceManager = null;
        }

        private void OnEnable()
        {
            StartInterop();
        }

        private void Start()
        {
            StartInterop();
            if (audioSourceManager != null && backgroundMusic != null)
                audioSourceManager.PlayNonDiegeticBackgroundSource(
                    audioSourceManager.CreateManagedAudioSourceFromTemplate(backgroundMusic)
                );
                
        }

        private void OnDisable()
        {
            StopInterop();
        }

        private void OnDestroy()
        {
            StopInterop();
        }
        public void NewGame()
        {
            sequence?.OnNewGame(this);
        }

        public void Quit()
        {
            sequence?.OnQuit(this);
        }
    }
}
