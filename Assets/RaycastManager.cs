using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RaycastManager : MonoBehaviour
{
    private HighlightSquare currentHighlightedSquare;
    private bool _isRayCastActive;
    public Transform StartingPoint;

    private void Start()
    {
    }
    void Update()
    {
        if (_isRayCastActive)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, -transform.right - 2 * transform.up, out hit))
            {
                HighlightSquare square = hit.collider.GetComponent<HighlightSquare>();

                if (square != null)
                {
                    if (currentHighlightedSquare != null && currentHighlightedSquare != square)
                    {
                        currentHighlightedSquare.ResetHighlight();
                    }
                    currentHighlightedSquare = square;
                    if (!currentHighlightedSquare.gameObject.GetComponent<PlanterCollisionManagerSimple>().IsLv1)
                    {
                        currentHighlightedSquare.ActivateHighlight();
                    }
                }
                else
                {
                    ResetHighlights();
                }
            }
            else
            {
                ResetHighlights();
            }
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

    public void PlantFlowers()
    {
        if (currentHighlightedSquare != null)
        {
            currentHighlightedSquare.gameObject.GetComponent<PlanterCollisionManagerSimple>().PlantBud(this.gameObject);
            currentHighlightedSquare.ResetHighlight();
            currentHighlightedSquare = null;
        }
    }

    public void ActivateRaycast()
    {
        _isRayCastActive = true;
    }
}

