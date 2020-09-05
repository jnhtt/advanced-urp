using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Chapter4
{
	public class FinalImageGrabRenderPass : ScriptableRenderPass
	{
		private const string Tag = "FinalImageGrabRenderPass";
		private RenderTargetIdentifier currentTarget;
		private RenderTargetIdentifier finalImageRenderTargetIdentifier;

		private ProfilingSampler profilingSampler;

		public FinalImageGrabRenderPass()
		{
			renderPassEvent = RenderPassEvent.AfterRendering + 2;
			finalImageRenderTargetIdentifier = new RenderTargetIdentifier(FinalImageGrabRenderPassFeature.finalImageRenderTexture);
			Debug.Log("finalImageRenderTargetIdentifier=" + finalImageRenderTargetIdentifier);

			profilingSampler = new ProfilingSampler(Tag);
		}
		public void SetRenderTarget(RenderTargetIdentifier target)
		{
			currentTarget = target;
		}

		public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
		{
			if (!FinalImageGrabRenderPassFeature.snapshot)
			{
				return;
			}
			var cmd = CommandBufferPool.Get(Tag);
			using (new ProfilingScope(cmd, profilingSampler))
			{
				context.ExecuteCommandBuffer(cmd);
				cmd.Clear();

				ref CameraData cameraData = ref renderingData.cameraData;
				var cameraTarget = (cameraData.targetTexture != null) ? new RenderTargetIdentifier(cameraData.targetTexture) : BuiltinRenderTextureType.CameraTarget;
				cmd.Blit(cameraTarget, finalImageRenderTargetIdentifier);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
			FinalImageGrabRenderPassFeature.EndSnapshot();
		}
	}
}
