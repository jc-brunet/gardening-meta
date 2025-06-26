using UnityEngine;

public class CollisionConstants : MonoBehaviour
{
    public GameObject Lv1Prefab;
    public GameObject Lv2Prefab;
    public GameObject Lv3Prefab;
    public Transform OriginTransform;
    public bool HasRespawned;

    private void Awake()
    {
        HasRespawned = false;
    }
}
