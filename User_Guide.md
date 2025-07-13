# User Guide - VR Avatar Interaction System
## Complete Guide for Testing, Usage, and Extension

### 🎯 Getting Started

Welcome to the comprehensive user guide for the VR Avatar Interaction System. This guide will walk you through every aspect of using, testing, and extending the system, from your first VR session to advanced haptic device integration.

### 📋 Quick Start Checklist

Before diving in, ensure you have completed these prerequisite steps:

- ✅ Unity 2023.2 LTS installed
- ✅ VR headset connected and recognized
- ✅ Project imported and packages loaded
- ✅ Network connection available for multiplayer testing
- ✅ (Optional) Haptic devices connected and drivers installed

### 🎮 First-Time Setup

#### 1. Project Launch

1. **Open Unity Hub** and load the VR Avatar Interaction System project
2. **Wait for package import** - This may take 5-10 minutes on first launch
3. **Check Console** for any import errors or warnings
4. **Open MainScene** located in `Assets/Scenes/MainScene.unity`

#### 2. VR Hardware Configuration

**For Oculus Users:**
1. Ensure Oculus software is running
2. Check that your headset is detected in Oculus app
3. In Unity: Edit → Project Settings → XR Plug-in Management
4. Enable **Oculus** provider
5. Verify **Oculus** appears in XR Providers list

**For OpenXR Compatible Devices:**
1. Ensure your VR runtime is active (SteamVR, Windows Mixed Reality, etc.)
2. In Unity: Edit → Project Settings → XR Plug-in Management
3. Enable **OpenXR** provider
4. Configure interaction profiles in OpenXR settings

#### 3. Initial Test Run

1. **Press Play** in Unity Editor
2. **Put on VR headset** - you should see the virtual environment
3. **Test hand tracking** - move VR controllers to see virtual hands
4. **Try locomotion** - use teleportation or smooth movement
5. **Check audio** - speak into headset microphone to test voice chat

### 🔧 System Configuration

#### Network Settings

Access network configuration through the in-game menu or by modifying the NetworkManager component:

**Basic Network Setup:**
```
Host Mode: Creates a new session that others can join
Client Mode: Joins an existing session
Server IP: Default is localhost (127.0.0.1) for local testing
Port: Default is 7777
Max Players: Set to 2 for optimal performance
```

**Advanced Network Configuration:**
1. Open NetworkManager in the scene hierarchy
2. Modify connection settings:
   - **Connection Timeout**: 30 seconds (default)
   - **Network Tick Rate**: 20Hz for VR optimization
   - **Enable Lag Compensation**: True for smooth movement

#### Touch Detection Settings

Configure touch sensitivity and haptic response:

**Touch Sensitivity:**
- **Low (0.1)**: Requires firm contact for detection
- **Medium (0.5)**: Balanced sensitivity (recommended)
- **High (0.9)**: Detects light touches and near-misses

**Haptic Intensity:**
- **Gentle (0.3)**: Subtle feedback for ambient touches
- **Standard (0.6)**: Normal interaction feedback
- **Strong (0.9)**: Pronounced feedback for important events

#### Audio Configuration

**Spatial Audio Setup:**
1. Check **Audio Listener** is attached to VR camera
2. Verify **Audio Source** on avatar prefab
3. Configure **3D Sound Settings**:
   - Doppler Level: 0.1 (minimal for VR comfort)
   - Spread: 0 (directional audio)
   - Max Distance: 50 (room-scale appropriate)

**Voice Chat Settings:**
- **Input Device**: Select active microphone
- **Noise Suppression**: Enable for cleaner audio
- **Echo Cancellation**: Enable to prevent feedback
- **Voice Activation**: Threshold of -40dB (adjustable)

### 🧪 Testing Procedures

#### Single User Testing

**Basic Functionality Test:**
1. Launch the application in Unity Editor
2. Verify VR tracking is working correctly
3. Test avatar hand movements and positioning
4. Check teleportation and movement systems
5. Test voice input/output (speak and listen for echo)

**Touch Detection Test:**
1. Enable **Debug Mode** in Touch Settings
2. Use VR controllers to touch different body regions
3. Observe visual feedback and console logs
4. Verify touch events are properly detected
5. Check haptic feedback (if devices connected)

#### Multiplayer Testing

**Two-User Local Test:**
1. **First Instance**: Launch in Unity Editor, click "Start Host"
2. **Second Instance**: Build project and run executable, click "Start Client"
3. **Alternative**: Use two separate machines on the same network

