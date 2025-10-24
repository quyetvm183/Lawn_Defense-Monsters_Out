using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
namespace RGame
{
    public class FlashScene : MonoBehaviour
    {
        //Load the scene name after the delay time
        public string sceneLoad = "scene name";
        public float delay = 2;

        // Use this for initialization
        void Awake()
        {
            //Start loading scene action
            StartCoroutine(LoadAsynchronously(sceneLoad));
        }

        public GameObject LoadingObj;
        public Slider slider;
        public Text progressText;
        IEnumerator LoadAsynchronously(string name)
        {
            LoadingObj.SetActive(false);
            yield return new WaitForSeconds(delay);
            LoadingObj.SetActive(true);
            //Do the loading action
            AsyncOperation operation = SceneManager.LoadSceneAsync(name);
            while (!operation.isDone)
            {
                //Show the information of the progress
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                slider.value = progress;
                progressText.text = (int)progress * 100f + "%";
                yield return null;
            }
        }
    }
}