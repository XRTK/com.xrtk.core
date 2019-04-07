using UnityEditor.Experimental.AssetImporters;

namespace XRTK.Utilities.Gltf.Serialization.Importers
{
    [ScriptedImporter(1, "gltf")]
    public class GltfAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext context)
        {
            GltfEditorImporter.OnImportGltfAsset(context);
        }
    }
}