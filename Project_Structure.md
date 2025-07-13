# Project Structure Documentation
## VR Avatar Interaction System - Complete File Organization

### 📁 Overview

This document provides a comprehensive overview of the VR Avatar Interaction System project structure, explaining the organization of files, folders, and components within the Unity project. Understanding this structure is essential for development, maintenance, and extension of the system.

### 🗂️ Root Directory Structure

```
VR_Avatar_Interaction_System/
├── Assets/                          # Unity assets and source files
├── Documentation/                   # Project documentation
├── Packages/                        # Unity Package Manager dependencies
├── ProjectSettings/                 # Unity project configuration
├── UserSettings/                    # User-specific Unity settings (gitignored)
├── Library/                         # Unity cache files (gitignored)
├── Logs/                           # Unity log files (gitignored)
├── Temp/                           # Temporary build files (gitignored)
├── README.md                       # Main project documentation
└── .gitignore                      # Git ignore rules
```

---

## 📂 Assets Directory Structure

### Main Assets Organization

```
Assets/
├── Scenes/                         # Unity scene files
├── Scripts/                        # C# source code
├── Prefabs/                        # Unity prefab assets
├── Materials/                      # Material assets
├── Textures/                       # Texture and image assets
├── Audio/                          # Audio clips and settings
├── Animations/                     # Animation clips and controllers
├── Resources/                      # Runtime-loadable assets
├── StreamingAssets/               # Raw files for runtime access
├── Third-Party/                   # External SDK integrations
├── Editor/                        # Unity Editor extensions
├── Plugins/                       # Native plugins and libraries
└── Settings/                      # Configuration asset files
```

### 🎬 Scenes Directory

```
Assets/Scenes/
├── MainScene.unity                # Primary VR interaction scene
├── MenuScene.unity                # Main menu and configuration
├── TestScene.unity                # Development and testing scene
├── CalibrationScene.unity         # VR and haptic device calibration
├── DemoScene.unity                # Demonstration and tutorial scene
└── NetworkTestScene.unity         # Network testing environment
```

**Scene Descriptions:**

- **MainScene.unity**: Core VR interaction environment with full networking and haptic support
- **MenuScene.unity**: Application startup, settings, and device configuration
- **TestScene.unity**: Isolated testing environment for individual components
- **CalibrationScene.unity**: VR tracking and haptic device calibration utilities
- **DemoScene.unity**: Showcase scene demonstrating all system features
- **NetworkTestScene.unity**: Dedicated environment for network performance testing

### 💻 Scripts Directory

```
Assets/Scripts/
├── Avatar/                        # Avatar control and animation
│   ├── AvatarController.cs
│   ├── AvatarAnimationManager.cs
│   ├── HandTrackingController.cs
│   ├── BodyPartController.cs
│   └── AvatarNetworkSync.cs
├── Network/                       # Networking and multiplayer
│   ├── NetworkManager.cs
│   ├── NetworkTouchManager.cs
│   ├── NetworkAvatar.cs
│   ├── VoiceChatManager.cs
│   ├── NetworkConfig.cs
│   └── ConnectionUI.cs
├── Touch/                         # Touch detection system
│   ├── TouchDetector.cs
│   ├── TouchEvent.cs
│   ├── TouchEventManager.cs
│   ├── TouchVisualizationManager.cs
│   ├── BodyRegionMapper.cs
│   └── TouchConfig.cs
├── Haptics/                       # Haptic integration framework
│   ├── Core/
│   │   ├── IHapticDevice.cs
│   │   ├── HapticManager.cs
│   │   ├── HapticFeedback.cs
│   │   ├── BodyRegion.cs
│   │   └── HapticConfig.cs
│   ├── Devices/
│   │   ├── BHapticsDevice.cs
│   │   ├── TeslasuitDevice.cs
│   │   ├── GenericHapticDevice.cs
│   │   └── CustomHapticDevice.cs
│   ├── Patterns/
│   │   ├── HapticPatternLibrary.cs
│   │   ├── BHapticsPatterns.cs
│   │   └── CustomPatterns.cs
│   └── Safety/
│       ├── HapticSafetyManager.cs
│       └── SafetyValidator.cs
├── VR/                           # VR-specific components
│   ├── VRInputManager.cs
│   ├── VRLocomotion.cs
│   ├── VRInteractionManager.cs
│   ├── HandPresence.cs
│   ├── TeleportationSystem.cs
│   └── VRComfortSettings.cs
├── UI/                           # User interface components
│   ├── MainMenuUI.cs
│   ├── SettingsUI.cs
│   ├── NetworkUI.cs
│   ├── HapticDebugUI.cs
│   ├── TouchDebugUI.cs
│   └── PerformanceUI.cs
├── Audio/                        # Audio system components
│   ├── SpatialAudioManager.cs
│   ├── VoiceChatController.cs
│   ├── AudioZoneManager.cs
│   └── AudioConfig.cs
├── Utils/                        # Utility and helper classes
│   ├── Singleton.cs
│   ├── ExtensionMethods.cs
│   ├── MathUtils.cs
│   ├── DebugLogger.cs
│   └── PerformanceProfiler.cs
├── Data/                         # Data structures and models
│   ├── UserProfile.cs
│   ├── SessionData.cs
│   ├── TouchAnalytics.cs
│   └── HapticResponseData.cs
└── Managers/                     # System managers
    ├── GameManager.cs
    ├── SceneTransitionManager.cs
    ├── InputManager.cs
    └── DeviceManager.cs
```

