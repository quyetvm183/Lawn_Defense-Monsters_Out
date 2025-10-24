
//Fix camera width so the game can play on any screen device
using UnityEngine;
namespace RGame
{
    public class FixedCamera : MonoBehaviour
    {
        [ReadOnly] public float fixedWidth;
        //Set the fixed size for the camera
        [ReadOnly] public float orthographicSize = 2.5f;
        void Start()
        {
            //Only action when begin game from the Logo scene
            if (GameMode.Instance)
            {
                fixedWidth = orthographicSize * (GameMode.Instance.resolution.x / GameMode.Instance.resolution.y);
                Camera.main.orthographicSize = fixedWidth / (Camera.main.aspect);
            }
        }
    }
}