using RPG.Utils;

namespace RPG.Managers.PersistentManagers.ClientSequences
{
    [System.Serializable]
    public class AreaSequence : AdditiveSceneSequenceBase
    {
        public AreaSequence(IClientSequenceManager manager, SceneReference sceneRef)
            : base(manager, sceneRef) { }

        public string ScenePath { get { return sceneRef.ScenePath; } }

        public void MoveObjectIntoSelf(UnityEngine.GameObject o)
        {
            var _wasActive = o.activeSelf;
            o.SetActive(false);
            o.transform.parent = null; // necessary for MoveGameObjectToScene to work
            UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(o, scene.Value);
            if (_wasActive) o.SetActive(true);
        }

#if UNITY_EDITOR
        public static AreaSequence EditorMock(
            IClientSequenceManager manager,
            UnityEngine.SceneManagement.Scene scene
        )
        {
            var _sr = new SceneReference();
            _sr.ScenePath = scene.path;
            var _o = new AreaSequence(manager, _sr)
            {
                scene = scene,
                State = GameSequence.State.ACTIVE
            };
            return _o;
        }
#endif
    }
}
