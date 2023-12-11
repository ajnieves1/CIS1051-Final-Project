using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class NPCInteractable : MonoBehaviour
{
    public string virtualCameraName = "Dialogue Camera"; // references cinemachine
    public void Interact() 
    {
        //CinemachineVirtualCamera virtualCamera = GameObject.Find(virtualCameraName)?.GetComponent<CinemachineVirtualCamera>();

        //if (virtualCamera != null)
        //{
        //    virtualCamera.Priority = 10;
        //}
        ChatBubble3D.Create(transform.transform, new Vector3(0f, 3.5f, 0f), "Stay away from atoms. They make up everything!");
        Debug.Log("Interact!");
    }
}
