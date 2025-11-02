# Wave Magic Survivor - Setup Guide

## Overview
This is a survivor-style game where you fight waves of enemies using wave-based attacks. The game features wave interference mechanics, upgrades, and increasing difficulty.

## Quick Setup Steps

### 1. Scene Setup
1. Open `Assets/Scenes/SampleScene.unity`
2. Delete any default objects you don't need
3. Set up the following GameObjects:

### 2. Create Player
1. Create an empty GameObject, name it "Player"
2. Add tag "Player" (you may need to create this tag first)
3. Add components:
   - `SpriteRenderer` (create a simple square sprite or use a circle)
   - `Rigidbody2D` (set Gravity Scale to 0, Drag to 10)
   - `CircleCollider2D` (set as Trigger)
   - `PlayerController` script
   - `PlayerInputHandler` script
4. Set the sprite color to something visible (e.g., blue)
5. Scale to appropriate size (around 0.5-1.0)

### 3. Create GameManager
1. Create empty GameObject, name it "GameManager"
2. Add `GameManager` script
3. Configure settings in inspector:
   - Wave Start Delay: 2
   - Time Between Waves: 5
   - Enemies Per Wave Base: 5
   - Enemy Increase Per Wave: 1.5
   - Game Area Radius: 10

### 4. Create Enemy Spawner
1. Create empty GameObject, name it "EnemySpawner"
2. Add `EnemySpawner` script
3. The script will create a default enemy prefab if none is assigned

### 5. Create Enemy Prefab (Optional but Recommended)
1. Create GameObject, name it "Enemy"
2. Add tag "Enemy"
3. Add components:
   - `SpriteRenderer` (red colored circle/square)
   - `Rigidbody2D` (Gravity Scale 0)
   - `CircleCollider2D` (set as Trigger, radius ~0.4)
   - `Enemy` script
4. Drag to `Assets` folder to create prefab
5. Assign this prefab to EnemySpawner's Enemy Prefab field

### 6. Create Wave Attack Manager
1. Create empty GameObject, name it "WaveAttackManager"
2. Add `WaveAttackManager` script
3. Create a Wave Attack Prefab:
   - Create GameObject, name it "WaveAttack"
   - Add tag "WaveAttack" (optional)
   - Add components:
     - `SpriteRenderer` (cyan colored)
     - `CircleCollider2D` (set as Trigger, radius ~0.5)
     - `WaveAttack` script
   - Drag to Assets as prefab
   - Assign to WaveAttackManager's Wave Attack Prefab field

### 7. Setup UI
1. Create Canvas (UI -> Canvas)
2. Create empty GameObject under Canvas, name it "UI Manager"
3. Add `UIManager` script
4. Create UI elements:

#### Health Bar:
- Create UI -> Slider, name "HealthSlider"
- Assign to UIManager's Health Slider field
- Create TextMeshPro text below it for health numbers

#### Wave Info:
- Create TextMeshPro text, name "WaveText"
- Assign to UIManager's Wave Text field
- Create another TextMeshPro for "EnemiesRemainingText"

#### Game Over Panel:
- Create Panel, name "GameOverPanel"
- Add TextMeshPro text inside it for "FinalWaveText"
- Assign panel and text to UIManager

#### Upgrade Panel:
1. **Create the Upgrade Panel:**
   - Right-click in Hierarchy (under Canvas) → UI → Panel
   - Name it "UpgradePanel"
   - Initially disable it (uncheck the GameObject in Inspector, or set `SetActive(false)` in code)

