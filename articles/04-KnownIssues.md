# Known issues 

> Current known issues up to and including the 0.2 release

Issue | Severity | Description | Mitigation
|---|---|---|---|
| 1 | **Low** | Currently XRTK assets are copied from the individual packages to the Unity project for developers to use. Existing assets may potentially be overwritten, corrupted, or conflict if a copy already exists in the project. We're working on updating the import process to provide developers a clean copy they can modify safely without fear of this. | Developers are advised not to replace assets in the XRTK.Generated folder.  When profiles / assets are cloned, place them outside this folder to prevent overwriting following an upgrade.
||||