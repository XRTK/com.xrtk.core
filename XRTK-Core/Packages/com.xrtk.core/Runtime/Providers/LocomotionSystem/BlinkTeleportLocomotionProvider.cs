// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Rendering;
using XRTK.Extensions;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;
using XRTK.Utilities;
using XRTK.Interfaces.InputSystem;

namespace XRTK.Providers.LocomotionSystem
{
    [System.Runtime.InteropServices.Guid("497d2054-a467-4d6d-9d79-bd01aa6b4c22")]
    public class BlinkTeleportLocomotionProvider : BaseTeleportLocomotionProvider
    {
        /// <inheritdoc />
        public BlinkTeleportLocomotionProvider(string name, uint priority, MixedRealityBlinkTeleportLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            fadeDuration = profile.FadeDuration;
            InputAction = profile.InputAction;
        }

        private static readonly int sourceBlend = Shader.PropertyToID("_SrcBlend");
        private static readonly int destinationBlend = Shader.PropertyToID("_DstBlend");
        private static readonly int zWrite = Shader.PropertyToID("_ZWrite");

        private readonly float fadeDuration;
        private Vector3 targetPosition;
        private Vector3 targetRotation;
        private LocomotionEventData locomotionEventData;
        private GameObject fadeSphere;
        private MeshRenderer fadeSphereRenderer;
        private Color fadeInColor = Color.clear;
        private Color fadeOutColor = Color.black;
        private bool isFadingOut;
        private bool isFadingIn;
        private float fadeTime;

        /// <inheritdoc />
        public override void Enable()
        {
            InitiailzeFadeSphere();
            base.Enable();
        }

        /// <inheritdoc />
        public override void Update()
        {
            if (isFadingOut)
            {
                fadeTime += Time.deltaTime;
                var t = fadeTime / fadeDuration;
                var frameColor = Color.Lerp(fadeInColor, fadeOutColor, t);
                SetColor(frameColor);

                if (t >= 1f)
                {
                    isFadingOut = false;
                    PerformTeleport();
                }
            }
            else if (isFadingIn)
            {
                fadeTime += Time.deltaTime;
                var t = fadeTime / fadeDuration;
                var frameColor = Color.Lerp(fadeOutColor, fadeInColor, t);
                SetColor(frameColor);

                if (t >= 1f)
                {
                    isFadingIn = false;
                    fadeSphere.SetActive(false);
                }
            }

            base.Update();
        }

        /// <inheritdoc />
        public override void Destroy()
        {
            if (!fadeSphere.IsNull())
            {
                fadeSphere.Destroy();
            }

            base.Destroy();
        }

