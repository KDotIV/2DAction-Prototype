using System.Collections;
using System.Collections.Generic;
using RPG.Utils;
using static RPG.Core.Shared.Utils;

namespace RPG.Managers.PersistentManagers.ClientSequences
{
    [System.Serializable]
    public class CampaignSequence : IClientSequence
    {
        protected IClientSequenceManager manager = null;

        public CampaignSequence(IClientSequenceManager manager, SceneReference sceneRef)
        {
            this.manager = manager;
            currentAreaScenePath = RegisterSequence(sceneRef).ScenePath;
        }

#if UNITY_EDITOR
        public CampaignSequence(IClientSequenceManager manager, UnityEngine.SceneManagement.Scene editorScene)
        {
            this.manager = manager;
            currentAreaScenePath = RegisterSequence(
                AreaSequence.EditorMock(manager, editorScene)
            ).ScenePath;
        }
#endif

        #region area sequences

        private string currentAreaScenePath = null;

        private Dictionary<string, AreaSequence> areaSequences = new Dictionary<string, AreaSequence>();

        private AreaSequence RegisterSequence(SceneReference sceneRef)
        {
            if (areaSequences.ContainsKey(sceneRef.ScenePath))
                return areaSequences[sceneRef.ScenePath];
            return RegisterSequence(new AreaSequence(manager, sceneRef));
        }

        private AreaSequence RegisterSequence(AreaSequence seq)
        {
            if (areaSequences.ContainsKey(seq.ScenePath))
            {
                //return areaSequences[seq.ScenePath];
                throw new System.Exception(
                    "Area scene '" + seq.ScenePath + "' sequence got registered as an instance several times;" +
                    " this is probably something we don't want"
                );
            }
            seq.StateChanged += (object sender, GameSequence.StateChangedEventArgs e) => stateChanged?.Invoke(this, e);
            areaSequences.Add(seq.ScenePath, seq);
            return seq;
        }

        #endregion

        #region IClientSequence impl

        public GameSequence.State State {
            get {
                return areaSequences[currentAreaScenePath].State;
            }
        }

        protected System.EventHandler<GameSequence.StateChangedEventArgs> stateChanged;

        public event System.EventHandler<GameSequence.StateChangedEventArgs> StateChanged
        {
            add
            {
                stateChanged += value;
                value(this, new GameSequence.StateChangedEventArgs { oldState = State, newState = State });
            }
            remove { stateChanged -= value; }
        }

        public IEnumerator Load(System.Action<float> progressCallback = null)
        {
            System.Exception exc = null;
            yield return manager.StartCoroutine(
                ThrowingEnumerator(
                    areaSequences[currentAreaScenePath].Load(progressCallback),
                    (e, r) => { exc = e; }
                )
            );
            if (exc != null) throw exc;
        }

        public void Activate(bool skipCamera = false)
        {
            areaSequences[currentAreaScenePath].Activate(skipCamera);
        }

        public void Deactivate()
        {
            areaSequences[currentAreaScenePath].Deactivate();
        }

        public void Reset()
        {
            areaSequences[currentAreaScenePath].Reset();
        }

        public IEnumerator Unload()
        {
            System.Exception exc = null;
            yield return manager.StartCoroutine(
                ThrowingEnumerator(
                    areaSequences[currentAreaScenePath].Unload(),
                    (e, r) => { exc = e; }
                )
            );
            if (exc != null) throw exc;
        }

        #endregion

        #region area persistence

        private Dictionary<string, Core.Shared.Campaign.AreaState> areaStates = new Dictionary<string, Core.Shared.Campaign.AreaState>();

        public Core.Shared.Campaign.AreaState GetAreaState(string key)
        {
            if (!areaStates.ContainsKey(key))
            {
                areaStates[key] = new Core.Shared.Campaign.AreaState();
            }
            return areaStates[key];
        }

        public Core.Shared.Campaign.AreaState GetAreaState(UnityEngine.GameObject go)
        {
            return GetAreaState(go.scene.path);
        }

        #endregion

        #region tile state shortcuts

        public Core.Shared.Campaign.AreaStateData.ITileState GetTileState(
            UnityEngine.Tilemaps.Tilemap tilemap,
            UnityEngine.Vector3Int location
        )
        {
            return GetAreaState(tilemap.gameObject)?.GetTileState(
                new Core.Shared.Campaign.AreaState.TilePointer
                {
                    tilemap = tilemap.gameObject.name,
                    location = (location.x, location.y, location.z)
                }
            );
        }

        public Core.Shared.Campaign.AreaStateData.ITileState GetTileState(
            UnityEngine.Tilemaps.ITilemap tilemap,
            UnityEngine.Vector3Int location
        )
        {
            return GetTileState(
                tilemap.GetComponent<UnityEngine.Tilemaps.Tilemap>(),
                location
            );
        }

        public Core.Shared.Campaign.AreaStateData.ITileState RegisterTileState(
            UnityEngine.Tilemaps.Tilemap tilemap,
            UnityEngine.Vector3Int location,
            Core.Shared.Campaign.AreaStateData.ITileState state
        )
        {
            return GetAreaState(tilemap.gameObject)?.RegisterTileState(
                new Core.Shared.Campaign.AreaState.TilePointer
                {
                    tilemap = tilemap.gameObject.name,
                    location = (location.x, location.y, location.z)
                },
                state
            );
        }

