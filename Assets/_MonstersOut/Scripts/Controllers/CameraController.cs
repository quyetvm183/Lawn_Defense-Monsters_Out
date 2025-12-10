using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RGame
{
    public class CameraController : MonoBehaviour
    {
        //set the limit Left and Right for the camera
        public float limitLeft = -6;
        public float limitRight = 1000;
        //The speed of the camera moving
        public float moveSpeed = 2;
        //multiple the camera distance
        public float distanceScale = 1;
        float beginX;
        float beginCamreaPosX;
        bool isDragging = false;
        Vector3 target;
        void Start()
        {
            //init the camera
            beginCamreaPosX = transform.position.x;
            //init the target
            target = transform.position;
            target.x = Mathf.Clamp(transform.position.x, limitLeft + CameraHalfWidth, limitRight - CameraHalfWidth);
        }

        void Update()
        {
            //set the camera follow the target
            transform.position = Vector3.Lerp(transform.position, target, moveSpeed * Time.fixedDeltaTime);
            //If the finger is dragging, get the value
            if (!isDragging)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    isDragging = true;
                    beginX = Input.mousePosition.x;
                    beginCamreaPosX = transform.position.x;
                }
            }
            else
            {
                //flag the dragging when finger up
                if (Input.GetMouseButtonUp(0))
                {
                    isDragging = false;
                }
                else
                {
                    target = new Vector3(beginCamreaPosX + (beginX - Input.mousePosition.x) * distanceScale * 0.01f, transform.position.y, transform.position.z);
                    target.x = Mathf.Clamp(target.x, limitLeft + CameraHalfWidth, limitRight - CameraHalfWidth);
                }
            }
        }

        public float CameraHalfWidth
        {
            //get the current screen of the device
            get { return (Camera.main.orthographicSize * ((float)Screen.width / Screen.height)); }
        }

        void OnDrawGizmos()
        {
            Gizmos.color = new Color(1, 0, 0, .5f);
            Gizmos.color = Color.yellow;
            Vector2 boxSize = new Vector2(limitRight - limitLeft, Camera.main.orthographicSize * 2);
            Vector2 center = new Vector2((limitRight + limitLeft) * 0.5f, transform.position.y);
            Gizmos.DrawWireCube(center, boxSize);

        }
    }
}