using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPickup : MonoBehaviour
{
    public GameObject whatPickUp;
    public GameObject playerAboveHead;

    public void PickUpObject() 
    {
        whatPickUp.transform.SetParent(playerAboveHead.transform);
        whatPickUp.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("PickableObject")) 
        {
            whatPickUp = other.gameObject;
            Debug.Log("It is pickable object " + other.gameObject.name);
        }
    }
}
