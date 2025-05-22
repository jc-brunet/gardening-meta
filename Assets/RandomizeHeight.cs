using UnityEngine;

public class RandomizeHeight : MonoBehaviour
{

    public RandomizeHeightSO MyConstants;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        float positionVariance = MyConstants.Positionvariance;
        float scaleVariance = MyConstants.ScaleVariance;
        float rotationVariance = MyConstants.MaxRotation;


        float posRandomLowerBound = -positionVariance;
        float posRandomUpperBound = positionVariance;
        float scaleFactorRandomUpperBound = 1 + scaleVariance;
        float scaleFactorRandomLowerBound = 1 - scaleVariance;
        foreach (Transform child in this.transform)
        {
            // Randomize the height of the grandchild
            Vector3 originalScale = child.localScale;
            Vector3 originalPos = child.localPosition;
            float randomScale = Random.Range(scaleFactorRandomLowerBound, scaleFactorRandomUpperBound);
            float randomEpsilonX = Random.Range(posRandomLowerBound, posRandomUpperBound);
            float randomEpsilonZ = Random.Range(posRandomLowerBound, posRandomUpperBound);
            originalScale *= randomScale;
            originalPos.x += randomEpsilonX;
            originalPos.z += randomEpsilonZ;
            child.localScale = originalScale;
            child.localPosition = originalPos;
            child.Rotate(Vector3.up, Random.Range(-rotationVariance, rotationVariance));
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

}
