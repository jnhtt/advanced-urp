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
			renderPassEvent = renderPassEvent = RenderPassEvent.AfterRendering + 2;
			FinalImageGrabRenderPassFeature.finalImageRenderTexture = new RenderTexture(1920, 1080, 0, RenderTextureFormat.Default);
			FinalImageGrabRenderPassFeature.finalImageRenderTexture.name = "FinalImage";
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

                RenderTargetIdentifier prev = BuiltinRenderTextureType.CameraTarget;
				cmd.Blit(BuiltinRenderTextureType.CameraTarget, finalImageRenderTargetIdentifier);
				cmd.SetRenderTarget(prev);
			}
			context.ExecuteCommandBuffer(cmd);
			CommandBufferPool.Release(cmd);
			FinalImageGrabRenderPassFeature.EndSnapshot();
		}
	}
}
