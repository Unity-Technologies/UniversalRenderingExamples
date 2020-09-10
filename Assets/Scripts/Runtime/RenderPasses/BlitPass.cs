namespace UnityEngine.Rendering.Universal
{
    /// <summary>
    /// Copy the given color buffer to the given destination color buffer.
    ///
    /// You can use this pass to copy a color buffer to the destination,
    /// so you can use it later in rendering. For example, you can copy
    /// the opaque texture to use it for distortion effects.
    /// </summary>
    internal class BlitPass : ScriptableRenderPass
    {
        public enum RenderTarget
        {
            Color,
            RenderTexture,
        }

        public FilterMode filterMode { get; set; }
        public Blit.BlitSettings settings;

        RenderTargetIdentifier source;
        RenderTargetIdentifier destination;
        int temporaryRTId = Shader.PropertyToID("_TempRT");

        int sourceId;
        int destinationId;
        bool isSourceAndDestinationSameTarget;

        string m_ProfilerTag;

        /// <summary>
        /// Create the CopyColorPass
        /// </summary>
        public BlitPass(string tag)
        {
            m_ProfilerTag = tag;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor blitTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            blitTargetDescriptor.depthBufferBits = 0;

            isSourceAndDestinationSameTarget = settings.sourceType == settings.destinationType &&
                (settings.sourceType == BufferType.CameraColor || settings.sourceTextureId == settings.destinationTextureId);

            var renderer = renderingData.cameraData.renderer;

            if (settings.sourceType == BufferType.CameraColor)
            {
                sourceId = -1;
                source = renderer.cameraColorTarget;
            }
            else
            {
                sourceId = Shader.PropertyToID(settings.sourceTextureId);
                cmd.GetTemporaryRT(sourceId, blitTargetDescriptor, filterMode);
                source = new RenderTargetIdentifier(sourceId);
            }

            if (isSourceAndDestinationSameTarget)
            {
                destinationId = temporaryRTId;
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
                destination = new RenderTargetIdentifier(destinationId);
            }
            else if (settings.destinationType == BufferType.CameraColor)
            {
                destinationId = -1;
                destination = renderer.cameraColorTarget;
            }
            else
            {
                destinationId = Shader.PropertyToID(settings.destinationTextureId);
                cmd.GetTemporaryRT(destinationId, blitTargetDescriptor, filterMode);
                destination = new RenderTargetIdentifier(destinationId);
            }
        }

        /// <inheritdoc/>
        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);

            CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);

            // Can't read and write to same color target, create a temp render target to blit. 
            if (isSourceAndDestinationSameTarget)
            {
                Blit(cmd, source, destination, settings.blitMaterial, passIndex);
                Blit(cmd, destination, source);
            }
            else
            {
                Blit(cmd, source, destination, settings.blitMaterial, passIndex);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        /// <inheritdoc/>
        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (destinationId != -1)
                cmd.ReleaseTemporaryRT(destinationId);

            if (source == destination && sourceId != -1)
                cmd.ReleaseTemporaryRT(sourceId);
        }
    }
}
