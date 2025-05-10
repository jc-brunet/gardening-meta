using UnityEngine;

public class PlanterCollisionManager : MonoBehaviour
{

    public bool isPlanted;
    public bool isWatered;

    [SerializeField] GameObject budPrefab;
    [SerializeField] Transform flowerSpawn;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isPlanted = false;
        isWatered = false;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnParticleCollision(GameObject other)
    {
        
        if (!isPlanted) { 
            if (other.CompareTag("SeedStream"))
            {
                isPlanted = true;
                Instantiate(budPrefab, flowerSpawn);
            }
            }
    }
}
