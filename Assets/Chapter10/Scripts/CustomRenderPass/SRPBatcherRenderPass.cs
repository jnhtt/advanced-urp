using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter10
{
    public class SRPBatcherRenderPass : ScriptableRenderPass
    {
        private const string Tag = "SRPBatcherRenderPass";
        private RenderTargetIdentifier currentTarget;
        private List<ShaderTagId> passList = new List<ShaderTagId>() { new ShaderTagId("Pass1"), new ShaderTagId("Pass2") };
        private ShaderTagId pass1 = new ShaderTagId("Pass1");
        private ShaderTagId pass2 = new ShaderTagId("Pass2");
        private FilteringSettings filteringSettings;
        private ProfilingSampler profilingSampler;

        public SRPBatcherRenderPass()
        {
            profilingSampler = new ProfilingSampler(Tag);
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        }

        public void SetRenderTarget(RenderTargetIdentifier target)
        {
            currentTarget = target;
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
                var drawSettings = CreateDrawingSettings(pass1, ref renderingData, sortFlags);

#if false
                RenderSeparately(context, ref renderingData, sortFlags);
#else
                RenderList(context, ref renderingData, sortFlags);
#endif
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        private void RenderSeparately(ScriptableRenderContext context, ref RenderingData renderingData, SortingCriteria sortFlags)
        {
            var drawSettings = CreateDrawingSettings(pass1, ref renderingData, sortFlags);
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);

            drawSettings = CreateDrawingSettings(pass2, ref renderingData, sortFlags);
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
        }

        private void RenderList(ScriptableRenderContext context, ref RenderingData renderingData, SortingCriteria sortFlags)
        {
            var drawSettings = CreateDrawingSettings(passList, ref renderingData, sortFlags);
            context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
        }
    }
}