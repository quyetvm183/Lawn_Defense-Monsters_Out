using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace RGame
{
    public class MenuManager : MonoBehaviour, IListener
    {
        //Place the UI elements
        public static MenuManager Instance;
        public GameObject Shoot;
        public GameObject StartUI;
        public GameObject UI;
        public GameObject VictotyUI;
        public GameObject FailUI;
        public GameObject PauseUI;
        public GameObject LoadingUI;
        public GameObject CharacterContainer;
        [Header("Sound and Music")]
        public Image soundImage;
        public Image musicImage;
        public Sprite soundImageOn, soundImageOff, musicImageOn, musicImageOff;
        UI_UI uiControl;

        private void Awake()
        {
            //Disable all the UI element on start
            Instance = this;
            StartUI.SetActive(false);
            VictotyUI.SetActive(false);
            FailUI.SetActive(false);
            PauseUI.SetActive(false);
            LoadingUI.SetActive(false);
            CharacterContainer.SetActive(false);
            uiControl = gameObject.GetComponentInChildren<UI_UI>(true);
        }

        IEnumerator Start()
        {
            //Set the music and sound state
            soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
            musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;
            if (!GlobalValue.isSound)
                SoundManager.SoundVolume = 0;
            if (!GlobalValue.isMusic)
                SoundManager.MusicVolume = 0;
            //active the start UI to allow play the game
            StartUI.SetActive(true);

            yield return new WaitForSeconds(1);
            StartUI.SetActive(false);
            //active the UI to allow play the game
            UI.SetActive(true);
            CharacterContainer.SetActive(true);
            //Play the game
            GameManager.Instance.StartGame();
        }

        public void UpdateHealthbar(float currentHealth, float maxHealth, HEALTH_CHARACTER healthBarType)
        {
            //update the health bar of UI
            uiControl.UpdateHealthbar(currentHealth, maxHealth, healthBarType);
        }

        public void UpdateEnemyWavePercent(float currentSpawn, float maxValue)
        {
            //update the level wave bar of UI
            uiControl.UpdateEnemyWavePercent(currentSpawn, maxValue);
        }

        float currentTimeScale;
        public void Pause()
        {
            SoundManager.PlaySfx(SoundManager.Instance.soundPause);
            //If scale = 1, pause the game
            if (Time.timeScale != 0)
            {
                currentTimeScale = Time.timeScale;
                //set time scale to 0 to pause the game
                Time.timeScale = 0;
                UI.SetActive(false);
                //active the pause UI
                PauseUI.SetActive(true);
                CharacterContainer.SetActive(false);
            }
            else
            {
                //set time scale to 1 to play the game
                Time.timeScale = currentTimeScale;
                UI.SetActive(true);
                //hide the pause UI
                PauseUI.SetActive(false);
                CharacterContainer.SetActive(true);
            }
        }

        public void IPlay()
        {

        }

        public void ISuccess()
        {
            StartCoroutine(VictoryCo());
        }

        IEnumerator VictoryCo()
        {
            //when victory, hide the UI the show the victory UI
            UI.SetActive(false);
            CharacterContainer.SetActive(false);
            yield return new WaitForSeconds(1.5f);
            VictotyUI.SetActive(true);
        }


        public void IPause()
        {

        }

        public void IUnPause()
        {

        }

        public void IGameOver()
        {
            StartCoroutine(GameOverCo());
        }

        IEnumerator GameOverCo()
        {
            //when victory, hide the UI the show the Fail UI
            UI.SetActive(false);
            CharacterContainer.SetActive(false);

            yield return new WaitForSeconds(1.5f);
            FailUI.SetActive(true);
        }

        public void IOnRespawn()
        {

        }

        public void IOnStopMovingOn()
        {

        }

        public void IOnStopMovingOff()
        {

        }


        #region Music and Sound
        public void TurnSound()
        {
            //Get the sound state and set the image state
            GlobalValue.isSound = !GlobalValue.isSound;
            soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
            //Set the sound
            SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
        }

        public void TurnMusic()
        {
            //Get the sound state and set the image state
            GlobalValue.isMusic = !GlobalValue.isMusic;
            musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;
            //Set the sound
            SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
        }
        #endregion

        #region Load Scene
        public void LoadHomeMenuScene()
        {
            //Play sound and load the scene
            SoundManager.Click();
            StartCoroutine(LoadAsynchronously("Menu"));
        }

        public void RestarLevel()
        {
            //Play sound and load the scene
            SoundManager.Click();
            StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
        }

        public void LoadNextLevel()
        {
            //Play sound and load the scene
            SoundManager.Click();
            GlobalValue.levelPlaying++;
            StartCoroutine(LoadAsynchronously(SceneManager.GetActiveScene().name));
        }

        [Header("Load scene")]
        public Slider slider;
        public Text progressText;
        IEnumerator LoadAsynchronously(string name)
        {
            //Show the loading UI
            LoadingUI.SetActive(true);

            AsyncOperation operation = SceneManager.LoadSceneAsync(name);
            while (!operation.isDone)
            {
                //Show the loading progress information
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                slider.value = progress;
                progressText.text = (int)progress * 100f + "%";
                //			Debug.LogError (progress);
                yield return null;
            }
        }
        #endregion

        private void OnDisable()
        {
            //make sure the game scale to 1
            Time.timeScale = 1;
        }
    }
}