### 🎭 Prefabs Directory

```
Assets/Prefabs/
├── Player/                       # Player-related prefabs
│   ├── VR_Player.prefab         # Complete VR player setup
│   ├── NetworkAvatar.prefab     # Networked avatar representation
│   ├── LocalAvatar.prefab       # Local player avatar
│   └── AvatarComponents/        # Individual avatar parts
│       ├── Head.prefab
│       ├── LeftHand.prefab
│       ├── RightHand.prefab
│       ├── Torso.prefab
│       └── Legs.prefab
├── UI/                          # User interface prefabs
│   ├── MainMenu.prefab
│   ├── SettingsPanel.prefab
│   ├── NetworkPanel.prefab
│   ├── HapticDebugPanel.prefab
│   ├── TouchVisualization.prefab
│   └── PerformanceHUD.prefab
├── Environment/                 # Environment and scene objects
│   ├── VR_Room.prefab
│   ├── InteractionZone.prefab
│   ├── TeleportArea.prefab
│   └── SafetyBounds.prefab
├── Effects/                     # Visual and audio effects
│   ├── TouchEffect.prefab
│   ├── HapticVisualizer.prefab
│   ├── ParticleEffects/
│   └── AudioEffects/
└── Debug/                       # Debug and development prefabs
    ├── TouchDebugSphere.prefab
    ├── NetworkDebugCube.prefab
    └── PerformanceProfiler.prefab
```

### 🎨 Materials Directory

```
Assets/Materials/
├── Avatar/                      # Avatar-specific materials
│   ├── Skin_Material.mat
│   ├── Clothing_Material.mat
│   ├── Hair_Material.mat
│   └── Eyes_Material.mat
├── Environment/                 # Environment materials
│   ├── Floor_Material.mat
│   ├── Wall_Material.mat
│   ├── Ceiling_Material.mat
│   └── Furniture_Materials/
├── UI/                         # UI-specific materials
│   ├── Button_Material.mat
│   ├── Panel_Material.mat
│   └── Text_Material.mat
├── Effects/                    # Effect materials
│   ├── Touch_Highlight.mat
│   ├── Haptic_Glow.mat
│   ├── Particle_Materials/
│   └── Shader_Materials/
└── Debug/                      # Debug visualization materials
    ├── Touch_Debug.mat
    ├── Collider_Wireframe.mat
    └── Network_Debug.mat
```

### 🖼️ Textures Directory

```
Assets/Textures/
├── Avatar/                     # Avatar textures
│   ├── Skin/
│   │   ├── SkinDiffuse.png
│   │   ├── SkinNormal.png
│   │   └── SkinRoughness.png
│   ├── Clothing/
│   └── Accessories/
├── Environment/                # Environment textures
│   ├── Walls/
│   ├── Floors/
│   ├── Furniture/
│   └── Lighting/
├── UI/                        # User interface textures
│   ├── Icons/
│   ├── Buttons/
│   ├── Backgrounds/
│   └── Logos/
└── Effects/                   # Effect textures
    ├── Particles/
    ├── Gradients/
    └── Noise/
```

### 🔊 Audio Directory

