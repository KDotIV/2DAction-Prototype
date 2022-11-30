using System.Collections.Generic;
using RPG.Managers.PersistentManagers.ClientSequences;
using static RPG.Core.Shared.Utils;
using RPG.Utils;

namespace RPG.Managers.PersistentManagers
{
    /// <summary>
    ///     Defines "intro - main menu - loading - campaign" flow.
    ///     
    ///     Should not be ever disabled during runtime.
    /// </summary>
    ///
    /// TODO can't see shit in the editor due to sequences having an abstract base
    ///     might solve this via SO-s
    ///         or not
    ///         let's see how much pain this brings
    ///
    [System.Serializable] public class ClientSequenceManager : PersistentManager<ClientSequenceManager, IClientSequenceManager>, IClientSequenceManager
    {
        private IAudioListenerManager audioListenerManager = null;

        [UnityEngine.SerializeField] private IntroSequence introSequence = null;
        public IntroSequence IntroSequence { get { return introSequence; } private set { introSequence = value; } }
        [UnityEngine.SerializeField] private SceneReference introSequenceScene = null;

        [UnityEngine.SerializeField] private MainMenuSequence mainMenuSequence = null;
        public MainMenuSequence MainMenuSequence { get { return mainMenuSequence; } private set { mainMenuSequence = value; } }
        [UnityEngine.SerializeField] private SceneReference mainMenuSequenceScene = null;

        [UnityEngine.SerializeField] private LoadingSequence loadingSequence = null;
        public LoadingSequence LoadingSequence { get { return loadingSequence; } private set { loadingSequence = value; } }
        [UnityEngine.SerializeField] private SceneReference loadingSequenceScene = null;

        [UnityEngine.SerializeField] private CampaignSequence campaignSequence = null;
        public CampaignSequence CampaignSequence { get { return campaignSequence; } private set { campaignSequence = value; } }
        [UnityEngine.SerializeField] private List<SceneReference> campaignSequenceScenes = new List<SceneReference>();

        [UnityEngine.SerializeField] private CurtainSequence curtainSequence = null;
        public CurtainSequence CurtainSequence { get { return curtainSequence; } private set { curtainSequence = value; } }
        [UnityEngine.SerializeField] private SceneReference curtainSequenceScene = null;

        protected override void Awake()
        {
            base.Awake();
#if UNITY_EDITOR
            if (EditorMockMode)
            {
                // we'll need the curtain for campaign mock
                curtainSequenceScene = new SceneReference();
                curtainSequenceScene.ScenePath = "Assets/DAP_Prototype/Scenes/Curtain.unity";
            }
#endif
            // Start loading curtain.
            CurtainSequence = new CurtainSequence(this, curtainSequenceScene);
            StartCoroutine(
                ThrowingEnumerator(
                    CurtainSequence.Load(),
                    (e, r) => HandleUnrecoverableException(e))
            );
#if UNITY_EDITOR
            if (EditorMockMode)
            {
                // Active scene is the last one to load non-additively, so in EditorMockMode
                // scenario it'll be the one that was loaded directly in editor.
                CampaignSequence = new CampaignSequence(this, UnityEngine.SceneManagement.SceneManager.GetActiveScene());
                // Wouldn't wanna start loading usual scenes if it's a mock that was
                // auto-created because a scene needed one in editor.
                return;
            }
#endif
            // Start loading intro.
            IntroSequence = new IntroSequence(this, introSequenceScene);
            StartCoroutine(
                ThrowingEnumerator(
                    IntroSequence.Load(),
                    (e, r) => {
                        if (HandleUnrecoverableException(e)) return;
                        try {
                            IntroSequence.Activate();
                        }
                        catch (System.Exception ex)
                        {
                            HandleUnrecoverableException(ex);
                            return;
                        }
                    }
                )
            );
            // Start loading main menu.
            bool _mainMenuReady = false;
            bool _loadingReady = false;
            MainMenuSequence = new MainMenuSequence(this, mainMenuSequenceScene);
            StartCoroutine(
                ThrowingEnumerator(
                    MainMenuSequence.Load(),
                    (e, r) => {
                        if (HandleUnrecoverableException(e)) return;
                        _mainMenuReady = true;
                        if (_loadingReady) IntroSequence.OnNextSequenceReady();
                    }
                )
            );
            // Start loading loading (sorry) screen.
            LoadingSequence = new LoadingSequence(this, loadingSequenceScene);
            StartCoroutine(
                ThrowingEnumerator(
                    LoadingSequence.Load(),
                    (e, r) => {
                        if (HandleUnrecoverableException(e)) return;
                        _loadingReady = true;
                        if (_mainMenuReady) IntroSequence.OnNextSequenceReady();
                    }
                )
            );
            // Start main menu and unload intro as soon as the former is ready
            // to be shown and the latter is done.
            IntroSequence.IntroDone += AttemptIntroToMainMenuTransition;
            IntroSequence.NextSequenceReady += AttemptIntroToMainMenuTransition;
        }
        protected void Start()
        {
            // @neuro: here we specifically return null from a getter when we mean
            // to, and if that's a null-like Unity object, we want this to error out.
#pragma warning disable UNT0008 // Null propagation on Unity objects
            audioListenerManager = AudioListenerManager.Instance;
#pragma warning restore UNT0008 // Null propagation on Unity objects
            if (audioListenerManager == null)
            {
                string _err = "[ClientSequenceManager] AudioListenerManager.Instance returned null in Start. This should not happen.";
                UnityEngine.Debug.LogError(_err, gameObject);
                throw new System.Exception(_err);
            }
        }

        private void AttemptIntroToMainMenuTransition(object sender, System.EventArgs _)
        {
            if (! IntroSequence.IsIntroDone) return;
            if (! IntroSequence.IsNextSequenceReady) return;
            IntroSequence.IntroDone -= AttemptIntroToMainMenuTransition;
            IntroSequence.NextSequenceReady -= AttemptIntroToMainMenuTransition;
            IntroSequence.Deactivate();
            MainMenuSequence.Activate();
            StartCoroutine(
                ThrowingEnumerator(
                    IntroSequence.Unload(),
                    (e, r) => {
                        if (HandleUnrecoverableException(e)) return;
                    }
                )
            );
        }

        private void AttemptLoadingToCampaignTransition(object sender, System.EventArgs _)
        {
            if (!LoadingSequence.IsNextSequenceReady) return;
            LoadingSequence.NextSequenceReady -= AttemptLoadingToCampaignTransition;
            LoadingSequence.Deactivate();
            CampaignSequence.Activate();
        }

        public void NewGame(object sender)
        {
            MainMenuSequence.Deactivate();
            // @neuro TODO: instead of just putting music in scenes, we should implement
            // a "jukebox" system, so that scenes and/or sequences schedule music
            // but the system handles transitions.
            LoadingSequence.Reset();
            LoadingSequence.Activate();
            // @neuro TODO: instead of hard-coding the scene index here we should
            // store it in the default profile SO, so that it could be redefined
            // in local override files. Don't have the infra yet, therefore this.
            CampaignSequence = new CampaignSequence(this, campaignSequenceScenes[0]);
            StartCoroutine(
                ThrowingEnumerator(
                    CampaignSequence.Load((p) => { LoadingSequence.NextSequenceProgress = p; }),
                    (e, r) =>
                    {
                        if (HandleUnrecoverableException(e)) return;
                        LoadingSequence.OnNextSequenceReady();
                    }
                )
            );
            LoadingSequence.NextSequenceReady += AttemptLoadingToCampaignTransition;
        }

        #region Additive Scene management

        public System.Collections.IEnumerator LoadAdditiveScene(SceneReference sceneRef, System.Action<float> progressCallback = null)
        {
            UnityEngine.SceneManagement.Scene? scene = null;
            if (progressCallback != null)
            {
                progressCallback(0.0f);
                yield return null; // @neuro: have at least 1 frame with 0% progress
            }
            UnityEngine.AsyncOperation _op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(
                sceneRef,
                UnityEngine.SceneManagement.LoadSceneMode.Additive
            );
            System.Action<UnityEngine.AsyncOperation> _onCompleted = null;
            _op.completed += _onCompleted = (_) =>
            {
                // awake and onEnable were called for scene's GOs, nothing we can do about it
                _op.completed -= _onCompleted;
                scene = UnityEngine.SceneManagement.SceneManager.GetSceneByPath(sceneRef);
                foreach (var _go in scene?.GetRootGameObjects()) _go.SetActive(false);
                // onDisable has just been called for scene's GOs
            };
            while (!_op.isDone)
            {
                progressCallback?.Invoke(_op.progress);
                yield return null;
            }
            if (progressCallback != null)
            {
                progressCallback(_op.progress);
                yield return null; // @neuro: have at least 1 frame with 100% progress
            }
            yield return scene;
        }

        public void ActivateAdditiveScene(UnityEngine.SceneManagement.Scene scene, bool skipCamera = false)
        {
            foreach (var _go in scene.GetRootGameObjects()) if (! skipCamera || _go.GetComponent<UnityEngine.Camera>() == null)
            {
                _go.SetActive(true);
                var _al = _go.GetComponent<UnityEngine.AudioListener>();
                if (_al != null && _al.enabled) audioListenerManager.RegisterActiveAudioListener(_al);
            }
        }

        public void DeactivateAdditiveScene(UnityEngine.SceneManagement.Scene scene)
        {
            foreach (var _go in scene.GetRootGameObjects())
            {
                var _al = _go.GetComponent<UnityEngine.AudioListener>();
                if (_al != null && _go.activeInHierarchy) audioListenerManager.UnregisterActiveAudioListener(_al);
                _go.SetActive(false);
            }
        }
        public System.Collections.IEnumerator UnloadAdditiveScene(UnityEngine.SceneManagement.Scene scene)
        {
            UnityEngine.AsyncOperation _op = UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(
                scene,
                UnityEngine.SceneManagement.UnloadSceneOptions.UnloadAllEmbeddedSceneObjects
            );
            while (!_op.isDone) yield return null;
        }

        #endregion

        #region Misc utils

        private bool HandleUnrecoverableException(System.Exception e)
        {
            if (e == null) return false;
            UnityEngine.Debug.LogException(e, this);
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPaused = true;
#else
            UnityEngine.Application.Quit();
#endif
            return true;
        }

        #endregion
    }
}