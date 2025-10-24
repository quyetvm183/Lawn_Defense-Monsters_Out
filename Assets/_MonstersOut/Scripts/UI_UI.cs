using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class UI_UI : MonoBehaviour
    {
        //Update the health speed
        public float lerpSpeed = 1;
        [Header("PLAYER HEALTHBAR")]
        public Slider healthSlider;
        //Place the health display text
        public Text health;
        float healthValue;

        [Header("ENEMY HEALTHBAR")]
        public Slider enemyHealthSlider;
        public Text enemyHealth;
        float enemyHealthValue;

        [Header("ENEMY WAVE")]
        public Slider enemyWavePercentSlider;
        float enemyWaveValue;

        [Space]
        //Place the coin, mana and level name text
        public Text coinTxt;
        public Text manaTxt;
        public Text levelName;

        private void Start()
        {
            //init the default value
            healthValue = 1;
            enemyWaveValue = 0;
            //reset the health bar
            healthSlider.value = 1;
            enemyWavePercentSlider.value = 0;
            levelName.text = "Level " + GlobalValue.levelPlaying;
        }

        private void Update()
        {
            //Update the health value to the bar
            healthSlider.value = Mathf.Lerp(healthSlider.value, healthValue, lerpSpeed * Time.deltaTime);
            enemyHealthSlider.value = Mathf.Lerp(enemyHealthSlider.value, enemyHealthValue, lerpSpeed * Time.deltaTime);
            //upgrade the enemey remains to the bar
            enemyWavePercentSlider.value = Mathf.Lerp(enemyWavePercentSlider.value, enemyWaveValue, lerpSpeed * Time.deltaTime);
            coinTxt.text = GlobalValue.SavedCoins + "";
            manaTxt.text = LevelManager.Instance.mana + "";
        }

        public void UpdateHealthbar(float currentHealth, float maxHealth, HEALTH_CHARACTER healthBarType)
        {
            //Upgrade the health manual of player and enemy
            if (healthBarType == HEALTH_CHARACTER.PLAYER)
            {
                healthValue = Mathf.Clamp01(currentHealth / maxHealth);
                health.text = (int)currentHealth + "/" + (int)maxHealth;
            }
            else if (healthBarType == HEALTH_CHARACTER.ENEMY)
            {
                enemyHealthValue = Mathf.Clamp01(currentHealth / maxHealth);
                enemyHealth.text = (int)currentHealth + "/" + (int)maxHealth;
            }
        }

        public void UpdateEnemyWavePercent(float currentSpawn, float maxValue)
        {
            //upgrade the enemy wave
            enemyWaveValue = Mathf.Clamp01(currentSpawn / maxValue);
        }
    }
}