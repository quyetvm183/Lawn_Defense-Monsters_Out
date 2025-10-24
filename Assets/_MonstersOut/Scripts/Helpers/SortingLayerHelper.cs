using UnityEngine;
namespace RGame
{
    public class SortingLayerHelper : MonoBehaviour
    {
        //Place sprite owner
        public SpriteRenderer spriteRenderer;
        // Start is called before the first frame update
        void Start()
        {
            //set the new sorting order for the object
            spriteRenderer.sortingOrder = (int)(transform.localPosition.y * -1000);
        }
    }
}