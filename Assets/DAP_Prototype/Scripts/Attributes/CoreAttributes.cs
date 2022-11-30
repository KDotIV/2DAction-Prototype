using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Attributes
{
    public class CoreAttributes : MonoBehaviour
    {
        #region Fields
        [SerializeField] private string charName;
        [SerializeField] private string charID;
        [SerializeField] private int maxStam = 5;
        [SerializeField] public int currentStam;
        [SerializeField] private float baseDamage;
        [SerializeField] private int initiative;
        private float currentDamage;
        [SerializeField] private int baseDef;
        private int currentDef;
        public bool isHostile;
        [SerializeField] public Stats charStats {get; private set;}
        [SerializeField] public Health charHealth { get; private set; }
        #endregion
        private float timeSinceAbility;
        private float timeUntilStamRegen = 1f;
        public bool surprised = false;
        public Image[] diamonds;
        public Sprite fullStam;
        public Sprite emptyStam;

        public static List<CoreAttributes> AllChars { get; set; }

        void Awake()
        {
            charStats = GetComponent<Stats>();
            charHealth = GetComponent<Health>();
        }
        private void OnEnable()
        {
            if (AllChars == null) { AllChars = new List<CoreAttributes>(); }
            AllChars.Add(this);

            for (var i = 0; i < AllChars.Count; i++)
            {
                Debug.Log("Characters Loaded: " + AllChars[i]);
            }
        }
        private void OnDisable()
        {
            AllChars.Remove(this);
            for (var i = 0; i < AllChars.Count; i++)
            {
                Debug.Log("Characters Maintained: " + AllChars[i]);
            }
        }
        void FixedUpdate() 
        {
            //UpdateStam();
        }

        public bool HasStamina(int _stam)
        {
            return currentStam >= _stam;
        }

        public void ChangeStamina(int _stam)
        {
            currentStam = Mathf.Clamp(_stam += currentStam, 0, maxStam);
        }
        public void UpdateStam()
        {
            timeSinceAbility += Time.deltaTime;
            if (timeSinceAbility > timeUntilStamRegen && currentStam != maxStam)
            {
                ChangeStamina(1);
                timeSinceAbility = 0;
                Debug.Log("Current: " + currentStam);
            }
            if(currentStam == maxStam)
            {
                timeSinceAbility = 0;
            }
            for (int i = 0; i < diamonds.Length; i++)
            {
                if(i < currentStam)
                {
                    diamonds[i].sprite = fullStam;
                } else { diamonds[i].sprite = emptyStam; }
                if(i < maxStam)
                {
                    diamonds[i].enabled = true;
                } else { diamonds[i].enabled = false; }
            }
        }
        public void SurpriseRound(bool isSurprised)
        {
            if(isSurprised) { surprised = true;} 
            else { surprised = false; }
        }

        #region CoreStat Getters/Setters

        public string GetName()
        {
            return charName;
        }
        public string GetID()
        {
            return charID;
        }
        public static IEnumerable<CoreAttributes> GetAllChars()
        {
            return AllChars;
        }
        public CoreAttributes GetPlayer()
        {
            if(this.tag == "Player") { return this; }
            return null;
        }
        public static List<CoreAttributes> GetEnemies(List<CoreAttributes> AllChars)
        {
            List<CoreAttributes> _enemiesList = new List<CoreAttributes>();
            foreach (CoreAttributes enemy in AllChars)
            {
                if(enemy.isHostile == true)
                {
                    _enemiesList.Add(enemy);
                }
            }
            return _enemiesList;
        }
        public int PropsInitiative
        {
            get { return initiative; }
            set { initiative = value; }
        }
        public int PropsCurrentStam
        {
            get { return currentStam; }
            set { currentStam = value; }
        }
        public int PropsMaxStam
        {
            get { return maxStam; }
            set { maxStam = value; }
        }
        public float PropsTimeUntilStamRegen
        {
            get { return timeUntilStamRegen; }
            set { timeUntilStamRegen = value; }
        }
        public float PropsTimeSinceAbility
        {
            get { return timeSinceAbility; }
            set { timeSinceAbility = value; }
        }
        public float PropsCurrentDamage
        {
            get { return currentDamage; }
            set { currentDamage = value; }
        }
        public float PropsBaseDamage
        {
            get { return baseDamage; }
            set { baseDamage = value; }
        }
        public int PropsBaseDef
        {
            get { return baseDef; }
            set { baseDef = value; }
        }
        public int PropsCurrentDef
        {
            get { return currentDef; }
            set { currentDef = value; }
        }
        #endregion
    }
}