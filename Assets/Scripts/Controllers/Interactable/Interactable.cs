using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour {
    [Header("Max Distance for Interact to trigger")]
    public float maxDistance = 1.0f;
    [HideInInspector]
    public Vector3 interactionWorldPoint;
    [HideInInspector]
    public GameObject interactor;

    public virtual void Init(Vector3 worldPoint)
    {
        interactionWorldPoint = worldPoint;
    }
    public virtual void Interact()
    {
       // Debug.Log("Interacting with " + gameObject.name);
    }

    public virtual void TryInteract(GameObject user)
    {
        interactor = user;
        if (Vector2.Distance(transform.position, user.transform.position) <= maxDistance)
        {
            Interact();
        }
        else
        {
            Debug.Log(user.name + " is too far to interact with " + transform.position);
        }
    }
}
