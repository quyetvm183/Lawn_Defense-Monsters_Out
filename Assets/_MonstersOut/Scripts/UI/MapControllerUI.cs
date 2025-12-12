using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace RGame
{
    public class MapControllerUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //	public Transform BlockLevel;
        public RectTransform BlockLevel;
        //set the number of the level block
        public int howManyBlocks = 3;
        //the distance of the block
        public float step = 720f;
        //place the dot images
        public Image[] Dots;
        private float newPosX = 0;

        int currentPos = 0;
        public AudioClip music;

        [Header("=== WORLD BACKGROUNDS ===")]
        //Background image component to display the world background
        public Image backgroundImage;
        //Array of background sprites for each world
        public Sprite[] worldBackgrounds;
        //Enable smooth fade transition between backgrounds
        public bool useFadeTransition = true;
        //Duration of fade effect in seconds
        public float fadeDuration = 0.3f;

        // Drag and swipe variables
        private Vector2 dragStartPos;
        private float dragStartX;
        private bool isDragging = false;
        private float dragVelocity = 0f;

        // Swipe detection
        public float swipeThreshold = 50f; // Minimum distance to consider as swipe
        public float swipeVelocityThreshold = 500f; // Minimum velocity to trigger swipe

        // Smooth transition
        private bool isTransitioning = false;
        private float targetPosX;
        public float smoothSpeed = 10f;
        // Use this for initialization
        void Start()
        {
            //init the dot images
            SetDots();
            targetPosX = newPosX;
        }

        void Update()
        {
            // Smooth transition to target position
            if (isTransitioning && !isDragging)
            {
                newPosX = Mathf.Lerp(newPosX, targetPosX, Time.deltaTime * smoothSpeed);
                BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);

                // Check if reached target
                if (Mathf.Abs(newPosX - targetPosX) < 0.1f)
                {
                    newPosX = targetPosX;
                    BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
                    isTransitioning = false;
                    UpdateCurrentPosFromX();
                    SetDots();
                }
            }
        }

        void SetDots()
        {
            //first, set all the dot image to the default state
            foreach (var obj in Dots)
            {
                obj.color = new Color(1, 1, 1, 0.5f);
                obj.rectTransform.sizeDelta = new Vector2(28, 28);
            }
            //then active the dot present for the world
            Dots[currentPos].color = Color.yellow;
            Dots[currentPos].rectTransform.sizeDelta = new Vector2(38, 38);

            //update the background for current world
            UpdateBackground();
        }

        void UpdateCurrentPosFromX()
        {
            // Calculate current position based on X position
            currentPos = Mathf.RoundToInt(-newPosX / step);
            currentPos = Mathf.Clamp(currentPos, 0, howManyBlocks - 1);
        }

        void OnEnable()
        {
            //play the own music
            SoundManager.PlayMusic(music);

        }

        void OnDisable()
        {
            //play the default music
            if (SoundManager.Instance != null)
                SoundManager.PlayMusic(SoundManager.Instance.musicsGame);
        }

        public void SetCurrentWorld(int world)
        {
            //set the current world nuber
            currentPos += (world - 1);
            //limit the block number to make sure never pass the last block
            newPosX -= step * (world - 1);
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
            //set the map position
            SetMapPosition();
            //update the dot state
            SetDots();
        }

        public void SetMapPosition()
        {
            //update the new map position
            BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);
        }

        // OLD BUTTON METHODS - Can be removed if not using buttons anymore
        // Keep these if you want to add back navigation buttons in the future
        bool allowPressButton = true;
        public void Next()
        {
            //call the next function
            if (allowPressButton)
            {
                StartCoroutine(NextCo());
            }
        }

        IEnumerator NextCo()
        {
            //prevent press again
            allowPressButton = false;
            //play the sound click
            SoundManager.Click();
            //make sure no over the limit block
            if (newPosX != (-step * (howManyBlocks - 1)))
            {
                currentPos++;

                newPosX -= step;
                newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

            }
            else
            {
                allowPressButton = true;
                yield break;
            }
            //show the black screen effect
            BlackScreenUI.instance.Show(0.15f);

            yield return new WaitForSeconds(0.15f);
            //update the map position
            SetMapPosition();
            BlackScreenUI.instance.Hide(0.15f);

            SetDots();

            //allow press button again
            allowPressButton = true;

        }

        public void Pre()
        {
            if (allowPressButton)
            {
                StartCoroutine(PreCo());
            }
        }

        IEnumerator PreCo()
        {
            //prevent press again
            allowPressButton = false;
            //play the sound click
            SoundManager.Click();
            //make sure no over the limit block
            if (newPosX != 0)
            {
                currentPos--;

                newPosX += step;
                newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);
            }
            else
            {
                allowPressButton = true;
                yield break;
            }
            //show the black screen effect
            BlackScreenUI.instance.Show(0.15f);

            yield return new WaitForSeconds(0.15f);
            //update the map position
            SetMapPosition();
            BlackScreenUI.instance.Hide(0.15f);

            SetDots();

            //allow press button again
            allowPressButton = true;

        }

        public void UnlockAllLevels()
        {
            //unlock all levels for testing
            GlobalValue.LevelPass = (GlobalValue.LevelPass + 1000);
            //Load the scene again
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
            SoundManager.Click();
        }

        /// <summary>
        /// Update the background image based on current world position
        /// </summary>
        void UpdateBackground()
        {
            // Check if background system is configured
            if (backgroundImage == null || worldBackgrounds == null || worldBackgrounds.Length == 0)
                return;

            // Check if current position has a corresponding background
            if (currentPos >= worldBackgrounds.Length)
            {
                Debug.LogWarning($"No background sprite for world {currentPos + 1}. Please add more sprites to worldBackgrounds array.");
                return;
            }

            // Use fade transition or instant change
            if (useFadeTransition)
            {
                StartCoroutine(FadeToNewBackground(currentPos));
            }
            else
            {
                backgroundImage.sprite = worldBackgrounds[currentPos];
            }
        }

        /// <summary>
        /// Smoothly fade from current background to new background
        /// </summary>
        IEnumerator FadeToNewBackground(int worldIndex)
        {
            if (backgroundImage == null || worldBackgrounds.Length <= worldIndex)
                yield break;

            // Store original color
            Color originalColor = backgroundImage.color;

            // Fade out
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
                backgroundImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            // Change sprite while invisible
            backgroundImage.sprite = worldBackgrounds[worldIndex];

            // Fade in
            elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float alpha = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
                backgroundImage.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
                yield return null;
            }

            // Ensure full opacity
            backgroundImage.color = originalColor;
        }

        // Drag handlers for swipe functionality
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
            isTransitioning = false;
            dragStartPos = eventData.position;
            dragStartX = newPosX;
            dragVelocity = 0f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!isDragging) return;

            // Calculate drag delta
            float dragDelta = eventData.position.x - dragStartPos.x;

            // Update position based on drag
            newPosX = dragStartX + dragDelta;

            // Clamp within bounds
            newPosX = Mathf.Clamp(newPosX, -step * (howManyBlocks - 1), 0);

            // Update BlockLevel position immediately
            BlockLevel.anchoredPosition = new Vector2(newPosX, BlockLevel.anchoredPosition.y);

            // Calculate velocity for swipe detection
            dragVelocity = eventData.delta.x / Time.deltaTime;

            // Update dots while dragging
            UpdateCurrentPosFromX();
            SetDots();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;

            float dragDistance = eventData.position.x - dragStartPos.x;
            float absDragDistance = Mathf.Abs(dragDistance);
            float absVelocity = Mathf.Abs(dragVelocity);

            // Check if it's a swipe (fast velocity)
            if (absVelocity > swipeVelocityThreshold && absDragDistance > swipeThreshold)
            {
                // Swipe detected - move to next/previous world
                if (dragVelocity > 0 && currentPos > 0)
                {
                    // Swipe right - go to previous
                    currentPos--;
                }
                else if (dragVelocity < 0 && currentPos < howManyBlocks - 1)
                {
                    // Swipe left - go to next
                    currentPos++;
                }
            }
            else
            {
                // Not a swipe - snap to nearest position
                currentPos = Mathf.RoundToInt(-newPosX / step);
                currentPos = Mathf.Clamp(currentPos, 0, howManyBlocks - 1);
            }

            // Set target position and start transition
            targetPosX = -currentPos * step;
            isTransitioning = true;

            // Play sound
            SoundManager.Click();
        }
    }
}