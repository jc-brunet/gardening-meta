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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("WateringCan"))
        {
            collision.collider.GetComponent<WateringInstantiation>().FillTank();
        }
    }
}
