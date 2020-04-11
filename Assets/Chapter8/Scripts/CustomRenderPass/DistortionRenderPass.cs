using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter8
{
    public class DistortionRenderPass : ScriptableRenderPass
    {
        private const string Tag = "DistortionRenderPass";

        private Material distortionMaterial;
        private RenderTargetIdentifier currentTarget;

        private int distortionRendererShaderPropertyId;
        private int distortionPowerShaderPropertyId;
        private int distortionSpeedShaderPropertyId;

        private DistortionRenderPassFeature.Data data;

        private ProfilingSampler profilingSampler;

        public DistortionRenderPass()
        {
            profilingSampler = new ProfilingSampler(Tag);

            distortionMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DistortionTexture");
            renderPassEvent = RenderPassEvent.AfterRenderingTransparents;

            distortionRendererShaderPropertyId = Shader.PropertyToID("_DistortionRenderer");
            distortionPowerShaderPropertyId = Shader.PropertyToID("_DistortionPower");
            distortionSpeedShaderPropertyId = Shader.PropertyToID("_DistortionSpeed");
        }

        public void SetData(DistortionRenderPassFeature.Data data)
        {
            this.data = data;
        }

        public void SetRenderTarget(RenderTargetIdentifier target)
        {
            currentTarget = target;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get(Tag);
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
                distortionMaterial.SetFloat(distortionPowerShaderPropertyId, data.speed);
                distortionMaterial.SetFloat(distortionSpeedShaderPropertyId, data.power);
                var cameraData = renderingData.cameraData;
                var w = cameraData.camera.scaledPixelWidth;
                var h = cameraData.camera.scaledPixelHeight;
                cmd.GetTemporaryRT(distortionRendererShaderPropertyId, w, h, 0, FilterMode.Bilinear, RenderTextureFormat.Default);
                cmd.Blit(currentTarget, distortionRendererShaderPropertyId);
                cmd.Blit(distortionRendererShaderPropertyId, currentTarget, distortionMaterial);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}