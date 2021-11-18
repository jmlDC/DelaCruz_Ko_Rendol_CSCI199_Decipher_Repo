using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float radius = 3f;

    bool isFocus = false;
    [SerializeField] Transform player;
    [SerializeField] string currentInteractable;



    void Update()
    {
        float distance = Vector3.Distance(player.position, transform.position);

        if (distance <= radius)
        {
            Debug.Log("Interaction possible with " + gameObject.name);
            currentInteractable = gameObject.name;
        } else {
            currentInteractable = null;
        }
    }

    public void setTransform(Transform transform){
        player = transform;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, radius);
    }


}
