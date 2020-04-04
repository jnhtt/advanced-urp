using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter6
{
    public class DepthNormalsRenderPassFeature : ScriptableRendererFeature
    {
        private DepthNormalsRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new DepthNormalsRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(currentPass);
        }
    }
}