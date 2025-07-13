using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using VRAvatar.Haptics;

namespace VRAvatar.VR
{
    /// <summary>
    /// Handles VR interactions with integrated haptic feedback
    /// </summary>
    public class VRInteractionHandler : MonoBehaviour
    {
        [Header("Interaction Settings")]
        public XRRayInteractor rayInteractor;
        public XRDirectInteractor directInteractor;
        
        [Header("Haptic Feedback")]
        public float grabHapticIntensity = 0.8f;
        public float touchHapticIntensity = 0.3f;
        public float hapticDuration = 0.1f;
        
        private void Start()
        {
            SetupInteractionEvents();
        }
        
        private void SetupInteractionEvents()
        {
            if (rayInteractor != null)
            {
                rayInteractor.selectEntered.AddListener(OnObjectGrabbed);
                rayInteractor.selectExited.AddListener(OnObjectReleased);
            }
            
            if (directInteractor != null)
            {
                directInteractor.selectEntered.AddListener(OnObjectGrabbed);
                directInteractor.selectExited.AddListener(OnObjectReleased);
                directInteractor.hoverEntered.AddListener(OnObjectTouched);
            }
        }
        
        private void OnObjectGrabbed(SelectEnterEventArgs args)
        {
            HapticManager.Instance?.TriggerHapticFeedback(
                HapticFeedbackType.Grab, 
                grabHapticIntensity, 
                hapticDuration
            );
        }
        
        private void OnObjectReleased(SelectExitEventArgs args)
        {
            HapticManager.Instance?.TriggerHapticFeedback(
                HapticFeedbackType.Touch, 
                touchHapticIntensity * 0.5f, 
                hapticDuration * 0.5f
            );
        }
        
        private void OnObjectTouched(HoverEnterEventArgs args)
        {
            HapticManager.Instance?.TriggerHapticFeedback(
                HapticFeedbackType.Touch, 
                touchHapticIntensity, 
                hapticDuration * 0.3f
            );
        }
    }
}
