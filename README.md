# VR Avatar Interaction System
## Unity-based Virtual Reality Multi-User Platform with Haptic Integration

### 📋 Project Overview

This Unity project provides a complete virtual reality system enabling real-time avatar interactions between two remote users. The system features networked avatar movement, voice chat, simulated physical touch through collider zones, and an extendable architecture for haptic feedback integration with devices like bHaptics TactSuit and Teslasuit.

### 🎯 Key Features

- **Real-time Networked Avatar Movement**: Synchronized VR avatar movements across multiple users
- **Voice Chat Integration**: Spatial audio communication between users
- **Touch Simulation System**: Collider-based touch detection on avatar body parts (hands, arms, torso, etc.)
- **Haptic Integration Framework**: Extendable code structure with integration hooks for bHaptics and Teslasuit
- **Debug UI System**: Real-time monitoring and testing interface
- **Cross-Platform VR Support**: Compatible with Oculus, OpenXR, and other VR platforms

### 🛠️ Technical Requirements

#### Unity Version
- **Unity 2023.2 LTS** (Latest LTS recommended)
- **Unity XR Interaction Toolkit** 2.5.2+
- **Unity Netcode for GameObjects** 1.7.1+

#### VR Hardware Support
- **Oculus Rift/Quest** (via Oculus Integration)
- **OpenXR Compatible Devices** (HTC Vive, Varjo, etc.)
- **Desktop Simulator** (for testing without VR hardware)

#### Network Requirements
- **Unity Netcode for GameObjects** (Primary networking solution)
- **Alternative**: Photon PUN2 or Mirror Networking (optional configurations included)
- **Internet Connection** for remote user testing

#### Haptic Device Support (Optional)
- **bHaptics TactSuit** integration ready
- **Teslasuit** integration hooks prepared
- **Generic haptic device** framework for custom integrations

### 📦 Installation & Setup

#### Step 1: Unity Project Setup

1. **Download and Extract**
   ```bash
   # Extract the VR_Avatar_Interaction_System.zip to your desired location
   unzip VR_Avatar_Interaction_System.zip
   cd VR_Avatar_Interaction_System
   ```

2. **Open in Unity Hub**
   - Launch Unity Hub
   - Click "Add project from disk"
   - Navigate to the extracted project folder
   - Select the folder and click "Add Project"
   - Open the project (Unity will import all packages automatically)

#### Step 2: Package Dependencies

The project automatically includes these packages via Package Manager:

```json
{
  "com.unity.xr.interaction.toolkit": "2.5.2",
  "com.unity.netcode.gameobjects": "1.7.1",
  "com.unity.xr.openxr": "1.9.1",
  "com.unity.xr.management": "4.4.0",
  "com.unity.inputsystem": "1.7.0"
}
```

**Manual Package Installation (if needed):**
1. Open Unity Package Manager (Window → Package Manager)
2. Install each package listed above if not already present
3. Enable XR Plug-in Management in Project Settings

#### Step 3: XR Configuration

1. **Configure XR Settings**
   - Go to Edit → Project Settings → XR Plug-in Management
   - Enable **OpenXR** for your target platform
   - Configure **Oculus** plugin if using Oculus devices

2. **Input System Setup**
   - Ensure **Input System Package** is active
   - Set Input Handling to "Both" in Player Settings if prompted

3. **XR Interaction Toolkit Setup**
   - The project includes pre-configured XR Origin and Interaction Manager
   - VR controller mappings are automatically configured

#### Step 4: Network Configuration

1. **Unity Netcode Setup**
   - The NetworkManager is pre-configured in the Main scene
   - Default transport uses Unity Transport (UTP)
   - Connection settings can be modified in NetworkManager component

2. **Testing Network Connection**
   - Build and run the project on two different machines, OR
   - Run one instance in Unity Editor and one as a build
   - Use the in-game Network UI to host/join sessions

### 🚀 Quick Start Guide

#### For VR Testing

1. **Connect VR Headset**
   - Ensure your VR device is properly connected and recognized
   - Launch SteamVR or Oculus software if required

2. **Launch the Application**
   - Open the `MainScene` in Unity
   - Press Play in Unity Editor, OR
   - Build and run the standalone application

3. **Network Connection**
   - First user: Click "Start Host" to create a session
   - Second user: Click "Start Client" to join the session
   - Both users should see each other's avatars in VR space

#### For Desktop Testing (No VR)

1. **Enable Simulator Mode**
   - In the XR Origin GameObject, enable the "Device Simulator"
   - Use keyboard/mouse controls to simulate VR movement
   - WASD for movement, mouse for head rotation

2. **Network Testing**
   - Follow the same network connection steps as VR testing
   - Use the Debug UI panel to monitor touch events and haptic triggers

### 🎮 Controls & Interaction

#### VR Controls
- **Movement**: Physical room-scale movement + teleportation
- **Hand Tracking**: VR controller hand presence
- **Touch Interaction**: Physical contact between avatar colliders
- **Voice Chat**: Automatic spatial audio transmission

#### Desktop Simulator Controls
- **WASD**: Move avatar
- **Mouse**: Look around
- **Space**: Jump/Teleport
- **Tab**: Toggle Debug UI

### 🔧 Configuration Options

#### Network Settings
Located in `Assets/Scripts/Network/NetworkConfig.cs`:

