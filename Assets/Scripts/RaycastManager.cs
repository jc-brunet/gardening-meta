using UnityEngine;
using System.Collections;

public class RaycastManager : MonoBehaviour
{
    private HighlightSquare currentHighlightedSquare;
    public Transform StartingPoint;
    private Rigidbody _thisRigidbody;

    private void Awake()
    {
        _thisRigidbody = GetComponent<Rigidbody>();
    }
    private IEnumerator _RaycastCoroutine()
    {
        _thisRigidbody.useGravity = false;
        int resetCounter = 0;
        while (true)
        {
            if (true)//_isRayCastActive)
            {
                RaycastHit hit = new();
                Vector3 rayDirection = -transform.up;

                if (Physics.Raycast(transform.position, rayDirection, out hit))
                {
                    HighlightSquare square = hit.collider.GetComponent<HighlightSquare>();
                    if (square != null)
                    {
                        resetCounter = 0;
                        if (square.gameObject.GetComponent<PlanterCollisionManagerSimple>().IsLv1)
                        {
                            ResetHighlights();
                        }
                        else
                        {
                            if (currentHighlightedSquare == null)
                            {
                                currentHighlightedSquare = square;
                                currentHighlightedSquare.ActivateHighlight();
                            }
                            else
                            {
                                if (currentHighlightedSquare != square)
                                {
                                    currentHighlightedSquare.ResetHighlight();
                                    currentHighlightedSquare = square;
                                    currentHighlightedSquare.ActivateHighlight();
                                }
                            }
                        }

                    }
                    else
                    {
                        resetCounter++;
                        if (resetCounter > 2)
                        {
                            ResetHighlights();
                            resetCounter = 0;
                        }
                    }
                }
                else
                {
                    resetCounter++;
                    if (resetCounter > 2)
                    {
                        ResetHighlights();
                        resetCounter = 0;
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void ResetHighlights()
    {
        if (currentHighlightedSquare != null)
        {
            currentHighlightedSquare.ResetHighlight();
            currentHighlightedSquare = null;
        }
    }

    public void OnUnselect()
    {
        _thisRigidbody.useGravity = true;
        StopCoroutine(_RaycastCoroutine());
        if (currentHighlightedSquare != null)
        {
            currentHighlightedSquare.ResetHighlight();
            currentHighlightedSquare.gameObject.GetComponent<PlanterCollisionManagerSimple>().PlantBud(this.gameObject);
            currentHighlightedSquare = null;
        }
    }

    public void ActivateRaycast()
    {
        StartCoroutine(_RaycastCoroutine());
    }

}
