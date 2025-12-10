using UnityEngine;
using System.Collections;
using UnityEngine.UI;
namespace RGame
{
    public class MapControllerUI : MonoBehaviour
    {
        //	public Transform BlockLevel;
        public RectTransform BlockLevel;
        //set the number of the level block
        public int howManyBlocks = 3;
        //the distance of the block
        public float step = 720f;
        //place the dot images
        public Image[] Dots;
        private float newPosX = 0;

        int currentPos = 0;
        public AudioClip music;
        // Use this for initialization
        void Start()
        {
            //init the dot images
            SetDots();
        }

        void SetDots()
        {
            //first, set all the dot image to the default state
            foreach (var obj in Dots)
            {
                obj.color = new Color(1, 1, 1, 0.5f);
                obj.rectTransform.sizeDelta = new Vector2(28, 28);
            }
            //then active the dot present for the world
            Dots[currentPos].color = Color.yellow;
            Dots[currentPos].rectTransform.sizeDelta = new Vector2(38, 38);
        }

        void OnEnable()
        {
            //play the own music
            SoundManager.PlayMusic(music);

        }

        void OnDisable()
        {
            //play the default music
            if (SoundManager.Instance != null)
                SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
        }

        public void SetCurrentWorld(int world)
        {
            //set the current world nuber
            currentPos += (world - 1);
            //limit the block number to make sure never pass the last block
            newPosX -= step * (world - 1);
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
            //set the map position
            SetMapPosition();
            //update the dot state
            SetDots();
        }

        public void SetMapPosition()
        {
            //update the new map position
            BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
        }

        bool allowPressButton = true;
        public void Next()
        {
            //call the next function
            if (allowPressButton)
            {
                StartCoroutine(NextCo());
            }
        }

        IEnumerator NextCo()
        {
            //prevent press again
            allowPressButton = false;
            //play the sound click
            SoundManager.Click();
            //make sure no over the limit block
            if (newPosX != (-step * (howManyBlocks - 1)))
            {
                currentPos++;

                newPosX -= step;
                newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

            }
            else
            {
                allowPressButton = true;
                yield break;
            }
            //show the black screen effect
            BlackScreenUI.instance.Show(0.15f);

            yield return new WaitForSeconds(0.15f);
            //update the map position
            SetMapPosition();
            BlackScreenUI.instance.Hide(0.15f);

            SetDots();

            //allow press button again
            allowPressButton = true;

        }

        public void Pre()
        {
            if (allowPressButton)
            {
                StartCoroutine(PreCo());
            }
        }

        IEnumerator PreCo()
        {
            //prevent press again
            allowPressButton = false;
            //play the sound click
            SoundManager.Click();
            //make sure no over the limit block
            if (newPosX != 0)
            {
                currentPos--;

                newPosX += step;
                newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
            }
            else
            {
                allowPressButton = true;
                yield break;
            }
            //show the black screen effect
            BlackScreenUI.instance.Show(0.15f);

            yield return new WaitForSeconds(0.15f);
            //update the map position
            SetMapPosition();
            BlackScreenUI.instance.Hide(0.15f);

            SetDots();

            //allow press button again
            allowPressButton = true;

        }

        public void UnlockAllLevels()
        {
            //unlock all levels for testing
            GlobalValue.LevelPass = (GlobalValue.LevelPass + 1000);
            //Load the scene again
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            SoundManager.Click();
        }
    }
}