using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter4
{
    public class FinalImageGrabRenderPassFeature : ScriptableRendererFeature
    {
        public static bool snapshot;
        private static Action onFinishSnapshot;
        public static RenderTexture finalImageRenderTexture;

        private FinalImageGrabRenderPass currentPass;

        public static void BeginSnapshot(Action onFinish)
        {
            onFinishSnapshot = onFinish;
            snapshot = true;
        }

        public static void EndSnapshot()
        {
            snapshot = false;
            if (onFinishSnapshot != null)
            {
                onFinishSnapshot();
            }
        }


        public override void Create()
        {
            if (currentPass == null)
            {
                finalImageRenderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.Default);
                finalImageRenderTexture.name = "FinalImage";

                currentPass = new FinalImageGrabRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}