        /// <inheritdoc />
        public override void OnTeleportStarted(LocomotionEventData eventData)
        {
            // Was this teleport provider's teleport started and did this provider
            // actually expect a teleport to start?
            if (OpenTargetRequests.ContainsKey(eventData.EventSource.SourceId))
            {
                locomotionEventData = eventData;
                targetRotation = Vector3.zero;
                targetPosition = eventData.Pose.Value.Position;
                targetRotation.y = eventData.Pose.Value.Rotation.eulerAngles.y;

                if (eventData.HotSpot != null)
                {
                    targetPosition = eventData.HotSpot.Position;
                    if (eventData.HotSpot.OverrideTargetOrientation)
                    {
                        targetRotation.y = eventData.HotSpot.TargetOrientation;
                    }
                }

                FadeOut();
            }

            base.OnTeleportStarted(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportCompleted(LocomotionEventData eventData)
        {
            if (OpenTargetRequests.ContainsKey(eventData.EventSource.SourceId))
            {
                FadeIn();
            }

            base.OnTeleportCompleted(eventData);
        }

        /// <inheritdoc />
        public override void OnTeleportCanceled(LocomotionEventData eventData)
        {
            if (OpenTargetRequests.ContainsKey(eventData.EventSource.SourceId))
            {
                fadeSphere.SetActive(false);
            }

            base.OnTeleportCanceled(eventData);
        }

        private void PerformTeleport()
        {
            var height = targetPosition.y;
            targetPosition -= LocomotionTargetTransform.position - LocomotionTargetTransform.position;
            targetPosition.y = height;
            LocomotionTargetTransform.position = targetPosition;
            LocomotionTargetTransform.RotateAround(LocomotionTargetTransform.position, Vector3.up, targetRotation.y - LocomotionTargetTransform.eulerAngles.y);
            LocomotionSystem.RaiseTeleportCompleted(this, (IMixedRealityInputSource)locomotionEventData.EventSource, locomotionEventData.Pose.Value, locomotionEventData.HotSpot);
        }

        private void FadeOut()
        {
            fadeSphere.SetActive(true);
            fadeTime = 0f;
            isFadingIn = false;
            isFadingOut = true;
        }

        private void FadeIn()
        {
            fadeSphere.SetActive(true);
            fadeTime = 0f;
            isFadingOut = false;
            isFadingIn = true;
        }

        private void InitiailzeFadeSphere()
        {
            if (fadeSphere.IsNull())
            {
                // We use a simple sphere around the camera / head, which
                // we can fade in/out to simulate the camera fading to black.
                fadeSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                fadeSphere.name = $"{nameof(BlinkTeleportLocomotionProvider)}_Fade";
                fadeSphere.transform.SetParent(CameraTransform);
                fadeSphere.transform.localPosition = Vector3.zero;
                fadeSphere.transform.localRotation = Quaternion.identity;
                fadeSphere.transform.localScale = new Vector3(.5f, .5f, .5f);
                fadeSphere.GetComponent<SphereCollider>().Destroy();

                // Invert the sphere normals to point inwards
                // (towards camera, so we can see the darkness in our life).
                var meshFilter = fadeSphere.GetComponent<MeshFilter>();
                var normals = meshFilter.mesh.normals;
                var triangles = meshFilter.mesh.triangles;
                for (int i = 0; i < normals.Length; i++)
                {
                    normals[i] = -normals[i];
                }

                for (int i = 0; i < triangles.Length; i += 3)
                {
                    var t = triangles[i];
                    triangles[i] = triangles[i + 2];
                    triangles[i + 2] = t;
                }

                meshFilter.mesh.normals = normals;
                meshFilter.mesh.triangles = triangles;

                // Configure the mesh renderer to not impact anything else
                // in the scene.
                fadeSphereRenderer = fadeSphere.GetComponent<MeshRenderer>();
                fadeSphereRenderer.shadowCastingMode = ShadowCastingMode.Off;
                fadeSphereRenderer.receiveShadows = false;
                fadeSphereRenderer.allowOcclusionWhenDynamic = false;
                fadeSphereRenderer.lightProbeUsage = LightProbeUsage.Off;
                fadeSphereRenderer.reflectionProbeUsage = ReflectionProbeUsage.Off;

                // Finally paint the sphere with a transparency enabled material.
                // We use the default material created on the sphere to clone its properties.
                var fadeMaterial = new Material(fadeSphereRenderer.material)
                {
                    color = fadeInColor
                };

                if (RenderPipelineUtilities.GetActiveRenderingPipeline() == Definitions.Utilities.RenderPipeline.Legacy)
                {
                    // Unity standard shader can be assumed since we created a primitive.
                    fadeMaterial.SetInt(sourceBlend, (int)BlendMode.One);
                    fadeMaterial.SetInt(destinationBlend, (int)BlendMode.OneMinusSrcAlpha);
                    fadeMaterial.SetInt(zWrite, 0);
                    fadeMaterial.DisableKeyword("_ALPHATEST_ON");
                    fadeMaterial.DisableKeyword("_ALPHABLEND_ON");
                    fadeMaterial.EnableKeyword("_ALPHAPREMULTIPLY_ON");
                    fadeMaterial.renderQueue = 3000;
                }
                else
                {
                    Debug.LogError($"{nameof(BlinkTeleportLocomotionProvider)} does not support render pipelines. The provider won't be able to fade in and out.");
                }

                fadeSphereRenderer.material = fadeMaterial;
            }

            // Initially hide the sphere, we only want it to be active when
            // fading.
            fadeSphere.SetActive(false);
        }

        private void SetColor(Color color)
        {
            var material = fadeSphereRenderer.material;
            material.color = color;
            fadeSphereRenderer.material = material;
        }
    }
}
