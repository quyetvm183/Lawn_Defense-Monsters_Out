using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace RGame
{
    /// <summary>
    /// Manages background images for different worlds during gameplay
    /// Automatically changes background based on current level
    /// </summary>
    public class WorldBackgroundManager : MonoBehaviour
    {
        [Header("=== BACKGROUND CONFIGURATION ===")]
        //The Image component that will display the background
        public Image backgroundImage;

        //Array of background sprites, one for each world
        public Sprite[] worldBackgrounds;

        //How many levels per world (default: 12 levels = 1 world)
        [Tooltip("Number of levels in each world. Example: 12 means Level 1-12 is World 1, 13-24 is World 2")]
        public int levelsPerWorld = 12;

        [Header("=== TRANSITION SETTINGS ===")]
        //Enable smooth fade transition when background changes
        public bool useFadeTransition = false;

        //Duration of fade effect in seconds
        public float fadeDuration = 0.5f;

        void Start()
        {
            UpdateBackgroundForCurrentLevel();
        }

        /// <summary>
        /// Updates the background based on the current level being played
        /// </summary>
        void UpdateBackgroundForCurrentLevel()
        {
            // Check if background system is configured
            if (backgroundImage == null || worldBackgrounds == null || worldBackgrounds.Length == 0)
            {
                Debug.LogWarning("WorldBackgroundManager: Background Image or World Backgrounds not configured!");
                return;
            }

            // Calculate which world the current level belongs to
            // Example: Level 1-12 = World 0, Level 13-24 = World 1, etc.
            int worldNumber = CalculateWorldNumber(GlobalValue.levelPlaying);

            // Clamp to prevent array out of bounds
            worldNumber = Mathf.Clamp(worldNumber, 0, worldBackgrounds.Length - 1);

            // Apply the background
            if (useFadeTransition)
            {
                StartCoroutine(FadeToBackground(worldNumber));
            }
            else
            {
                SetBackgroundImmediate(worldNumber);
            }

            Debug.Log($"Level {GlobalValue.levelPlaying} -> World {worldNumber + 1} background applied");
        }

        /// <summary>
        /// Calculate which world a level belongs to
        /// </summary>
        int CalculateWorldNumber(int level)
        {
            // Convert level number to 0-based index
            // Example: Level 1 = index 0, Level 13 = index 12
            int levelIndex = level - 1;

            // Divide by levels per world to get world number
            // Example: levelIndex 0-11 = world 0, 12-23 = world 1
            int worldNumber = levelIndex / levelsPerWorld;

            return worldNumber;
        }

        /// <summary>
        /// Immediately change the background without transition
        /// </summary>
        void SetBackgroundImmediate(int worldIndex)
        {
            if (worldIndex < 0 || worldIndex >= worldBackgrounds.Length)
                return;

            backgroundImage.sprite = worldBackgrounds[worldIndex];
        }

        /// <summary>
        /// Smoothly fade from current background to new background
        /// </summary>
        IEnumerator FadeToBackground(int worldIndex)
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

        /// <summary>
        /// Public method to manually change background (can be called from other scripts)
        /// </summary>
        public void SetBackground(int worldIndex)
        {
            if (useFadeTransition)
            {
                StartCoroutine(FadeToBackground(worldIndex));
            }
            else
            {
                SetBackgroundImmediate(worldIndex);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor helper to preview background changes
        /// </summary>
        [ContextMenu("Preview World 1 Background")]
        void PreviewWorld1() { SetBackgroundImmediate(0); }

        [ContextMenu("Preview World 2 Background")]
        void PreviewWorld2() { SetBackgroundImmediate(1); }

        [ContextMenu("Preview World 3 Background")]
        void PreviewWorld3() { SetBackgroundImmediate(2); }
#endif
    }
}
