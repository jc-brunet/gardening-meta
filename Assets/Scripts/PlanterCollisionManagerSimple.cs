using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;
using Oculus.Interaction;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public enum GrowthStage
{
    None,
    Lv1,
    Lv2,
    Lv3
}
public class PlanterCollisionManagerSimple : NetworkBehaviour
{

    //public bool IsLv1;
    //public bool IsLv2;
    //public bool IsLv3;
    //public bool IsWilted;
    public NetworkVariable<GrowthStage> PlantStage = new(GrowthStage.None, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    //private GameObject _lv1Prefab;
    //private GameObject _lv2Prefab;
    //private GameObject _lv3Prefab;

    private string _cachedLv1Name;
    private string _cachedLv2Name;
    private string _cachedLv3Name;

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
    private float _wateringCounter = 0f;
    private bool _hasGrown = false;


 
    void Start()
    {
        Vector3 flowerSpawnPos = FlowerSpawn.transform.position;
        flowerSpawnPos.x += Random.Range(-0.01f, 0.01f);
        flowerSpawnPos.z += Random.Range(-0.01f, 0.01f);
        FlowerSpawn.transform.position = flowerSpawnPos;
        SetGrowthStageServerRpc(GrowthStage.None);
    }

    //public void OnParticleCollision(GameObject other)
    //{
    //    if (other.CompareTag("WaterStream"))
    //    {
    //        var wateringCan = other.GetComponentInParent<NetworkObject>();
    //        if (wateringCan != null)
    //        {
    //            AddWater(Time.deltaTime, wateringCan);
    //        }
    //    }
    //}

    public void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("WaterStream"))
        {
            NetworkObject wateringCan = other.GetComponentInParent<NetworkObject>();

            // Only proceed if we're the client who owns the watering can
            if (wateringCan != null && wateringCan.IsOwner)
            {
                RequestAddWaterServerRpc(Time.deltaTime, wateringCan.OwnerClientId, _cachedLv2Name, _cachedLv3Name);
            }
        }
    }

    //private void AddWater(float amount, NetworkObject wateringCanNO)
    //{
    //    if (wateringCanNO.OwnerClientId != NetworkManager.Singleton.LocalClientId)
    //        return; // Not the owner, ignore

    //    if (_hasGrown) return;

    //    _wateringCounter += amount;

    //    if (_wateringCounter > WaterNeededToGrow)
    //    {
    //        _hasGrown = true;
    //        OnSufficientWater();
    //    }
    //}

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void RequestAddWaterServerRpc(float amount, ulong requesterClientId, string lv2PrefabName, string lv3PrefabName)
    {
        if (_hasGrown) return;

        _wateringCounter += amount;

        if (_wateringCounter > WaterNeededToGrow)
        {
            _hasGrown = true;

            switch (PlantStage.Value)
            {
                case GrowthStage.Lv1:
                    DespawnLvServerRPC(_thisLv1);
                    SpawnLv2ServerRPC(lv2PrefabName); // use string passed earlier
                    SetGrowthStageServerRpc(GrowthStage.Lv2);
                    break;

                case GrowthStage.Lv2:
                    DespawnLvServerRPC(_thisLv2);
                    SpawnLv3ServerRPC(lv3PrefabName);
                    SetGrowthStageServerRpc(GrowthStage.Lv3);
                    break;
            }

            _wateringCounter = 0f;
            _hasGrown = false;
        }
    }


    private void OnSufficientWater()
    {
        Debug.Log("Watering threshold met!");

        if (PlantStage.Value == GrowthStage.Lv1)
        {
            DespawnLvServerRPC(_thisLv1.GetComponent<NetworkObject>());
            SpawnLv2ServerRPC(_cachedLv2Name);
            WateringSound.Play();
            SetGrowthStageServerRpc(GrowthStage.Lv2);
        }
        else if (PlantStage.Value == GrowthStage.Lv2)
        {
            DespawnLvServerRPC(_thisLv2.GetComponent<NetworkObject>());
            SpawnLv3ServerRPC(_cachedLv3Name);
            WateringSound.Play();
            SetGrowthStageServerRpc(GrowthStage.Lv3);
        }

        _wateringCounter = 0f;
        _hasGrown = false; // reset for next growth phase
    }

    private void OnTriggerEnter(UnityEngine.Collider other)
    {

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
        SetGrowthStageServerRpc(GrowthStage.None);
    }

    public void PlantBud(GameObject other, GameObject prefabOfOther)
    {

        if (PlantStage.Value == GrowthStage.None)
        {
            CollisionConstants constants = other.GetComponent<CollisionConstants>();
            //_lv1Prefab = collisionConstants.Lv1Prefab;
            //_lv2Prefab = collisionConstants.Lv2Prefab;
            //_lv3Prefab = collisionConstants.Lv3Prefab;
            //SpawnLv1ServerRPC(_lv1Prefab.name);
            //PlantingSound.Play();

            _cachedLv1Name = constants.Lv1Prefab.name;
            _cachedLv2Name = constants.Lv2Prefab.name;
            _cachedLv3Name = constants.Lv3Prefab.name;

            SpawnLv1ServerRPC(_cachedLv1Name, _cachedLv2Name,_cachedLv3Name);
            SetGrowthStageServerRpc(GrowthStage.Lv1);
            //PlantingSound.Play();
            _parentTransformForNewTray = constants.OriginTransform;
            _oldTray = other;
            _trayPrefab = prefabOfOther;
            string trayName = _trayPrefab.name.Replace("(Clone)", "").Trim();
            Vector3 spawnPos = Vector3.zero;
            Quaternion spawnRot = Quaternion.identity;
            if (_parentTransformForNewTray != null)
            {
                spawnPos = _parentTransformForNewTray.position;
                spawnRot = _parentTransformForNewTray.rotation;
            }
            NetworkObjectReference oldTrayNO = _oldTray.GetComponent<NetworkObject>();
            SpawnNewTrayServerRPC(trayName, spawnPos, spawnRot, oldTrayNO);
            _newTray.GetComponent<Rigidbody>().isKinematic = false;
            //Destroy(other);

        }
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    public void SetGrowthStageServerRpc(GrowthStage newStage)
    {
        Debug.Log("SetGrowthStageServerRpc called with " + newStage);
        PlantStage.Value = newStage;
    }



    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnLv1ServerRPC(string lv1PrefabName, string lv2PrefabName, string lv3PrefabName, RpcParams rpcParams = default)
    {
        GameObject prefab = null;

        _cachedLv1Name = lv1PrefabName;
        _cachedLv2Name = lv2PrefabName;
        _cachedLv3Name = lv3PrefabName; 

        foreach (NetworkPrefab networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
        {
            if (networkPrefab.Prefab != null && networkPrefab.Prefab.name == lv1PrefabName)
            {
                prefab = networkPrefab.Prefab;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogError($"Server could not find prefab with name: {lv1PrefabName}");
            return;
        }

        _thisLv1 = Instantiate(prefab, FlowerSpawn.position, FlowerSpawn.rotation, FlowerSpawn);
        NetworkObject no = _thisLv1.GetComponent<NetworkObject>();
        no.Spawn();
        no.TrySetParent(gameObject);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnLv2ServerRPC(string prefabName, RpcParams rpcParams = default)
    {
        GameObject prefab = null;

        foreach (NetworkPrefab networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
        {
            if (networkPrefab.Prefab != null && networkPrefab.Prefab.name == prefabName)
            {
                prefab = networkPrefab.Prefab;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogError($"Server could not find prefab with name: {prefabName}");
            return;
        }

        _thisLv2 = Instantiate(prefab, FlowerSpawn.position, FlowerSpawn.rotation, FlowerSpawn);
        NetworkObject no = _thisLv2.GetComponent<NetworkObject>();
        no.Spawn();
        no.TrySetParent(gameObject);
    }




    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnLv3ServerRPC(string prefabName, RpcParams rpcParams = default)
    {
        GameObject prefab = null;

        foreach (NetworkPrefab networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
        {
            if (networkPrefab.Prefab != null && networkPrefab.Prefab.name == prefabName)
            {
                prefab = networkPrefab.Prefab;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogError($"Server could not find prefab with name: {prefabName}");
            return;
        }

        _thisLv3 = Instantiate(prefab, FlowerSpawn.position, FlowerSpawn.rotation, FlowerSpawn);
        NetworkObject no = _thisLv3.GetComponent<NetworkObject>();
        no.Spawn();
        no.TrySetParent(gameObject);
    }

    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void SpawnNewTrayServerRPC(string trayPrefabName, Vector3 spawnPosition, Quaternion spawnRotation, NetworkObjectReference oldTrayRef, RpcParams rpcParams = default)
    {
        Debug.Log($"Entered spawnNewTray. SpawnPos = {spawnPosition} ; SpawnRot = {spawnRotation}");
        // Look up the tray prefab by name
        GameObject prefab = null;
        foreach (NetworkPrefab networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
        {
            if (networkPrefab.Prefab != null && networkPrefab.Prefab.name == trayPrefabName)
            {
                prefab = networkPrefab.Prefab;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogError($"[Server] Could not find tray prefab with name: {trayPrefabName}");
            return;
        }
        else { Debug.Log($"Found tray prefab with name: {trayPrefabName}"); }

        Transform oldParent = null;

        // Try get the parent of the old tray before despawning it
        if (oldTrayRef.TryGet(out NetworkObject oldTrayNO))
        {
            oldParent = oldTrayNO.transform.parent;
            Debug.Log("OldParent found: "+oldParent.name);
        }
        else
        {
            Debug.LogWarning("[Server] Failed to get old tray NetworkObject from reference.");
        }

        // Instantiate the new tray
        _newTray = Instantiate(prefab, oldParent);

        CollisionConstants collisionConstants = _newTray.GetComponent<CollisionConstants>();
        collisionConstants.OriginTransform = oldParent; // Or any logic to assign it

        NetworkObject newTrayNO = _newTray.GetComponent<NetworkObject>();
        newTrayNO.SpawnWithObservers = true;
        newTrayNO.Spawn();
        newTrayNO.TrySetParent(oldParent);

        // Despawn the old tray after setting up the new one
        if (oldTrayNO != null)
        {
            oldTrayNO.Despawn(true);
        }
        else
        {
            Debug.Log($"oldTray not found");
        }
    }

    //[Rpc(SendTo.Server, RequireOwnership = false)]
    //private void SpawnNewTrayServerRPC(string trayPrefabName, Vector3 spawnPosition, Quaternion spawnRotation, NetworkObjectReference oldTrayRef, RpcParams rpcParams = default)
    //{
    //    // Look up the tray prefab by name
    //    GameObject prefab = null;
    //    foreach (var networkPrefab in NetworkManager.Singleton.NetworkConfig.Prefabs.Prefabs)
    //    {
    //        if (networkPrefab.Prefab != null && networkPrefab.Prefab.name == trayPrefabName)
    //        {
    //            prefab = networkPrefab.Prefab;
    //            break;
    //        }
    //    }

    //    if (prefab == null)
    //    {
    //        Debug.LogError($"[Server] Could not find tray prefab with name: {trayPrefabName}");
    //        return;
    //    }

    //    // Instantiate the new tray
    //    _newTray = Instantiate(prefab, spawnPosition, spawnRotation);
    //    var collisionConstants = _newTray.GetComponent<CollisionConstants>();
    //    collisionConstants.OriginTransform = _newTray.transform; // Or any logic to assign it

    //    NetworkObject newTrayNO = _newTray.GetComponent<NetworkObject>();
    //    newTrayNO.SpawnWithObservers = true;
    //    newTrayNO.Spawn();

    //    newTrayNO.TrySetParent(_newTray.transform); // Adjust if needed

    //    // Despawn the old tray
    //    if (oldTrayRef.TryGet(out NetworkObject oldTrayNO))
    //    {
    //        oldTrayNO.Despawn();
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[Server] Failed to get old tray NetworkObject from reference.");
    //    }
    //}


    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void DespawnLvServerRPC(NetworkObjectReference lvRef, RpcParams rpcParams = default)
    {
        if (lvRef.TryGet(out NetworkObject lvNO))
        {
            lvNO.Despawn();
        }
        else
        {
            Debug.LogWarning("[Server] Could not get NetworkObject for lvRef.");
        }
    }



    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void DespawnEverythingServerRPC(RpcParams rpcParams = default)
    {


        DespawnLvServerRPC(_thisLv1);
        DespawnLvServerRPC(_thisLv2);
        DespawnLvServerRPC(_thisLv3);

    }
}

