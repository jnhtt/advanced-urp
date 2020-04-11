using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Chapter8
{
    public class DistortionRenderPassFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class Data
        {
            [SerializeField, Range(0.001f, 1f)]
            public float speed = 0.3f;
            [SerializeField, Range(0.001f, 1f)]
            public float power = 0.2f;
        }

        private DistortionRenderPass currentPass;
        [SerializeField]
        private Data data = default;

        public override void Create()
        {
            if (currentPass == null)
            {
                currentPass = new DistortionRenderPass();
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            currentPass.SetRenderTarget(renderer.cameraColorTarget);
            currentPass.SetData(data);
            renderer.EnqueuePass(currentPass);
        }
    }
}