```
Assets/Audio/
├── Voice/                     # Voice chat and communication
│   ├── VoiceFilters/
│   └── SpatialSettings/
├── SFX/                       # Sound effects
│   ├── TouchSounds/
│   ├── HapticAudio/
│   ├── UIFeedback/
│   └── EnvironmentSounds/
├── Music/                     # Background music
│   ├── Ambient/
│   └── Menu/
├── Haptic/                    # Audio-haptic coordination
│   ├── AudioPatterns/
│   └── SyncSettings/
└── Spatial/                   # 3D audio configuration
    ├── ReverbZones/
    └── AudioMixers/
```

### 📦 Resources Directory

```
Assets/Resources/
├── Haptics/                   # Runtime-loadable haptic patterns
│   ├── BHaptics/
│   │   ├── GentleTouch.tact
│   │   ├── FirmGrip.tact
│   │   ├── Heartbeat.tact
│   │   └── Notification.tact
│   ├── Teslasuit/
│   │   ├── Patterns/
│   │   └── Configurations/
│   └── Generic/
│       └── Patterns/
├── Configurations/            # Runtime configuration files
│   ├── DefaultHapticConfig.json
│   ├── NetworkSettings.json
│   └── TouchSensitivity.json
├── Localization/             # Multi-language support
│   ├── English.json
│   ├── Spanish.json
│   └── French.json
└── Debug/                    # Debug resources
    ├── TestPatterns/
    └── DiagnosticData/
```

### 🔌 Third-Party Directory

```
Assets/Third-Party/
├── bHaptics/                 # bHaptics SDK integration
│   ├── SDK/
│   │   ├── Scripts/
│   │   ├── Prefabs/
│   │   └── Resources/
│   ├── Patterns/
│   └── Examples/
├── Teslasuit/               # Teslasuit SDK integration
│   ├── SDK/
│   ├── API/
│   ├── Configurations/
│   └── Examples/
├── OpenXR/                  # OpenXR extensions
│   ├── Extensions/
│   └── Configurations/
├── Photon/                  # Alternative networking (optional)
│   ├── PUN2/
│   └── Fusion/
└── Other/                   # Additional third-party assets
    ├── ParticleEffects/
    └── AudioProcessing/
```

### 🛠️ Editor Directory

```
Assets/Editor/
├── HapticDeviceEditor.cs    # Custom editor for haptic devices
├── TouchDetectorEditor.cs   # Custom editor for touch detectors
├── NetworkConfigEditor.cs   # Custom editor for network settings
├── BuildSettings/           # Custom build configurations
├── Tools/                   # Editor tools and utilities
│   ├── HapticPatternImporter.cs
│   ├── TouchZoneValidator.cs
│   └── NetworkTestTool.cs
└── Windows/                 # Custom editor windows
    ├── HapticDebugWindow.cs
    ├── TouchAnalyticsWindow.cs
    └── PerformanceWindow.cs
```

### ⚙️ Settings Directory

```
Assets/Settings/
├── HapticConfigurations/    # Haptic device configurations
│   ├── DefaultHapticConfig.asset
│   ├── BHapticsConfig.asset
│   ├── TeslasuitConfig.asset
│   └── CustomDeviceConfig.asset
├── NetworkSettings/         # Network configuration assets
│   ├── LocalNetworkConfig.asset
│   ├── InternetConfig.asset
│   └── TestNetworkConfig.asset
├── TouchSettings/          # Touch detection configurations
│   ├── HighSensitivity.asset
│   ├── MediumSensitivity.asset
│   └── LowSensitivity.asset
├── VRSettings/             # VR-specific configurations
│   ├── OculusSettings.asset
│   ├── OpenXRSettings.asset
│   └── SimulatorSettings.asset
└── AudioSettings/          # Audio configuration assets
    ├── SpatialAudioConfig.asset
    ├── VoiceChatConfig.asset
    └── EffectsConfig.asset
```

---

## 📋 Package Dependencies

### Package Manifest (Packages/manifest.json)

```json
{
  "dependencies": {
    "com.unity.xr.interaction.toolkit": "2.5.2",
    "com.unity.netcode.gameobjects": "1.7.1",
    "com.unity.xr.openxr": "1.9.1",
    "com.unity.xr.management": "4.4.0",
    "com.unity.inputsystem": "1.7.0",
    "com.unity.render-pipelines.universal": "14.0.9",
    "com.unity.textmeshpro": "3.0.6",
    "com.unity.audio.dspgraph": "0.1.0-preview.10",
    "com.unity.multiplayer.tools": "1.1.0"
  }
}
```

