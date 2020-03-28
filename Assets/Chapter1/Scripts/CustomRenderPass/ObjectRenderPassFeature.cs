using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter1
{
    public class ObjectRenderPassFeature : ScriptableRendererFeature
    {
        private ObjectRenderPass currentPass;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new ObjectRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            renderer.EnqueuePass(currentPass);
        }
    }
}