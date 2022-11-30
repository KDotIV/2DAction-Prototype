using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.Managers
{
    public class UIManager : MonoBehaviour
    {
        public List<GameObject> prefabList;
        private List<GameObject> uiCanvases;
        void Start()
        {
            if(uiCanvases == null)
            {
                uiCanvases = new List<GameObject>();
                DontDestroyOnLoad(gameObject);
                // foreach (GameObject uiPrefab in prefabList)
                // {
                //     GameObject uiAdd = Instantiate(uiPrefab);
                //     Health player = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
                //     player.healthBar = uiAdd.GetComponent<HealthBarUI>(); 
                //     Debug.Log("Healthbar UI COMP... " + player.healthBar.name);
                //     uiAdd.name = uiPrefab.name;
                //     uiCanvases.Add(uiAdd);
                // }
            }   
        }
    }
}
