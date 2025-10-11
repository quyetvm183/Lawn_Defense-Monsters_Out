using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class ShopCharacterUpgrade : MonoBehaviour
    {
        //Place the character ID
        public UpgradedCharacterParameter characterID;
        //allow upgrade parts
        public bool upgradeHealth, upgradeMelee, upgradeRange, upgradeCrit;
        //Set the Text information
        public Text currentHealth, upgradeHealthStep,
            currentMeleeDamage, upgradeMeleeDamageStep,
            currentRangeDamage, upgradeRangeDamageStep,
            currentCritical, upgradeCriticleStep;
        public Text price, unlockLevel, hitTargetTxt;
        //Place the dot object
        public GameObject dot;
        public GameObject dotHoder;
        List<Image> upgradeDots;
        public Sprite dotImageOn, dotImageOff;
        public GameObject lockedPanel;
        [ReadOnly] public bool isUnlock = false;
        //Place the ability icons
        public GameObject poisonFX, freezeFX;
        public GameObject abilityMelee, abilityRange, abilityHealer;
        public GameObject holderRange, holderMelee, holderCrit;

        // Start is called before the first frame update
        void Start()
        {
            //Display the character ability and power
            hitTargetTxt.text = "HIT TARGET = " + characterID.maxTargetPerHit;
            poisonFX.SetActive(characterID.weaponEffect.effectType == WEAPON_EFFECT.POISON);
            freezeFX.SetActive(characterID.weaponEffect.effectType == WEAPON_EFFECT.FREEZE);

            abilityMelee.SetActive(characterID.playerAbility == Abitity.Melee);
            abilityRange.SetActive(characterID.playerAbility == Abitity.Range);
            abilityHealer.SetActive(characterID.playerAbility == Abitity.Healer);

            holderRange.SetActive(upgradeRange);
            holderMelee.SetActive(upgradeMelee);
            holderCrit.SetActive(upgradeCrit);
            //Check unlock the character
            isUnlock = GlobalValue.LevelPass >= characterID.unlockAtLevel - 1;
            lockedPanel.SetActive(!isUnlock);

            if (!isUnlock)
                unlockLevel.text = characterID.unlockAtLevel + "";
            //Update the information
            upgradeDots = new List<Image>();
            upgradeDots.Add(dot.GetComponent<Image>());
            for (int i = 1; i < characterID.UpgradeSteps.Length; i++)
            {
                upgradeDots.Add(Instantiate(dot, dotHoder.transform).GetComponent<Image>());
            }
            //Update the upgraded values
            UpdateParameter();
        }

        void UpdateParameter()
        {
            //Display the character information
            currentHealth.text = "HEALTH: " + characterID.UpgradeHealth;
            currentMeleeDamage.text = "DAMAGE: " + characterID.UpgradeMeleeDamage;
            currentRangeDamage.text = "DAMAGE: " + characterID.UpgradeRangeDamage;
            currentCritical.text = "CRIT: " + characterID.UpgradeCriticalDamage;
            //Meaning the character can upgrade more
            if (characterID.CurrentUpgrade != -1)
            {
                price.text = characterID.UpgradeSteps[characterID.CurrentUpgrade].price + "";
                upgradeHealthStep.text = "+" + characterID.UpgradeSteps[characterID.CurrentUpgrade].healthStep;
                upgradeMeleeDamageStep.text = "+" + characterID.UpgradeSteps[characterID.CurrentUpgrade].meleeDamageStep;
                upgradeRangeDamageStep.text = "+" + characterID.UpgradeSteps[characterID.CurrentUpgrade].rangeDamageStep;
                upgradeCriticleStep.text = "+" + characterID.UpgradeSteps[characterID.CurrentUpgrade].criticalStep;

                SetDots(characterID.CurrentUpgrade);
            }
            else
            {
                //The character is upgraded to max
                price.text = "MAX";
                upgradeHealthStep.enabled = false;
                upgradeMeleeDamageStep.enabled = false;
                upgradeRangeDamageStep.enabled = false;
                upgradeCriticleStep.enabled = false;

                SetDots(upgradeDots.Count);
            }

        }

        void SetDots(int number)
        {
            //Set the dot present the upgraded times
            for (int i = 0; i < upgradeDots.Count; i++)
            {
                if (i < number)
                    upgradeDots[i].sprite = dotImageOn;
                else
                    upgradeDots[i].sprite = dotImageOff;
            }
        }

        public void Upgrade()
        {
            //No upgrade if the character is not unlocked
            if (!isUnlock)
                return;
            //Or max
            if (characterID.CurrentUpgrade == -1)
                return;
            //Check the coins and upgrade the character
            if (GlobalValue.SavedCoins >= characterID.UpgradeSteps[characterID.CurrentUpgrade].price)
            {
                GlobalValue.SavedCoins -= characterID.UpgradeSteps[characterID.CurrentUpgrade].price;
                SoundManager.PlaySfx(SoundManager.Instance.soundUpgrade);

                characterID.UpgradeCharacter(upgradeHealth, upgradeMelee, upgradeRange, upgradeCrit);
                UpdateParameter();
            }
        }
    }
}