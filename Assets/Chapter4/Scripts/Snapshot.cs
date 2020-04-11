using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Chapter4
{
    public class Snapshot : MonoBehaviour
    {
        [SerializeField]
        private Button snapshotButton = default;
        [SerializeField]
        private RawImage snapshotImage = default;

        public void OnSnapshot()
        {
            FinalImageGrabRenderPassFeature.BeginSnapshot(OnFinishSnapshot);
        }

        private void OnFinishSnapshot()
        {
            snapshotImage.texture = FinalImageGrabRenderPassFeature.finalImageRenderTexture;
        }
    }
}
