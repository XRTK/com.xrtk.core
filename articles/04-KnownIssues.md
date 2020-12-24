# Known issues 

> Current known issues up to and including the 0.2 release

Issue | Severity | Description | Mitigation
|---|---|---|---|
| 1 | **Low** | Currently XRTK assets are copied from the individual packages to the Unity project for developers to use. Existing assets may potentially be overwritten, corrupted, or conflict if a copy already exists in the project. We're working on updating the import process to provide developers a clean copy they can modify safely without fear of this. | Developers are advised not to replace assets in the XRTK.Generated folder.  When profiles / assets are cloned, place them outside this folder to prevent overwriting following an upgrade.
| 2 | **Medium** | An issue can occur when importing the XRTK for the first time, or importing additional platforms without an active Mixed Reality Toolkit configuration present.  This leads to some corruption of the configuration of the toolkit and can cause unintended issues. When creating a new project, the SDK / COre should be Installed first and a new scene should be created and configured for the XRTK, only after this is complete should new platforms (and their configuration) be added. | If a configuration corruption should happen, it is best to delete the XRTK.Generated folder and use the Editor menu options `Mixed Reality Toolkit -> Packages` to install the SDK/Core first, then configure your scene and then finally add new platforms.
| 3 | **Low** | The Windows Mixed Reality Boundary configuration is not installed with the Windows Mixed Reality Platform. [Logged under this issue](https://github.com/XRTK/WindowsMixedReality/issues/113) | Simply add the Windows Mixed Reality Boundary system and configuration to the Boundary System configuration profiles.  WIll be addressed in 0.3.
||||