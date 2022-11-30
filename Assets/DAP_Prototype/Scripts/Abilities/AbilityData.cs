using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Abilities
{
    [CreateAssetMenu(menuName = "Abilities/newAbility")]
    public class AbilityData : ScriptableObject
    {
        public string abilityName = "New Ability";
        public Material abilityImage;
        public GameObject[] animationPrefab;
        public AbilityType abilityType;
        public string animTrigger;
        public int damage = 4;
        public float hitRadius;
        public int statCheck = 8;
        public float coolDown;
        public float nextCoolDown = 1;
        public float travelDistance;

        public void ResetCooldown()
        {
            coolDown = 0;
        }
    }
}
