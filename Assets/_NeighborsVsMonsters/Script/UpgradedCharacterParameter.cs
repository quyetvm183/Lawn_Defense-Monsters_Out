using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    [System.Serializable]

    public class UpgradeStep
    {
        public int price;
        public int healthStep, meleeDamageStep, rangeDamageStep, criticalStep;
    }
    //The ability of the character
    public enum Abitity { Melee, Range, Healer }

    public class UpgradedCharacterParameter : MonoBehaviour
    {
        //Place the unique ID for the item
        public string ID = "unique ID";
        //Unlock the player after finish this level
        public int unlockAtLevel = 1;
        //Set the player ability
        public Abitity playerAbility;

        [Header("EFFECT")]
        public WeaponEffect weaponEffect;
        [Space]
        public UpgradeStep[] UpgradeSteps;

        [Header("MELEE ATTACK")]
        //how many target damaged per hit
        public int maxTargetPerHit = 1; 

        [Header("Default")]
        public int defaultHealth = 100;
        public int meleeDamage = 100;
        public int rangeDamage = 100;
        [Range(1, 100)]
        public int criticalDamagePercent = 10;

        //Get the current upgrade of the character
        public int CurrentUpgrade
        {
            get
            {
                //Check and return the current upgrade
                int current = PlayerPrefs.GetInt(ID + "upgradeHealth" + "Current", 0);
                if (current >= UpgradeSteps.Length)
                    //-1 mean overload
                    current = -1;  
                return current;
            }
            set
            {
                PlayerPrefs.SetInt(ID + "upgradeHealth" + "Current", value);
            }
        }

        public void UpgradeCharacter(bool health, bool melee, bool range, bool crit)
        {
            //If != -1 mean upgrade available
            if (CurrentUpgrade == -1)
                return;
            //Upgrade health
            if (health)
            {
                UpgradeHealth += UpgradeSteps[CurrentUpgrade].healthStep;
            }
            //Upgrade melee
            if (melee)
            {
                UpgradeMeleeDamage += UpgradeSteps[CurrentUpgrade].meleeDamageStep;
            }
            //Upgrade range
            if (range)
            {
                UpgradeRangeDamage += UpgradeSteps[CurrentUpgrade].rangeDamageStep;
            }
            //Upgrade crit
            if (crit)
            {
                UpgradeCriticalDamage += UpgradeSteps[CurrentUpgrade].criticalStep;
            }
            //Increase the upgrade value
            CurrentUpgrade++;
        }

        public int UpgradeHealth
        {
            get { return PlayerPrefs.GetInt(ID + "upgradeHealth", defaultHealth); }
            set { PlayerPrefs.SetInt(ID + "upgradeHealth", value); }
        }

        public int UpgradeMeleeDamage
        {
            get { return PlayerPrefs.GetInt(ID + "UpgradedMeleeDamage", meleeDamage); }
            set { PlayerPrefs.SetInt(ID + "UpgradedMeleeDamage", value); }
        }

        public int UpgradeRangeDamage
        {
            get { return PlayerPrefs.GetInt(ID + "UpgradeRangeDamage", rangeDamage); }
            set { PlayerPrefs.SetInt(ID + "UpgradeRangeDamage", value); }
        }

        public int UpgradeCriticalDamage
        {
            get { return PlayerPrefs.GetInt(ID + "UpgradeCriticalDamage", criticalDamagePercent); }
            set { PlayerPrefs.SetInt(ID + "UpgradeCriticalDamage", value); }
        }
    }
}