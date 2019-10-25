using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    public Camera mainCamera;

    private bool firstTouchHappened = false;
    private Vector3 touchStartPosition;
    private float groundZ = 0;

    
    
    void Update()
    {

        #if UNITY_EDITOR

        ProcessDesktopInputs();

        
        #elif UNITY_IOS || UNITY_ANDROID
        
        ProcessMobileInputs();

        #endif
        

    }
    

    void ProcessDesktopInputs()
    {

        //Left Click - Selection
         if(Input.GetKeyDown(KeyCode.Mouse0)){

            if (!EventSystem.current.IsPointerOverGameObject ()){
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
            //GameManager.Instance.ObjectClicked(hit.collider.gameObject, hit.point);
        }

    }
    
}
