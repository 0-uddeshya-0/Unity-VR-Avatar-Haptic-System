using UnityEngine;
using UnityEngine.XR;
using System.Collections;
using System.Collections.Generic;
using VRAvatar.Haptics;

namespace VRAvatar.Haptics
{
    /// <summary>
    /// Simulates haptic feedback without physical haptic devices
    /// Uses controller vibration, visual cues, and audio feedback
    /// </summary>
    public class HapticSimulator : MonoBehaviour, IHapticDevice
    {
        [Header("Simulation Settings")]
        public bool useControllerVibration = true;
        public bool useVisualFeedback = true;
        public bool useAudioFeedback = true;
        public bool showDebugLogs = true;
        
        [Header("Vibration Settings")]
        [Range(0f, 1f)]
        public float vibrationMultiplier = 0.8f;
        public AnimationCurve vibrationCurve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        
        [Header("Visual Feedback")]
        public Material hapticHighlightMaterial;
        public float visualFeedbackDuration = 0.5f;
        
        [Header("Audio Feedback")]
        public AudioClip[] hapticSounds;
        public AudioSource audioSource;
        [Range(0f, 1f)]
        public float audioVolume = 0.3f;
        
        // IHapticDevice implementation
        public string DeviceName => "Haptic Simulator";
        public bool IsConnected => gameObject.activeInHierarchy;
        
        // Internal state
        private Dictionary<BodyRegion, Coroutine> activeVibrations = new Dictionary<BodyRegion, Coroutine>();
        private Dictionary<BodyRegion, Renderer> bodyRenderers = new Dictionary<BodyRegion, Renderer>();
        private List<InputDevice> controllers = new List<InputDevice>();
        
        // Events
        public event System.Action<string> OnDeviceStatusChanged;
        
        private void Start()
        {
            Initialize(null);
            InvokeRepeating(nameof(UpdateControllers), 0f, 1f);
        }
        
        public bool Initialize(HapticConfig config)
        {
            // Setup audio source
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            audioSource.volume = audioVolume;
            audioSource.spatialBlend = 1f; // 3D audio
            
            // Find body renderers for visual feedback
            SetupBodyRenderers();
            
            // Register with HapticManager
            if (HapticManager.Instance != null)
            {
                HapticManager.Instance.RegisterDevice(this);
            }
            
            if (showDebugLogs)
            {
                Debug.Log($"[{DeviceName}] Initialized successfully");
            }
            
            OnDeviceStatusChanged?.Invoke("Initialized");
            return true;
        }
        
        public void SendHapticFeedback(BodyRegion region, HapticFeedback feedback)
        {
            if (!IsConnected) return;
            
            if (showDebugLogs)
            {
                Debug.Log($"[{DeviceName}] Haptic feedback: {region}, intensity={feedback.intensity}, duration={feedback.duration}");
            }
            
            // Controller vibration
            if (useControllerVibration)
            {
                TriggerControllerVibration(region, feedback);
            }
            
            // Visual feedback
            if (useVisualFeedback)
            {
                TriggerVisualFeedback(region, feedback);
            }
            
            // Audio feedback
            if (useAudioFeedback)
            {
                TriggerAudioFeedback(region, feedback);
            }
        }
        
        private void TriggerControllerVibration(BodyRegion region, HapticFeedback feedback)
        {
            // Stop existing vibration for this region
            if (activeVibrations.ContainsKey(region))
            {
                StopCoroutine(activeVibrations[region]);
            }
            
            // Determine which controller to vibrate
            InputDevice targetController = GetControllerForRegion(region);
            if (!targetController.isValid) return;
            
            // Start vibration coroutine
            activeVibrations[region] = StartCoroutine(VibrateController(targetController, feedback));
        }
        
        private void TriggerVisualFeedback(BodyRegion region, HapticFeedback feedback)
        {
            if (bodyRenderers.ContainsKey(region))
            {
                StartCoroutine(HighlightBodyPart(bodyRenderers[region], feedback));
            }
        }
        
