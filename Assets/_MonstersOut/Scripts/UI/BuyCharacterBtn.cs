using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class BuyCharacterBtn : MonoBehaviour
    {
        //place the character ID
        public UpgradedCharacterParameter characterID;
        //set the price for the character
        public int price = 888;
        //maximum player can be show in the scene at the same time
        public int max = 5;
        [ReadOnly] public int current = 0;
        //place the character prefab
        public GameObject character;
        //set the information objects
        public GameObject lockIcon;
        public Text priceTxt;
        public Text unlockLevelTxt;
        public Text numberTxt;
        public AudioClip soundPurchase;
        Button ownBtn;
        bool isUnlocked = false;
        [ReadOnly] public List<GameObject> listCharacters;

        [Header("COOL DOWN")]
        //delay on start
        public float delayOnStart = 2;
        //the waiting time for the next buy
        public float coolDown = 3f;
        float coolDownCounter = 0;
        public Image image;
        bool allowWork = true;
        bool canUse = true;
        public CanvasGroup canvasGroup;

        public GameObject poisonFX, freezeFX;
        public GameObject abilityMelee, abilityRange, abilityHealer;
        void Start()
        {
            //init the button
            ownBtn = GetComponent<Button>();
            ownBtn.onClick.AddListener(OnBtnClick);
            //display the character power
            poisonFX.SetActive(characterID.weaponEffect.effectType == WEAPON_EFFECT.POISON);
            freezeFX.SetActive(characterID.weaponEffect.effectType == WEAPON_EFFECT.FREEZE);
            abilityMelee.SetActive(characterID.playerAbility == Abitity.Melee);
            abilityRange.SetActive(characterID.playerAbility == Abitity.Range);
            abilityHealer.SetActive(characterID.playerAbility == Abitity.Healer);
            //init the list of the characters
            listCharacters = new List<GameObject>();
            //check to unlock the character
            if (GameMode.Instance == null || ((GlobalValue.LevelPass + 1) >= characterID.unlockAtLevel))
            {
                isUnlocked = true;
            }
            //set the price for the character
            if (isUnlocked)
            {
                priceTxt.text = price + "";
                unlockLevelTxt.enabled = false;
                InvokeRepeating("CheckAvailable", 0, 0.1f);
            }
            else
            {
                priceTxt.text = "LOCKED";
                unlockLevelTxt.text = "" + characterID.unlockAtLevel;
                ownBtn.interactable = false;
            }
            //show the lock icon
            lockIcon.SetActive(!isUnlocked);
            //init the clickable for the button
            if (image == null)
                image = GetComponent<Image>();
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            allowWork = false;
            coolDownCounter = delayOnStart;
        }

        int numberCharacterAlive()
        {
            //check and return the current characters
            int alives = 0;
            foreach (var cha in listCharacters)
            {
                if (cha.activeInHierarchy)
                    alives++;
            }
            return alives;
        }

        void Update()
        {
            //show update number 
            numberTxt.text = numberCharacterAlive() + "/" + max;
            if (!allowWork)
            {
                coolDownCounter -= Time.deltaTime;

                if (coolDownCounter <= 0)
                    allowWork = true;
            }
            //show the cool down effect for the image
            image.fillAmount = Mathf.Clamp01((coolDown - coolDownCounter) / coolDown);
            //allow or disable the button
            canvasGroup.interactable = coolDownCounter <= 0;

            canUse = canvasGroup.interactable && canvasGroup.blocksRaycasts;
        }

        void CheckAvailable()
        {
            //return if the button can click or not
            ownBtn.interactable = LevelManager.Instance.mana >= price && numberCharacterAlive() < max;
        }

        private void OnBtnClick()
        {
            //no action when using
            if (!canUse)
                return;

            if (!allowWork)
                return;
            //check the remain mana to spawn the character
            if (LevelManager.Instance.mana >= price)
            {
                LevelManager.Instance.mana -= price;
                SoundManager.PlaySfx(soundPurchase);
                listCharacters.Add(CharacterManager.Instance.SpawnCharacter(character));
                allowWork = false;
                coolDownCounter = coolDown;
            }
        }
    }
}