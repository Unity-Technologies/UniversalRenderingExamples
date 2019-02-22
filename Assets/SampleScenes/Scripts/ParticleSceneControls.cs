using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.Effects;


namespace UnityStandardAssets.SceneUtils
{
    public class ParticleSceneControls : MonoBehaviour
    {
        public enum Mode
        {
            Activate,
            Instantiate,
            Trail
        }

        public enum AlignMode
        {
            Normal,
            Up
        }


        public DemoParticleSystemList demoParticles;
        public float spawnOffset = 0.5f;
        public float multiply = 1;
        public bool clearOnChange = false;
        public Text titleText;
        public Transform sceneCamera;
        public Text instructionText;
        public Button previousButton;
        public Button nextButton;
        public GraphicRaycaster graphicRaycaster;
        public EventSystem eventSystem;


        private ParticleSystemMultiplier m_ParticleMultiplier;
        private List<Transform> m_CurrentParticleList = new List<Transform>();
        private Transform m_Instance;
        private static int s_SelectedIndex = 0;
        private Vector3 m_CamOffsetVelocity = Vector3.zero;
        private Vector3 m_LastPos;
        private static DemoParticleSystem s_Selected;


        private void Awake()
        {
            Select(s_SelectedIndex);

            previousButton.onClick.AddListener(Previous);
            nextButton.onClick.AddListener(Next);
        }


        private void OnDisable()
        {
			previousButton.onClick.RemoveListener (Previous);
			nextButton.onClick.RemoveListener (Next);
        }


        private void Previous()
        {
            s_SelectedIndex--;
            if (s_SelectedIndex == -1)
            {
                s_SelectedIndex = demoParticles.items.Length - 1;
            }
            Select(s_SelectedIndex);
        }


        public void Next()
        {
            s_SelectedIndex++;
            if (s_SelectedIndex == demoParticles.items.Length)
            {
                s_SelectedIndex = 0;
            }
            Select(s_SelectedIndex);
        }


        private void Update()
        {

#if !MOBILE_INPUT
            KeyboardInput();
#endif



            sceneCamera.localPosition = Vector3.SmoothDamp(sceneCamera.localPosition, Vector3.forward*-s_Selected.camOffset,
                                                       ref m_CamOffsetVelocity, 1);

            if (s_Selected.mode == Mode.Activate)
            {
                // this is for a particle system that just needs activating, and needs no interaction (eg, duststorm)
                return;
            }

            if (CheckForGuiCollision()) return;

            bool oneShotClick = (Input.GetMouseButtonDown(0) && s_Selected.mode == Mode.Instantiate);
            bool repeat = (Input.GetMouseButton(0) && s_Selected.mode == Mode.Trail);

            if (oneShotClick || repeat)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    var rot = Quaternion.LookRotation(hit.normal);

                    if (s_Selected.align == AlignMode.Up)
                    {
                        rot = Quaternion.identity;
                    }

                    var pos = hit.point + hit.normal*spawnOffset;

                    if ((pos - m_LastPos).magnitude > s_Selected.minDist)
                    {
                        if (s_Selected.mode != Mode.Trail || m_Instance == null)
                        {
                            m_Instance = (Transform) Instantiate(s_Selected.transform, pos, rot);

                            if (m_ParticleMultiplier != null)
                            {
                                m_Instance.GetComponent<ParticleSystemMultiplier>().multiplier = multiply;
                            }

                            m_CurrentParticleList.Add(m_Instance);

                            if (s_Selected.maxCount > 0 && m_CurrentParticleList.Count > s_Selected.maxCount)
                            {
                                if (m_CurrentParticleList[0] != null)
                                {
                                    Destroy(m_CurrentParticleList[0].gameObject);
                                }
                                m_CurrentParticleList.RemoveAt(0);
                            }
                        }
                        else
                        {
                            m_Instance.position = pos;
                            m_Instance.rotation = rot;
                        }

                        if (s_Selected.mode == Mode.Trail)
                        {
                            var emission = m_Instance.transform.GetComponent<ParticleSystem>().emission;
                            emission.enabled = false;
                            m_Instance.transform.GetComponent<ParticleSystem>().Emit(1);
                        }

                        m_Instance.parent = hit.transform;
                        m_LastPos = pos;
                    }
                }
            }
        }


#if !MOBILE_INPUT
        void KeyboardInput()
        {
            if(Input.GetKeyDown(KeyCode.LeftArrow))
                Previous();

            if (Input.GetKeyDown(KeyCode.RightArrow))
                Next();
        }
#endif


        bool CheckForGuiCollision()
        {
            PointerEventData eventData = new PointerEventData(eventSystem);
            eventData.pressPosition = Input.mousePosition;
            eventData.position = Input.mousePosition;

            List<RaycastResult> list = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, list);
            return list.Count > 0;
        }

        private void Select(int i)
        {
            s_Selected = demoParticles.items[i];
            m_Instance = null;
            foreach (var otherEffect in demoParticles.items)
            {
                if ((otherEffect != s_Selected) && (otherEffect.mode == Mode.Activate))
                {
                    otherEffect.transform.gameObject.SetActive(false);
                }
            }
            if (s_Selected.mode == Mode.Activate)
            {
                s_Selected.transform.gameObject.SetActive(true);
            }
            m_ParticleMultiplier = s_Selected.transform.GetComponent<ParticleSystemMultiplier>();
            multiply = 1;
            if (clearOnChange)
            {
                while (m_CurrentParticleList.Count > 0)
                {
                    Destroy(m_CurrentParticleList[0].gameObject);
                    m_CurrentParticleList.RemoveAt(0);
                }
            }

            instructionText.text = s_Selected.instructionText;
            titleText.text = s_Selected.transform.name;
        }


        [Serializable]
        public class DemoParticleSystem
        {
            public Transform transform;
            public Mode mode;
            public AlignMode align;
            public int maxCount;
            public float minDist;
            public int camOffset = 15;
            public string instructionText;
        }

        [Serializable]
        public class DemoParticleSystemList
        {
            public DemoParticleSystem[] items;
        }
    }
}