2. **Create the Button Prefab (Step-by-Step):**
   
   **Option A: Create from scratch**
   - In Hierarchy, right-click → UI → Button - TextMeshPro
   - Name it "UpgradeButton"
   - Select the button and configure it:
     - Set size: Width 400, Height 120 (or your preferred size)
     - Adjust colors if desired (Background color, highlight, etc.)
   
   **Option B: Use Unity's default button and modify**
   - In Hierarchy, right-click → UI → Button
   - Name it "UpgradeButton"
   - Delete the default "Text" child (we'll add TextMeshPro instead)
   
   **Both options continue here:**
   - The Button GameObject should have:
     - `RectTransform` component
     - `Canvas Renderer` component
     - `Image` component (this is the button background)
     - `Button` component
   
3. **Add Title Text (First TextMeshPro):**
   - Right-click on the "UpgradeButton" GameObject → UI → TextMeshPro - Text (UI)
   - Name it "TitleText"
   - In RectTransform:
     - Anchor: Top Center
     - Position: X = 0, Y = 20 (or adjust based on your button size)
     - Width: ~380, Height: 40
   - In TextMeshPro component:
     - Text: "Upgrade Title" (placeholder)
     - Font Size: 24-28
     - Font Style: **Bold**
     - Alignment: Center
     - Color: White or your preference
   
4. **Add Description Text (Second TextMeshPro):**
   - Right-click on the "UpgradeButton" GameObject → UI → TextMeshPro - Text (UI)
   - Name it "DescriptionText"
   - In RectTransform:
     - Anchor: Bottom Center
     - Position: X = 0, Y = -20 (or adjust based on your button size)
     - Width: ~380, Height: 40
   - In TextMeshPro component:
     - Text: "Upgrade Description" (placeholder)
     - Font Size: 16-18
     - Font Style: Normal (not bold)
     - Alignment: Center
     - Color: Light gray or your preference

5. **Finalize Button Structure:**
   Your UpgradeButton hierarchy should look like:
   ```
   UpgradeButton
   ├── TitleText (TextMeshPro)
   └── DescriptionText (TextMeshPro)
   ```

6. **Create the Prefab:**
   - Drag the "UpgradeButton" from Hierarchy into your `Assets` folder (or a subfolder like `Assets/Prefabs/UI/`)
   - This creates the prefab
   - You can now delete the button from the scene (the prefab is saved)

7. **Set up Upgrade Panel Structure:**
   - Select the "UpgradePanel"
   - Add the `UpgradeSystem` script component
   - Create an empty GameObject as a child of UpgradePanel, name it "ButtonContainer"
   - Add a `Vertical Layout Group` component to ButtonContainer:
     - Child Alignment: Upper Center
     - Spacing: 10
     - Padding: Top/Bottom/Left/Right: 10
   - Add a `Content Size Fitter` component (optional, for auto-sizing)

8. **Assign References in UpgradeSystem:**
   - Select "UpgradePanel" (with UpgradeSystem script)
   - In Inspector, find UpgradeSystem component:
     - **Upgrade Panel**: Drag the "UpgradePanel" GameObject here
     - **Upgrade Button Parent**: Drag the "ButtonContainer" GameObject here
     - **Upgrade Button Prefab**: Drag the "UpgradeButton" prefab from Assets here
     - **Upgrade Title Text** (if visible): Leave empty or assign title text component

**Visual Layout Tips:**
- Make buttons wide enough for readable text (400-500px width recommended)
- Use spacing between buttons (10-20px)
- Consider using a semi-transparent background panel to darken the game
- Position the UpgradePanel in the center of the screen
- You can add an icon/image to the button if desired (add Image child with icon sprite)

### 8. Input System Setup
1. Make sure Input System package is installed (should already be)
2. In PlayerController GameObject, you may need to add Input Action component
3. The game uses the new Input System - map WASD/Arrow keys to movement
4. You can use the existing `InputSystem_Actions.inputactions` or create new bindings

### 9. Camera Setup
1. Set Camera orthographic size to show the game area (around 5-10)
2. Position camera at (0, 0, -10)

### 10. Layers (Optional but Recommended)
Create layers:
- Player
- Enemy
- WaveAttack
- Set collision matrix appropriately

## Testing
1. Press Play
2. Use WASD or Arrow Keys to move
3. Waves should automatically fire from player
4. Enemies should spawn and move toward player
5. Defeat enemies to progress waves

## Customization Ideas

### Visual Enhancements:
- Replace default sprites with actual art
- Add particle effects for wave attacks
- Add enemy death animations
- Add screen shake on hits
- Add trail renderers to waves

### Gameplay Enhancements:
- Add more wave types (fire waves, ice waves, etc.)
- Implement wave interference visual effects
- Add power-ups that drop from enemies
- Create boss enemies that spawn every 5-10 waves
- Add different enemy types with unique behaviors

### Audio:
- Add background music (synthwave theme!)
- Add sound effects for:
  - Wave attacks
  - Enemy hits
  - Enemy deaths
  - Player damage
  - Wave completion
  - Upgrade selection

## Code Structure
- `Core/` - GameManager, core systems
- `Player/` - PlayerController, input handling
- `Enemies/` - Enemy AI, EnemySpawner
- `Attacks/` - WaveAttack, WaveAttackManager
- `UI/` - UIManager, UpgradeSystem
- `Utils/` - Helper classes, tags

## Troubleshooting

**Enemies not spawning:**
- Check that GameManager has EnemySpawner reference
- Verify Enemy tag exists
- Check console for errors

**Waves not firing:**
- Verify WaveAttackManager has WaveAttack prefab assigned
- Check that Player tag exists
- Ensure GameManager game is active

**Player not moving:**
- Check Input System bindings
- Verify PlayerInputHandler is on player GameObject
- Ensure Rigidbody2D is not kinematic

**UI not updating:**
- Verify UI references are assigned in UIManager
- Check that events are being subscribed properly
- Ensure Canvas is set up correctly

Good luck with your game jam! 🌊

