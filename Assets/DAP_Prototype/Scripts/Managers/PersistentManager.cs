namespace RPG.Managers
{
    /// <summary>
    ///     This type of component is supposed to start with the game and never be
    ///     disabled during runtime until the game shuts down.
    ///     
    ///     E.g. app state manager, or input manager.
    ///     
    ///     There can be only one of these in memory, so it makes sense to expose
    ///     them in a singleton-esque manner (this should not prevent testing as
    ///     long as such a class implements an interface, and references to these
    ///     can be injected into dependants).
    ///     
    ///     These all should probably sit on a special game object in the
    ///     preload/main scene and be configured from there.
    ///     
    ///     Then, in a build, accessing a non-initialized static instance of this
    ///     means there is a bug in initialization logic.
    ///     
    ///     In editor, however, a scene that's supposed to be loaded additively
    ///     could try to access this, expecting that the initialization happened
    ///     earlier.
    ///     
    ///     Therefore, such components should define initial data for this kind
    ///     of editor-time scenario, and expose their state in a way that allows
    ///     a developer to tweak it once they enter play-mode during editor-time.
    /// </summary>
    public class PersistentManager<TSelf, TInterface> : UnityEngine.MonoBehaviour
        where TSelf : PersistentManager<TSelf, TInterface>, TInterface
        where TInterface : class
    {
        private static readonly object threadLock = new object();
        private static TSelf instance = null;
        protected static bool EditorMockMode { get; private set; } = false;
#if UNITY_EDITOR
        private static bool shuttingDown = false;

        protected virtual void OnApplicationQuit()
        {
            shuttingDown = true;
        }

        protected virtual void OnDestroy()
        {
            shuttingDown = true;
        }

        public static TInterface Instance
        {
            get
            {
                if (instance == null)
                {
                    UnityEngine.Debug.Log(
                        "[PersistentManager] Instance '" + typeof(TSelf) + " : " + typeof(TInterface) + "'" +
                            " accessed before assignment in Awake." +
                            " If this happened because an additive scene is being edited," +
                            " an instance will be auto-created."
                    );
                    if (shuttingDown)
                    {
                        UnityEngine.Debug.LogWarning(
                            "[PersistentManager] Instance '" + typeof(TSelf) + " : " + typeof(TInterface) + "'" +
                                " accessed during shutdown. Returning null."
                        );
                        return null;
                    }
                    lock (PersistentManagersSharedThreadLock.threadLock) lock (threadLock)
                    {
                        if (instance == null)
                        {
                            var exists = (TSelf)FindObjectOfType(typeof(TSelf));
                            if (exists != null)
                            {
                                string _err = "[PersistentManager] Instance '" + typeof(TSelf) + " : " + typeof(TInterface) + "'" +
                                    " accessed before assignment in Awake," +
                                    " but an object exists. This should not happen.";
                                UnityEngine.Debug.LogError(_err, exists.gameObject);
                                throw new System.Exception(_err);
                            }
                            EditorMockMode = true;
                            var PersistentManagersObject = new UnityEngine.GameObject();
                            instance = PersistentManagersObject.AddComponent<TSelf>();
                            PersistentManagersObject.hideFlags = UnityEngine.HideFlags.DontSave;
                            PersistentManagersObject.name = "PersistentManagers" +
                                " (auto-created from '" + typeof(TSelf) + " : " + typeof(TInterface) + "')";
                            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(
                                PersistentManagersObject,
                                // This will insert a new scene additively.
                                UnityEngine.SceneManagement.SceneManager.CreateScene(
                                    "StartupEditorMock" +
                                    " (auto-created from '" + typeof(TSelf) + " : " + typeof(TInterface) + "')"
                                )
                            );
                        }
                    }
                }
#else
        public static TSelf Instance
        {
            get
            {
                if (instance == null)
                {
                    string _err = "[PersistentManager] Instance '" + typeof(TSelf) + " : " + typeof(TInterface) + "'" +
                        " accessed before assignment in Awake. This should not happen.";
                    UnityEngine.Debug.LogError(_err);
                    throw new System.Exception(_err);
                }
#endif
                return instance;
            }
        }

        private void SetInstance(TSelf value)
        {
            lock (threadLock)
            {
                if (instance != null)
                {
                    string _err = "[PersistentManager] Instance '" + typeof(TSelf) + " : " + typeof(TInterface) + "'" +
                        " already assigned. This should not happen.";
                    UnityEngine.Debug.LogError(_err, instance.gameObject);
                    throw new System.Exception(_err);
                }
                instance = value;
            }
        }

        protected virtual void Awake()
        {
            SetInstance((TSelf)this);
        }
    }

#if UNITY_EDITOR
    /// <summary>
    ///     PersistentManagers most likely would be on a single game object,
    ///     therefore when auto-creating said object we should use a shared lock.
    ///     This is basically a friend class, which C# does not have.
    ///     Don't use this anywhere else.
    /// </summary>
    internal class PersistentManagersSharedThreadLock
    {
        public static object threadLock = new object();
    }
#endif
}