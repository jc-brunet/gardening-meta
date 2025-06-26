using UnityEngine;

public class RandomizeTreeLine : MonoBehaviour
{

    private Transform[] _childrenTransforms;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        _childrenTransforms = this.GetComponentsInChildren<Transform>();
        foreach (Transform t in _childrenTransforms) {
            //Quaternion tEuler = t.localRotation;
            Vector3 tScale = t.localScale;
            //float randomAngle = Random.Range(0f, 360f);
            float randomFactor = Random.Range(0.8f, 1.2f);

            //tEuler.y += randomAngle;
            tScale.y *= randomFactor;

            //t.localRotation = tEuler;
            t.localScale = tScale;
            //t.RotateAround(t.position, Vector3.up, r  andomAngle);
            //t.Rotate(new Vector3(0, randomAngle, 0),Space.Self);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
