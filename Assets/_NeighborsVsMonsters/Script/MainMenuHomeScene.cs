using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace RGame
{
    public class MainMenuHomeScene : MonoBehaviour
    {
        public static MainMenuHomeScene Instance;
        //Place the UI object panel
        public GameObject MapUI;
        public GameObject ShopUI;
        public GameObject Loading;
        public GameObject Settings;
        //The facebook and twitter Link
        public string facebookLink;
        public string twitterLink = "https://twitter.com/";
        //The coin text object
        public Text[] coinTxt;

        [Header("Sound and Music")]
        public Image soundImage;
        public Image musicImage;
        public Sprite soundImageOn, soundImageOff, musicImageOn, musicImageOff;

        void Awake()
        {
            //Init the UI panels
            Instance = this;
            if (Loading != null)
                Loading.SetActive(false);
            if (MapUI != null)
                MapUI.SetActive(false);
            if (Settings)
                Settings.SetActive(false);
            if (ShopUI)
                ShopUI.SetActive(false);
        }

        public void LoadScene()
        {
            //Show the loading scene
            if (Loading != null)
                Loading.SetActive(true);
            //Load the playing scene
            StartCoroutine(LoadAsynchronously("Playing"));
        }

        public void LoadScene(string sceneNamage)
        {
            //Show the loading scene
            if (Loading != null)
                Loading.SetActive(true);
            //Load the scene name
            StartCoroutine(LoadAsynchronously(sceneNamage));
        }

        IEnumerator Start()
        {
            //always check the audio state
            CheckSoundMusic();
            if (GlobalValue.isFirstOpenMainMenu)
            {
                GlobalValue.isFirstOpenMainMenu = false;
                SoundManager.Instance.PauseMusic(true);
                SoundManager.PlaySfx(SoundManager.Instance.beginSoundInMainMenu);
                yield return new WaitForSeconds(SoundManager.Instance.beginSoundInMainMenu.length);
                SoundManager.Instance.PauseMusic(false);
                SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
            }

            if (AdsManager.Instance)
                AdsManager.Instance.ShowAdmobBanner(false);
        }

        void Update()
        {
            //always check the audio state
            CheckSoundMusic();
            //always update the coin amount
            foreach (var ct in coinTxt)
            {
                ct.text = GlobalValue.SavedCoins + "";
            }
        }

        public void OpenMap(bool open)
        {
            //play sound and open/close the map
            SoundManager.Click();
            StartCoroutine(OpenMapCo(open));
        }

        IEnumerator OpenMapCo(bool open)
        {
            yield return null;
            //Wait a time before show/close the map
            BlackScreenUI.instance.Show(0.2f);
            MapUI.SetActive(open);
            BlackScreenUI.instance.Hide(0.2f);
        }

        public void Facebook()
        {
            SoundManager.Click();
            Application.OpenURL(facebookLink);
        }

        public void Twitter()
        {
            SoundManager.Click();
            Application.OpenURL(twitterLink);
        }

        public void ExitGame()
        {
            SoundManager.Click();
            Application.Quit();
        }

        public void Setting(bool open)
        {
            SoundManager.Click();
            Settings.SetActive(open);
        }

        #region Music and Sound
        public void TurnSound()
        {
            //Check the music state then display the new image
            GlobalValue.isSound = !GlobalValue.isSound;
            soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;

            SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
        }

        public void TurnMusic()
        {
            //Check the music state then display the new image
            GlobalValue.isMusic = !GlobalValue.isMusic;
            musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;

            SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
        }
        #endregion

        private void CheckSoundMusic()
        {
            //Check the music and sound state
            soundImage.sprite = GlobalValue.isSound ? soundImageOn : soundImageOff;
            musicImage.sprite = GlobalValue.isMusic ? musicImageOn : musicImageOff;
            SoundManager.SoundVolume = GlobalValue.isSound ? 1 : 0;
            SoundManager.MusicVolume = GlobalValue.isMusic ? SoundManager.Instance.musicsGameVolume : 0;
        }

        public void OpenShop(bool open)
        {
            //play sound and set the shop close/open
            SoundManager.Click();
            ShopUI.SetActive(open);
        }

        public void Tutorial()
        {
            SoundManager.Click();
            SceneManager.LoadScene("Tutorial");
        }

        public Slider slider;
        public Text progressText;
        IEnumerator LoadAsynchronously(string name)
        {
            //Call the loading state
            AsyncOperation operation = SceneManager.LoadSceneAsync(name);
            while (!operation.isDone)
            {
                //Show the progress information
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                slider.value = progress;
                //Show the progress information
                progressText.text = (int)progress * 100f + "%";
                yield return null;
            }
        }

        public void ResetData()
        {
            //call delete all data
            if (GameMode.Instance)
                GameMode.Instance.ResetDATA();
        }
    }
}