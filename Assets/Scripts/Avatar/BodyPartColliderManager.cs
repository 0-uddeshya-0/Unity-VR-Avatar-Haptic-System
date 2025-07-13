using UnityEngine;
using System.Collections.Generic;
using System;

namespace VRAvatarSystem.Avatar
{
    /// <summary>
    /// Body Part Collider Manager for VR Avatar Touch Detection
    /// Manages colliders for different body parts and handles touch events
    /// </summary>
    public class BodyPartColliderManager : MonoBehaviour
    {
        [Header("Body Part Configuration")]
        [SerializeField] private bool autoSetupOnStart = true;
        [SerializeField] private LayerMask touchDetectionLayer = -1;
        [SerializeField] private bool enableDebugLogging = false;
        
        [Header("Collider Settings")]
        [SerializeField] private float handColliderRadius = 0.08f;
        [SerializeField] private float armColliderRadius = 0.06f;
        [SerializeField] private float torsoColliderSize = 0.3f;
        [SerializeField] private float headColliderRadius = 0.12f;
        
        // Body part collections
        private Dictionary<BodyPart, List<Collider>> bodyPartColliders;
        private Dictionary<Collider, BodyPart> colliderToBodyPart;
        private Animator avatarAnimator;
        
        // Events
        public static event Action<BodyPart, Collision, GameObject> OnBodyPartTouched;
        public static event Action<BodyPart, Collision, GameObject> OnBodyPartTouchEnded;
        public static event Action<BodyPart, Collider, GameObject> OnBodyPartTriggerEntered;
        public static event Action<BodyPart, Collider, GameObject> OnBodyPartTriggerExited;
        
        /// <summary>
        /// Body part enumeration for touch detection
        /// </summary>
        public enum BodyPart
        {
            Head,
            Torso,
            LeftHand,
            RightHand,
            LeftArm,
            RightArm,
            LeftShoulder,
            RightShoulder,
            Chest,
            Back
        }
        
        void Start()
        {
            if (autoSetupOnStart)
            {
                SetupColliders(touchDetectionLayer);
            }
        }
        
        /// <summary>
        /// Setup colliders for all body parts
        /// </summary>
        public void SetupColliders(LayerMask detectionLayer)
        {
            touchDetectionLayer = detectionLayer;
            
            // Initialize collections
            bodyPartColliders = new Dictionary<BodyPart, List<Collider>>();
            colliderToBodyPart = new Dictionary<Collider, BodyPart>();
            
            // Get avatar animator for bone mapping
            avatarAnimator = GetComponent<Animator>();
            if (avatarAnimator == null)
            {
                Debug.LogError("BodyPartColliderManager: No Animator found on avatar!");
                return;
            }
            
            // Setup colliders for each body part
            SetupHeadColliders();
            SetupTorsoColliders();
            SetupHandColliders();
            SetupArmColliders();
            SetupShoulderColliders();
            
            if (enableDebugLogging)
            {
                Debug.Log($"BodyPartColliderManager: Setup completed with {colliderToBodyPart.Count} colliders");
            }
        }
        
        /// <summary>
        /// Setup head colliders
        /// </summary>
        private void SetupHeadColliders()
        {
            Transform headBone = avatarAnimator.GetBoneTransform(HumanBodyBones.Head);
            if (headBone != null)
            {
                SphereCollider headCollider = CreateSphereCollider(headBone.gameObject, headColliderRadius, "Head_Collider");
                AddColliderToBodyPart(BodyPart.Head, headCollider);
            }
        }
        
