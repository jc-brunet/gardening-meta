using UnityEngine;

public class PlanterCollisionManager : MonoBehaviour
{

    public bool IsBud;
    public bool IsFlower;
    public bool IsWilted;
    public bool IsDecayed;
    public bool IsWeed;

    [SerializeField] GameObject BudPrefab;
    [SerializeField] GameObject FlowerPrefab;
    [SerializeField] GameObject WiltedPrefab;
    [SerializeField] GameObject DecayedPrefab;
    [SerializeField] GameObject WeedPrefab;

    [SerializeField] Transform FlowerSpawn;

    [SerializeField] float TimeToWilted;
    [SerializeField] float TimeToDecayed;
    [SerializeField] float TimeToWeed;

    private GameObject _thisBud;
    private GameObject _thisFlower;
    private GameObject _thisWilted;
    private GameObject _thisDecayed;
    private GameObject _thisWeed;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 flowerSpawnPos = FlowerSpawn.transform.position;
        flowerSpawnPos.x += Random.Range(-0.01f, 0.01f);
        flowerSpawnPos.z += Random.Range(-0.01f, 0.01f);
        FlowerSpawn.transform.position = flowerSpawnPos;

        IsBud = false;
        IsFlower = false;
        IsWilted = false;
        IsDecayed = false;
        IsWeed = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnParticleCollision(GameObject other)
    {

        if (!IsBud)
        {
            if (other.CompareTag("SeedStream"))
            {
                IsBud = true;
                _thisBud = Instantiate(BudPrefab, FlowerSpawn);
            }
        }
        else
        {
            if (!IsFlower)
            {
                if (other.CompareTag("WaterStream"))
                {
                    {
                        IsFlower = true;
                        GameObject.Destroy(_thisBud);
                        _thisFlower = Instantiate(FlowerPrefab, FlowerSpawn);
                        Invoke("TransitionToWilted", TimeToWilted);
                    }
                }
            }
            else
            {
                if (!IsWilted)
                {
                    if (other.CompareTag("WaterStream"))
                    {
                        CancelInvoke("TransitionToWilted");
                        Invoke("TransitionToWilted", TimeToWilted);
                    }
                }
                else
                {
                    if (!IsDecayed)
                    {
                        if (other.CompareTag("WaterStream"))
                        {
                            {
                                IsWilted = false;
                                GameObject.Destroy(_thisWilted);
                                _thisFlower = Instantiate(FlowerPrefab, FlowerSpawn);
                                CancelInvoke("TransitionToDecayed");
                                Invoke("TransitionToWilted", TimeToWilted);

                            }
                        }
                    }
                    else
                    {
                        if (IsWeed)
                        {
                            if (other.CompareTag("FlameStream"))
                            {
                                {
                                    IsWeed = false;
                                    GameObject.Destroy(_thisWeed);
                                    _thisDecayed = Instantiate(DecayedPrefab, FlowerSpawn);
                                    Invoke("TransitionToWeed", TimeToWeed);

                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsDecayed && !IsWeed && collision.relativeVelocity.magnitude > 1f && collision.collider.CompareTag("HoeBlade"))
        {
            GameObject.Destroy(_thisDecayed);
            IsFlower = false;
            IsBud = false;
            IsWilted = false;
            IsDecayed = false;
        }

        if (IsBud && !IsDecayed && collision.relativeVelocity.magnitude > 1f && collision.collider.CompareTag("TrowelPoint"))
        {
            GameObject.Destroy(_thisBud);
            GameObject.Destroy(_thisFlower);
            GameObject.Destroy(_thisWilted);
            IsBud = false;
            IsFlower = false;
            IsWilted = false;
        }
    }

    private void TransitionToWilted()
    {
        GameObject.Destroy(_thisFlower);
        _thisWilted = Instantiate(WiltedPrefab, FlowerSpawn);
        IsWilted = true;
        Invoke("TransitionToDecayed", TimeToDecayed);
    }

    private void TransitionToDecayed()
    {
        GameObject.Destroy(_thisWilted);
        _thisDecayed = Instantiate(DecayedPrefab, FlowerSpawn);
        IsDecayed = true;
        Invoke("TransitionToWeed", TimeToWeed);
    }

    private void TransitionToWeed()
    {
        GameObject.Destroy(_thisDecayed);
        _thisWeed = Instantiate(WeedPrefab, FlowerSpawn);
        IsWeed = true;
    }
}
