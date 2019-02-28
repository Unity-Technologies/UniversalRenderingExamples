using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.LWRP;
using UnityEngine.SceneManagement;

[ExecuteAlways]
public class AutoLoadPipelineAsset : MonoBehaviour
{
    public LightweightRenderPipelineAsset pipelineAsset;
    
    private void OnEnable()
    {
        UpdatePipeline();
    }

    void UpdatePipeline()
    {
        if (pipelineAsset)
        {
            GraphicsSettings.renderPipelineAsset = pipelineAsset;
        }
    }
}
