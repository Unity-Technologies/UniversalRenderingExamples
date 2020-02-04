using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    
    public Camera mainCamera;
    public Camera interactionCamera;
    public NavMeshAgent agent;
    public Animator animator;
    
    private bool m_FirstTouchHappened = false;
    private Vector3 m_TouchStartPosition;
    private float m_GroundZ = 0;

    private void OnEnable()
    {
        if (!Instance)
            Instance = this;
    }

    void Update()
    {
        #if UNITY_EDITOR

        ProcessDesktopInputs();

        #elif UNITY_IOS || UNITY_ANDROID
        
        ProcessMobileInputs();

        #endif

        if (agent.velocity.sqrMagnitude > 0.01f)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
    

    void ProcessDesktopInputs()
    {
        //Left Click - Selection
         if(Input.GetKeyDown(KeyCode.Mouse0))
         {
            if (!EventSystem.current || !EventSystem.current.IsPointerOverGameObject ())
            {
                RaycastAgainstTheWorld(Input.mousePosition);
            }
         }

    }

    

    void ProcessMobileInputs()
    {
        if(Input.touchCount > 0)
        {
            Touch firstTouch = Input.GetTouch(0);

             if (EventSystem.current.IsPointerOverGameObject(0))    // is the touch on the GUI
                return;

            switch (firstTouch.phase)
            {
                case TouchPhase.Began:
                    RaycastAgainstTheWorld(firstTouch.position);
                    break; 

            }
        }
    }
    
    void RaycastAgainstTheWorld(Vector3 inputPosition)
    {
        var ray = mainCamera.ScreenPointToRay(inputPosition);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit))
        {
            Interactable interaction;

            if (hit.transform.gameObject.TryGetComponent(out interaction))
            {
                interaction.StartInteraction();
                Interact();
            }
            else if(!agent.isStopped)
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    void Interact()
    {
        interactionCamera.enabled = true;
        agent.isStopped = true;
    }

    public void ExitInteraction()
    {
        interactionCamera.enabled = false;
        agent.isStopped = false;
    }
}
