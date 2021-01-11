# XRTK Testplan: Import XRTK.SDK into a project and configure

This testplan tests the XRTK import path via UPM where the user installs the `XRTK.SDK`
package first.

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

## Test 2 - Import XRTK.SDK

### Instructions

- Open the `Package Manager` window in the editor
- Change the Registry drop down from "Unity Registry" to "My Registries"
- Select the `XRTK.SDK` package
- Select `Install` to import the package into the project
- Wait for the import process to complete

### Expected Result

- The SDK and Core projects show as "Installed" with the latest versions
- The project compiles without errors
- The project compiles without XRTK specific warnings
- A menu item `Mixed Reality Toolkit` is now available at the top menu bar

## Test 3 - Install XRTK.SDK Package Assets

### Instructions

- If it already exists, Delete the "XRTK.Generated\SDK" folder in your project assets
- Select `Mixed Reality Toolkit/Packages/Install SDK Package Assets....` in the menu bar
- Wait for the import process to complete

### Expected Result

- A folder `XRTK.Generated/SDK` was created in the project's `Assets` folder
- The project compiles without errors
- The project compiles without XRTK specific warnings

## 4. Configure XRTK in scene

### Instructions

- Select `Mixed Reality Toolkit/Configure...` in the menu bar

### Expected Result

- A GameObject `MixedRealityToolkit` was added to the scene
- A GameObject `MixedRealityPlayspace` was added to the scene
- Selecting the `MixedRealityToolkit` game object shows the toolkit is using the `MixedRealityToolkitRootProfile`
- Running the project produces no errors

## Test 4 - Add an SDK component to a scene object

### Instructions

- Save the current scene and ensure it is in the Build Settings
- Add a 3D Cube to the scene and reposition to X: 0, Y: 1, Z: 10
- Add the "ManipulationHandler" script to the Cube
- Configure the "Select" action to "MixedRealityInputActionsProfile -> Select"
- Run the project

### Expected Result

- The Input Actions should be available for the "Select" ManipulationHandler Action
- The project should run without error
