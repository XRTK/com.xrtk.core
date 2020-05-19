using System.Collections.Generic;
using TMPro;
using UnityEngine;
using XRTK.Definitions.Controllers.Hands;
using XRTK.Interfaces.Providers.Controllers.Hands;
using XRTK.Services;

public class SetPhysicsMode : MonoBehaviour
{
    private Vector3[] startPositions;
    private Quaternion[] startRotations;
    private List<IMixedRealityHandControllerDataProvider> providers;
    private bool physicsEnabled;
    private HandBoundsMode boundsMode;

    [SerializeField]
    private TextMeshPro physicsStateText = null;

    [SerializeField]
    private Transform[] objects = null;

    private void Start()
    {
        startPositions = new Vector3[objects.Length];
        startRotations = new Quaternion[objects.Length];
        providers = MixedRealityToolkit.GetActiveServices<IMixedRealityHandControllerDataProvider>();
        physicsEnabled = providers[0].HandPhysicsEnabled;
        boundsMode = providers[0].BoundsMode;

        for (int i = 0; i < objects.Length; i++)
        {
            startPositions[i] = objects[i].position;
            startRotations[i] = objects[i].rotation;
        }
    }

    public void EnablePhysics()
    {
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].HandPhysicsEnabled = true;    
        }

        physicsEnabled = true;
        UpdateStateText();
    }

    public void DisablePhysics()
    {
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].position = startPositions[i];
            objects[i].rotation = startRotations[i];
        }

        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].HandPhysicsEnabled = false;
        }

        physicsEnabled = false;
        UpdateStateText();
    }

    public void SetHandBoundsMode()
    {
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].BoundsMode = HandBoundsMode.Hand;
        }

        boundsMode = HandBoundsMode.Hand;
        UpdateStateText();
    }

    public void SetFingerBoundsMode()
    {
        for (int i = 0; i < providers.Count; i++)
        {
            providers[i].BoundsMode = HandBoundsMode.Fingers;
        }

        boundsMode = HandBoundsMode.Fingers;
        UpdateStateText();
    }

    private void UpdateStateText()
    {
        if (physicsEnabled)
        {
            physicsStateText.text = $"Physics: On / {boundsMode}";
        }
        else
        {
            physicsStateText.text = $"Physics: Off / {boundsMode}";
        }
    }
}