        /// <summary>
        /// Setup torso colliders
        /// </summary>
        private void SetupTorsoColliders()
        {
            // Chest collider
            Transform chestBone = avatarAnimator.GetBoneTransform(HumanBodyBones.Chest);
            if (chestBone != null)
            {
                BoxCollider chestCollider = CreateBoxCollider(chestBone.gameObject, 
                    new Vector3(torsoColliderSize, torsoColliderSize, torsoColliderSize * 0.5f), "Chest_Collider");
                AddColliderToBodyPart(BodyPart.Chest, chestCollider);
                AddColliderToBodyPart(BodyPart.Torso, chestCollider);
            }
            
            // Spine collider for back
            Transform spineBone = avatarAnimator.GetBoneTransform(HumanBodyBones.Spine);
            if (spineBone != null)
            {
                BoxCollider backCollider = CreateBoxCollider(spineBone.gameObject,
                    new Vector3(torsoColliderSize, torsoColliderSize * 0.8f, torsoColliderSize * 0.3f), "Back_Collider");
                backCollider.center = new Vector3(0, 0, -0.1f); // Position behind the spine
                AddColliderToBodyPart(BodyPart.Back, backCollider);
            }
        }
        
        /// <summary>
        /// Setup hand colliders
        /// </summary>
        private void SetupHandColliders()
        {
            // Left hand
            Transform leftHandBone = avatarAnimator.GetBoneTransform(HumanBodyBones.LeftHand);
            if (leftHandBone != null)
            {
                SphereCollider leftHandCollider = CreateSphereCollider(leftHandBone.gameObject, handColliderRadius, "LeftHand_Collider");
                AddColliderToBodyPart(BodyPart.LeftHand, leftHandCollider);
            }
            
            // Right hand
            Transform rightHandBone = avatarAnimator.GetBoneTransform(HumanBodyBones.RightHand);
            if (rightHandBone != null)
            {
                SphereCollider rightHandCollider = CreateSphereCollider(rightHandBone.gameObject, handColliderRadius, "RightHand_Collider");
                AddColliderToBodyPart(BodyPart.RightHand, rightHandCollider);
            }
        }
        
        /// <summary>
        /// Setup arm colliders
        /// </summary>
        private void SetupArmColliders()
        {
            // Left arm (forearm and upper arm)
            SetupArmBone(HumanBodyBones.LeftLowerArm, BodyPart.LeftArm, "LeftForearm_Collider");
            SetupArmBone(HumanBodyBones.LeftUpperArm, BodyPart.LeftArm, "LeftUpperArm_Collider");
            
            // Right arm (forearm and upper arm)
            SetupArmBone(HumanBodyBones.RightLowerArm, BodyPart.RightArm, "RightForearm_Collider");
            SetupArmBone(HumanBodyBones.RightUpperArm, BodyPart.RightArm, "RightUpperArm_Collider");
        }
        
        private void SetupArmBone(HumanBodyBones bone, BodyPart bodyPart, string colliderName)
        {
            Transform armBone = avatarAnimator.GetBoneTransform(bone);
            if (armBone != null)
            {
                CapsuleCollider armCollider = CreateCapsuleCollider(armBone.gameObject, armColliderRadius, 0.25f, colliderName);
                AddColliderToBodyPart(bodyPart, armCollider);
            }
        }
        
        /// <summary>
        /// Setup shoulder colliders
        /// </summary>
        private void SetupShoulderColliders()
        {
            // Left shoulder
            Transform leftShoulderBone = avatarAnimator.GetBoneTransform(HumanBodyBones.LeftShoulder);
            if (leftShoulderBone != null)
            {
                SphereCollider leftShoulderCollider = CreateSphereCollider(leftShoulderBone.gameObject, armColliderRadius * 1.2f, "LeftShoulder_Collider");
                AddColliderToBodyPart(BodyPart.LeftShoulder, leftShoulderCollider);
            }
            
            // Right shoulder
            Transform rightShoulderBone = avatarAnimator.GetBoneTransform(HumanBodyBones.RightShoulder);
            if (rightShoulderBone != null)
            {
                SphereCollider rightShoulderCollider = CreateSphereCollider(rightShoulderBone.gameObject, armColliderRadius * 1.2f, "RightShoulder_Collider");
                AddColliderToBodyPart(BodyPart.RightShoulder, rightShoulderCollider);
            }
        }
        
