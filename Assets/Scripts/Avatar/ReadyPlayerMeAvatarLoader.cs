using UnityEngine;
using UnityEngine.XR;
using ReadyPlayerMe.Core;

namespace VRAvatarSystem.Avatar
{
    /// <summary>
    /// ReadyPlayerMe Avatar Loader for VR Avatar System
    /// Handles avatar loading, VR tracking setup, and body part collider configuration
    /// </summary>
    public class ReadyPlayerMeAvatarLoader : MonoBehaviour
    {
        [Header("Avatar Configuration")]
        [SerializeField] private string avatarUrl = "";
        [SerializeField] private bool loadOnStart = false;
        [SerializeField] private Transform vrTrackingRoot;
        
        [Header("VR Tracking Setup")]
        [SerializeField] private Transform headTracker;
        [SerializeField] private Transform leftHandTracker;
        [SerializeField] private Transform rightHandTracker;
        
        [Header("Body Part Colliders")]
        [SerializeField] private bool autoConfigureColliders = true;
        [SerializeField] private LayerMask touchDetectionLayer = -1;
        
        private GameObject loadedAvatar;
        private AvatarObjectLoader avatarLoader;
        private BodyPartColliderManager colliderManager;
        
        // Events
        public System.Action<GameObject> OnAvatarLoaded;
        public System.Action<string> OnAvatarLoadFailed;
        
        void Start()
        {
            if (loadOnStart && !string.IsNullOrEmpty(avatarUrl))
            {
                LoadAvatar(avatarUrl);
            }
            
            SetupVRTracking();
        }
        
        /// <summary>
        /// Load avatar from ReadyPlayerMe URL
        /// </summary>
        public void LoadAvatar(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError("Avatar URL is empty!");
                OnAvatarLoadFailed?.Invoke("Empty URL");
                return;
            }
            
            avatarUrl = url;
            
            // Create avatar loader
            if (avatarLoader == null)
            {
                avatarLoader = new AvatarObjectLoader();
            }
            
            // Set up loading callbacks
            avatarLoader.OnCompleted += OnAvatarLoadCompleted;
            avatarLoader.OnFailed += OnAvatarLoadFailedCallback;
            
            // Start loading
            Debug.Log($"Loading ReadyPlayerMe avatar from: {url}");
            avatarLoader.LoadAvatar(url);
        }
        
        private void OnAvatarLoadCompleted(object sender, CompletionEventArgs args)
        {
            loadedAvatar = args.Avatar;
            
            Debug.Log("Avatar loaded successfully!");
            
            // Position avatar
            if (loadedAvatar != null)
            {
                loadedAvatar.transform.SetParent(transform);
                loadedAvatar.transform.localPosition = Vector3.zero;
                loadedAvatar.transform.localRotation = Quaternion.identity;
                
                // Setup body part colliders
                if (autoConfigureColliders)
                {
                    SetupBodyPartColliders();
                }
                
                // Setup VR tracking
                ConfigureVRTracking();
                
                OnAvatarLoaded?.Invoke(loadedAvatar);
            }
            
            // Cleanup
            if (avatarLoader != null)
            {
                avatarLoader.OnCompleted -= OnAvatarLoadCompleted;
                avatarLoader.OnFailed -= OnAvatarLoadFailedCallback;
            }
        }
        
        private void OnAvatarLoadFailedCallback(object sender, FailureEventArgs args)
        {
            Debug.LogError($"Avatar loading failed: {args.Message}");
            OnAvatarLoadFailed?.Invoke(args.Message);
            
            // Cleanup
            if (avatarLoader != null)
            {
                avatarLoader.OnCompleted -= OnAvatarLoadCompleted;
                avatarLoader.OnFailed -= OnAvatarLoadFailedCallback;
            }
        }
        
        /// <summary>
        /// Setup VR tracking for the loaded avatar
        /// </summary>
        private void SetupVRTracking()
        {
            if (!XRSettings.enabled)
            {
                Debug.LogWarning("XR is not enabled. VR tracking setup skipped.");
                return;
            }
            
            // Find or create VR tracking root
            if (vrTrackingRoot == null)
            {
                GameObject trackingRoot = new GameObject("VR_Tracking_Root");
                trackingRoot.transform.SetParent(transform);
                vrTrackingRoot = trackingRoot.transform;
            }
            
            // Setup head tracking
            SetupHeadTracking();
            
            // Setup hand tracking
            SetupHandTracking();
        }
        
