using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter7
{
    public class OutlineRenderPass : ScriptableRenderPass
    {
        private const string Tag = "OutlineRenderPass";

        private Material outlineMaterial;
        private RenderTargetIdentifier currentTarget;
        private int temporaryRT;

        private ProfilingSampler profilingSampler;

        public OutlineRenderPass()
        {
            profilingSampler = new ProfilingSampler(Tag);

            outlineMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-OutlineTexture");
            temporaryRT = Shader.PropertyToID("temporaryRT");
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        }

        public void SetRenderTarget(RenderTargetIdentifier target)
        {
            currentTarget = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            ref CameraData cameraData = ref renderingData.cameraData;
            var w = cameraData.camera.scaledPixelWidth;
            var h = cameraData.camera.scaledPixelHeight;

            CommandBuffer cmd = CommandBufferPool.Get(Tag);
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                cmd.GetTemporaryRT(temporaryRT, w, h, 0, FilterMode.Point, RenderTextureFormat.Default);
                Blit(cmd, currentTarget, temporaryRT, outlineMaterial, 0);
                Blit(cmd, temporaryRT, currentTarget);

                cmd.ReleaseTemporaryRT(temporaryRT);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}