**Multiplayer Test Checklist:**
- ✅ Both avatars visible to each other
- ✅ Avatar movements synchronized in real-time
- ✅ Voice chat working bidirectionally
- ✅ Touch interactions detected between avatars
- ✅ Haptic feedback triggered on both users
- ✅ Network performance stable (check debug UI)

#### Performance Testing

**VR Performance Metrics:**
- **Frame Rate**: Maintain 90 FPS for smooth VR experience
- **Motion-to-Photon Latency**: < 20ms for comfort
- **Network Latency**: < 100ms for responsive interactions

**Performance Monitoring:**
1. Enable **Unity Profiler** (Window → Analysis → Profiler)
2. Monitor **CPU Usage** (keep below 80%)
3. Check **GPU Performance** (maintain frame rate)
4. Watch **Network Statistics** in debug UI
5. Monitor **Memory Usage** (watch for leaks)

### 🎛️ Using the Debug Interface

#### Touch Debug Panel

The touch debug interface provides real-time visualization of interaction events:

**Touch Visualization:**
- **Green Spheres**: Active touch points
- **Red Outlines**: Touch detection zones
- **Blue Lines**: Touch force vectors
- **Yellow Indicators**: Haptic feedback triggers

**Debug Information Display:**
```
Active Touches: 2/10
Current Region: LeftHand → RightHand
Touch Pressure: 0.7 (70%)
Haptic Device: bHaptics Connected
Network Latency: 45ms
```

#### Network Debug Panel

Monitor network performance and connection status:

**Connection Status:**
- **Green**: Connected and stable
- **Yellow**: Connected with high latency
- **Red**: Connection issues or disconnected

**Data Transfer Monitor:**
- **Bytes Sent/Received**: Real-time data flow
- **Messages/Second**: Network message frequency
- **Packet Loss**: Connection reliability indicator

### 🔌 Haptic Device Usage

#### bHaptics Setup

**Initial Setup:**
1. **Install bHaptics Player** on your computer
2. **Connect TactSuit** via Bluetooth or USB
3. **Launch bHaptics Player** and verify device connection
4. **In Unity**: Enable bHaptics in Haptic Settings
5. **Test Connection**: Use the haptic test interface

**Calibration Process:**
1. Open **bHaptics Player**
2. Run **Device Calibration** wizard
3. Follow on-screen instructions for positioning
4. **Test Patterns** to verify proper functionality
5. **Save Calibration** settings

**Usage Tips:**
- Ensure TactSuit is properly fitted before use
- Check battery level before long sessions
- Keep bHaptics Player running during VR sessions
- Use moderate intensity for extended use

#### Teslasuit Setup

**Advanced Setup Process:**
1. **Install Teslasuit SDK** and drivers
2. **Connect Teslasuit** and complete pairing process
3. **Run Teslasuit Configurator** for initial setup
4. **Calibrate electrode placement** for optimal feedback
5. **Test stimulation patterns** at safe intensity levels

**Safety Considerations:**
- Start with low intensity settings (< 30%)
- Gradually increase intensity as comfortable
- Take breaks every 30 minutes of use
- Discontinue use if any discomfort occurs
- Follow all manufacturer safety guidelines

### 🛠️ Customization and Extension

#### Adding Custom Touch Zones

**Step 1: Create Touch Collider**
```csharp
1. Select avatar prefab in Project window
2. Add child GameObject at desired location
3. Add Collider component (set as Trigger)
4. Add TouchDetector script
5. Configure BodyRegion in TouchDetector
```

**Step 2: Configure Touch Response**
```csharp
// Example: Adding custom elbow touch zone
public class ElbowTouchDetector : TouchDetector
{
    protected override void ProcessTouchEvent(TouchEvent touchEvent)
    {
        // Custom elbow touch logic
        var customFeedback = new HapticFeedback
        {
            intensity = 0.4f,
            duration = 0.2f,
            pattern = HapticPattern.Pulse
        };
        
        TriggerHapticResponse(BodyRegion.LeftForearm, customFeedback);
    }
}
```

#### Custom Haptic Patterns

**Creating New Patterns:**
1. **Open bHaptics Designer** (for bHaptics devices)
2. **Design haptic pattern** using the visual editor
3. **Export pattern** as .tact file
4. **Import into Unity** in Resources/Haptics folder
5. **Register pattern** in HapticPatternManager

