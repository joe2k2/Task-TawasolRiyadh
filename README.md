# Gameplay Scene - Complete Documentation

**Scene Path:** `/Assets/Scenes/Gameplay.unity`  
**Architecture:** Backward-scrolling tiles with stationary player  
**Platform:** PC & Android (Mobile)

---

## 🎮 Scene Overview

The Gameplay scene implements the classic endless runner architecture where:
- Player stays in a fixed position
- Tiles move backward toward the player
- Player jumps to avoid obstacles and collect items
- Speed increases progressively over time
- Ghost player shows previous best run

---


---

## 🎯 Core Components

### **1. GameManager**

**Path:** `/GameManager`  
**Script:** `/Assets/Scripts/Managers/GameManager.cs`

**Responsibilities:**
- Central game state management
- Score tracking
- Game over detection
- Scene flow control

**Key Features:**
- Singleton pattern for global access
- Event system for state changes
- Persistent across game sessions

---

### **2. Player Controller**

**Path:** `/Player - Parent/Player`  
**Script:** `/Assets/Scripts/Player/PlayerController.cs`

**Components:**
- `PlayerController` - Jump logic and input
- `Rigidbody` - Physics simulation
- `BoxCollider` - Collision detection
- `MeshRenderer` - Visual representation

**Features:**
- Jump mechanic with physics
- Ground detection
- State broadcasting for ghost replay
- Dual input support (Keyboard + Mobile button)

**Controls:**
- **PC:** Space bar to jump
- **Mobile:** Tap Jump Button

**Inspector Settings:**
- **Jump Force:** `7` (adjustable for jump height)
- **Gravity Multiplier:** `1` (affect fall speed)
- **Jump Button:** Reference to UI button

**Physics Configuration:**
- Rigidbody Constraints: Freeze Rotation + Freeze Position Z
- Interpolation: Interpolate (smooth movement)
- Collision Detection: Continuous (prevents tunneling)

---

### **3. SpawnManager**

**Path:** `/SpawnManager`  
**Script:** `/Assets/Scripts/Managers/SpawnManager.cs`

**Purpose:** Spawns and manages ground tiles

**How It Works:**
1. Creates object pools for each tile prefab
2. Spawns initial tiles at game start
3. Monitors tile positions
4. Spawns new tiles when needed
5. Recycles tiles behind player

**Difficulty Progression:**
- **Beginner:** Easy tile patterns
- **Intermediate:** Medium obstacles (speed threshold)
- **Advanced:** Hard challenges (higher speed threshold)

**Pool System:**
- Uses `PoolManager` for efficient tile recycling
- No runtime instantiation after initialization
- Memory-efficient for mobile

---

### **4. PoolManager**

**Path:** `/PoolManager`  
**Script:** `/Assets/Scripts/Managers/PoolManager.cs`

**Purpose:** Object pooling for performance optimization

**Benefits:**
- Reduces garbage collection
- Prevents frame rate drops
- Efficient memory usage

**Pooled Objects:**
- Ground tiles
- Obstacles
- Collectibles

---

### **5. GameSpeedManager**

**Path:** `/GameSpeedManager`  
**Script:** `/Assets/Scripts/Managers/GameSpeedManager.cs`

**Purpose:** Progressive difficulty through speed increase

**Features:**
- Gradual speed acceleration over time
- Configurable speed curve
- Speed-based difficulty thresholds
- Global speed access for all moving objects

---

### **6. UIManager**

**Path:** `/UIManager`  
**Script:** `/Assets/Scripts/Managers/UIManager.cs`

**Purpose:** Manages all UI elements

**Managed UI:**
- Score display
- Distance counter
- Game over screen
- Restart/Main Menu buttons
- Mobile controls

---

### **7. SyncManager**

**Path:** `/SyncManager`  
**Script:** Custom sync script

**Purpose:** Ghost player replay synchronization

**Features:**
- Records player movement
- Plays back previous best run
- Visual comparison with ghost

---

### **8. Ghost Player System**

**Path:** `/GhostPlayer - Parent/GhostPlayer`

**Purpose:** Displays previous best performance

**Features:**
- Semi-transparent visual
- Follows recorded path
- Helps player improve
- Separate camera view option

---

## 🎨 UI System

### **HUD (Panel - Info)**

**Elements:**
- Score counter
- Distance traveled
- Real-time updates during gameplay

---

### **Game Over Screen (Dark - End Game)**

**Components:**
- Dark overlay background
- Game Over title
- Final score display
- Action buttons:
  - **Restart Button:** Reload Gameplay scene
  - **Main Menu Button:** Return to MainMenu scene

**Activation:**
- Hidden during gameplay
- Shows when `GameManager.IsGameOver = true`
- Pauses game time

---

### **Mobile Controls (Button - Jump)**

**Path:** `/Canvas/Button - Jump`

**Components:**
- `Image` - Button visual
- `Button` - Click detection
- `CanvasGroup` - Opacity/interaction control

