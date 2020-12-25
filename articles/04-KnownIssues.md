# Known issues

> Current known issues up to and including the 0.2 release

Issue | Severity | Description | Mitigation
|---|---|---|---|
|  | **Low** | Currently XRTK assets are copied from the individual packages to the Unity project for developers to use. Existing assets may potentially be overwritten, corrupted, or conflict if a copy already exists in the project. We're working on updating the import process to provide developers a clean copy they can modify safely without fear of this. | Developers are advised to make backups of their XRTK.Generated folder and assets before upgrading.
| [WMR #113](https://github.com/XRTK/WindowsMixedReality/issues/113) | **Low** | The Windows Mixed Reality Boundary configuration is not installed with the Windows Mixed Reality Platform. | Simply add the Windows Mixed Reality Boundary system and configuration to the Boundary System configuration profiles.  WIll be addressed in 0.3.
||||
