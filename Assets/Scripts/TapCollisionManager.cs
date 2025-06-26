using UnityEngine;

public class TapCollisionManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {
        if (other.CompareTag("WateringCan"))
        {
            other.GetComponent<WateringInstantiation>().FillTank();
        }
    }
}
