using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter8
{
    public class DistortionMarkRenderPass : ScriptableRenderPass
    {
        public static readonly Color CLEAR = new Color(0, 0, 0, 0);
        private const string Tag = "DistortionMarkRenderPass";
        public static RenderTargetHandle distortionMarkRenderTextureHandle;
        private int distortionTexShaderPropertyId;

        private List<ShaderTagId> distortionMarkShaderTagIdList;
        private FilteringSettings distorionMarkFilteringSettings;
        private ProfilingSampler profilingSampler;

        public DistortionMarkRenderPass()
        {
            profilingSampler = new ProfilingSampler(Tag);

            distortionMarkShaderTagIdList = new List<ShaderTagId>();
            distortionMarkShaderTagIdList.Add(new ShaderTagId("DistortionMarker"));

            distortionTexShaderPropertyId = Shader.PropertyToID("_DistortionTex");

            renderPassEvent = RenderPassEvent.BeforeRenderingTransparents;
            distorionMarkFilteringSettings = new FilteringSettings(
                RenderQueueRange.transparent,
                1 << LayerMask.NameToLayer("Marker"));
                //1 << DistortionMarkRenderPassFeature.DistortionMarkRenderingMaskLayer);
            distortionMarkRenderTextureHandle.Init(Tag);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(distortionMarkRenderTextureHandle.id, cameraTextureDescriptor);

            ConfigureTarget(distortionMarkRenderTextureHandle.Identifier());
            ConfigureClear(ClearFlag.Color, CLEAR);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(Tag);
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var cam = renderingData.cameraData.camera;
                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(distortionMarkShaderTagIdList, ref renderingData, sortFlags);
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref distorionMarkFilteringSettings);
                cmd.SetGlobalTexture(distortionTexShaderPropertyId, distortionMarkRenderTextureHandle.Identifier());
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            base.FrameCleanup(cmd);
            cmd.ReleaseTemporaryRT(distortionMarkRenderTextureHandle.id);
        }
    }
}