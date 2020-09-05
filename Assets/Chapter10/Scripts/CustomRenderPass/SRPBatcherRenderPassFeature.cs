using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter10
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
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}