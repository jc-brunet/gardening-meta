using UnityEngine;
using Oculus.Interaction;
using Unity.XR.CoreUtils;

public class RackReturn : MonoBehaviour
{
    public Transform placeHolderTransform; // Assign the CenterSpot transform in the Inspector
    public bool grabbedOther;

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.CompareTag("Seed Pack"))
        {

            Debug.Log("collision");
            ReturnToCenter(other.gameObject);
            // Check if the object is being held by a Meta XR Grab Interactable
            GrabInteractable grabInteractable = other.GetComponentInChildren<GrabInteractable>();
            foreach (GrabInteractor selector in grabInteractable.SelectingInteractors)
            {
                grabInteractable.RemoveSelectingInteractor(selector);
            }
        }
    }

    private void ReturnToCenter(GameObject objToCenter)
    {
        // Move the object to the center spot
        Transform transformToCenter = objToCenter.GetComponent<Transform>();
        transformToCenter.position = placeHolderTransform.position;

        // Make the object upright
        transformToCenter.rotation = placeHolderTransform.rotation;

        objToCenter.GetComponent<Rigidbody>().ResetInertiaTensor();
    }
}

