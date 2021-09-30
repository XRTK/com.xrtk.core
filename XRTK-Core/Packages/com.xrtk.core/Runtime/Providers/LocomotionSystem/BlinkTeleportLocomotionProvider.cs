﻿// Copyright (c) XRTK. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.Rendering;
using XRTK.Extensions;
using XRTK.Definitions.LocomotionSystem;
using XRTK.Interfaces.LocomotionSystem;
using XRTK.Services.LocomotionSystem;
using XRTK.Interfaces.InputSystem;
using XRTK.Definitions.Utilities;

namespace XRTK.Providers.LocomotionSystem
{
    /// <summary>
    /// A <see cref="ITeleportLocomotionProvider"/> implementation that teleports the player rig
    /// to a target location by simulating "blink of an eye". The camera fades out for movement and then
    /// fades back in, this can help to avoid fatigue for some players.
    /// </summary>
    [System.Runtime.InteropServices.Guid("497d2054-a467-4d6d-9d79-bd01aa6b4c22")]
    public class BlinkTeleportLocomotionProvider : BaseTeleportLocomotionProvider
    {
        /// <inheritdoc />
        public BlinkTeleportLocomotionProvider(string name, uint priority, BlinkTeleportLocomotionProviderProfile profile, ILocomotionSystem parentService)
            : base(name, priority, profile, parentService)
        {
            fadeDuration = profile.FadeDuration;
            fadeMaterial = profile.FadeMaterial;
            fadeInColor = profile.FadeInColor;
            fadeOutColor = profile.FadeOutColor;
        }

        private readonly float fadeDuration;
        private readonly Material fadeMaterial;
        private readonly Color fadeInColor;
        private readonly Color fadeOutColor;
        private IMixedRealityInputSource inputSource;
        private MixedRealityPose targetPose;
        private ITeleportAnchor targetAnchor;
        private GameObject fadeSphere;
        private MeshRenderer fadeSphereRenderer;
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
                inputSource = (IMixedRealityInputSource)eventData.EventSource;
                targetPose = eventData.Pose.Value;
                targetAnchor = eventData.Anchor;

                if (eventData.Anchor != null)
                {
                    targetPose.Position = targetAnchor.Position;
                    if (targetAnchor.OverrideTargetOrientation)
                    {
                        targetPose.Rotation = Quaternion.Euler(0f, targetAnchor.TargetOrientation, 0f);
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
            var height = targetPose.Position.y;
            targetPose.Position -= LocomotionTargetTransform.position - LocomotionTargetTransform.position;

            var targetPosition = targetPose.Position;
            targetPosition.y = height;
            targetPose.Position = targetPosition;

            LocomotionTargetTransform.position = targetPose.Position;
            LocomotionTargetTransform.RotateAround(LocomotionTargetTransform.position, Vector3.up, targetPose.Rotation.eulerAngles.y - LocomotionTargetTransform.eulerAngles.y);
            LocomotionSystem.RaiseTeleportCompleted(this, inputSource, targetPose, targetAnchor);
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
