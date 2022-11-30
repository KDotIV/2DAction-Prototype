using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Missions
{
    public class MissionsUI : MonoBehaviour
    {
        [SerializeField]
        Mission mission;

        private void Start()
        {
            foreach (string task in mission.GetTasks())
            {
                Debug.Log($"Has tasks: {task}");
            }
        }
    }
}
