using UnityEngine;
using UnityEngine.Tilemaps;

namespace RPG.ScriptableObjects
{
    [CreateAssetMenu(fileName = "New Destination", menuName = "Destination")]
    public class Destination : ScriptableObject
    {
        public Utils.SceneReference areaScene;

        public string GetScenePath()
        {
            return areaScene.ScenePath;
        }
    }
}