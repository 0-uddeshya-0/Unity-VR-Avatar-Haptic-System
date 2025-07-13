using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using Photon.Realtime;
using VRAvatar.Avatar;
using VRAvatar.Haptics;

namespace VRAvatar.Networking
{
    /// <summary>
    /// Photon PUN 2 networking synchronization for VR avatars
    /// Handles avatar position, rotation, animation, and interaction data
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
