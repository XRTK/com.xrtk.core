using TMPro;
using UnityEngine;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Services;

public class SetPhysicsMode : MonoBehaviour
{
    private Vector3[] startPositions;
    private Quaternion[] startRotations;

    [SerializeField]
    private TextMeshPro physicsStateText = null;

    [SerializeField]
    private Transform[] objects = null;

    private void Start()
    {
        startPositions = new Vector3[objects.Length];
        startRotations = new Quaternion[objects.Length];

        for (int i = 0; i < objects.Length; i++)
        {
            startPositions[i] = objects[i].position;
            startRotations[i] = objects[i].rotation;
        }
    }

    public void EnablePhysics()
    {
        physicsStateText.text = "Physics: On";

        var providers = MixedRealityToolkit.GetActiveServices<IMixedRealityHandControllerDataProvider>();
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].HandPhysicsEnabled = true;
        }
    }

    public void DisablePhysics()
    {
        physicsStateText.text = "Physics: Off";

        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].position = startPositions[i];
            objects[i].rotation = startRotations[i];
        }

        var providers = MixedRealityToolkit.GetActiveServices<IMixedRealityHandControllerDataProvider>();
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].HandPhysicsEnabled = false;
        }
    }
}
