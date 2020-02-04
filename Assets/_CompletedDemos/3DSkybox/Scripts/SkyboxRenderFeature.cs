using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class SkyboxRenderFeature : ScriptableRendererFeature
{

    DrawSkyboxPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new DrawSkyboxPass(RenderPassEvent.AfterRenderingSkybox);

        // Configures where the render pass should be injected.
        m_ScriptablePass.renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    // Here you can inject one or multiple render passes in the renderer.
    // This method is called when setting up the renderer once per-camera.
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(m_ScriptablePass);
    }
}


