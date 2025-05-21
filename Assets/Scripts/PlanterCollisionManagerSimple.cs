using Unity.VisualScripting;
using UnityEngine;

public class PlanterCollisionManagerSimple : MonoBehaviour
{

    public bool IsBud;
    public bool IsFlower;
    //public bool IsWilted;
    private GameObject _flowerPrefab;

    [SerializeField] Transform FlowerSpawn;

    //[SerializeField] float TimeToWilted;

    private GameObject _thisBud;
    private GameObject _thisFlower;
    //private GameObject _thisWilted;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 flowerSpawnPos = FlowerSpawn.transform.position;
        flowerSpawnPos.x += Random.Range(-0.01f, 0.01f);
        flowerSpawnPos.z += Random.Range(-0.01f, 0.01f);
        FlowerSpawn.transform.position = flowerSpawnPos;

        IsBud = false;
        IsFlower = false;
        //IsWilted = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnParticleCollision(GameObject other)
    {

        //if (!IsBud)
        //{
        //    if (other.CompareTag("SeedStream"))
        //    {
        //        GameObject budPrefab = other.GetComponentInParent<SeedInstantiation>().BudPrefab;
        //        _thisBud = Instantiate(budPrefab, FlowerSpawn);
        //        _flowerPrefab = other.GetComponentInParent<SeedInstantiation>().FlowerPrefab;
        //        IsBud = true;
        //    }
        //}
        //else
        //{
            if (IsBud && !IsFlower)
            {
                if (other.CompareTag("WaterStream"))
                {

                    Destroy(_thisBud);
                    _thisFlower = Instantiate(_flowerPrefab, FlowerSpawn);
                    IsFlower = true;
                    //Invoke("TransitionToWilted", TimeToWilted);

                }
            }
            //else
            //{
            //    if (!IsWilted)
            //    {
            //        if (other.CompareTag("WaterStream"))
            //        {
            //            CancelInvoke("TransitionToWilted");
            //            Invoke("TransitionToWilted", TimeToWilted);
            //        }
            //    }
            //    else
            //    {
            //        if (other.CompareTag("WaterStream"))
            //        {
            //            {
            //                IsWilted = false;
            //                GameObject.Destroy(_thisWilted);
            //                _thisFlower = Instantiate(FlowerPrefab, FlowerSpawn);
            //                Invoke("TransitionToWilted", TimeToWilted);

            //            }
            //        }
            //    }
            //}
        //}
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!IsBud && collision.collider.CompareTag("FlowerTray"))
        //{
        //    _thisBud = GameObject.Instantiate(BudPrefab, FlowerSpawn);
        //}

        GameObject other = collision.collider.gameObject;
        if (other.CompareTag("SpawnTray"))
        {
            CollisionConstants collisionConstants = other.GetComponent<CollisionConstants>();
            GameObject budPrefab = collisionConstants.BudPrefab;
            _flowerPrefab = collisionConstants.FlowerPrefab;
            _thisBud = Instantiate(budPrefab, FlowerSpawn);
            Instantiate(other, collisionConstants.OriginTransform.position, collisionConstants.OriginTransform.rotation);
            Destroy(other);
            IsBud = true;
        }

        if (collision.relativeVelocity.magnitude > 1f && other.CompareTag("HoeBlade"))
        {
            GameObject.Destroy(_thisBud);
            GameObject.Destroy(_thisFlower);
            //GameObject.Destroy(_thisWilted);
            IsBud = false;
            IsFlower = false;
            ////IsWilted = false;
            //CancelInvoke("TransitionToWilted");
        }
    }

    //private void TransitionToWilted()
    //{
    //    GameObject.Destroy(_thisFlower);
    //    //_thisWilted = Instantiate(WiltedPrefab, FlowerSpawn);
    //    //IsWilted = true;
    //}
}
