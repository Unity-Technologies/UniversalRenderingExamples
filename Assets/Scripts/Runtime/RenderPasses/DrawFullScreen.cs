namespace UnityEngine.Rendering.LWRP
{
    public class DrawFullScreen : ScriptableRendererFeature
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

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (m_Settings.material != null)
                renderer.EnqueuePass(m_RenderPass);
        }
    }
}