**Configuration:**
1. Connected to `PlayerController.OnJumpButtonPressed()`
2. Only visible/active on mobile builds
3. Positioned for thumb access (bottom-right recommended)

**Setup:**
- Assign in `PlayerController` Inspector
- OnClick event linked via code
- Properly cleaned up on destroy

---

## 🎯 Platform-Specific Features

### **PC Build:**
- Keyboard input (Space)
- Jump button hidden or disabled
- Higher resolution graphics

### **Android Build:**
- Touch input via Jump button
- Optimized performance settings
- Mobile-appropriate UI scaling
- Device safe area support

---

## 🔧 Scene Setup Checklist

### **Initial Setup:**

1. **GameManager:**
   - ✅ Exists in scene
   - ✅ Singleton instance configured

2. **Player:**
   - ✅ Tag set to "Player"
   - ✅ `PlayerController` component attached
   - ✅ Jump Button assigned in Inspector
   - ✅ Rigidbody configured (not kinematic)
   - ✅ Ground collision detection working

3. **SpawnManager:**
   - ✅ Tile prefabs assigned
   - ✅ Difficulty lists populated
   - ✅ Pool capacity set (recommended: 5-10)

4. **PoolManager:**
   - ✅ Singleton instance active

5. **UI:**
   - ✅ Canvas set to Screen Space - Overlay
   - ✅ Jump button positioned correctly
   - ✅ Event System present
   - ✅ Game Over screen initially hidden

6. **Cameras:**
   - ✅ Player camera active
   - ✅ Ghost camera configured

---

## 🎮 Gameplay Flow

---

## 🐛 Debugging

### **Enable Debug Features:**

**Player:**
- Gizmos show ground detection (green = grounded, red = airborne)

**Spawn Manager:**
- Enable logging to see tile spawn events
- Check pool status in console

**Common Issues & Fixes:**

**Player not jumping:**
- ✅ Check `isGrounded` is true
- ✅ Verify Jump Button assigned
- ✅ Check GameManager.IsGameOver is false
- ✅ Ensure Button onClick is connected

**Tiles not spawning:**
- ✅ Check SpawnManager has prefabs assigned
- ✅ Verify PoolManager exists
- ✅ Check prefabs have GroundTile component

**Button not working on mobile:**
- ✅ EventSystem must be in scene
- ✅ Button must have onClick listener
- ✅ PlayerController must reference button
- ✅ Check Canvas raycast target settings

**Ghost not showing:**
- ✅ Verify SyncManager active
- ✅ Check previous run was recorded
- ✅ Ghost camera configured

---

## 📱 Mobile Optimization

### **Performance Tips:**

1. **Object Pooling:**
   - All dynamic objects use pools
   - No runtime instantiation

2. **UI:**
   - Minimal overdraw
   - Efficient canvas structure
   - Button size optimized for touch

3. **Physics:**
   - Continuous collision detection only on player
   - Static colliders for tiles
   - Minimal rigidbody count

4. **Graphics:**
   - URP optimized
   - Mobile-appropriate shaders
   - Texture compression

---

## 🎨 Tags & Layers

### **Required Tags:**
- `Player` → Player GameObject
- `Ground` → Ground tiles
- `Ghost` → Ghost player
- `Collectible` → Collectible items
- `Obstacle` → Obstacles

### **Required Layers:**
- `Ground` → For collision filtering
- `UI` → For UI elements

---

## 📝 Scene-Specific Notes

### **Key Differences from Forward-Moving Variant:**

**Gameplay Scene (Current):**
- Uses `SpawnManager` and `GroundTile`
- Player stationary, tiles move
- Script: `PlayerController.cs`
- Architecture: Classic endless runner

---

## 🔗 Related Assets

**Scripts:**
- `/Assets/Scripts/Player/PlayerController.cs`
- `/Assets/Scripts/Managers/GameManager.cs`
- `/Assets/Scripts/Managers/SpawnManager.cs`
- `/Assets/Scripts/Managers/PoolManager.cs`
- `/Assets/Scripts/Managers/GameSpeedManager.cs`
- `/Assets/Scripts/Managers/UIManager.cs`
- `/Assets/Scripts/Environment/GroundTile.cs`

**Prefabs:**
- Ground tile variants (Easy, Medium, Hard)
- Obstacles
- Collectibles

**Input:**
- `/Assets/PlayerInputActions.inputactions`

---

## 🚀 Build Settings

### **For Android:**

1. **Player Settings:**
   - Minimum API Level: Android 7.0 (API 24)
   - Target API Level: Latest
   - Scripting Backend: IL2CPP
   - ARM64 architecture

2. **Graphics:**
   - Auto Graphics API
   - URP Asset configured

3. **Input:**
   - Active Input Handling: Both (Input Manager + Input System)

4. **Optimization:**
   - Strip Engine Code: Enabled
   - Managed Stripping Level: Medium
   - Vertex Compression: Everything

---

**Last Updated:** [Add date when you create this]
**Unity Version:** 6000.0
**Scene Version:** 1.0


