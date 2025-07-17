using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TraySpawnerManager : NetworkBehaviour
{
    [System.Serializable]
    public class ShelfTraySetup
    {
        public GameObject shelfObject;    // Shelf GameObject (parent of spawners)
        public GameObject trayPrefab;     // Tray prefab for this shelf
    }

    [SerializeField] private ShelfTraySetup[] shelfSetups; // 6 shelves in the inspector

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            // Delay one frame to allow all scene NetworkObjects to spawn
            StartCoroutine(DelayedSpawn());
        }
    }

    private IEnumerator DelayedSpawn()
    {
        yield return null; // Wait 1 frame
        SpawnAllTrays();
    }

    private void SpawnAllTrays()
    {
        foreach (var setup in shelfSetups)
        {
            if (setup.shelfObject == null || setup.trayPrefab == null)
                continue;

            Transform top = setup.shelfObject.transform.Find("Spawner_Top");
            Transform mid = setup.shelfObject.transform.Find("Spawner_Mid");
            Transform bottom = setup.shelfObject.transform.Find("Spawner_Bottom");

            if (top != null)
                SpawnTray(setup.trayPrefab, top);
            else
                Debug.LogWarning($"Spawner_Top not found under {setup.shelfObject.name}");

            if (mid != null)
                SpawnTray(setup.trayPrefab, mid);
            else
                Debug.LogWarning($"Spawner_Mid not found under {setup.shelfObject.name}");

            if (bottom != null)
                SpawnTray(setup.trayPrefab, bottom);
            else
                Debug.LogWarning($"Spawner_Bottom not found under {setup.shelfObject.name}");
        }
    }

    private void SpawnTray(GameObject trayPrefab, Transform spawner)
    {

        GameObject trayInstance = Instantiate(trayPrefab, spawner.position, spawner.rotation);

        NetworkObject netObj = trayInstance.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
            netObj.TrySetParent(spawner);
        }

        CollisionConstants collisionConstants = trayInstance.GetComponent<CollisionConstants>();
        if (collisionConstants != null)
        {
            collisionConstants.OriginTransform = spawner;
        }
    }
}
