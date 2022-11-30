namespace RPG.SceneScripts.ClientSequenceInterfaces
{
    using RPG.Managers.PersistentManagers;

    public class Curtain : UnityEngine.MonoBehaviour
    {
        private Managers.PersistentManagers.ClientSequences.CurtainSequence sequence = null;
        private UnityEngine.Animator animator;

        private void StartInterop()
        {
            if (sequence != null) return;
            // @neuro: here we specifically return null from a getter when we mean
            // to, and if that's a null-like Unity object, we want this to error out.
#pragma warning disable UNT0008 // Null propagation on Unity objects
            sequence = ClientSequenceManager.Instance?.CurtainSequence;
#pragma warning restore UNT0008 // Null propagation on Unity objects
            if (sequence == null) return;
            animator = GetComponent<UnityEngine.Animator>();
            if (animator == null) return;
            {
                System.EventHandler _cb = null;
                sequence.ReadyToLift += _cb = (object sender, System.EventArgs e) => {
                    sequence.ReadyToLift -= _cb;
                    animator.SetTrigger("readyToLift");
                };
            }
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

        public void Lowered()
        {
            sequence?.OnLowered(this);
        }

        public void Lifted()
        {
            sequence?.OnLifted(this);
        }
    }
}
