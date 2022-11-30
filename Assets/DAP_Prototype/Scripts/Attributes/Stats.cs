using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class Stats : MonoBehaviour
    {
        #region Fields
        [SerializeField] public int currentStr = 10;
        [SerializeField] private int maxStr = 10;
        [SerializeField] public int currentInt = 10;
        [SerializeField] private int maxInt = 10;
        [SerializeField] public int currentDex = 10;
        [SerializeField] private int maxDex = 10;
        [SerializeField] public int currentCon = 10;
        [SerializeField] private int maxCon = 10;
        [SerializeField] public int currentWill = 10;
        [SerializeField] private int maxWill = 10;
        [SerializeField] public int currentDef = 10;
        [SerializeField] private int maxDef = 10;
        #endregion

        #region Stat Increasers

        public void IncreaseStrength(int strAmount)
        {
            if ((currentStr + strAmount) > maxStr)
            {
                currentStr += strAmount;
            }
        }

        public void IncreaseDexterity(int dexAmount)
        {
            if ((currentDex + dexAmount) > maxDex)
            {
                currentDex += dexAmount;
            }
        }

        public void IncreaseIntelligence(int intAmount)
        {
            if ((currentInt + intAmount) > maxInt)
            {
                currentInt += intAmount;
            }
        }

        public void IncreaseConstitution(int conAmount)
        {
            if ((currentCon + conAmount) > maxCon)
            {
                currentCon += conAmount;
            }
        }

        public void IncreaseWillpower(int willAmount)
        {
            if ((currentWill + willAmount) > maxWill)
            {
                currentWill += willAmount;
            }
        }
        #endregion
    }
}