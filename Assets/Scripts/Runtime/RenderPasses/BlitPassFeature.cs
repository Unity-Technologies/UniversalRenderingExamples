using System.Collections.Generic;

namespace UnityEngine.Rendering.LWRP
{
    [CreateAssetMenu(fileName = "BlitFeature", menuName = "Rendering/Lightweight Render Pipeline/Renderer Features/BlitFeature", order = CoreUtils.assetCreateMenuPriority1)]
    public class BlitPassFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class BlitSettings
        {
            public RenderPassEvent Event = RenderPassEvent.AfterRenderingOpaques;
            
            public Material blitMaterial = null;
            public int blitMaterialPassIndex = -1;
            public Target dest = Target.Color;
            public string textureId = "_BlitPassTexture";
        }
        
        public enum Target
        {
            Color,
            Texture
        }

        public BlitSettings settings = new BlitSettings();
        RenderTargetHandle m_RenderTextureHandle;

        BlitPass blitPass;

        public override void Create()
        {
            var passIndex = settings.blitMaterial != null ? settings.blitMaterial.passCount - 1 : 1;
            settings.blitMaterialPassIndex = Mathf.Clamp(settings.blitMaterialPassIndex, -1, passIndex);
            blitPass = new BlitPass(settings.Event, settings.blitMaterial, settings.blitMaterialPassIndex, name);
            m_RenderTextureHandle.Init(settings.textureId);
        }

        public override void AddRenderPasses(List<ScriptableRenderPass> renderPasses,
            RenderTextureDescriptor baseDescriptor,
            RenderTargetHandle colorAttachmentHandle,
            RenderTargetHandle depthAttachmentHandle)
        {
            var src = colorAttachmentHandle;
            var dest = (settings.dest == Target.Color) ? colorAttachmentHandle : m_RenderTextureHandle;
            
            blitPass.Setup(src, dest);
            renderPasses.Add(blitPass);
        }
    }
}

