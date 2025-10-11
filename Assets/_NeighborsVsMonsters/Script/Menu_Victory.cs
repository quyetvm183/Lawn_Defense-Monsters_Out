using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;
namespace RGame
{
    /// <summary>
    /// Handle Level Complete UI of Menu object
    /// </summary>
    public class Menu_Victory : MonoBehaviour
    {
        //Place the UI elements
        public GameObject Menu;
        public GameObject Restart;
        public GameObject Next;
        public GameObject Star1;
        public GameObject Star2;
        public GameObject Star3;

        void Awake()
        {
            //Disable all the UI element on start
            Menu.SetActive(false);
            Restart.SetActive(false);
            Next.SetActive(false);
            Star1.SetActive(false);
            Star2.SetActive(false);
            Star3.SetActive(false);
        }

        IEnumerator Start()
        {
            //Play the victory sound
            SoundManager.PlaySfx(SoundManager.Instance.soundVictoryPanel);
            //Disable all the star icon for checking
            Star1.SetActive(false);
            Star2.SetActive(false);
            Star3.SetActive(false);
            //Get the fortress state and show the 
            var theFortress = FindObjectsOfType<TheFortrest>();
            foreach (var fortrest in theFortress)
            {
                if (fortrest.healthCharacter == HEALTH_CHARACTER.PLAYER)
                {
                    //Check and set the star if the current health larger the condition value
                    if ((fortrest.currentHealth / fortrest.maxHealth) > 0)
                    {
                        yield return new WaitForSeconds(0.6f);
                        Star1.SetActive(true);
                        SoundManager.PlaySfx(SoundManager.Instance.soundStar1);
                        //Set the star for the level
                        GameManager.Instance.levelStarGot = 1;
                    }
                    //Check and set the star if the current health larger the condition value
                    if ((fortrest.currentHealth / fortrest.maxHealth) > 0.5f)
                    {
                        yield return new WaitForSeconds(0.6f);
                        Star2.SetActive(true);
                        SoundManager.PlaySfx(SoundManager.Instance.soundStar2);
                        //Set the star for the level
                        GameManager.Instance.levelStarGot = 2;
                    }
                    //Check and set the star if the current health larger the condition value
                    if ((fortrest.currentHealth / fortrest.maxHealth) > 0.8f)
                    {
                        yield return new WaitForSeconds(0.6f);
                        Star3.SetActive(true);
                        SoundManager.PlaySfx(SoundManager.Instance.soundStar3);
                        //Set the star for the level
                        GameManager.Instance.levelStarGot = 3;
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
            //when finish counting, enable those button for user choose
            Menu.SetActive(true);
            Restart.SetActive(true);
            //only show the Next button when there are a new level ahead
            Next.SetActive(GlobalValue.levelPlaying < GlobalValue.finishGameAtLevel); ;
        }
    }
}