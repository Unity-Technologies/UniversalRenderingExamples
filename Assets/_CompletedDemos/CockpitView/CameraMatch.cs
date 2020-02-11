using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Camera)), ExecuteAlways]
public class CameraMatch : MonoBehaviour
{
    public Camera camera;
    private Camera m_ThisCamera;
    private UniversalAdditionalCameraData m_CamData;
    private UniversalAdditionalCameraData m_ThisCamData;

    private void OnEnable()
    {
        if (!camera) return;

        camera.TryGetComponent(out m_CamData);
        TryGetComponent(out m_ThisCamera);
        TryGetComponent(out m_ThisCamData);
    }

    private void LateUpdate()
    {
        if(!camera || !m_CamData || !m_ThisCamera || !m_ThisCamData) return;

        m_ThisCamera.projectionMatrix = camera.projectionMatrix;
    }
}
