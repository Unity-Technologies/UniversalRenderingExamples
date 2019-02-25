using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

[CreateAssetMenu(fileName = "DrawFullscreenFeature", menuName = "Rendering/Lightweight Render Pipeline/Renderer Features/DrawFullscreen", order = CoreUtils.assetCreateMenuPriority1)]
public class DrawFullScreenFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public struct DrawFullScreenFeatureSettings
    {
        public RenderPassEvent renderPassEvent;
        public Material material;
    }

    public DrawFullScreenFeatureSettings m_Settings;
    DrawFullScreenPass m_RenderPass;

    public override void Create()
    {
        m_RenderPass = new DrawFullScreenPass(m_Settings);
    }

    public override void AddRenderPasses(List<ScriptableRenderPass> renderPasses, RenderTextureDescriptor cameraDescriptor, RenderTargetHandle colorAttachmentHandle, RenderTargetHandle depthAttachmentHandle)
    {
        if (m_Settings.material != null)
            renderPasses.Add(m_RenderPass);
    }
}