        private void TriggerAudioFeedback(BodyRegion region, HapticFeedback feedback)
        {
            if (hapticSounds != null && hapticSounds.Length > 0 && audioSource != null)
            {
                // Select sound based on haptic type
                int soundIndex = ((int)feedback.type) % hapticSounds.Length;
                AudioClip clip = hapticSounds[soundIndex];
                
                if (clip != null)
                {
                    // Vary pitch and volume based on intensity
                    audioSource.pitch = 0.7f + (feedback.intensity * 0.6f);
                    audioSource.PlayOneShot(clip, feedback.intensity * audioVolume);
                }
            }
        }
        
        private IEnumerator VibrateController(InputDevice controller, HapticFeedback feedback)
        {
            float elapsedTime = 0f;
            float intensity = feedback.intensity * vibrationMultiplier;
            
            while (elapsedTime < feedback.duration)
            {
                float normalizedTime = elapsedTime / feedback.duration;
                float currentIntensity = intensity * vibrationCurve.Evaluate(normalizedTime);
                
                // Apply vibration pattern
                switch (feedback.pattern)
                {
                    case HapticPattern.Pulse:
                        float pulseValue = Mathf.Sin(normalizedTime * feedback.frequency * 2 * Mathf.PI);
                        currentIntensity *= Mathf.Abs(pulseValue);
                        break;
                    case HapticPattern.Heartbeat:
                        float heartbeat = GetHeartbeatPattern(normalizedTime);
                        currentIntensity *= heartbeat;
                        break;
                }
                
                // Send haptic impulse
                controller.SendHapticImpulse(0, currentIntensity, Time.deltaTime);
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
        }
        
        private IEnumerator HighlightBodyPart(Renderer bodyRenderer, HapticFeedback feedback)
        {
            if (bodyRenderer == null || hapticHighlightMaterial == null) yield break;
            
            Material originalMaterial = bodyRenderer.material;
            Material highlightMaterial = new Material(hapticHighlightMaterial);
            
            // Set highlight color based on haptic type
            Color highlightColor = GetHapticColor(feedback.type);
            highlightColor.a = feedback.intensity;
            highlightMaterial.color = highlightColor;
            
            bodyRenderer.material = highlightMaterial;
            
            yield return new WaitForSeconds(Mathf.Min(feedback.duration, visualFeedbackDuration));
            
            // Fade out
            float fadeTime = 0.3f;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeTime)
            {
                float alpha = Mathf.Lerp(feedback.intensity, 0f, elapsedTime / fadeTime);
                Color color = highlightMaterial.color;
                color.a = alpha;
                highlightMaterial.color = color;
                
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            bodyRenderer.material = originalMaterial;
            Destroy(highlightMaterial);
        }
        
        private InputDevice GetControllerForRegion(BodyRegion region)
        {
            // Map body regions to controllers
            bool useLeftController = region == BodyRegion.LeftHand || 
                                   region == BodyRegion.LeftUpperArm || 
                                   region == BodyRegion.LeftForearm ||
                                   region == BodyRegion.LeftShoulder;
            
            bool useRightController = region == BodyRegion.RightHand || 
                                    region == BodyRegion.RightUpperArm || 
                                    region == BodyRegion.RightForearm ||
                                    region == BodyRegion.RightShoulder;
            
            foreach (var controller in controllers)
            {
                if (useLeftController && controller.characteristics.HasFlag(InputDeviceCharacteristics.Left))
                    return controller;
                if (useRightController && controller.characteristics.HasFlag(InputDeviceCharacteristics.Right))
                    return controller;
            }
            
            // Default to first available controller
            return controllers.Count > 0 ? controllers[0] : default(InputDevice);
        }
        
        private void SetupBodyRenderers()
        {
            // Find renderers for each body region
            // This assumes avatar has appropriately named child objects
            var avatarRoot = transform.root;
            
            foreach (BodyRegion region in System.Enum.GetValues(typeof(BodyRegion)))
            {
                Transform bodyPart = FindBodyPart(avatarRoot, region.ToString());
                if (bodyPart != null)
                {
                    Renderer renderer = bodyPart.GetComponent<Renderer>();
                    if (renderer != null)
                    {
                        bodyRenderers[region] = renderer;
                    }
                }
            }
        }
        
        private Transform FindBodyPart(Transform root, string bodyPartName)
        {
            // Search for body part by name (case insensitive)
            return FindChildRecursive(root, bodyPartName);
        }
        
        private Transform FindChildRecursive(Transform parent, string childName)
        {
            foreach (Transform child in parent)
            {
                if (child.name.ToLower().Contains(childName.ToLower()))
                    return child;
                
                Transform found = FindChildRecursive(child, childName);
                if (found != null)
                    return found;
            }
            return null;
        }
        
        private void UpdateControllers()
        {
            controllers.Clear();
            InputDevices.GetDevicesWithCharacteristics(InputDeviceCharacteristics.Controller, controllers);
        }
        
        private float GetHeartbeatPattern(float normalizedTime)
        {
            // Simple heartbeat pattern: two quick beats followed by pause
            float cycleTime = normalizedTime * 2f; // 2 cycles per duration
            float cycle = cycleTime % 1f;
            
            if (cycle < 0.1f) return 1f;      // First beat
            if (cycle < 0.3f) return 0f;      // Pause
            if (cycle < 0.4f) return 0.8f;    // Second beat
            return 0f;                        // Long pause
        }
        
        private Color GetHapticColor(HapticType hapticType)
        {
            switch (hapticType)
            {
                case HapticType.Touch: return Color.green;
                case HapticType.Pressure: return Color.blue;
                case HapticType.Vibration: return Color.yellow;
                case HapticType.Temperature: return Color.red;
                case HapticType.Electrical: return Color.cyan;
                default: return Color.white;
            }
        }
        
        public void Shutdown()
        {
            // Stop all active vibrations
            foreach (var vibration in activeVibrations.Values)
            {
                if (vibration != null)
                    StopCoroutine(vibration);
            }
            activeVibrations.Clear();
            
            OnDeviceStatusChanged?.Invoke("Shutdown");
            
            if (showDebugLogs)
            {
                Debug.Log($"[{DeviceName}] Shutdown completed");
            }
        }
        
        private void OnDestroy()
        {
            Shutdown();
        }
    }
    
