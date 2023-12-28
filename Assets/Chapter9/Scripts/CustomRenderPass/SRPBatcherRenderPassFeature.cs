using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter9
{
    public class SRPBatcherRenderPassFeature : ScriptableRendererFeature
    {
        private SRPBatcherRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new SRPBatcherRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //currentPass.SetRenderTarget(renderer.cameraColorTargetHandle);
            renderer.EnqueuePass(currentPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            base.SetupRenderPasses(renderer, renderingData);
            currentPass.SetRenderTarget(renderer.cameraColorTargetHandle);
        }
    }
}