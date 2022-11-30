using System.Collections;
using static RPG.Core.Shared.Utils;

namespace RPG.Managers
{
    using GameSequence;

    /// <summary>
    ///     A game sequence is basically a combination of message handlers and
    ///     callbacks to communicate its state back to the manager.
    ///     Sequence state transitions are defined as follows:
    ///     
    ///         (instantiation)
    ///            |
    ///            V
    ///          UNLOADED state <-----------------------
    ///            |                                    |
    ///          (Load)                                 |
    ///            |                                    |
    ///            V                                    |
    ///          LOADING state                          |
    ///            |                                    |
    ///            |        ----------------------      |
    ///            V       |                      |     |
    ///        - INACTIVE state <---------------(Reset) |
    ///       |    |              |               |     |
    ///       |  (Activate)   (Deactivate)        |     |
    ///       |    |              |               |     |
    ///       |    V              |               |     |
    ///       |  ACTIVE state --------------------      |
    ///       |                                         |
    ///        --(Unload)                               |
    ///            |                                    |
    ///            |                                    |
    ///            V                                    |
    ///          UNLOADING state                        |
    ///            |                                    |
    ///             ------------------------------------
    ///     
    ///     Load and Unload transitions are guaranteed to take at least a single
    ///     frame.
    ///
    ///     Activate, Deactivate and Reset transitions are supposed to finish
    ///     during the current frame.
    ///     
    ///     Note in relation to IGameSequenceManager: this is sort of like a
    ///     state, but not really: see the note in IGameSequenceManager for more
    ///     info.
    ///     
    ///     Note: this does remind of a Unity MonoBehaviour interface, but it is
    ///     not. Implementing this with MonoBehaviours means dealing with
    ///     lifecycle callback quirks; e.g. to provide sequence object fields
    ///     with values before activation you have to invoke setters before
    ///     Awake is called, to which end you have to disable the hosting GO
    ///     first, which nukes coroutines and it's unclear if it forces other
    ///     MonoBehaviours' onDisable being called right away, and it's all a
    ///     mess - if not to implement but to provide a consistent interface
    ///     that will not break if we are to update our Unity version.
    /// </summary>
    /// <remarks>
    ///     @neuro: I wonder if this should be made thread-safe? Probably not,
    ///     since coroutines are single-threaded, and scene loading (which will
    ///     most likely drive much of (un-)loading behaviour), while it's on
    ///     another thread, is not supposed to introduce any contention for
    ///     GameSequence instances.
    ///
    ///     @neuro: This could be moved to Core.Shared, but starting coroutines
    ///     needs to be abstracted out first.
    /// </remarks>
    public abstract class GameSequenceBase : IGameSequence
    {
        public State State { get; protected set; } = State.UNLOADED;

        protected virtual IGameSequenceManager Manager { get; set; } = null;

        public GameSequenceBase(IGameSequenceManager manager)
        {
            this.Manager = manager;
        }

        private void ChangeState(State targetState)
        {
            if (targetState == State) throw new InvalidTransitionException(
                "Already in state " + State.ToString("g")
            );
            // Actual checks for whether current state can transition into
            // target state are done in state transition methods.
            var _oldState = State;
            State = targetState;
            stateChanged?.Invoke(
                this,
                new StateChangedEventArgs { oldState = _oldState, newState = State }
            );
        }

        #region Event management

        protected System.EventHandler<StateChangedEventArgs> stateChanged;

        public event System.EventHandler<StateChangedEventArgs> StateChanged
        {
            add
            {
                stateChanged += value;
                value(this, new StateChangedEventArgs { oldState = State, newState = State });
            }
            remove { stateChanged -= value; }
        }

        #endregion

        #region State transition methods

        public IEnumerator Load(System.Action<float> progressCallback = null)
        {
            if (State == State.LOADING) yield break;
            if (State != State.UNLOADED) throw new InvalidTransitionException(
                "Cannot Load while in " + State.ToString("g")
            );
            ChangeState(State.LOADING);
            System.Exception exc = null;
            yield return Manager.StartCoroutine(
                ThrowingEnumerator(
                    PerformLoading(progressCallback),
                    (e, r) => { exc = e; }
                )
            );
            if (exc != null) throw exc;
            ChangeState(State.INACTIVE);
        }

        public void Activate(bool skipCamera = false)
        {
            if (State == State.ACTIVE) return;
            if (State != State.INACTIVE) throw new InvalidTransitionException(
                "Cannot Activate while in " + State.ToString("g")
            );
            PerformActivation(skipCamera);
            ChangeState(State.ACTIVE);
        }

        public void Deactivate()
        {
            if (State == State.INACTIVE) return;
            if (State != State.ACTIVE) throw new InvalidTransitionException(
                "Cannot Deactivate while in " + State.ToString("g")
            );
            PerformDeactivation();
            ChangeState(State.INACTIVE);
        }

        public void Reset()
        {
            Deactivate();
            PerformResetting();
        }

        public IEnumerator Unload()
        {
            if (State == State.UNLOADING) yield break;
            if (State != State.INACTIVE) throw new InvalidTransitionException(
                "Cannot Unload while in " + State.ToString("g")
            );
            ChangeState(State.UNLOADING);
            System.Exception exc = null;
            yield return Manager.StartCoroutine(
                 ThrowingEnumerator(
                    PerformUnloading(),
                    (e, r) => { exc = e; }
                )
            );
            if (exc != null) throw exc;
            ChangeState(State.UNLOADED);
        }

        #endregion

        #region Work to be done during state transitions - override these!

        protected virtual IEnumerator PerformLoading(System.Action<float> progressCallback = null)
        {
            yield return null; // does nothing by default
        }

        /// <summary>
        ///     Has to finish during the current frame!
        /// </summary>
        protected virtual void PerformActivation(bool skipCamera = false)
        {
            // does nothing by default
        }

        /// <summary>
        ///     Has to finish during the current frame!
        /// </summary>
        protected virtual void PerformDeactivation()
        {
            // does nothing by default
        }

        /// <summary>
        ///     Has to finish during the current frame!
        /// </summary>
        protected virtual void PerformResetting()
        {
            // does nothing by default
        }

        protected virtual IEnumerator PerformUnloading()
        {
            yield return null; // does nothing by default
        }

        #endregion
    }
}