    // Supporting enums (add to existing HapticFeedback structure)
    public enum HapticPattern
    {
        Continuous,
        Pulse,
        Heartbeat,
        Custom
    }
    
    public enum HapticType
    {
        Touch,
        Pressure,
        Vibration,
        Temperature,
        Electrical
    }
    
    // Enhanced HapticFeedback structure
    [System.Serializable]
    public struct HapticFeedback
    {
        public float intensity;
        public float duration;
        public HapticPattern pattern;
        public HapticType type;
        public float frequency;
        
        public static HapticFeedback CreateTouch(float intensity, float duration)
        {
            return new HapticFeedback
            {
                intensity = intensity,
                duration = duration,
                pattern = HapticPattern.Continuous,
                type = HapticType.Touch,
                frequency = 10f
            };
        }
        
        public static HapticFeedback CreatePulse(float intensity, float duration, float frequency)
        {
            return new HapticFeedback
            {
                intensity = intensity,
                duration = duration,
                pattern = HapticPattern.Pulse,
                type = HapticType.Vibration,
                frequency = frequency
            };
        }
    }
    
    // Add BodyRegion enum if not exists
    public enum BodyRegion
    {
        Head, Neck, Face,
        ChestUpper, ChestLower, Abdomen,
        BackUpper, BackMiddle, BackLower,
        LeftShoulder, LeftUpperArm, LeftElbow, LeftForearm, LeftWrist, LeftHand, LeftPalm, LeftFingers,
        RightShoulder, RightUpperArm, RightElbow, RightForearm, RightWrist, RightHand, RightPalm, RightFingers,
        LeftHip, LeftThigh, LeftKnee, LeftCalf, LeftAnkle, LeftFoot,
        RightHip, RightThigh, RightKnee, RightCalf, RightAnkle, RightFoot,
        Core, FullBody, Unknown
    }
}