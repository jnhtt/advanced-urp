using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chapter8
{
    [RequireComponent(typeof(Renderer))]
    [ExecuteInEditMode()]
    public class MarkerComponent : MonoBehaviour
    {
        [SerializeField]
        private bool useDistortion;

        private void Awake()
        {
            var renderer = GetComponent<Renderer>();
            if (useDistortion)
            {
                renderer.renderingLayerMask |= 1 << DistortionMarkRenderPassFeature.DistortionMarkRenderingMaskLayer;
            }
        }
    }
}
