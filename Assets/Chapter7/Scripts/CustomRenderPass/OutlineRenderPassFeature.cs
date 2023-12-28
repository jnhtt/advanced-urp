using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter7
{
    public class OutlineRenderPassFeature : ScriptableRendererFeature
    {
        private OutlineRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new OutlineRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(currentPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            base.SetupRenderPasses(renderer, renderingData);
            currentPass.SetRenderTarget(renderer.cameraColorTargetHandle);
        }
    }
}