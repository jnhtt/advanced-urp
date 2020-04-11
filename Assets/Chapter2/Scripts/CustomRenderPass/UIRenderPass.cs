using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter2
{
    public class UIRenderPass : ScriptableRenderPass
    {
        private const string Tag = "UIRenderPass";
        private RenderTargetIdentifier currentTarget;
        private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
        private FilteringSettings filteringSettings;
        private ProfilingSampler profilingSampler;

        public UIRenderPass()
        {
            profilingSampler = new ProfilingSampler(Tag);
            renderPassEvent = RenderPassEvent.AfterRendering + 10;
            shaderTagIdList.Add(new ShaderTagId("UniversalForward"));
            shaderTagIdList.Add(new ShaderTagId("LightweightForward"));
            shaderTagIdList.Add(new ShaderTagId("SRPDefaultUnlit"));
            filteringSettings = new FilteringSettings(RenderQueueRange.transparent, 1 << LayerMask.NameToLayer("UI"));
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

                cmd.ClearRenderTarget(true, false, Color.clear);
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var cam = renderingData.cameraData.camera;
                var sortFlags = SortingCriteria.BackToFront;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortFlags);

                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }
}