### Third-Party Packages

**bHaptics Integration:**
```
Packages/com.bhaptics.sdk/
├── Runtime/
├── Editor/
└── Documentation/
```

**Teslasuit Integration:**
```
Packages/com.teslasuit.sdk/
├── Runtime/
├── Native/
└── Documentation/
```

---

## 🔧 Project Settings

### Key Project Settings Files

```
ProjectSettings/
├── ProjectSettings.asset    # Core Unity project settings
├── InputManager.asset      # Input system configuration
├── NetworkManager.asset    # Network settings
├── XRSettings.asset        # XR/VR configuration
├── AudioManager.asset      # Audio system settings
├── QualitySettings.asset   # Graphics quality settings
├── TimeManager.asset       # Time and physics settings
└── TagManager.asset        # Tags and layers configuration
```

### Custom Project Settings

**XR Configuration:**
- OpenXR Provider enabled
- Oculus Provider (optional)
- Hand Tracking enabled
- Eye Tracking (if supported)

**Physics Settings:**
- Fixed Timestep: 0.02 (50 Hz)
- Touch detection layers configured
- Collision matrix optimized for VR

**Audio Settings:**
- Spatial Audio enabled
- Voice chat optimized
- Low-latency audio processing

---

## 📊 Build Structure

### Build Output Organization

```
Builds/
├── Development/            # Development builds
│   ├── Windows/
│   ├── Android/
│   └── macOS/
├── Release/               # Production releases
│   ├── v1.0.0/
│   ├── v1.1.0/
│   └── v1.2.0/
└── Test/                  # Test builds
    ├── Network/
    ├── Haptic/
    └── Performance/
```

### Platform-Specific Builds

**Windows VR Build:**
- Target: Windows 10/11
- Graphics API: DirectX 11/12
- VR Support: OpenXR, Oculus
- Architecture: x64

**Android VR Build (Quest):**
- Target: Android 10+
- Graphics API: OpenGL ES 3.0
- VR Support: Oculus Android
- Architecture: ARM64

---

## 🔍 File Naming Conventions

### General Naming Rules

- **Scripts**: PascalCase (e.g., `HapticManager.cs`)
- **Prefabs**: PascalCase with type suffix (e.g., `NetworkAvatar.prefab`)
- **Materials**: PascalCase with type suffix (e.g., `Skin_Material.mat`)
- **Textures**: PascalCase with resolution (e.g., `SkinDiffuse_2048.png`)
- **Audio**: lowercase with underscores (e.g., `touch_feedback.wav`)

### Component-Specific Conventions

**Haptic Files:**
- bHaptics patterns: `PatternName.tact`
- Teslasuit configs: `ConfigName_teslasuit.json`
- Generic patterns: `PatternName_generic.json`

**Network Files:**
- Network prefabs: `Network_` prefix
- RPC methods: `MethodNameRpc` suffix
- Network variables: `networkVariableName` camelCase

**Touch Detection:**
- Touch detectors: `BodyPart_TouchDetector.cs`
- Touch events: descriptive names (e.g., `HandToHandTouchEvent`)

---

## 📚 Documentation Structure

### Documentation Directory Organization

```
Documentation/
├── README.md                    # Main project documentation
├── Technical_Documentation.md   # Technical architecture guide
├── User_Guide.md               # User manual and tutorials
├── API_Reference.md            # Complete API documentation
├── Project_Structure.md        # This file - project organization
├── Setup_Guides/               # Installation and setup guides
│   ├── Unity_Setup.md
│   ├── VR_Setup.md
│   ├── Network_Setup.md
│   └── Haptic_Setup.md
├── Integration_Guides/         # Third-party integration guides
│   ├── bHaptics_Integration.md
│   ├── Teslasuit_Integration.md
│   └── Custom_Device_Integration.md
├── Troubleshooting/           # Problem-solving guides
│   ├── Common_Issues.md
│   ├── Performance_Optimization.md
│   └── Debug_Procedures.md
└── Development/               # Development documentation
    ├── Coding_Standards.md
    ├── Architecture_Decisions.md
    └── Future_Roadmap.md
```

---

This project structure documentation provides a complete overview of how the VR Avatar Interaction System is organized. Following this structure ensures maintainability, scalability, and ease of development for the entire project.