using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace RGame
{
    [CustomEditor(typeof(GameMode))]
    public class GameModeEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("RESET ALL"))
            {
                PlayerPrefs.DeleteAll();
                Debug.Log("RESET ALL!");
            }


            if (GUILayout.Button("UNLOCK ALL"))
            {
                GlobalValue.LevelPass = 1000;
                GlobalValue.SavedCoins = 99999;
                Debug.Log("UNLOCKED ALL!");
            }
        }
    }
}