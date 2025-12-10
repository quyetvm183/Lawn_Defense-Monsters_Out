using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class GiveCoinWhenDie : MonoBehaviour
    {
        //Give the random coins from Min and Max value
        public int coinGiveMin = 5;
        public int coinGiveMax = 10;

        public void GiveCoin()
        {
            //Play sound
            SoundManager.PlaySfx(SoundManager.Instance.coinCollect);
            //Get the random coins
            int random = Random.Range(coinGiveMin, coinGiveMax);
            GlobalValue.SavedCoins += random;
            //Show the text
            FloatingTextManager.Instance.ShowText((int)random + "", Vector2.up * 1, Color.yellow, transform.position);
        }
    }
}