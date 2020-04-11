using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter8
{
    public class DistortionMarkRenderPassFeature : ScriptableRendererFeature
    {
        public const int DistortionMarkRenderingMaskLayer = 2;
        private DistortionMarkRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new DistortionMarkRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderer.EnqueuePass(currentPass);
        }
    }
}