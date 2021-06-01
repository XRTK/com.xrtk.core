// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;

namespace XRTK.Providers.LocomotionSystem
{
    [System.Runtime.InteropServices.Guid("497d2054-a467-4d6d-9d79-bd01aa6b4c22")]
    public class MixedRealityBlinkTeleportLocomotionProvider : BaseLocomotionProvider, IMixedRealityTeleportLocomotionProvider
    {
        /// <inheritdoc />
        public MixedRealityBlinkTeleportLocomotionProvider(string name, uint priority, MixedRealityBlinkTeleportLocomotionProviderProfile profile, IMixedRealityLocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            fadeDuration = profile.FadeDuration;
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
        public override void Initialize()
        {
            base.Initialize();
            InitiailzeFadeSphere();
        }

        /// <inheritdoc />
        public override void Update()
        {
            base.Update();

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
        public override void OnLocomotionStarted(LocomotionEventData eventData)
        {
            if (eventData.used)
            {
                return;
            }

            eventData.Use();

            locomotionEventData = eventData;
            targetRotation = Vector3.zero;
            targetPosition = eventData.Pointer.Result.EndPoint;
            targetRotation.y = eventData.Pointer.PointerOrientation;

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

        /// <inheritdoc />
        public override void OnLocomotionCompleted(LocomotionEventData eventData) => FadeIn();

        /// <inheritdoc />
        public override void OnLocomotionCanceled(LocomotionEventData eventData) => fadeSphere.SetActive(false);

        private void PerformTeleport()
        {
            var height = targetPosition.y;
            targetPosition -= LocomotionTargetTransform.position - LocomotionTargetTransform.position;
            targetPosition.y = height;
            LocomotionTargetTransform.position = targetPosition;
            LocomotionTargetTransform.RotateAround(LocomotionTargetTransform.position, Vector3.up, targetRotation.y - LocomotionTargetTransform.eulerAngles.y);

            LocomotionSystem.RaiseTeleportComplete(locomotionEventData.Pointer, locomotionEventData.HotSpot);
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
                fadeSphere.name = $"{nameof(BlinkTeleportProvider)}_Fade";
                fadeSphere.transform.SetParent(CameraTransform);
                fadeSphere.transform.localPosition = Vector3.zero;
                fadeSphere.transform.localRotation = Quaternion.identity;
                fadeSphere.transform.localScale = new Vector3(.5f, .5f, .5f);
                Destroy(fadeSphere.GetComponent<SphereCollider>());

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

                if (GraphicsSettings.renderPipelineAsset.IsNull())
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
                    Debug.LogError($"{nameof(BlinkTeleportProvider)} does not support render pipelines. The handler won't be able to fade in and out.");
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
