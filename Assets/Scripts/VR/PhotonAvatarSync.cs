using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Realtime;
using VRAvatarSystem.Avatar;
using VRAvatar.Haptics;
using System.Collections;

namespace VRAvatar.Networking
{
    /// <summary>
    /// Complete Photon PUN 2 networking synchronization for VR avatars
    /// Handles avatar position, rotation, animation, and touch interaction data
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PhotonAvatarSync : MonoBehaviourPunPV, IPunObservable
    {
        [Header("Avatar Components")]
        public Transform headTransform;
        public Transform leftHandTransform;
        public Transform rightHandTransform;
        public Animator avatarAnimator;
        public ReadyPlayerMeAvatarLoader avatarLoader;
        
        [Header("Synchronization Settings")]
        [Range(1, 30)]
        public int sendRate = 20;
        [Range(1, 30)]
        public int observationRate = 20;
        public bool synchronizeVoice = true;
        public bool synchronizeHaptics = true;
        
        [Header("Network Smoothing")]
        public float positionLerp = 10f;
        public float rotationLerp = 10f;
        
        // Network position data
        private Vector3 networkHeadPosition;
        private Quaternion networkHeadRotation;
        private Vector3 networkLeftHandPosition;
        private Quaternion networkLeftHandRotation;
        private Vector3 networkRightHandPosition;
        private Quaternion networkRightHandRotation;
        
        // Touch event management
        private TouchEventManager touchEventManager;
        private HapticManager hapticManager;
        
        private void Start()
        {
            // Configure Photon send rates
            PhotonNetwork.SendRate = sendRate;
            PhotonNetwork.SerializationRate = observationRate;
            
            // Initialize components
            touchEventManager = FindObjectOfType<TouchEventManager>();
            hapticManager = HapticManager.Instance;
            
            // Subscribe to touch events if this is our avatar
            if (photonView.IsMine)
            {
                BodyPartColliderManager.OnBodyPartTouched += OnLocalBodyPartTouched;
                BodyPartColliderManager.OnBodyPartTouchEnded += OnLocalBodyPartTouchEnded;
            }
        }
        
        private void Update()
        {
            if (!photonView.IsMine)
            {
                // Interpolate remote avatar positions
                InterpolateRemoteAvatar();
            }
        }
        
        /// <summary>
        /// Handle local touch events and send to network
        /// </summary>
        private void OnLocalBodyPartTouched(BodyPartColliderManager.BodyPart bodyPart, Collision collision, GameObject toucher)
        {
            if (!synchronizeHaptics) return;
            
            // Create touch event data
            var touchData = new TouchEventData
            {
                targetBodyPart = bodyPart.ToString(),
                sourcePlayerId = photonView.ViewID,
                contactPoint = collision.contacts[0].point,
                contactNormal = collision.contacts[0].normal,
                intensity = CalculateTouchIntensity(collision),
                timestamp = (float)PhotonNetwork.Time
            };
            
            // Send to all clients
            photonView.RPC("OnRemoteTouchEvent", RpcTarget.Others, 
                touchData.targetBodyPart, 
                touchData.sourcePlayerId, 
                touchData.contactPoint.x, touchData.contactPoint.y, touchData.contactPoint.z,
                touchData.intensity, 
                touchData.timestamp);
        }
        
        private void OnLocalBodyPartTouchEnded(BodyPartColliderManager.BodyPart bodyPart, Collision collision, GameObject toucher)
        {
            if (!synchronizeHaptics) return;
            
            photonView.RPC("OnRemoteTouchEnded", RpcTarget.Others, bodyPart.ToString(), photonView.ViewID);
        }
        
        /// <summary>
        /// Receive remote touch events
        /// </summary>
        [PunRPC]
        void OnRemoteTouchEvent(string bodyPartString, int sourcePlayerId, float posX, float posY, float posZ, float intensity, float timestamp)
        {
            if (photonView.ViewID == sourcePlayerId) return; // Don't process our own events
            
            var bodyPart = (BodyPartColliderManager.BodyPart)System.Enum.Parse(typeof(BodyPartColliderManager.BodyPart), bodyPartString);
            var contactPoint = new Vector3(posX, posY, posZ);
            
            // Trigger haptic feedback for remote touch
            if (hapticManager != null)
            {
                var hapticFeedback = new HapticFeedbackType();
                var mappedRegion = MapBodyPartToHapticRegion(bodyPart);
                hapticManager.TriggerHapticFeedback(hapticFeedback, intensity, 0.3f);
            }
            
            // Trigger visual feedback
            if (touchEventManager != null)
            {
                touchEventManager.ProcessRemoteTouchEvent(bodyPart, contactPoint, intensity);
            }
        }
        
        [PunRPC]
        void OnRemoteTouchEnded(string bodyPartString, int sourcePlayerId)
        {
            var bodyPart = (BodyPartColliderManager.BodyPart)System.Enum.Parse(typeof(BodyPartColliderManager.BodyPart), bodyPartString);
            
            if (touchEventManager != null)
            {
                touchEventManager.ProcessRemoteTouchEnd(bodyPart);
            }
        }
        
        /// <summary>
        /// Synchronize avatar transform data
        /// </summary>
        [PunRPC]
        public void SyncAvatarData(float[] headPos, float[] headRot, float[] leftHandPos, float[] leftHandRot, float[] rightHandPos, float[] rightHandRot)
        {
            if (photonView.IsMine) return;
            
            networkHeadPosition = new Vector3(headPos[0], headPos[1], headPos[2]);
            networkHeadRotation = new Quaternion(headRot[0], headRot[1], headRot[2], headRot[3]);
            networkLeftHandPosition = new Vector3(leftHandPos[0], leftHandPos[1], leftHandPos[2]);
            networkLeftHandRotation = new Quaternion(leftHandRot[0], leftHandRot[1], leftHandRot[2], leftHandRot[3]);
            networkRightHandPosition = new Vector3(rightHandPos[0], rightHandPos[1], rightHandPos[2]);
            networkRightHandRotation = new Quaternion(rightHandRot[0], rightHandRot[1], rightHandRot[2], rightHandRot[3]);
        }
        
        /// <summary>
        /// IPunObservable implementation for automatic data streaming
        /// </summary>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send local avatar data
                if (headTransform != null)
                {
                    stream.SendNext(headTransform.position);
                    stream.SendNext(headTransform.rotation);
                }
                if (leftHandTransform != null)
                {
                    stream.SendNext(leftHandTransform.position);
                    stream.SendNext(leftHandTransform.rotation);
                }
                if (rightHandTransform != null)
                {
                    stream.SendNext(rightHandTransform.position);
                    stream.SendNext(rightHandTransform.rotation);
                }
            }
            else
            {
                // Receive remote avatar data
                networkHeadPosition = (Vector3)stream.ReceiveNext();
                networkHeadRotation = (Quaternion)stream.ReceiveNext();
                networkLeftHandPosition = (Vector3)stream.ReceiveNext();
                networkLeftHandRotation = (Quaternion)stream.ReceiveNext();
                networkRightHandPosition = (Vector3)stream.ReceiveNext();
                networkRightHandRotation = (Quaternion)stream.ReceiveNext();
            }
        }
        
        private void InterpolateRemoteAvatar()
        {
            if (headTransform != null)
            {
                headTransform.position = Vector3.Lerp(headTransform.position, networkHeadPosition, Time.deltaTime * positionLerp);
                headTransform.rotation = Quaternion.Lerp(headTransform.rotation, networkHeadRotation, Time.deltaTime * rotationLerp);
            }
            
            if (leftHandTransform != null)
            {
                leftHandTransform.position = Vector3.Lerp(leftHandTransform.position, networkLeftHandPosition, Time.deltaTime * positionLerp);
                leftHandTransform.rotation = Quaternion.Lerp(leftHandTransform.rotation, networkLeftHandRotation, Time.deltaTime * rotationLerp);
            }
            
            if (rightHandTransform != null)
            {
                rightHandTransform.position = Vector3.Lerp(rightHandTransform.position, networkRightHandPosition, Time.deltaTime * positionLerp);
                rightHandTransform.rotation = Quaternion.Lerp(rightHandTransform.rotation, networkRightHandRotation, Time.deltaTime * rotationLerp);
            }
        }
        
        private float CalculateTouchIntensity(Collision collision)
        {
            // Calculate intensity based on collision force
            float intensity = collision.relativeVelocity.magnitude / 10f;
            return Mathf.Clamp01(intensity);
        }
        
        private string MapBodyPartToHapticRegion(BodyPartColliderManager.BodyPart bodyPart)
        {
            // Map body parts to haptic regions
            switch (bodyPart)
            {
                case BodyPartColliderManager.BodyPart.LeftHand:
                case BodyPartColliderManager.BodyPart.RightHand:
                    return "Hand";
                case BodyPartColliderManager.BodyPart.Chest:
                case BodyPartColliderManager.BodyPart.Torso:
                    return "Chest";
                case BodyPartColliderManager.BodyPart.Back:
                    return "Back";
                default:
                    return "General";
            }
        }
        
        private void OnDestroy()
        {
            if (photonView.IsMine)
            {
                BodyPartColliderManager.OnBodyPartTouched -= OnLocalBodyPartTouched;
                BodyPartColliderManager.OnBodyPartTouchEnded -= OnLocalBodyPartTouchEnded;
            }
        }
    }
    
    [System.Serializable]
    public struct TouchEventData
    {
        public string targetBodyPart;
        public int sourcePlayerId;
        public Vector3 contactPoint;
        public Vector3 contactNormal;
        public float intensity;
        public float timestamp;
    }
}