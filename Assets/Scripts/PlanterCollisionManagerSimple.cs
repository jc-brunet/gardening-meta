using System.Collections;
using Oculus.Interaction;
using Unity.VisualScripting;
using UnityEngine;

public class PlanterCollisionManagerSimple : MonoBehaviour
{

    public bool IsLv1;
    public bool IsLv2;
    public bool IsLv3;
    //public bool IsWilted;
    private GameObject _lv2Prefab;
    private GameObject _lv3Prefab;

    [SerializeField] Transform FlowerSpawn;
    [SerializeField] float WaterNeededToGrow;
    [SerializeField] AudioSource PlantingSound;
    [SerializeField] AudioSource WateringSound;

    //[SerializeField] float TimeToWilted;

    private GameObject _thisLv1;
    private GameObject _thisLv2;
    private GameObject _thisLv3;
    //private GameObject _thisWilted;

    private float _wateringCounter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Vector3 flowerSpawnPos = FlowerSpawn.transform.position;
        flowerSpawnPos.x += Random.Range(-0.01f, 0.01f);
        flowerSpawnPos.z += Random.Range(-0.01f, 0.01f);
        FlowerSpawn.transform.position = flowerSpawnPos;

        IsLv1 = false;
        IsLv2 = false;
        IsLv3 = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_wateringCounter > WaterNeededToGrow)
        {
            if (IsLv1 && !IsLv2)
            {
                Destroy(_thisLv1);
                _thisLv2 = Instantiate(_lv2Prefab, FlowerSpawn);
                WateringSound.Play();
                IsLv2 = true;
                _wateringCounter = 0f;
            }
            else if (IsLv2 && !IsLv3)
            {
                Destroy(_thisLv2);
                _thisLv3 = Instantiate(_lv3Prefab, FlowerSpawn);
                WateringSound.Play();
                IsLv3 = true;
                _wateringCounter = 0f;
            }
        }
    }

    public void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("WaterStream"))
        {
            _wateringCounter += Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!IsBud && collision.collider.CompareTag("FlowerTray"))
        //{
        //    _thisBud = GameObject.Instantiate(BudPrefab, FlowerSpawn);
        //}

        //GameObject other = collision.collider.gameObject;
        //if (!IsLv1 && other.CompareTag("SpawnTray"))
        //{
        //    //PlantBud(other);
        //}

        if (collision.collider.CompareTag("HoeBlade") && collision.relativeVelocity.magnitude > 0.3f)
        {
            collision.collider.GetComponentInChildren<AudioSource>().Play();
            DestroyEverything();
        }
    }

    public void DestroyEverything()
    {
        GameObject.Destroy(_thisLv1);
        GameObject.Destroy(_thisLv2);
        GameObject.Destroy (_thisLv3);
        //GameObject.Destroy(_thisWilted);
        IsLv1 = false;
        IsLv2 = false;
        IsLv3 = false;
        ////IsWilted = false;
        //CancelInvoke("TransitionToWilted");
    }

    public void PlantBud(GameObject other)
    {
        if (!IsLv1)
        {
            CollisionConstants collisionConstants = other.GetComponent<CollisionConstants>();
            GameObject lv1Prefab = collisionConstants.Lv1Prefab;
            _lv2Prefab = collisionConstants.Lv2Prefab;
            _lv3Prefab = collisionConstants.Lv3Prefab;
            _thisLv1 = Instantiate(lv1Prefab, FlowerSpawn);
            PlantingSound.Play();
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
    }
}
