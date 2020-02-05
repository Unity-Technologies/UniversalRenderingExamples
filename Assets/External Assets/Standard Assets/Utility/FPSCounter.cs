using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets.Utility
{
    public class FPSCounter : MonoBehaviour
    {
        const float fpsMeasurePeriod = 0.5f;
        private int m_FpsAccumulator = 0;
        private float m_FpsNextPeriod = 0;
        private int m_CurrentFps;
        const string display = "{0} FPS";
        public Component m_Text;


        private void Start()
        {
            m_FpsNextPeriod = Time.realtimeSinceStartup + fpsMeasurePeriod;
        }


        private void Update()
        {
            if (!m_Text) return;

            // measure average frames per second
            m_FpsAccumulator++;
            if (Time.realtimeSinceStartup > m_FpsNextPeriod)
            {
                m_CurrentFps = (int) (m_FpsAccumulator/fpsMeasurePeriod);
                m_FpsAccumulator = 0;
                m_FpsNextPeriod += fpsMeasurePeriod;
                if (m_Text.GetType() == typeof(Text))
                {
                    var t = m_Text as Text;
                    t.text = $"{m_CurrentFps} fps\n{(Time.deltaTime * 1000f):0.00} ms";
                }
                else if (m_Text.GetType() == typeof(TextMeshProUGUI))
                {
                    var t = m_Text as TextMeshProUGUI;
                    t.text = $"{m_CurrentFps} fps\n{(Time.deltaTime * 1000f):0.00} ms";
                }
            }
        }
    }
}
