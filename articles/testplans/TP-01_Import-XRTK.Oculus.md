# XRTK Testplan: Import XRTK.Oculus into a project

This testplan tests the XRTK import path via UPM where the user installs the `XRTK.Oculus` package.

## Test Preparations

- Create a new Unity 2019.4 project using the 3D template using the Unity Hub

## Test 1 - Add XRTK Scoped Registry

### Instructions

- Open the created Unity project
- Open the `Package Manager` window in the editor
- Click on the "Advanced" drop down and select "Advanced Project Settings"
- Add a new scoped registry with the following details and click "Save"
  - Name: XRTK
  - URL: http://upm.xrtk.io:4873/
  - Scope(s): com.xrtk
- Close the Package Manager Settings window
- Change the Registry drop down from "Unity Registry" to "My Registries"
- In the `Package Manager` select `Advanced -> Show Preview Packages`
- Wait for the `Package Manager` to finish loading packages

### Expected Result

- The list of available packages in the Unity Package Manager now includes XRTK modules, such as e.g.
  - XRTK.Core
  - XRTK.Oculus
  - XRTK.SDK
  - ...

## Test 2 - Import XRTK.SDK and configure

### Instructions

- Open the `Package Manager` window in the editor
- Change the Registry drop down from "Unity Registry" to "My Registries"
- Select the `XRTK.SDK` package
- Select `Install` to import the package into the project
- Wait for the import process to complete
- Return to the scene and Select `Mixed Reality Toolkit/Configure...` in the menu bar

### Expected Result

- The SDK and Core projects show as "Installed" with the latest versions
- A GameObject `MixedRealityToolkit` was added to the scene
- A GameObject `MixedRealityPlayspace` was added to the scene
- Selecting the `MixedRealityToolkit` game object shows the toolkit is using the `MixedRealityToolkitRootProfile`
- Running the project produces no errors

## Test 3 - Import XRTK.Oculus

### Instructions

- Open the `Package Manager` window in the editor
- Change the Registry drop down from "Unity Registry" to "My Registries"
- Select the `XRTK.Oculus` package
- Select `Install` to import the package into the project
- Wait for the import process to complete
- When prompted to install the "Oculus Platform Service Configurations", select "Later"

### Expected Result

- The project compiles without errors
- The project compiles without XRTK specific warnings
- A menu item `Mixed Reality Toolkit` is now available at the top menu bar
- A folder `XRTK.Generated/Oculus` was NOT created in the project's `Assets` folder

## Test 4 - Install XRTK.Oculus Package Assets

### Instructions

- If it already exists, Delete the "XRTK.Generated\Oculus" folder in your project assets
- Select `Mixed Reality Toolkit/Packages/Install Oculus Package Assets....` in the menu bar
- Wait for the import process to complete
- When prompted to install the "Oculus Platform Service Configurations", select "Yes, Absolutely"
- Select `Mixed Reality Toolkit/Configure...` in the menu bar

### Expected Result

- A folder `XRTK.Generated/Oculus` was created in the project's `Assets` folder
- A GameObject `MixedRealityToolkit` was added to the scene
- A GameObject `MixedRealityPlayspace` was added to the scene
- Selecting the `MixedRealityToolkit` game object shows the toolkit is using the `MixedRealityToolkitRootProfile`
- Opening the 'Camera System Profile' in the `MixedRealityToolkitRootProfile`, there is a 'Oculus Camera Data Provider' registered in the 'IMixedRealityCameraDataProvider' configuration
- Opening the 'Input System Profile' in the `MixedRealityToolkitRootProfile`, there is a 'Oculus Controller Data Provider' registered in the 'IMixedRealityInputDataProvider' configuration
- Opening the 'Input System Profile' in the `MixedRealityToolkitRootProfile`, there is a 'Oculus Hand Controller Data Provider' registered in the 'IMixedRealityInputDataProvider' configuration

## Test 5 - Run Oculus platform via Oculus Link in Windows Standalone (or alternatively, using an Oculus Rift)

### Instructions

- Add a floor to your scene using "Mixed Reality Toolkit -> Tools -> Create Floor" in the editor menu
- Add a Cube to the Scene with the "Manipulation Handler" component added and configured for "Select"
- Enable VR settings for the Oculus platform in the Unity project
- Save the current scene and ensure it is registered in the Unity "Build Settings"
- Connect your Oculus Quest via a cable to your PC and start the Oculus Client - Confirm the headset shows as connected
- Run the Project

### Expected Result

- The project compiles without errors
- The project compiles without XRTK specific warnings
- The project runs without errors
- The view shows above the floor, relative to the headset's current physical location
- When starting each controller, they appear in the scene and move
- Clicking the trigger causes the Pointer lines to appear
- Using the Thumbsticks enables teleportation and the user can move around the floor
- Clicking on the Cube enables the cube to be dragged relative to the controllers position
- Putting controllers down and raising hands replaces the controller models with hands that move relative to the users hands

## Test 6 - Run Oculus platform native on device

### Instructions

- Switch the current runtime platform to "Android" via the Unity "Build Settings" window
- Open the Unity "Project Settings" window on the "Player" tab
- Update the following settings:
  - Other -> "Color space" to "Gamma"
  - Other -> Enable "Auto Graphics API"
  - Other -> Change "Minimum API level" to 25
  - Other -> Set "Scripting Backend" to "IL2CPP"
- Add an Android Manifest to the project using "Mixed Reality Toolkit -> Tools -> Oculus -> Create Oculus Quest Compatible AndroidManifest.xml"
- Connect your Oculus Quest via a cable to your PC
- Use "Build and Run" in the Unity Build Window

### Expected Result

- The project compiles without errors
- The project compiles without XRTK specific warnings
- The project runs without errors
- In the headset, the view shows above the floor, relative to the headset's current physical location
- When starting each controller, they appear in the scene and move
- Clicking the trigger causes the Pointer lines to appear
- Using the Thumbsticks enables teleportation and the user can move around the floor
- Clicking on the Cube enables the cube to be dragged relative to the controllers position
- Putting controllers down and raising hands replaces the controller models with hands that move relative to the users hands
