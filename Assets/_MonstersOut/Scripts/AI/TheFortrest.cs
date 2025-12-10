using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public enum HEALTH_CHARACTER { PLAYER, ENEMY }

    [System.Serializable]
    public class FortrestLevel
    {
        //The default health of the fortress
        public float maxHealth = 1000;
    }
    public class TheFortrest : MonoBehaviour, ICanTakeDamage
    {
        public HEALTH_CHARACTER healthCharacter;
        public FortrestLevel[] fortrestLevels;

        [ReadOnly] public int fortrestLevel = 1;
        public int[] enemyFortrestHealth;

        [HideInInspector]
        public float maxHealth;
        //the extra health from the upgrade from shop
        [ReadOnly] public float extraHealth = 0;
        [ReadOnly] public float currentHealth;

        [Header("SHAKNG")]
        //how fast it shakes
        public float speed = 30f;
        //how much it shakes
        public float amount = 0.5f; 
        public float shakeTime = 0.3f;
        public bool shakeX, shakeY;

        Vector2 startingPos;
        IEnumerator ShakeCoDo;

        void Awake()
        {
            //store the original position
            startingPos = transform.position;

            if (healthCharacter == HEALTH_CHARACTER.PLAYER)
            {
                //get and set the health from the extra health
                maxHealth = fortrestLevels[Mathf.Min(fortrestLevels.Length - 1, GlobalValue.UpgradeStrongWall)].maxHealth;
            }
            else
            {
                maxHealth = GameLevelSetup.Instance ? fortrestLevels[GameLevelSetup.Instance.GetEnemyFortrestLevel() - 1].maxHealth : 100;
            }
        }

        IEnumerator ShakeCo(float time)
        {
            //Do the shaking when be hit by the enemy
            float counter = 0;
            while (counter < time)
            {
                transform.position = startingPos + new Vector2(Mathf.Sin(Time.time * speed) * amount * (shakeX ? 1 : 0), Mathf.Sin(Time.time * speed) * amount * (shakeY ? 1 : 0));

                yield return null;
                counter += Time.deltaTime;
            }
            //reset the position to the start after the shaking
            transform.position = startingPos;
        }

        // Start is called before the first frame update
        void Start()
        {
            extraHealth = maxHealth * GlobalValue.StrongWallExtra;
            maxHealth += extraHealth;
            currentHealth = maxHealth;
            MenuManager.Instance.UpdateHealthbar(currentHealth, maxHealth, healthCharacter);
        }

        public void TakeDamage(float damage, Vector2 force, Vector2 hitPoint, GameObject instigator, BODYPART bodyPart = BODYPART.NONE, WeaponEffect weaponEffect = null)
        {
            //take damage and caculating the health
            currentHealth -= damage;
            FloatingTextManager.Instance.ShowText("" + (int)damage, Vector2.up * 2, Color.yellow, transform.position);

            MenuManager.Instance.UpdateHealthbar(currentHealth, maxHealth, healthCharacter);
            //finish the game if the health is zero
            if (currentHealth <= 0)
            {
                if (healthCharacter == HEALTH_CHARACTER.PLAYER)
                    GameManager.Instance.GameOver();
                else
                    GameManager.Instance.Victory();
            }
            else
            {
                //Do shaking if health still larger than zero
                if (ShakeCoDo != null)
                    StopCoroutine(ShakeCoDo);

                ShakeCoDo = ShakeCo(shakeTime);
                StartCoroutine(ShakeCoDo);
            }
        }
    }
}