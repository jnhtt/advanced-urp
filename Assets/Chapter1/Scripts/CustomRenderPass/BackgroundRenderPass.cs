using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter1
{
    public class BackgroundRenderPass : ScriptableRenderPass
    {
        private const string Tag = "BackgroundRenderPass";
        private RenderTargetIdentifier currentTarget;
        private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
        private FilteringSettings filteringSettings;
        private ProfilingSampler profilingSampler;

        public BackgroundRenderPass()
        {
            profilingSampler = new ProfilingSampler(Tag);
            renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;
            shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, 1 << LayerMask.NameToLayer("Background"));
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
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortFlags);

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}