        private void SetupHeadTracking()
        {
            if (headTracker == null)
            {
                GameObject headTrackerObj = new GameObject("Head_Tracker");
                headTrackerObj.transform.SetParent(vrTrackingRoot);
                headTracker = headTrackerObj.transform;
                
                // Add XR tracking component for head
                var headTracking = headTrackerObj.AddComponent<TrackedPoseDriver>();
                headTracking.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRDevice, TrackedPoseDriver.TrackedPose.Head);
            }
        }
        
        private void SetupHandTracking()
        {
            // Left hand tracking
            if (leftHandTracker == null)
            {
                GameObject leftHandObj = new GameObject("LeftHand_Tracker");
                leftHandObj.transform.SetParent(vrTrackingRoot);
                leftHandTracker = leftHandObj.transform;
                
                var leftHandTracking = leftHandObj.AddComponent<TrackedPoseDriver>();
                leftHandTracking.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);
            }
            
            // Right hand tracking
            if (rightHandTracker == null)
            {
                GameObject rightHandObj = new GameObject("RightHand_Tracker");
                rightHandObj.transform.SetParent(vrTrackingRoot);
                rightHandTracker = rightHandObj.transform;
                
                var rightHandTracking = rightHandObj.AddComponent<TrackedPoseDriver>();
                rightHandTracking.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);
            }
        }
        
        /// <summary>
        /// Configure VR tracking for the loaded avatar
        /// </summary>
        private void ConfigureVRTracking()
        {
            if (loadedAvatar == null) return;
            
            // Find avatar's bone structure
            Animator avatarAnimator = loadedAvatar.GetComponent<Animator>();
            if (avatarAnimator == null)
            {
                Debug.LogWarning("Avatar doesn't have an Animator component for bone mapping.");
                return;
            }
            
            // Map head tracking
            Transform headBone = avatarAnimator.GetBoneTransform(HumanBodyBones.Head);
            if (headBone != null && headTracker != null)
            {
                // Create constraint or direct mapping for head tracking
                ConfigureBoneTracking(headBone, headTracker);
            }
            
            // Map hand tracking
            Transform leftHandBone = avatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
            Transform rightHandBone = avatarAnimator.GetBoneTransform(HumanBodyBones.RightHand);
            
            if (leftHandBone != null && leftHandTracker != null)
            {
                ConfigureBoneTracking(leftHandBone, leftHandTracker);
            }
            
            if (rightHandBone != null && rightHandTracker != null)
            {
                ConfigureBoneTracking(rightHandBone, rightHandTracker);
            }
        }
        
        private void ConfigureBoneTracking(Transform bone, Transform tracker)
        {
            // Simple direct tracking - can be enhanced with constraints
            var trackingScript = bone.gameObject.AddComponent<VRBoneTracker>();
            trackingScript.SetTracker(tracker);
        }
        
        /// <summary>
        /// Setup body part colliders for touch detection
        /// </summary>
        private void SetupBodyPartColliders()
        {
            if (loadedAvatar == null) return;
            
            colliderManager = loadedAvatar.GetComponent<BodyPartColliderManager>();
            if (colliderManager == null)
            {
                colliderManager = loadedAvatar.AddComponent<BodyPartColliderManager>();
            }
            
            colliderManager.SetupColliders(touchDetectionLayer);
        }
        
        /// <summary>
        /// Get the currently loaded avatar
        /// </summary>
        public GameObject GetLoadedAvatar()
        {
            return loadedAvatar;
        }
        
        /// <summary>
        /// Check if avatar is currently loaded
        /// </summary>
        public bool IsAvatarLoaded()
        {
            return loadedAvatar != null;
        }
        
        /// <summary>
        /// Unload current avatar
        /// </summary>
        public void UnloadAvatar()
        {
            if (loadedAvatar != null)
            {
                DestroyImmediate(loadedAvatar);
                loadedAvatar = null;
                Debug.Log("Avatar unloaded.");
            }
        }
        
        void OnDestroy()
        {
            // Cleanup
            if (avatarLoader != null)
            {
                avatarLoader.OnCompleted -= OnAvatarLoadCompleted;
                avatarLoader.OnFailed -= OnAvatarLoadFailedCallback;
            }
        }
    }
    
    /// <summary>
    /// Simple VR bone tracking component
    /// </summary>
    public class VRBoneTracker : MonoBehaviour
    {
        private Transform trackerTransform;
        
        public void SetTracker(Transform tracker)
        {
            trackerTransform = tracker;
        }
        
        void Update()
        {
            if (trackerTransform != null)
            {
                transform.position = trackerTransform.position;
                transform.rotation = trackerTransform.rotation;
            }
        }
    }
}
