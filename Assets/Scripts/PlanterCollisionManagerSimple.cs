using Unity.VisualScripting;
using UnityEngine;

public class PlanterCollisionManagerSimple : MonoBehaviour
{

    public bool IsLv1;
    public bool IsLv2;
    //public bool IsWilted;
    private GameObject _lv2Prefab;

    [SerializeField] Transform FlowerSpawn;

    //[SerializeField] float TimeToWilted;

    private GameObject _thisLv1;
    private GameObject _thisLv2;
    //private GameObject _thisWilted;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 flowerSpawnPos = FlowerSpawn.transform.position;
        flowerSpawnPos.x += Random.Range(-0.01f, 0.01f);
        flowerSpawnPos.z += Random.Range(-0.01f, 0.01f);
        FlowerSpawn.transform.position = flowerSpawnPos;

        IsLv1 = false;
        IsLv2 = false;
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
            if (IsLv1 && !IsLv2)
            {
                if (other.CompareTag("WaterStream"))
                {

                    Destroy(_thisLv1);
                    _thisLv2 = Instantiate(_lv2Prefab, FlowerSpawn);
                    IsLv2 = true;
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
        if (!IsLv1 && other.CompareTag("SpawnTray"))
        {
            CollisionConstants collisionConstants = other.GetComponent<CollisionConstants>();
            GameObject budPrefab = collisionConstants.BudPrefab;
            _lv2Prefab = collisionConstants.FlowerPrefab;
            _thisLv1 = Instantiate(budPrefab, FlowerSpawn);
            Transform parentTransform = collisionConstants.OriginTransform;
            IsLv1 = true;
            if (collisionConstants.HasRespawned) { return; }
            else
            {
                collisionConstants.HasRespawned = true;
                GameObject newTray = Instantiate(other, parentTransform.position, parentTransform.rotation, parentTransform);
                newTray.GetComponent<Rigidbody>().isKinematic = false;
                newTray.GetComponent<CollisionConstants>().HasRespawned = false;
                Destroy(other);
            }
        }

        if (collision.relativeVelocity.magnitude > 1f && other.CompareTag("HoeBlade"))
        {
            GameObject.Destroy(_thisLv1);
            GameObject.Destroy(_thisLv2);
            //GameObject.Destroy(_thisWilted);
            IsLv1 = false;
            IsLv2 = false;
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
