using UnityEngine;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Services;

public class SetRenderingMode : MonoBehaviour
{
    public void SetNoneMode()
    {
        var providers = MixedRealityToolkit.GetActiveServices<IMixedRealityHandControllerDataProvider>();
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].RenderingMode = XRTK.Definitions.Controllers.Hands.HandRenderingMode.None;
        }
    }

    public void SetJointsMode()
    {
        var providers = MixedRealityToolkit.GetActiveServices<IMixedRealityHandControllerDataProvider>();
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].RenderingMode = XRTK.Definitions.Controllers.Hands.HandRenderingMode.Joints;
        }
    }

    public void SetMeshMode()
    {
        var providers = MixedRealityToolkit.GetActiveServices<IMixedRealityHandControllerDataProvider>();
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].RenderingMode = XRTK.Definitions.Controllers.Hands.HandRenderingMode.Mesh;
        }
    }
}
