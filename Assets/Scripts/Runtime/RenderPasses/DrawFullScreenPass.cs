using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;

public class DrawFullScreenPass : ScriptableRenderPass
{
    string m_ProfilerTag = "DrawFullScreenPass";

    DrawFullScreenFeature.DrawFullScreenFeatureSettings m_Settings;

    public DrawFullScreenPass(DrawFullScreenFeature.DrawFullScreenFeatureSettings settings)
    {
        renderPassEvent = settings.renderPassEvent;
        m_Settings = settings;
    }

    public override bool ShouldExecute(ref RenderingData renderingData)
    {
        return m_Settings.material != null;
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        DrawFullscreen(context, ref renderingData.cameraData, m_Settings.material);
    }
}
