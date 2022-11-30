using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Attributes;
using RPG.Abilities;

namespace RPG.Data
{
    public class CharacterData : MonoBehaviour
    {
        protected CoreAttributes coreAttributes;

        void Start()
        {
            Initialize();
        }

        void Update()
        {

        }

        public void Initialize()
        {
            coreAttributes = GetComponent<CoreAttributes>();
            coreAttributes.PropsCurrentStam = coreAttributes.PropsMaxStam;
        }
    }
}
