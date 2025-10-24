using UnityEngine;
using System.Collections;
namespace RGame
{
    public class RotateAround : MonoBehaviour, IListener
    {
        public enum Type { Clk, CClk }
        //the rotate direction to left or right
        public Type rotateType;
        //set the rotate speed
        public float speed = 0.5f;

        void Update()
        {
            if (isStop)
                return;
            //rotate the object with the given speed and direction
            transform.Rotate(Vector3.forward, Mathf.Abs(speed) * (rotateType == Type.CClk ? 1 : -1));
        }

        bool isStop = false;
        #region IListener implementation

        public void IPlay()
        {
        }

        public void ISuccess()
        {
        }

        public void IPause()
        {
        }

        public void IUnPause()
        {
        }

        public void IGameOver()
        {
        }

        public void IOnRespawn()
        {
        }

        public void IOnStopMovingOn()
        {
            Debug.Log("IOnStopMovingOn");
            isStop = true;

        }

        public void IOnStopMovingOff()
        {
            isStop = false;
        }

        #endregion
    }
}