```csharp
public class NetworkConfig : MonoBehaviour
{
    public string serverIP = "127.0.0.1";
    public ushort serverPort = 7777;
    public int maxConnections = 2;
    public bool autoStartHost = false;
}
```

#### Touch Sensitivity Settings
Located in `Assets/Scripts/Touch/TouchConfig.cs`:

```csharp
public class TouchConfig : ScriptableObject
{
    public float touchSensitivity = 0.1f;
    public float hapticIntensity = 0.5f;
    public bool enableDebugMode = true;
}
```

#### Haptic Device Settings
Located in `Assets/Scripts/Haptics/HapticConfig.cs`:

```csharp
public class HapticConfig : ScriptableObject
{
    public bool enableBHaptics = false;
    public bool enableTeslasuit = false;
    public string bHapticsDeviceID = "TactSuit_X40";
    public string teslasuitDeviceID = "Default";
}
```

### 🐛 Troubleshooting

#### Common Issues

1. **VR Not Detected**
   - Ensure VR runtime is running (SteamVR/Oculus)
   - Check XR Plug-in Management settings
   - Verify device drivers are up to date

2. **Network Connection Failed**
   - Check firewall settings (Unity needs network access)
   - Verify IP address and port configuration
   - Ensure both clients are on the same network

3. **Touch Events Not Triggering**
   - Check collider settings on avatar prefabs
   - Verify TouchDetector components are attached
   - Enable Debug Mode to see collision visualization

4. **Audio Issues**
   - Check microphone permissions
   - Verify audio input device in Unity Audio settings
   - Test spatial audio settings in AudioSource components

#### Debug Tools

1. **Touch Debug Visualizer**
   - Enable in Touch Settings to see collider interactions
   - Real-time touch event logging in console

2. **Network Debug Panel**
   - Shows connection status and data transfer
   - Client/server synchronization monitoring

3. **Haptic Test Interface**
   - Simulates haptic events without physical devices
   - Integration testing for device communication

### 📁 Project Structure

```
VR_Avatar_Interaction_System/
├── Assets/
│   ├── Scenes/
│   │   ├── MainScene.unity          # Primary VR interaction scene
│   │   ├── MenuScene.unity          # Main menu and settings
│   │   └── TestScene.unity          # Development testing scene
│   ├── Scripts/
│   │   ├── Avatar/                  # Avatar control and animation
│   │   ├── Network/                 # Networking and multiplayer
│   │   ├── Touch/                   # Touch detection system
│   │   ├── Haptics/                 # Haptic integration framework
│   │   ├── UI/                      # User interface and menus
│   │   └── Utils/                   # Utility and helper scripts
│   ├── Prefabs/
│   │   ├── NetworkAvatar.prefab     # Networked VR avatar
│   │   ├── VR_Player.prefab         # Local VR player setup
│   │   └── UI_Elements/             # UI prefab components
│   ├── Materials/                   # Avatar and environment materials
│   ├── Textures/                    # Texture assets
│   ├── Audio/                       # Sound effects and spatial audio
│   └── Third-Party/                 # External SDK integrations
│       ├── bHaptics/                # bHaptics SDK integration
│       └── Teslasuit/               # Teslasuit SDK integration
├── Documentation/                   # Additional documentation files
├── ProjectSettings/                 # Unity project configuration
└── Packages/                        # Package dependencies
```

### 🔗 Integration Guides

#### bHaptics Integration
See `Documentation/bHaptics_Integration_Guide.md` for detailed setup instructions.

#### Teslasuit Integration
See `Documentation/Teslasuit_Integration_Guide.md` for detailed setup instructions.

#### Custom Haptic Devices
See `Documentation/Custom_Haptic_Integration.md` for extending the haptic framework.

### 🤝 Contributing & Extension

#### Adding New Touch Zones
1. Create new collider on avatar mesh
2. Attach `TouchDetector` component
3. Configure body region mapping in `BodyRegionMapper`
4. Update haptic response mapping

#### Network Protocol Extension
1. Extend `NetworkBehaviour` classes for new features
2. Add new RPC methods for data synchronization
3. Update client/server state management

#### Haptic Device Integration
1. Implement `IHapticDevice` interface
2. Add device-specific communication protocols
3. Register device in `HapticManager`

### 📄 License & Credits

This project is provided as an educational and development resource. Please review individual component licenses:

- **Unity XR Interaction Toolkit**: Unity Technologies License
- **Unity Netcode for GameObjects**: Unity Technologies License
- **bHaptics SDK**: bHaptics License Agreement
- **Teslasuit SDK**: Teslasuit License Agreement

### 📞 Support & Resources

#### Documentation
- `Documentation/Technical_Documentation.md` - Detailed technical specifications
- `Documentation/API_Reference.md` - Complete API documentation
- `Documentation/User_Guide.md` - Comprehensive user guide

#### Community & Support
- GitHub Issues: Report bugs and feature requests
- Unity Forums: Community discussions and support
- Developer Documentation: Extended technical resources

### 🔄 Version History

- **v1.0.0** - Initial release with core VR interaction features
- **v1.1.0** - Added haptic integration framework
- **v1.2.0** - Enhanced networking and touch detection
- **v1.3.0** - Cross-platform VR support and UI improvements

---

**Ready to build the future of VR social interaction!** 🚀

For detailed technical information, please refer to the documentation files in the `/Documentation` folder.