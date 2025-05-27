using UnityEngine;

public class CollisionConstants : MonoBehaviour
{
    public GameObject BudPrefab;
    public GameObject FlowerPrefab;
    public Transform OriginTransform;
    public bool HasRespawned;

    private void Awake()
    {
        HasRespawned = false;
    }
}
