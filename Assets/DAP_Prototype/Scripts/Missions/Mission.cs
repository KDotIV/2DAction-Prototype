using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Missions
{
    [CreateAssetMenu(fileName = "Missions", menuName = "Missions/New Mission")]
    public class Mission : ScriptableObject
    {
        [SerializeField]
        string[] tasks;

        public IEnumerable<string> GetTasks()
        {
            yield return "Task 1";
            Debug.Log("Do some work");
            yield return "Task 2";
        }
    }
}
