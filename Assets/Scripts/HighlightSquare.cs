using UnityEngine;

public class HighlightSquare : MonoBehaviour
{
    [SerializeField] GameObject Highlight;


    public void ActivateHighlight()
    {
        Highlight.SetActive(true);
    }

    public void ResetHighlight()
    {
        Highlight.SetActive(false);
    }
}