        /// <summary>
        /// Create a sphere collider on the specified game object
        /// </summary>
        private SphereCollider CreateSphereCollider(GameObject target, float radius, string name)
        {
            GameObject colliderObj = new GameObject(name);
            colliderObj.transform.SetParent(target.transform);
            colliderObj.transform.localPosition = Vector3.zero;
            colliderObj.transform.localRotation = Quaternion.identity;
            colliderObj.layer = GetLayerFromMask(touchDetectionLayer);
            
            SphereCollider collider = colliderObj.AddComponent<SphereCollider>();
            collider.radius = radius;
            collider.isTrigger = false; // Set to false for collision detection
            
            // Add collision detector
            BodyPartCollisionDetector detector = colliderObj.AddComponent<BodyPartCollisionDetector>();
            detector.Initialize(this);
            
            return collider;
        }
        
        /// <summary>
        /// Create a box collider on the specified game object
        /// </summary>
        private BoxCollider CreateBoxCollider(GameObject target, Vector3 size, string name)
        {
            GameObject colliderObj = new GameObject(name);
            colliderObj.transform.SetParent(target.transform);
            colliderObj.transform.localPosition = Vector3.zero;
            colliderObj.transform.localRotation = Quaternion.identity;
            colliderObj.layer = GetLayerFromMask(touchDetectionLayer);
            
            BoxCollider collider = colliderObj.AddComponent<BoxCollider>();
            collider.size = size;
            collider.isTrigger = false;
            
            // Add collision detector
            BodyPartCollisionDetector detector = colliderObj.AddComponent<BodyPartCollisionDetector>();
            detector.Initialize(this);
            
            return collider;
        }
        
        /// <summary>
        /// Create a capsule collider on the specified game object
        /// </summary>
        private CapsuleCollider CreateCapsuleCollider(GameObject target, float radius, float height, string name)
        {
            GameObject colliderObj = new GameObject(name);
            colliderObj.transform.SetParent(target.transform);
            colliderObj.transform.localPosition = Vector3.zero;
            colliderObj.transform.localRotation = Quaternion.identity;
            colliderObj.layer = GetLayerFromMask(touchDetectionLayer);
            
            CapsuleCollider collider = colliderObj.AddComponent<CapsuleCollider>();
            collider.radius = radius;
            collider.height = height;
            collider.direction = 1; // Y-axis
            collider.isTrigger = false;
            
            // Add collision detector
            BodyPartCollisionDetector detector = colliderObj.AddComponent<BodyPartCollisionDetector>();
            detector.Initialize(this);
            
            return collider;
        }
        
        /// <summary>
        /// Add collider to body part mapping
        /// </summary>
        private void AddColliderToBodyPart(BodyPart bodyPart, Collider collider)
        {
            if (!bodyPartColliders.ContainsKey(bodyPart))
            {
                bodyPartColliders[bodyPart] = new List<Collider>();
            }
            
            bodyPartColliders[bodyPart].Add(collider);
            colliderToBodyPart[collider] = bodyPart;
        }
        
        /// <summary>
        /// Get layer number from layer mask
        /// </summary>
        private int GetLayerFromMask(LayerMask mask)
        {
            int layer = 0;
            int maskValue = mask.value;
            while (maskValue > 1)
            {
                maskValue >>= 1;
                layer++;
            }
            return layer;
        }
        
        /// <summary>
        /// Handle collision detection from child colliders
        /// </summary>
        public void HandleCollision(Collider collider, Collision collision)
        {
            if (colliderToBodyPart.ContainsKey(collider))
            {
                BodyPart bodyPart = colliderToBodyPart[collider];
                OnBodyPartTouched?.Invoke(bodyPart, collision, collision.gameObject);
                
                if (enableDebugLogging)
                {
                    Debug.Log($"Body part touched: {bodyPart} by {collision.gameObject.name}");
                }
            }
        }
        
