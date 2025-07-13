# API Reference - VR Avatar Interaction System
## Complete API Documentation for Touch Simulation and Haptic Systems

### 📚 Overview

This API reference provides comprehensive documentation for all public interfaces, classes, and methods in the VR Avatar Interaction System. The API is organized into logical modules covering networking, touch detection, haptic integration, and VR interaction components.

### 🗂️ API Modules

- **[Core Systems](#core-systems)** - Foundation classes and interfaces
- **[Touch Detection](#touch-detection)** - Touch simulation and collision detection
- **[Haptic Integration](#haptic-integration)** - Haptic device management and feedback
- **[Network Components](#network-components)** - Multiplayer networking and synchronization
- **[VR Interaction](#vr-interaction)** - VR-specific input and avatar control
- **[Configuration](#configuration)** - System settings and configuration
- **[Utilities](#utilities)** - Helper classes and tools

---

## Core Systems

### IHapticDevice Interface

Primary interface for all haptic device implementations.

```csharp
public interface IHapticDevice
{
    /// <summary>
    /// Gets the display name of the haptic device.
    /// </summary>
    string DeviceName { get; }
    
    /// <summary>
    /// Gets the current connection status of the device.
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// Initializes the haptic device with the provided configuration.
    /// </summary>
    /// <param name="config">Device configuration settings</param>
    /// <returns>True if initialization was successful</returns>
    bool Initialize(HapticConfig config);
    
    /// <summary>
    /// Sends haptic feedback to a specific body region.
    /// </summary>
    /// <param name="region">Target body region for feedback</param>
    /// <param name="feedback">Haptic feedback parameters</param>
    void SendHapticFeedback(BodyRegion region, HapticFeedback feedback);
    
    /// <summary>
    /// Properly shuts down the haptic device and releases resources.
    /// </summary>
    void Shutdown();
    
    /// <summary>
    /// Event triggered when device connection status changes.
    /// </summary>
    event System.Action<string> OnDeviceStatusChanged;
}
```

### HapticManager Class

Central manager for all haptic devices and feedback coordination.

```csharp
public class HapticManager : MonoBehaviour, IHapticManager
{
    /// <summary>
    /// Singleton instance of the HapticManager.
    /// </summary>
    public static HapticManager Instance { get; private set; }
    
    /// <summary>
    /// Gets a list of all registered haptic devices.
    /// </summary>
    public List<IHapticDevice> RegisteredDevices { get; }
    
    /// <summary>
    /// Gets a list of currently connected haptic devices.
    /// </summary>
    public List<IHapticDevice> ConnectedDevices { get; }
    
    /// <summary>
    /// Global haptic intensity multiplier (0.0 to 1.0).
    /// </summary>
    [Range(0f, 1f)]
    public float GlobalIntensity { get; set; } = 0.7f;
    
    /// <summary>
    /// Registers a new haptic device with the manager.
    /// </summary>
    /// <param name="device">Haptic device to register</param>
    /// <returns>True if registration was successful</returns>
    public bool RegisterDevice(IHapticDevice device);
    
    /// <summary>
    /// Unregisters a haptic device from the manager.
    /// </summary>
    /// <param name="device">Haptic device to unregister</param>
    /// <returns>True if unregistration was successful</returns>
    public bool UnregisterDevice(IHapticDevice device);
    
    /// <summary>
    /// Triggers haptic feedback on all connected devices.
    /// </summary>
    /// <param name="region">Target body region</param>
    /// <param name="feedback">Feedback parameters</param>
    public void TriggerHapticFeedback(BodyRegion region, HapticFeedback feedback);
    
    /// <summary>
    /// Triggers haptic feedback on a specific device.
    /// </summary>
    /// <param name="deviceName">Name of target device</param>
    /// <param name="region">Target body region</param>
    /// <param name="feedback">Feedback parameters</param>
    public void TriggerHapticFeedback(string deviceName, BodyRegion region, HapticFeedback feedback);
    
    /// <summary>
    /// Event triggered when a haptic device is connected or disconnected.
    /// </summary>
    public event System.Action<IHapticDevice, bool> OnDeviceConnectionChanged;
}
```

---

## Touch Detection

### TouchDetector Class

Detects and processes touch interactions on avatar body parts.

```csharp
public class TouchDetector : MonoBehaviour
{
    /// <summary>
    /// The body region this detector monitors.
    /// </summary>
    [SerializeField]
    public BodyRegion bodyRegion;
    
    /// <summary>
    /// Sensitivity threshold for touch detection (0.0 to 1.0).
    /// </summary>
    [SerializeField, Range(0f, 1f)]
    public float touchSensitivity = 0.5f;
    
    /// <summary>
    /// Layer mask for objects that can trigger touch events.
    /// </summary>
    [SerializeField]
    public LayerMask touchLayers = -1;
    
    /// <summary>
    /// Enable visual debugging for this touch detector.
    /// </summary>
    [SerializeField]
    public bool enableDebugVisualization = false;
    
    /// <summary>
    /// Gets the current touch state of this detector.
    /// </summary>
    public bool IsTouched { get; private set; }
    
    /// <summary>
    /// Gets the number of active touches on this detector.
    /// </summary>
    public int ActiveTouchCount { get; private set; }
    
    /// <summary>
    /// Event triggered when a touch is first detected.
    /// </summary>
    public event System.Action<TouchEvent> OnTouchDetected;
    
    /// <summary>
    /// Event triggered while a touch is ongoing.
    /// </summary>
    public event System.Action<TouchEvent> OnTouchUpdated;
    
    /// <summary>
    /// Event triggered when a touch is released.
    /// </summary>
    public event System.Action<TouchEvent> OnTouchReleased;
    
    /// <summary>
    /// Manually triggers a touch event (useful for testing).
    /// </summary>
    /// <param name="touchEvent">Touch event to simulate</param>
    public void SimulateTouchEvent(TouchEvent touchEvent);
    
    /// <summary>
    /// Gets all currently active touch events.
    /// </summary>
    /// <returns>Array of active touch events</returns>
    public TouchEvent[] GetActiveTouches();
    
    /// <summary>
    /// Sets the touch sensitivity for this detector.
    /// </summary>
    /// <param name="sensitivity">New sensitivity value (0.0 to 1.0)</param>
    public void SetTouchSensitivity(float sensitivity);
}
```

### TouchEvent Structure

Data structure representing a touch interaction event.

```csharp
[System.Serializable]
public struct TouchEvent
{
    /// <summary>
    /// The body region being touched.
    /// </summary>
    public BodyRegion targetRegion;
    
    /// <summary>
    /// The body region of the touching object (if applicable).
    /// </summary>
    public BodyRegion sourceRegion;
    
    /// <summary>
    /// World space position of the touch contact point.
    /// </summary>
    public Vector3 contactPoint;
    
    /// <summary>
    /// Normal vector at the contact point.
    /// </summary>
    public Vector3 contactNormal;
    
    /// <summary>
    /// Pressure of the touch (0.0 to 1.0).
    /// </summary>
    public float pressure;
    
    /// <summary>
    /// Estimated contact area in square units.
    /// </summary>
    public float contactArea;
    
    /// <summary>
    /// Haptic feedback parameters for this touch.
    /// </summary>
    public HapticFeedback feedback;
    
    /// <summary>
    /// Network ID of the player initiating the touch.
    /// </summary>
    public ulong networkId;
    
    /// <summary>
    /// Timestamp when the touch event occurred.
    /// </summary>
    public float timestamp;
    
    /// <summary>
    /// Unique identifier for this touch event.
    /// </summary>
    public int touchId;
    
    /// <summary>
    /// Duration of the touch (for ongoing touches).
    /// </summary>
    public float duration;
    
    /// <summary>
    /// Creates a new TouchEvent with default values.
    /// </summary>
    /// <param name="target">Target body region</param>
    /// <param name="contact">Contact point in world space</param>
    /// <returns>New TouchEvent instance</returns>
    public static TouchEvent Create(BodyRegion target, Vector3 contact);
}
```

### TouchEventManager Class

Manages touch event processing and distribution across the system.

```csharp
public class TouchEventManager : MonoBehaviour
{
    /// <summary>
    /// Singleton instance of the TouchEventManager.
    /// </summary>
    public static TouchEventManager Instance { get; private set; }
    
    /// <summary>
    /// Maximum number of simultaneous touch events allowed.
    /// </summary>
    public int maxSimultaneousTouches = 10;
    
    /// <summary>
    /// Global touch sensitivity multiplier.
    /// </summary>
    [Range(0f, 2f)]
    public float globalTouchSensitivity = 1f;
    
    /// <summary>
    /// Enable automatic haptic feedback for touch events.
    /// </summary>
    public bool enableAutoHapticFeedback = true;
    
    /// <summary>
    /// Processes a touch event and distributes it to relevant systems.
    /// </summary>
    /// <param name="touchEvent">Touch event to process</param>
    public void ProcessTouchEvent(TouchEvent touchEvent);
    
    /// <summary>
    /// Registers a touch detector with the manager.
    /// </summary>
    /// <param name="detector">Touch detector to register</param>
    public void RegisterTouchDetector(TouchDetector detector);
    
    /// <summary>
    /// Unregisters a touch detector from the manager.
    /// </summary>
    /// <param name="detector">Touch detector to unregister</param>
    public void UnregisterTouchDetector(TouchDetector detector);
    
    /// <summary>
    /// Gets all currently active touch events in the system.
    /// </summary>
    /// <returns>List of active touch events</returns>
    public List<TouchEvent> GetActiveTouchEvents();
    
    /// <summary>
    /// Event triggered when any touch event occurs in the system.
    /// </summary>
    public event System.Action<TouchEvent> OnTouchEventProcessed;
}
```

---

## Haptic Integration

### BHapticsDevice Class

Implementation of IHapticDevice for bHaptics TactSuit integration.

```csharp
public class BHapticsDevice : MonoBehaviour, IHapticDevice
{
    public string DeviceName => "bHaptics TactSuit";
    public bool IsConnected => Bhaptics.IsConnect();
    
    /// <summary>
    /// bHaptics application ID for registration.
    /// </summary>
    [SerializeField]
    public string applicationId = "VR_Avatar_System";
    
    /// <summary>
    /// Default haptic pattern intensity (0.0 to 1.0).
    /// </summary>
    [Range(0f, 1f)]
    public float defaultIntensity = 0.6f;
    
    /// <summary>
    /// Initializes the bHaptics device and registers patterns.
    /// </summary>
    /// <param name="config">Haptic configuration</param>
    /// <returns>True if initialization succeeded</returns>
    public bool Initialize(HapticConfig config);
    
    /// <summary>
    /// Sends haptic feedback to the specified body region.
    /// </summary>
    /// <param name="region">Target body region</param>
    /// <param name="feedback">Feedback parameters</param>
    public void SendHapticFeedback(BodyRegion region, HapticFeedback feedback);
    
    /// <summary>
    /// Registers a custom haptic pattern from a .tact file.
    /// </summary>
    /// <param name="patternName">Name for the pattern</param>
    /// <param name="tactFile">TextAsset containing .tact file data</param>
    public void RegisterHapticPattern(string patternName, TextAsset tactFile);
    
    /// <summary>
    /// Plays a predefined haptic pattern by name.
    /// </summary>
    /// <param name="patternName">Name of the pattern to play</param>
    /// <param name="intensity">Intensity multiplier (0.0 to 1.0)</param>
    public void PlayHapticPattern(string patternName, float intensity = 1f);
    
    /// <summary>
    /// Stops all currently playing haptic patterns.
    /// </summary>
    public void StopAllPatterns();
    
    /// <summary>
    /// Shuts down the bHaptics connection.
    /// </summary>
    public void Shutdown();
    
    /// <summary>
    /// Event triggered when device status changes.
    /// </summary>
    public event System.Action<string> OnDeviceStatusChanged;
}
```

### TeslasuitDevice Class

Implementation of IHapticDevice for Teslasuit integration.

```csharp
public class TeslasuitDevice : MonoBehaviour, IHapticDevice
{
    public string DeviceName => "Teslasuit";
    public bool IsConnected => teslasuitAPI?.IsConnected ?? false;
    
    /// <summary>
    /// Maximum stimulation intensity for safety (0.0 to 1.0).
    /// </summary>
    [Range(0f, 1f)]
    public float maxStimulationIntensity = 0.8f;
    
    /// <summary>
    /// Enable temperature feedback (if supported by device).
    /// </summary>
    public bool enableTemperatureFeedback = false;
    
    /// <summary>
    /// Safety cooling period between intense stimulations (seconds).
    /// </summary>
    public float safetyCoolingPeriod = 1f;
    
    /// <summary>
    /// Initializes the Teslasuit device and API connection.
    /// </summary>
    /// <param name="config">Haptic configuration</param>
    /// <returns>True if initialization succeeded</returns>
    public bool Initialize(HapticConfig config);
    
    /// <summary>
    /// Sends electrical stimulation to the specified body region.
    /// </summary>
    /// <param name="region">Target body region</param>
    /// <param name="feedback">Feedback parameters</param>
    public void SendHapticFeedback(BodyRegion region, HapticFeedback feedback);
    
    /// <summary>
    /// Sends temperature feedback to a body region (if supported).
    /// </summary>
    /// <param name="region">Target body region</param>
    /// <param name="temperature">Target temperature (-1.0 to 1.0, 0 = neutral)</param>
    /// <param name="duration">Duration in seconds</param>
    public void SendTemperatureFeedback(BodyRegion region, float temperature, float duration);
    
    /// <summary>
    /// Calibrates electrode placement for optimal stimulation.
    /// </summary>
    /// <returns>True if calibration was successful</returns>
    public bool CalibrateElectrodes();
    
    /// <summary>
    /// Emergency stop for all stimulation.
    /// </summary>
    public void EmergencyStop();
    
    /// <summary>
    /// Shuts down the Teslasuit connection.
    /// </summary>
    public void Shutdown();
    
    /// <summary>
    /// Event triggered when device status changes.
    /// </summary>
    public event System.Action<string> OnDeviceStatusChanged;
}
```

### HapticFeedback Structure

Data structure defining haptic feedback parameters.

```csharp
[System.Serializable]
public struct HapticFeedback
{
    /// <summary>
    /// Intensity of the haptic feedback (0.0 to 1.0).
    /// </summary>
    [Range(0f, 1f)]
    public float intensity;
    
    /// <summary>
    /// Duration of the feedback in seconds.
    /// </summary>
    public float duration;
    
    /// <summary>
    /// Haptic pattern type to use.
    /// </summary>
    public HapticPattern pattern;
    
    /// <summary>
    /// Frequency for vibrotactile devices (Hz).
    /// </summary>
    public float frequency;
    
    /// <summary>
    /// Direction vector for directional haptics.
    /// </summary>
    public Vector3 direction;
    
    /// <summary>
    /// Type of haptic feedback.
    /// </summary>
    public HapticType type;
    
    /// <summary>
    /// Custom pattern data (device-specific).
    /// </summary>
    public byte[] customData;
    
    /// <summary>
    /// Creates a simple vibration feedback.
    /// </summary>
    /// <param name="intensity">Vibration intensity</param>
    /// <param name="duration">Duration in seconds</param>
    /// <returns>HapticFeedback instance</returns>
    public static HapticFeedback CreateVibration(float intensity, float duration);
    
    /// <summary>
    /// Creates a pulsed haptic feedback.
    /// </summary>
    /// <param name="intensity">Pulse intensity</param>
    /// <param name="duration">Total duration</param>
    /// <param name="frequency">Pulse frequency</param>
    /// <returns>HapticFeedback instance</returns>
    public static HapticFeedback CreatePulse(float intensity, float duration, float frequency);
}
```

### BodyRegion Enumeration

Defines all supported body regions for haptic feedback.

```csharp
public enum BodyRegion
{
    // Head and Neck
    Head = 0,
    Neck = 1,
    Face = 2,
    
    // Torso - Front
    ChestUpper = 10,
    ChestLower = 11,
    Abdomen = 12,
    
    // Torso - Back
    BackUpper = 20,
    BackMiddle = 21,
    BackLower = 22,
    
    // Arms - Left
    LeftShoulder = 30,
    LeftUpperArm = 31,
    LeftElbow = 32,
    LeftForearm = 33,
    LeftWrist = 34,
    LeftHand = 35,
    LeftPalm = 36,
    LeftFingers = 37,
    
    // Arms - Right
    RightShoulder = 40,
    RightUpperArm = 41,
    RightElbow = 42,
    RightForearm = 43,
    RightWrist = 44,
    RightHand = 45,
    RightPalm = 46,
    RightFingers = 47,
    
    // Legs - Left
    LeftHip = 50,
    LeftThigh = 51,
    LeftKnee = 52,
    LeftCalf = 53,
    LeftAnkle = 54,
    LeftFoot = 55,
    
    // Legs - Right
    RightHip = 60,
    RightThigh = 61,
    RightKnee = 62,
    RightCalf = 63,
    RightAnkle = 64,
    RightFoot = 65,
    
    // Special Regions
    Core = 100,
    FullBody = 101,
    Unknown = 999
}
```

---

## Network Components

### NetworkTouchManager Class

Manages synchronization of touch events across the network.

```csharp
public class NetworkTouchManager : NetworkBehaviour
{
    /// <summary>
    /// Sends a touch event to all connected clients.
    /// </summary>
    /// <param name="touchEvent">Touch event to synchronize</param>
    [Rpc(SendTo.All)]
    public void SynchronizeTouchEventRpc(TouchEvent touchEvent);
    
    /// <summary>
    /// Processes a touch event received from a remote client.
    /// </summary>
    /// <param name="touchEvent">Remote touch event</param>
    /// <param name="senderId">Network ID of the sender</param>
    public void ProcessRemoteTouchEvent(TouchEvent touchEvent, ulong senderId);
    
    /// <summary>
    /// Requests haptic feedback synchronization from server.
    /// </summary>
    /// <param name="region">Body region for feedback</param>
    /// <param name="feedback">Haptic feedback parameters</param>
    [Rpc(SendTo.Server)]
    public void RequestHapticFeedbackRpc(BodyRegion region, HapticFeedback feedback);
}
```

### NetworkAvatar Class

Networked avatar component for multiplayer VR interactions.

```csharp
public class NetworkAvatar : NetworkBehaviour
{
    /// <summary>
    /// Network variable for head position.
    /// </summary>
    public NetworkVariable<Vector3> headPosition = new NetworkVariable<Vector3>();
    
    /// <summary>
    /// Network variable for head rotation.
    /// </summary>
    public NetworkVariable<Quaternion> headRotation = new NetworkVariable<Quaternion>();
    
    /// <summary>
    /// Network variable for left hand position.
    /// </summary>
    public NetworkVariable<Vector3> leftHandPosition = new NetworkVariable<Vector3>();
    
    /// <summary>
    /// Network variable for right hand position.
    /// </summary>
    public NetworkVariable<Vector3> rightHandPosition = new NetworkVariable<Vector3>();
    
    /// <summary>
    /// Updates avatar transform data across the network.
    /// </summary>
    /// <param name="headPos">Head position</param>
    /// <param name="headRot">Head rotation</param>
    /// <param name="leftHandPos">Left hand position</param>
    /// <param name="rightHandPos">Right hand position</param>
    [Rpc(SendTo.Server)]
    public void UpdateAvatarTransformRpc(Vector3 headPos, Quaternion headRot, 
                                        Vector3 leftHandPos, Vector3 rightHandPos);
}
```

---

## VR Interaction

### VRInputManager Class

Manages VR input and controller interactions.

```csharp
public class VRInputManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the XR Origin component.
    /// </summary>
    public XROrigin xrOrigin;
    
    /// <summary>
    /// Left hand controller reference.
    /// </summary>
    public XRController leftController;
    
    /// <summary>
    /// Right hand controller reference.
    /// </summary>
    public XRController rightController;
    
    /// <summary>
    /// Gets the current position of the VR headset.
    /// </summary>
    public Vector3 HeadPosition { get; }
    
    /// <summary>
    /// Gets the current rotation of the VR headset.
    /// </summary>
    public Quaternion HeadRotation { get; }
    
    /// <summary>
    /// Gets the current position of the left hand controller.
    /// </summary>
    public Vector3 LeftHandPosition { get; }
    
    /// <summary>
    /// Gets the current position of the right hand controller.
    /// </summary>
    public Vector3 RightHandPosition { get; }
    
    /// <summary>
    /// Event triggered when a controller button is pressed.
    /// </summary>
    public event System.Action<XRController, InputFeatureUsage<bool>> OnButtonPressed;
    
    /// <summary>
    /// Event triggered when controller tracking is lost or regained.
    /// </summary>
    public event System.Action<XRController, bool> OnTrackingStateChanged;
}
```

---

## Configuration

### HapticConfig ScriptableObject

Configuration asset for haptic system settings.

```csharp
[CreateAssetMenu(fileName = "HapticConfig", menuName = "VR System/Haptic Configuration")]
public class HapticConfig : ScriptableObject
{
    [Header("Device Settings")]
    public bool enableBHaptics = true;
    public bool enableTeslasuit = false;
    public bool enableGenericHaptics = true;
    
    [Header("Global Settings")]
    [Range(0f, 1f)] public float globalIntensity = 0.7f;
    [Range(0f, 1f)] public float touchIntensity = 0.5f;
    [Range(0f, 1f)] public float pressureIntensity = 0.8f;
    
    [Header("Safety Settings")]
    [Range(0f, 1f)] public float maxIntensity = 0.9f;
    public float maxDuration = 10f;
    public float coolingPeriod = 1f;
    
    [Header("Region-Specific Settings")]
    public RegionSettings[] regionSettings;
    
    [System.Serializable]
    public struct RegionSettings
    {
        public BodyRegion region;
        [Range(0f, 2f)] public float intensityMultiplier;
        public bool enableHaptics;
        public HapticPattern defaultPattern;
    }
}
```

---

## Utilities

### HapticPatternLibrary Class

Static library of predefined haptic patterns.

```csharp
public static class HapticPatternLibrary
{
    /// <summary>
    /// Creates a gentle touch haptic feedback.
    /// </summary>
    /// <returns>HapticFeedback for gentle touch</returns>
    public static HapticFeedback GentleTouch();
    
    /// <summary>
    /// Creates a firm grip haptic feedback.
    /// </summary>
    /// <returns>HapticFeedback for firm grip</returns>
    public static HapticFeedback FirmGrip();
    
    /// <summary>
    /// Creates a heartbeat pattern haptic feedback.
    /// </summary>
    /// <returns>HapticFeedback for heartbeat pattern</returns>
    public static HapticFeedback Heartbeat();
    
    /// <summary>
    /// Creates a notification haptic feedback.
    /// </summary>
    /// <returns>HapticFeedback for notification</returns>
    public static HapticFeedback Notification();
}
```

### TouchVisualizationManager Class

Provides visual debugging for touch interactions.

```csharp
public class TouchVisualizationManager : MonoBehaviour
{
    /// <summary>
    /// Enable visual debugging for touch events.
    /// </summary>
    public bool enableTouchVisualization = true;
    
    /// <summary>
    /// Material used for touch point visualization.
    /// </summary>
    public Material touchPointMaterial;
    
    /// <summary>
    /// Shows visual feedback for a touch event.
    /// </summary>
    /// <param name="touchEvent">Touch event to visualize</param>
    public void ShowTouchFeedback(TouchEvent touchEvent);
    
    /// <summary>
    /// Hides visual feedback for a touch event.
    /// </summary>
    /// <param name="touchId">ID of touch event to hide</param>
    public void HideTouchFeedback(int touchId);
    
    /// <summary>
    /// Clears all visual touch feedback.
    /// </summary>
    public void ClearAllVisualization();
}
```

---

## Events and Delegates

### System Events

Common event signatures used throughout the system:

```csharp
/// <summary>
/// Delegate for touch detection events.
/// </summary>
/// <param name="touchEvent">The touch event data</param>
public delegate void TouchEventHandler(TouchEvent touchEvent);

/// <summary>
/// Delegate for haptic device status changes.
/// </summary>
/// <param name="device">The haptic device</param>
/// <param name="isConnected">Connection status</param>
public delegate void HapticDeviceStatusHandler(IHapticDevice device, bool isConnected);

/// <summary>
/// Delegate for network events.
/// </summary>
/// <param name="clientId">Network client ID</param>
/// <param name="eventData">Event-specific data</param>
public delegate void NetworkEventHandler(ulong clientId, object eventData);
```

---

## Usage Examples

### Basic Touch Detection Setup

```csharp
// Setup touch detector on avatar hand
var handCollider = avatarHand.GetComponent<Collider>();
var touchDetector = avatarHand.AddComponent<TouchDetector>();
touchDetector.bodyRegion = BodyRegion.LeftHand;
touchDetector.touchSensitivity = 0.5f;

// Subscribe to touch events
touchDetector.OnTouchDetected += (touchEvent) => {
    Debug.Log($"Touch detected on {touchEvent.targetRegion}");
    
    // Trigger haptic feedback
    var feedback = HapticPatternLibrary.GentleTouch();
    HapticManager.Instance.TriggerHapticFeedback(touchEvent.targetRegion, feedback);
};
```

### Custom Haptic Device Integration

```csharp
public class CustomHapticDevice : MonoBehaviour, IHapticDevice
{
    public string DeviceName => "Custom Device";
    public bool IsConnected { get; private set; }
    
    public bool Initialize(HapticConfig config)
    {
        // Custom initialization logic
        IsConnected = ConnectToDevice();
        return IsConnected;
    }
    
    public void SendHapticFeedback(BodyRegion region, HapticFeedback feedback)
    {
        // Custom haptic feedback implementation
        SendCustomFeedback(region, feedback.intensity, feedback.duration);
    }
    
    // Register the device with the haptic manager
    private void Start()
    {
        HapticManager.Instance.RegisterDevice(this);
    }
}
```

---

This API reference provides comprehensive documentation for integrating with and extending the VR Avatar Interaction System. For additional examples and implementation details, refer to the source code and accompanying documentation files.