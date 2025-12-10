/*
 * Use for weapon
 */
using UnityEngine;
namespace RGame
{
    //The weapon effect type
    public enum WEAPON_EFFECT { NORMAL, POISON, FREEZE }

    [System.Serializable]
    public class WeaponEffect
    {
        //Set the default effect type
        public WEAPON_EFFECT effectType = WEAPON_EFFECT.NORMAL;
        [Header("NORMAL")]
        //Set the min and max damage
        public float normalDamageMin = 30;
        public float normalDamageMax = 50;

        [Header("POISON")]
        //set the poison value
        public float poisonTime = 5;
        public float poisonDamagePerSec = 80;
        [Space]
        [Header("FREEZE")]
        public float freezeTime = 5;
    }
}