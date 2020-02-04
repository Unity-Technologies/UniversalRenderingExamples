using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public GameObject ativateGameObject;
    
    public void StartInteraction()
    {
        ativateGameObject.SetActive(true);
    }

    public void DisableInteraction()
    {
        InputManager.Instance.ExitInteraction();
    }
}
