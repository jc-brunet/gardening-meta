using System.Collections;
using System.Runtime.CompilerServices;
using Oculus.Interaction;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class PlanterCollisionManagerSimple : NetworkBehaviour
{

    public bool IsLv1;
    public bool IsLv2;
    public bool IsLv3;
    //public bool IsWilted;
    private GameObject _lv1Prefab;
    private GameObject _lv2Prefab;
    private GameObject _lv3Prefab;

    [SerializeField] Transform FlowerSpawn;
    [SerializeField] float WaterNeededToGrow;
    [SerializeField] AudioSource PlantingSound;
    [SerializeField] AudioSource WateringSound;
    [SerializeField] float HoeSpeedThreshold;

    private GameObject _thisLv1;
    private GameObject _thisLv2;
    private GameObject _thisLv3;
    private GameObject _newTray;
    private GameObject _oldTray;
    private GameObject _trayPrefab;
    private Transform _parentTransformForNewTray;
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
                DespawnLv1ServerRPC();
                SpawnLv2ServerRPC();
                WateringSound.Play();
                IsLv2 = true;
                _wateringCounter = 0f;
            }
            else if (IsLv2 && !IsLv3)
            {
                DespawnLv2ServerRPC();
                SpawnLv3ServerRPC();
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

    private void OnTriggerEnter(UnityEngine.Collider other)
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

        if (other.CompareTag("HoeBlade")
            //&& Mathf.Abs(other.GetComponentInParent<Rigidbody>().linearVelocity.y) > HoeSpeedThreshold
            )
        {
            other.GetComponent<AudioSource>().Play();
            DespawnEverythingServerRPC();
            DestroyEverything();
        }
    }

    public void DestroyEverything()
    {
        //GameObject.Destroy(_thisLv1);
        //GameObject.Destroy(_thisLv2);
        //GameObject.Destroy(_thisLv3);
        //GameObject.Destroy(_thisWilted);
        IsLv1 = false;
        IsLv2 = false;
        IsLv3 = false;
        ////IsWilted = false;
        //CancelInvoke("TransitionToWilted");
    }

    public void PlantBud(GameObject other, GameObject prefabOfOther)
    {
        if (!IsLv1)
        {
            CollisionConstants collisionConstants = other.GetComponent<CollisionConstants>();
            _lv1Prefab = collisionConstants.Lv1Prefab;
            _lv2Prefab = collisionConstants.Lv2Prefab;
            _lv3Prefab = collisionConstants.Lv3Prefab;
            SpawnLv1ServerRPC();
            PlantingSound.Play();
            _parentTransformForNewTray = collisionConstants.OriginTransform;
            IsLv1 = true;
            _oldTray = other;
            _trayPrefab = prefabOfOther;
            SpawnNewTrayServerRPC();
            _newTray.GetComponent<Rigidbody>().isKinematic = false;
            //Destroy(other);

        }
    }

    [ServerRpc]
    private void SpawnLv1ServerRPC(ServerRpcParams rpcParams = default)
    {
        _thisLv1 = Instantiate(_lv1Prefab, FlowerSpawn);
        NetworkObject thisLv1NO = _thisLv1.GetComponent<NetworkObject>();
        thisLv1NO.Spawn();
        thisLv1NO.TrySetParent(gameObject);
    }


    [ServerRpc]
    private void SpawnLv2ServerRPC(ServerRpcParams rpcParams = default)
    {
        _thisLv2 = Instantiate(_lv2Prefab, FlowerSpawn);
        NetworkObject thisLv2NO = _thisLv2.GetComponent<NetworkObject>();
        thisLv2NO.Spawn();
        thisLv2NO.TrySetParent(gameObject);
    }

    [ServerRpc]
    private void SpawnLv3ServerRPC(ServerRpcParams rpcParams = default)
    {
        _thisLv3 = Instantiate(_lv3Prefab, FlowerSpawn);
        NetworkObject thisLv3NO = _thisLv3.GetComponent<NetworkObject>();
        thisLv3NO.Spawn();
        thisLv3NO.TrySetParent(gameObject);
    }

    [ServerRpc]
    private void SpawnNewTrayServerRPC(ServerRpcParams rpcParams = default)
    {
        _newTray = Instantiate(_trayPrefab, _parentTransformForNewTray.position, _parentTransformForNewTray.rotation, _parentTransformForNewTray);
        _newTray.GetComponent<CollisionConstants>().OriginTransform = _parentTransformForNewTray;
        NetworkObject newTrayNO = _newTray.GetComponent<NetworkObject>();
        newTrayNO.SpawnWithObservers = true;
        newTrayNO.Spawn();
        newTrayNO.TrySetParent(_parentTransformForNewTray);
        _oldTray.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc]
    private void DespawnLv1ServerRPC(ServerRpcParams rpcParams = default)
    {
        _thisLv1.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc]
    private void DespawnLv2ServerRPC(ServerRpcParams rpcParams = default)
    {
        _thisLv2.GetComponent<NetworkObject>().Despawn();
    }

    [ServerRpc]
    private void DespawnEverythingServerRPC(ServerRpcParams rpcParams = default) { 

        if (_thisLv1){ _thisLv1.GetComponent<NetworkObject>().Despawn(); }
        if (_thisLv2){ _thisLv2.GetComponent<NetworkObject>().Despawn(); }
        if (_thisLv3){ _thisLv3.GetComponent<NetworkObject>().Despawn(); }


        //NetworkObject thisLv1NO = _thisLv1.GetComponent<NetworkObject>();
        //NetworkObject thisLv2NO = _thisLv2.GetComponent<NetworkObject>();
        //NetworkObject thisLv3NO = _thisLv3.GetComponent<NetworkObject>();

        //if (thisLv1NO != null) {thisLv1NO.Despawn(); }
        //if (thisLv2NO != null) {thisLv2NO.Despawn(); }  
        //if (thisLv3NO != null) {thisLv3NO.Despawn(); }
    }
}

