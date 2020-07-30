using System;
using UnityEngine;
using UnityEngine.Rendering;


public class BlurGrabPass : UnityEngine.Rendering.Universal.ScriptableRendererFeature
{
    [Serializable]
    public class Settings
    {
        public Vector2 m_BlurAmount;
    }
    
    const string k_BasicBlitShader = "Hidden/BasicBlit";
    private Material m_BasicBlitMaterial;

    const string k_BlurShader = "Hidden/Blur";
    private Material m_BlurMaterial;

    private Vector2 currentBlurAmount;

    public Settings settings;

    private GrabPassImpl m_grabPass;

    public override void Create()
    {
        m_grabPass = new GrabPassImpl(m_BlurMaterial, currentBlurAmount, m_BasicBlitMaterial);
        m_grabPass.renderPassEvent = UnityEngine.Rendering.Universal.RenderPassEvent.AfterRenderingSkybox;
        m_BasicBlitMaterial = CoreUtils.CreateEngineMaterial(Shader.Find(k_BasicBlitShader));
        m_BlurMaterial = CoreUtils.CreateEngineMaterial(Shader.Find(k_BlurShader));
        currentBlurAmount = settings.m_BlurAmount;
    }

    public override void AddRenderPasses(UnityEngine.Rendering.Universal.ScriptableRenderer renderer, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        renderer.EnqueuePass(m_grabPass);
    }

    void Update()
    {
        if(m_grabPass != null)
        {
            if(currentBlurAmount != settings.m_BlurAmount)
            {
                currentBlurAmount = settings.m_BlurAmount;
                m_grabPass.UpdateBlurAmount(currentBlurAmount);
            }
        } 
    }
}


public class GrabPassImpl : UnityEngine.Rendering.Universal.ScriptableRenderPass
{
    ProfilingSampler m_ProfilingSampler = new ProfilingSampler("Blur Refraction Pass");

    private Material m_BlurMaterial;
    
    private Material m_BlitMaterial;

    private Vector2 m_BlurAmount;

    ShaderTagId screenCopyID = new ShaderTagId("_ScreenCopyTexture");
    private RenderTextureDescriptor m_OpaqueDesc;
    private RenderTextureDescriptor m_BaseDescriptor;
    private UnityEngine.Rendering.Universal.RenderTargetHandle m_ColorHandle;

    public GrabPassImpl(Material blurMaterial, Vector2 blurAmount, Material blitMaterial)
    {
        m_BlurMaterial = blurMaterial;
        m_ColorHandle = UnityEngine.Rendering.Universal.RenderTargetHandle.CameraTarget;
        m_BlitMaterial = blitMaterial;
        m_BlurAmount = blurAmount;
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        cameraTextureDescriptor.msaaSamples = 1;
        m_OpaqueDesc = cameraTextureDescriptor;
        cmd.GetTemporaryRT(m_ColorHandle.id, cameraTextureDescriptor, FilterMode.Bilinear);
    }

    public void UpdateBlurAmount(Vector2 newBlurAmount)
    {
        m_BlurAmount = newBlurAmount;
    }

    public override void Execute(ScriptableRenderContext context, ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        CommandBuffer buf = CommandBufferPool.Get();
        using (new ProfilingScope(buf, m_ProfilingSampler))
        {
            // copy screen into temporary RT
            int screenCopyID = Shader.PropertyToID("_ScreenCopyTexture");
            buf.GetTemporaryRT(screenCopyID, m_OpaqueDesc, FilterMode.Bilinear);
            buf.Blit(m_ColorHandle.Identifier(), screenCopyID);

            m_OpaqueDesc.width /= 2;
            m_OpaqueDesc.height /= 2;

            // get two smaller RTs
            int blurredID = Shader.PropertyToID("_BlurRT1");
            int blurredID2 = Shader.PropertyToID("_BlurRT2");
            buf.GetTemporaryRT(blurredID, m_OpaqueDesc, FilterMode.Bilinear);
            buf.GetTemporaryRT(blurredID2, m_OpaqueDesc, FilterMode.Bilinear);

            // downsample screen copy into smaller RT, release screen RT
            buf.Blit(screenCopyID, blurredID);
            buf.ReleaseTemporaryRT(screenCopyID);
            
            // horizontal blur
            buf.SetGlobalVector("offsets", new Vector4(m_BlurAmount.x / Screen.width, 0, 0, 0));
            buf.Blit(blurredID, blurredID2, m_BlurMaterial);
            // vertical blur
            buf.SetGlobalVector("offsets", new Vector4(0, m_BlurAmount.y / Screen.height, 0, 0));
            buf.Blit(blurredID2, blurredID, m_BlurMaterial);

            // horizontal blur
            buf.SetGlobalVector("offsets", new Vector4(m_BlurAmount.x * 2 / Screen.width, 0, 0, 0));
            buf.Blit(blurredID, blurredID2, m_BlurMaterial);
            // vertical blur
            buf.SetGlobalVector("offsets", new Vector4(0, m_BlurAmount.y * 2 / Screen.height, 0, 0));
            buf.Blit(blurredID2, blurredID, m_BlurMaterial);

            //Set Texture for Shader Graph
            buf.SetGlobalTexture("_GrabBlurTexture", blurredID);
        }

        context.ExecuteCommandBuffer(buf);
        CommandBufferPool.Release(buf);
    }
}
