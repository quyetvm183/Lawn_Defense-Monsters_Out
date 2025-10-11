using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RGame
{
    public class AutoAddManaUI : MonoBehaviour
    {
        //place the mana text icon
        public Text manaIcon;
        int manaAdd = 5;
        float rate = 3;

        void Start()
        {
            //Only action when the game played from the Init scene
            if (GameLevelSetup.Instance)
            {
                //get mana and rate
                manaAdd = GameLevelSetup.Instance.amountMana;
                rate = GameLevelSetup.Instance.rate;
            }
            //display the mana amount
            manaIcon.text = "+" + manaAdd;
            manaIcon.gameObject.SetActive(false);
            //begin adding mana
            StartCoroutine(AutoFillManaCo());
        }

        IEnumerator AutoFillManaCo()
        {
            //wait delay time
            yield return new WaitForSeconds(rate);
            while (true)
            {

                while (GameManager.Instance.State != GameManager.GameState.Playing)
                    yield return null;
                //add mana
                LevelManager.Instance.mana += manaAdd;
                manaIcon.gameObject.SetActive(true);

                //wait delay time
                yield return new WaitForSeconds(rate);
                manaIcon.gameObject.SetActive(false);
            }
        }
    }
}