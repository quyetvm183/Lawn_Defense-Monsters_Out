using UnityEngine;
namespace RGame
{
    public class LevelManager : MonoBehaviour
    {
        [ReadOnly] public static LevelManager Instance;
        //set the default mana
        public int mana = 1000;

        private void Awake()
        {
            Instance = this;

            if (GameLevelSetup.Instance)
            {
                //Get the mana from the level prefab
                mana = GameLevelSetup.Instance.GetGivenMana();
            }
        }
    }
}