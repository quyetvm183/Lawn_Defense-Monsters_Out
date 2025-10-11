using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class Tutorial : MonoBehaviour
    {
        //Delay before show the tutorial if the player still don't drag the screen
        public float delayShow = 3;
        public CanvasGroup canvasG;

        // Start is called before the first frame update
        void Start()
        {
            //init the default value
            canvasG.alpha = 0;

            InvokeRepeating("CheckFireOrNot", 0, 0.1f);
            Invoke("ShowTutorial", delayShow);
        }

        void CheckFireOrNot()
        {
            //Check the arrow if shoot or not
            if (FindObjectOfType<ArrowProjectile>())
            {
                CancelInvoke();
                gameObject.SetActive(false);
            }
        }

        void ShowTutorial()
        {
            //set alpha to 1 to show the panel
            canvasG.alpha = 1;
        }
    }
}