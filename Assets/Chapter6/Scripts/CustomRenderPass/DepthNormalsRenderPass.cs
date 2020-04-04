using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter6
{
    public class DepthNormalsRenderPass : ScriptableRenderPass
    {
        private const string Tag = "DepthNormalsRenderPass";

        private RenderTargetHandle depthNormalsTextureHandle;
        private Material depthNormalsMaterial;
        private List<ShaderTagId> shaderTagIdList = new List<ShaderTagId>();
        private FilteringSettings filteringSettings;
        private ProfilingSampler profilingSampler;

        public DepthNormalsRenderPass()
        {
            profilingSampler = new ProfilingSampler(Tag);

            depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");
            depthNormalsTextureHandle.Init("_CameraDepthNormalsTexture");

            renderPassEvent = RenderPassEvent.AfterRenderingPrePasses;
            shaderTagIdList.Add(new ShaderTagId("DepthOnly"));
            filteringSettings = new FilteringSettings(RenderQueueRange.opaque, -1);
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var descriptor = cameraTextureDescriptor;
            descriptor.colorFormat = RenderTextureFormat.ARGB32;
            descriptor.depthBufferBits = 32;
            descriptor.msaaSamples = 1;

            cmd.GetTemporaryRT(depthNormalsTextureHandle.id, descriptor, FilterMode.Point);
            ConfigureTarget(depthNormalsTextureHandle.Identifier());
            ConfigureClear(ClearFlag.All, Color.black);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get(Tag);
            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                var sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                var drawSettings = CreateDrawingSettings(shaderTagIdList, ref renderingData, sortFlags);
                drawSettings.perObjectData = PerObjectData.None;

                ref CameraData cameraData = ref renderingData.cameraData;
                Camera cam = cameraData.camera;
                if (cameraData.isStereoEnabled)
                {
                    context.StartMultiEye(cam);
                }

                drawSettings.overrideMaterial = depthNormalsMaterial;
                context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref filteringSettings);
                cmd.SetGlobalTexture("_CameraDepthNormalsTexture", depthNormalsTextureHandle.id);
            }
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null)
            {
                throw new ArgumentNullException("cmd");
            }

            if (depthNormalsTextureHandle != RenderTargetHandle.CameraTarget)
            {
                cmd.ReleaseTemporaryRT(depthNormalsTextureHandle.id);
                //depthNormalsTextureHandle = RenderTargetHandle.CameraTarget;
            }
        }
    }
}