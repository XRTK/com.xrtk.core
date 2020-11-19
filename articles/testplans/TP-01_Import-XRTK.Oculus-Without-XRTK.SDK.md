# XRTK Testplan: Import XRTK.Oculus into a project without XRTK.SDK

This testplan tests the XRTK import path via UPM where the user installs the `XRTK.Oculus`
package without having the `XRTK.SDK` package installed.

## Test Preparations

- Create a new Unity 2019.1.14f1 project using the 3D template using the Unity Hub

## Test Steps

### 1. Edit project manifest.json

#### Instructions

- Open the created Unity project
- Open the `manifest.json` file found in the project's `Packages` folder in a text editor and add ```
"scopedRegistries": [
    {
      "name": "XRTK",
      "url": "http://upm.xrtk.io:4873/",
      "scopes": [
        "com.xrtk"
      ]
    }
  ],```
  at the top, right before `"dependencies"`.

 - Return to the Unity Editor
 - Open the `Package Manager` window
 - In the `Package Manager` select `Advanced -> Show Preview Packages`
 - In the `Package Manager` select `All Packages` at the displayed packages filter dropdown
 - Wait for the `Package Manager` to finish loading packages

#### Expected Result

- The list of available packages now includes XRTK modules, such as e.g.
  - XRTK.Core
  - XRTK.Oculus
  - XRTK.SDK
  - ...

### 2. Import XRTK.Oculus

#### Instructions

- Open the `Package Manager` window in the editor
- Select the `XRTK.Oculus` package
- Select `Install` to import the package into the project
- Wait for the import process to complete

#### Expected Result

- The project compiles without errors
- The project compiles without XRTK specific warnings
- A menu item `Mixed Reality Toolkit` is now available at the top menu bar