#if UNITY_2020_1_OR_NEWER
using UnityEditor.AssetImporters;
#else
using UnityEditor.Experimental.AssetImporters;
#endif

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