using UnityEngine;
using System.Collections.Generic;

namespace VRAvatar.Haptics
{
    /// <summary>
    /// Manages haptic feedback systems for VR avatar interactions
    /// </summary>
    public class HapticManager : MonoBehaviour
    {
        [Header("Haptic Devices")]
        public List<HapticDevice> hapticDevices = new List<HapticDevice>();
        
        [Header("Feedback Settings")]
        public float globalIntensityMultiplier = 1.0f;
        public bool enableHaptics = true;
        
        private static HapticManager instance;
        public static HapticManager Instance => instance;
        
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public void TriggerHapticFeedback(HapticFeedbackType type, float intensity, float duration)
        {
            if (!enableHaptics) return;
            
            foreach (var device in hapticDevices)
            {
                device.PlayHapticFeedback(type, intensity * globalIntensityMultiplier, duration);
            }
        }
    }
    
    public enum HapticFeedbackType
    {
        Touch,
        Grab,
        Impact,
        Vibration,
        Texture
    }
    
    [System.Serializable]
    public class HapticDevice
    {
        public string deviceName;
        public bool isActive = true;
        
        public void PlayHapticFeedback(HapticFeedbackType type, float intensity, float duration)
        {
            if (!isActive) return;
            // Implement device-specific haptic feedback
            Debug.Log($"Haptic feedback on {deviceName}: {type}, intensity={intensity}, duration={duration}");
        }
    }
}