        /// <summary>
        /// Handle collision exit from child colliders
        /// </summary>
        public void HandleCollisionExit(Collider collider, Collision collision)
        {
            if (colliderToBodyPart.ContainsKey(collider))
            {
                BodyPart bodyPart = colliderToBodyPart[collider];
                OnBodyPartTouchEnded?.Invoke(bodyPart, collision, collision.gameObject);
                
                if (enableDebugLogging)
                {
                    Debug.Log($"Body part touch ended: {bodyPart} from {collision.gameObject.name}");
                }
            }
        }
        
        /// <summary>
        /// Handle trigger detection from child colliders
        /// </summary>
        public void HandleTriggerEnter(Collider collider, Collider other)
        {
            if (colliderToBodyPart.ContainsKey(collider))
            {
                BodyPart bodyPart = colliderToBodyPart[collider];
                OnBodyPartTriggerEntered?.Invoke(bodyPart, other, other.gameObject);
                
                if (enableDebugLogging)
                {
                    Debug.Log($"Body part trigger entered: {bodyPart} by {other.gameObject.name}");
                }
            }
        }
        
        /// <summary>
        /// Handle trigger exit from child colliders
        /// </summary>
        public void HandleTriggerExit(Collider collider, Collider other)
        {
            if (colliderToBodyPart.ContainsKey(collider))
            {
                BodyPart bodyPart = colliderToBodyPart[collider];
                OnBodyPartTriggerExited?.Invoke(bodyPart, other, other.gameObject);
                
                if (enableDebugLogging)
                {
                    Debug.Log($"Body part trigger exited: {bodyPart} from {other.gameObject.name}");
                }
            }
        }
        
        /// <summary>
        /// Get all colliders for a specific body part
        /// </summary>
        public List<Collider> GetCollidersForBodyPart(BodyPart bodyPart)
        {
            return bodyPartColliders.ContainsKey(bodyPart) ? bodyPartColliders[bodyPart] : new List<Collider>();
        }
        
        /// <summary>
        /// Get body part for a specific collider
        /// </summary>
        public BodyPart? GetBodyPartForCollider(Collider collider)
        {
            return colliderToBodyPart.ContainsKey(collider) ? colliderToBodyPart[collider] : (BodyPart?)null;
        }
        
        /// <summary>
        /// Enable/disable colliders for a specific body part
        /// </summary>
        public void SetBodyPartCollidersEnabled(BodyPart bodyPart, bool enabled)
        {
            if (bodyPartColliders.ContainsKey(bodyPart))
            {
                foreach (Collider collider in bodyPartColliders[bodyPart])
                {
                    collider.enabled = enabled;
                }
            }
        }
        
        /// <summary>
        /// Remove all body part colliders
        /// </summary>
        public void RemoveAllColliders()
        {
            if (colliderToBodyPart != null)
            {
                foreach (Collider collider in colliderToBodyPart.Keys)
                {
                    if (collider != null && collider.gameObject != null)
                    {
                        DestroyImmediate(collider.gameObject);
                    }
                }
                
                colliderToBodyPart.Clear();
                bodyPartColliders.Clear();
            }
        }
        
        void OnDestroy()
        {
            RemoveAllColliders();
        }
    }
    
    /// <summary>
    /// Collision detector component for body part colliders
    /// </summary>
    public class BodyPartCollisionDetector : MonoBehaviour
    {
        private BodyPartColliderManager manager;
        private Collider thisCollider;
        
        public void Initialize(BodyPartColliderManager colliderManager)
        {
            manager = colliderManager;
            thisCollider = GetComponent<Collider>();
        }
        
        void OnCollisionEnter(Collision collision)
        {
            if (manager != null)
            {
                manager.HandleCollision(thisCollider, collision);
            }
        }
        
        void OnCollisionExit(Collision collision)
        {
            if (manager != null)
            {
                manager.HandleCollisionExit(thisCollider, collision);
            }
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (manager != null)
            {
                manager.HandleTriggerEnter(thisCollider, other);
            }
        }
        
        void OnTriggerExit(Collider other)
        {
            if (manager != null)
            {
                manager.HandleTriggerExit(thisCollider, other);
            }
        }
    }
}
