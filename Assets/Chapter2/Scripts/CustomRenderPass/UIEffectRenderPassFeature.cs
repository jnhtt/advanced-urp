using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter2
{
    public class UIEffectRenderPassFeature : ScriptableRendererFeature
    {
        private UIEffectRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new UIEffectRenderPass();
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