**Example Pattern Registration:**
```csharp
public static class CustomHapticPatterns
{
    public const string CUSTOM_GREETING = "CustomGreeting";
    
    public static void RegisterCustomPatterns()
    {
        HapticManager.RegisterPattern(CUSTOM_GREETING, 
            Resources.Load<TextAsset>("Haptics/CustomGreeting"));
    }
}
```

#### Extending Network Features

**Adding Custom Network Events:**
```csharp
public class CustomNetworkBehaviour : NetworkBehaviour
{
    [Rpc(SendTo.All)]
    public void SendCustomInteractionRpc(CustomInteractionData data)
    {
        ProcessCustomInteraction(data);
    }
    
    private void ProcessCustomInteraction(CustomInteractionData data)
    {
        // Handle custom interaction logic
    }
}
```

### 📊 Troubleshooting Guide

#### Common Issues and Solutions

**VR Tracking Problems:**
- **Issue**: Avatar hands not following controllers
- **Solution**: Check XR Origin configuration and controller bindings
- **Check**: Ensure Input System is properly configured

**Network Connection Issues:**
- **Issue**: Cannot connect to multiplayer session
- **Solution**: Verify firewall settings and network configuration
- **Check**: Ensure both clients are on same network/internet

**Audio Problems:**
- **Issue**: Voice chat not working
- **Solution**: Check microphone permissions and audio device settings
- **Check**: Verify AudioSource components are properly configured

**Haptic Device Issues:**
- **Issue**: Haptic feedback not working
- **Solution**: Verify device connection and driver installation
- **Check**: Test device with manufacturer's software first

#### Performance Optimization

**VR Performance Tuning:**
1. **Reduce Shadow Quality**: Medium or Low for better performance
2. **Optimize Lighting**: Use baked lighting where possible
3. **LOD Settings**: Configure Level of Detail for distant objects
4. **Culling Distance**: Limit rendering distance for complex objects

**Network Optimization:**
1. **Reduce Update Rate**: Lower for non-critical objects
2. **Compression**: Enable for large data transfers
3. **Batching**: Group small network messages
4. **Prediction**: Use client-side prediction for smooth movement

### 🎯 Advanced Usage Scenarios

#### Multi-User VR Meetings

**Setup for Group Sessions:**
1. Configure NetworkManager for more than 2 users
2. Implement voice chat mixing for multiple speakers
3. Add user identification and avatar customization
4. Create shared interaction objects and whiteboards

#### Therapeutic Applications

**Haptic Therapy Setup:**
1. Create gentle, therapeutic haptic patterns
2. Implement session tracking and progress monitoring
3. Add safety controls and emergency stops
4. Design calming virtual environments

#### Training and Education

**Interactive Learning Setup:**
1. Create instructional haptic feedback sequences
2. Implement progress tracking and assessment
3. Add guided interaction tutorials
4. Design educational content and scenarios

### 📈 Best Practices

#### VR Comfort Guidelines

**Motion Sickness Prevention:**
- Use teleportation for primary locomotion
- Provide comfort settings for sensitive users
- Maintain consistent frame rate above 90 FPS
- Implement vignetting during movement

**Haptic Comfort:**
- Start with low intensity and allow user adjustment
- Provide haptic intensity controls in settings
- Implement haptic pause/stop functionality
- Use varied patterns to prevent adaptation

#### Development Best Practices

**Code Organization:**
- Separate concerns between networking, haptics, and VR
- Use interfaces for extensibility
- Implement proper error handling and logging
- Follow Unity coding standards and conventions

**Testing Strategy:**
- Test on multiple VR platforms
- Validate network performance across different connections
- Test haptic devices with various intensity levels
- Perform extended session testing for comfort

### 🔄 System Updates and Maintenance

#### Regular Maintenance Tasks

**Weekly:**
- Check for Unity and package updates
- Verify VR device driver updates
- Test network connectivity and performance
- Review error logs and fix issues

**Monthly:**
- Update haptic device firmware
- Review and optimize performance metrics
- Test compatibility with new VR hardware
- Update documentation for any changes

#### Version Control Best Practices

**Git Configuration:**
- Use Unity-specific .gitignore
- Include Package Manager manifests
- Exclude temporary and cache files
- Tag stable releases for easy rollback

---

This user guide provides comprehensive coverage of the VR Avatar Interaction System. For additional technical details, refer to the Technical Documentation and API Reference files. Remember to prioritize user safety and comfort in all VR and haptic applications.