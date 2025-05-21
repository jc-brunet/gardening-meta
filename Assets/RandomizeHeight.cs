using UnityEngine;

public class RandomizeHeight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        foreach (Transform child in this.transform)
        {
            // Check if the child has children (i.e., it's a grandparent)
            if (child.childCount > 0)
            {
                foreach (Transform grandchild in child)
                {
                    // Randomize the height of the grandchild
                    Vector3 originalScale = grandchild.localScale;
                    Vector3 originalPos = grandchild.localPosition;
                    float randomScale = Random.Range(0.8f, 1.2f);
                    float randomEpsilonX = Random.Range(-0.01f, 0.01f);
                    float randomEpsilonY = Random.Range(-0.01f, 0.01f);
                    originalScale *= randomScale;
                    originalPos.x += randomEpsilonX;
                    originalPos.y += randomEpsilonY;
                    grandchild.localScale = originalScale;
                    grandchild.localPosition = originalPos;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
