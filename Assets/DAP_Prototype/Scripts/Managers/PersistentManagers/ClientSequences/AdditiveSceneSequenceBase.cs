using RPG.Utils;
using static RPG.Core.Shared.Utils;

namespace RPG.Managers.PersistentManagers.ClientSequences
{
    public abstract class AdditiveSceneSequenceBase : ClientSequenceBase
    {
        public AdditiveSceneSequenceBase(IClientSequenceManager manager, SceneReference sceneRef)
            : base(manager)
        {
            this.sceneRef = sceneRef;
        }

        protected UnityEngine.SceneManagement.Scene? scene = null;
        protected SceneReference sceneRef = null;

        protected override System.Collections.IEnumerator PerformLoading(System.Action<float> progressCallback = null)
        {
            System.Exception exc = null;
            yield return Manager.StartCoroutine(
                ThrowingEnumerator(
                    Manager.LoadAdditiveScene(sceneRef, progressCallback),
                    (e, r) => { exc = e; if (r != null) scene = (UnityEngine.SceneManagement.Scene)r; }
                )
            );
            if (exc != null) throw exc;
            if (scene == null) throw new System.Exception("LoadAdditiveScene did not throw but did not load a scene either");
        }

        /// <remarks>
        ///     Note that this assumes an AudioListener component, if any, to be
        ///     activated resides on a root-level game object in the scene (such
        ///     as a camera, for example) and is enabled. Disabled AudioListener-s
        ///     are ignored for the purposes of RegisterActiveAudioListener calls,
        ///     so you'll have to call that manually if AudioListener in question
        ///     is to be enabled later.
        /// </remarks>
        protected override void PerformActivation(bool skipCamera = false)
        {
            if (scene == null) throw new System.Exception("Tried to activate a null scene");
            Manager.ActivateAdditiveScene(scene.Value, skipCamera);
        }

        protected override void PerformDeactivation()
        {
            if (scene == null) throw new System.Exception("Tried to activate a null scene");
            Manager.DeactivateAdditiveScene(scene.Value);
        }

        protected override System.Collections.IEnumerator PerformUnloading()
        {
            if (scene == null) yield break;
            System.Exception exc = null;
            yield return Manager.StartCoroutine(
                ThrowingEnumerator(
                    Manager.UnloadAdditiveScene(scene.Value),
                    (e, r) => { exc = e; }
                )
            );
            if (exc != null) throw exc;
            scene = null;
        }
    }
}
