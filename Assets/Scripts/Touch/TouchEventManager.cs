using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using VRAvatarSystem.Avatar;

namespace VRAvatar.Touch
{
    /// <summary>
    /// Central manager for processing and distributing touch events
    /// Handles both local and remote touch interactions
    /// </summary>
    public class TouchEventManager : MonoBehaviour
    {
        [Header("Touch Settings")]
        public float touchVisualizationDuration = 2f;
        public float touchCooldownTime = 0.1f;
        public int maxSimultaneousTouches = 10;
        
        [Header("Audio Feedback")]
        public AudioClip[] touchSounds;
        public float audioVolume = 0.5f;
        
        // Singleton instance
        public static TouchEventManager Instance { get; private set; }
        
        // Component references
        private TouchVisualizationManager visualizationManager;
        private AudioSource audioSource;
        
        // Touch tracking
        private Dictionary<string, float> lastTouchTimes = new Dictionary<string, float>();
        private List<ActiveTouch> activeTouches = new List<ActiveTouch>();
        
        // Events
        public System.Action<BodyPartColliderManager.BodyPart, Vector3, float> OnTouchProcessed;
        public System.Action<BodyPartColliderManager.BodyPart> OnTouchEnded;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void Start()
        {
            // Initialize components
            visualizationManager = FindObjectOfType<TouchVisualizationManager>();
            if (visualizationManager == null)
            {
                Debug.LogWarning("TouchVisualizationManager not found. Visual feedback will be disabled.");
            }
            
            // Setup audio
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.volume = audioVolume;
            audioSource.spatialBlend = 1f; // 3D audio
        }
        
        /// <summary>
        /// Process a remote touch event received from network
        /// </summary>
        public void ProcessRemoteTouchEvent(BodyPartColliderManager.BodyPart bodyPart, Vector3 contactPoint, float intensity)
        {
            string touchKey = $"{bodyPart}_{contactPoint}";
            
            // Check cooldown
            if (lastTouchTimes.ContainsKey(touchKey) && 
                Time.time - lastTouchTimes[touchKey] < touchCooldownTime)
            {
                return;
            }
            
            lastTouchTimes[touchKey] = Time.time;
            
            // Limit simultaneous touches
            if (activeTouches.Count >= maxSimultaneousTouches)
            {
                RemoveOldestTouch();
            }
            
            // Create active touch
            var activeTouch = new ActiveTouch
            {
                bodyPart = bodyPart,
                contactPoint = contactPoint,
                intensity = intensity,
                startTime = Time.time,
                touchKey = touchKey
            };
            
            activeTouches.Add(activeTouch);
            
            // Trigger visual feedback
            if (visualizationManager != null)
            {
                visualizationManager.ShowTouchEffect(contactPoint, GetTouchColor(bodyPart), intensity);
            }
            
            // Trigger audio feedback
            PlayTouchSound(bodyPart, intensity);
            
            // Notify listeners
            OnTouchProcessed?.Invoke(bodyPart, contactPoint, intensity);
            
            // Auto-remove after duration
            StartCoroutine(RemoveTouchAfterDuration(activeTouch));
        }
        
        /// <summary>
        /// Process end of remote touch event
        /// </summary>
        public void ProcessRemoteTouchEnd(BodyPartColliderManager.BodyPart bodyPart)
        {
            // Remove all touches for this body part
            for (int i = activeTouches.Count - 1; i >= 0; i--)
            {
                if (activeTouches[i].bodyPart == bodyPart)
                {
                    var touch = activeTouches[i];
                    activeTouches.RemoveAt(i);
                    
                    // Hide visual feedback
                    if (visualizationManager != null)
                    {
                        visualizationManager.HideTouchEffect(touch.contactPoint);
                    }
                }
            }
            
            OnTouchEnded?.Invoke(bodyPart);
        }
        
        private void PlayTouchSound(BodyPartColliderManager.BodyPart bodyPart, float intensity)
        {
            if (touchSounds == null || touchSounds.Length == 0) return;
            
            // Select sound based on body part
            int soundIndex = (int)bodyPart % touchSounds.Length;
            AudioClip clip = touchSounds[soundIndex];
            
            if (clip != null && audioSource != null)
            {
                audioSource.pitch = 0.8f + (intensity * 0.4f); // Vary pitch by intensity
                audioSource.PlayOneShot(clip, intensity * audioVolume);
            }
        }
        
        private Color GetTouchColor(BodyPartColliderManager.BodyPart bodyPart)
        {
            // Return different colors for different body parts
            switch (bodyPart)
            {
                case BodyPartColliderManager.BodyPart.Head:
                    return Color.yellow;
                case BodyPartColliderManager.BodyPart.LeftHand:
                case BodyPartColliderManager.BodyPart.RightHand:
                    return Color.green;
                case BodyPartColliderManager.BodyPart.Chest:
                case BodyPartColliderManager.BodyPart.Torso:
                    return Color.red;
                case BodyPartColliderManager.BodyPart.Back:
                    return Color.blue;
                default:
                    return Color.white;
            }
        }
        
        private void RemoveOldestTouch()
        {
            if (activeTouches.Count > 0)
            {
                var oldestTouch = activeTouches[0];
                activeTouches.RemoveAt(0);
                
                if (visualizationManager != null)
                {
                    visualizationManager.HideTouchEffect(oldestTouch.contactPoint);
                }
            }
        }
        
        private IEnumerator RemoveTouchAfterDuration(ActiveTouch touch)
        {
            yield return new WaitForSeconds(touchVisualizationDuration);
            
            if (activeTouches.Contains(touch))
            {
                activeTouches.Remove(touch);
                
                if (visualizationManager != null)
                {
                    visualizationManager.HideTouchEffect(touch.contactPoint);
                }
            }
        }
        
        /// <summary>
        /// Get all currently active touches
        /// </summary>
        public List<ActiveTouch> GetActiveTouches()
        {
            return new List<ActiveTouch>(activeTouches);
        }
        
        /// <summary>
        /// Clear all active touches
        /// </summary>
        public void ClearAllTouches()
        {
            if (visualizationManager != null)
            {
                foreach (var touch in activeTouches)
                {
                    visualizationManager.HideTouchEffect(touch.contactPoint);
                }
            }
            
            activeTouches.Clear();
            lastTouchTimes.Clear();
        }
    }
    
    [System.Serializable]
    public class ActiveTouch
    {
        public BodyPartColliderManager.BodyPart bodyPart;
        public Vector3 contactPoint;
        public float intensity;
        public float startTime;
        public string touchKey;
    }
}