        public Core.Shared.Campaign.AreaStateData.ITileState RegisterTileState(
            UnityEngine.Tilemaps.ITilemap tilemap,
            UnityEngine.Vector3Int location,
            Core.Shared.Campaign.AreaStateData.ITileState state
        )
        {
            return RegisterTileState(
                tilemap.GetComponent<UnityEngine.Tilemaps.Tilemap>(),
                location,
                state
            );
        }

        #endregion

        #region teleports and destinations

        private Dictionary<ScriptableObjects.Destination, UnityEngine.GameObject> destinations = new Dictionary<ScriptableObjects.Destination, UnityEngine.GameObject>();

        public void TeleportViaCurtain(ScriptableObjects.Destination dest, UnityEngine.GameObject player)
        {
            if (manager.CurtainSequence == null) throw new System.Exception("Curtain sequence is not initialized!");
            if (IsPaused) throw new System.Exception("Tried to teleport while paused - it's probably a TeleportViaCurtain re-entry bug!");
            Pause();
            AreaSequence _target_seq = RegisterSequence(dest.areaScene);
            bool _curtain_lowered = false;
            System.Action<System.Exception> _attempt_to_teleport = (System.Exception exc) => {
                if (exc != null)
                {
                    UnityEngine.Debug.LogError("_attempt_to_teleport received an exception signal");
                    UnityEngine.Debug.LogException(exc);
                    Unpause();
                    return;
                }
                if (! _curtain_lowered) return;
                if (_target_seq.ScenePath != currentAreaScenePath)
                {
                    if (_target_seq.State != GameSequence.State.INACTIVE) return;
                    Deactivate();
                    _target_seq.MoveObjectIntoSelf(player);
                    currentAreaScenePath = _target_seq.ScenePath;
                    Activate();
                }
                player.transform.position = destinations[dest].transform.position;
                manager.CurtainSequence.OnReadyToLift(this);
                {
                    System.EventHandler _cb = null;
                    manager.CurtainSequence.Lifted += _cb = (object sender, System.EventArgs e) => {
                        manager.CurtainSequence.Reset();
                        Unpause();
                    };
                }
            };
            // set up scene transition if necessary
            if (_target_seq.ScenePath != currentAreaScenePath) switch (_target_seq.State)
            {
                // if never loaded, or unloading, or already unloaded:
                case GameSequence.State.UNLOADING:
                case GameSequence.State.UNLOADED:
                    {
                        // wait for it to unload if in progress
                        System.EventHandler<GameSequence.StateChangedEventArgs> _cb = null;
                        _target_seq.StateChanged += _cb = (object sender, GameSequence.StateChangedEventArgs evt) => {
                            if (evt.newState != GameSequence.State.UNLOADED) return;
                            _target_seq.StateChanged -= _cb;
                            // load it again
                            manager.StartCoroutine(
                                ThrowingEnumerator(
                                    _target_seq.Load(),
                                    // when it's loaded, signal we're ready to teleport if other parts are ready
                                    (e, r) => _attempt_to_teleport(e)
                                )
                            );
                        };
                    }
                    break;
                // if already loading, or was loaded previously:
                case GameSequence.State.LOADING:
                case GameSequence.State.INACTIVE:
                    {
                        // wait for it to load if in progress
                        System.EventHandler<GameSequence.StateChangedEventArgs> _cb = null;
                        _target_seq.StateChanged += _cb = (object sender, GameSequence.StateChangedEventArgs e) => {
                            if (e.newState != GameSequence.State.INACTIVE) return;
                            _target_seq.StateChanged -= _cb;
                            // signal we're ready to teleport if other parts are ready
                            _attempt_to_teleport(null);
                        };
                    }
                    break;
                default:
                    throw new System.Exception(
                       "Teleport target's sequence (scene '" + _target_seq.ScenePath + "') is in a weird state"
                   );
            }
            // activate the curtain, upon which it starts to lower
            {
                System.EventHandler<GameSequence.StateChangedEventArgs> _cb = null;
                manager.CurtainSequence.StateChanged += _cb = (object sender, GameSequence.StateChangedEventArgs e) => {
                    if (e.newState != GameSequence.State.INACTIVE) return;
                    manager.CurtainSequence.StateChanged -= _cb;
                    manager.CurtainSequence.Activate(skipCamera: true);
                };
            }
            // when it's lowered, signal we're ready to teleport if other parts are ready
            {
                System.EventHandler _cb = null;
                manager.CurtainSequence.Lowered += _cb = (object sender, System.EventArgs e) => {
                    _curtain_lowered = true;
                    _attempt_to_teleport(null);
                };
            }
        }

        public void RegisterDestination(ScriptableObjects.Destination dest, UnityEngine.GameObject go)
        {
            if (destinations.ContainsKey(dest)) return;
            destinations.Add(dest, go);
        }

        #endregion

        #region gameplay

        public bool IsPaused { get; private set; } = false;

        private float fixedDeltaTime;

        public void Pause()
        {
            fixedDeltaTime = UnityEngine.Time.fixedDeltaTime;
            IsPaused = true;
            UnityEngine.Time.timeScale = 0f;
            UnityEngine.Time.fixedDeltaTime = 0f;
        }

        public void Unpause()
        {
            IsPaused = false;
            UnityEngine.Time.timeScale = 1f;
            UnityEngine.Time.fixedDeltaTime = fixedDeltaTime;
        }

        #endregion
    }
}
