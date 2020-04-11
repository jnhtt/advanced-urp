using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter1
{
    public class BackgroundRenderPassFeature : ScriptableRendererFeature
    {
        private BackgroundRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new BackgroundRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}