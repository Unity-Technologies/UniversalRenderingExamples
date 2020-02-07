using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SkyboxRenderFeature : ScriptableRendererFeature
{

    DrawSkyboxPass m_SkyboxPass;

    public override void Create()
    {
        m_SkyboxPass = new DrawSkyboxPass(RenderPassEvent.AfterRenderingSkybox);

        // Configures where the render pass should be injected.
        m_SkyboxPass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_SkyboxPass);
    }
}


