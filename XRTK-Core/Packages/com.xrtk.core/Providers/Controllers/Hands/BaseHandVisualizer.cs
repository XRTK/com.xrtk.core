// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using UnityEngine;
using XRTK.Definitions.HandTracking;
using XRTK.Definitions.Utilities;
using XRTK.EventDatum.Input;
using XRTK.Interfaces.InputSystem.Handlers;
using XRTK.Interfaces.Providers.Controllers;
using XRTK.Services;

namespace XRTK.Providers.Controllers.Hands
{
    public class BaseHandVisualizer : MonoBehaviour, IMixedRealityControllerVisualizer, IMixedRealitySourceStateHandler, IMixedRealityHandJointHandler, IMixedRealityHandMeshHandler
    {
        public virtual Handedness Handedness { get; set; }

        public GameObject GameObjectProxy => gameObject;

        public IMixedRealityController Controller { get; set; }

        protected readonly Dictionary<TrackedHandJoint, Transform> joints = new Dictionary<TrackedHandJoint, Transform>();
        protected MeshFilter handMeshFilter;

        private void OnEnable()
        {
            MixedRealityToolkit.InputSystem?.Register(gameObject);
        }

        private void OnDisable()
        {
            MixedRealityToolkit.InputSystem?.Unregister(gameObject);
        }

        private void OnDestroy()
        {
            foreach (var joint in joints)
            {
                Destroy(joint.Value.gameObject);
            }

            if (handMeshFilter != null)
            {
                Destroy(handMeshFilter.gameObject);
            }
        }

        [Obsolete("Use HandJointUtils.TryGetJointPose instead of this")]
        public bool TryGetJointTransform(TrackedHandJoint joint, out Transform jointTransform)
        {
            if (joints == null)
            {
                jointTransform = null;
                return false;
            }

            if (joints.TryGetValue(joint, out jointTransform))
            {
                return true;
            }

            jointTransform = null;
            return false;
        }

        void IMixedRealitySourceStateHandler.OnSourceDetected(SourceStateEventData eventData) { }

        void IMixedRealitySourceStateHandler.OnSourceLost(SourceStateEventData eventData)
        {
            if (Controller?.InputSource.SourceId == eventData.SourceId)
            {
                Destroy(gameObject);
            }
        }

        void IMixedRealityHandJointHandler.OnJointUpdated(InputEventData<IDictionary<TrackedHandJoint, MixedRealityPose>> eventData)
        {
            if (eventData.Handedness != Controller?.ControllerHandedness)
            {
                return;
            }

            MixedRealityHandTrackingProfile handTrackingProfile = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile;
            if (handTrackingProfile != null && !handTrackingProfile.EnableHandJointVisualization)
            {
                // clear existing joint GameObjects / meshes
                foreach (var joint in joints)
                {
                    Destroy(joint.Value.gameObject);
                }

                joints.Clear();
                return;
            }

            foreach (TrackedHandJoint handJoint in eventData.InputData.Keys)
            {
                Transform jointTransform;
                if (joints.TryGetValue(handJoint, out jointTransform))
                {
                    jointTransform.position = eventData.InputData[handJoint].Position;
                    jointTransform.rotation = eventData.InputData[handJoint].Rotation;
                }
                else
                {
                    GameObject prefab = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile.JointPrefab;
                    if (handJoint == TrackedHandJoint.Palm)
                    {
                        prefab = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile.PalmJointPrefab;
                    }
                    else if (handJoint == TrackedHandJoint.IndexTip)
                    {
                        prefab = MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile.FingerTipPrefab;
                    }

                    GameObject jointObject;
                    if (prefab != null)
                    {
                        jointObject = Instantiate(prefab);
                    }
                    else
                    {
                        jointObject = new GameObject();
                    }

                    jointObject.name = handJoint.ToString() + " Proxy Transform";
                    jointObject.transform.position = eventData.InputData[handJoint].Position;
                    jointObject.transform.rotation = eventData.InputData[handJoint].Rotation;
                    jointObject.transform.parent = transform;

                    joints.Add(handJoint, jointObject.transform);
                }
            }
        }

        public void OnMeshUpdated(InputEventData<HandMeshUpdatedEventData> eventData)
        {
            if (eventData.Handedness != Controller?.ControllerHandedness)
            {
                return;
            }

            if (handMeshFilter == null &&
                MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile?.HandMeshPrefab != null)
            {
                handMeshFilter = Instantiate(MixedRealityToolkit.Instance.ActiveProfile.InputSystemProfile.HandTrackingProfile.HandMeshPrefab).GetComponent<MeshFilter>();
            }

            if (handMeshFilter != null)
            {
                Mesh mesh = handMeshFilter.mesh;

                mesh.vertices = eventData.InputData.Vertices;
                mesh.normals = eventData.InputData.Normals;
                mesh.triangles = eventData.InputData.Triangles;

                if (eventData.InputData.Uvs != null && eventData.InputData.Uvs.Length > 0)
                {
                    mesh.uv = eventData.InputData.Uvs;
                }

                handMeshFilter.transform.position = eventData.InputData.Position;
                handMeshFilter.transform.rotation = eventData.InputData.Rotation;
            }
        }
    }
}