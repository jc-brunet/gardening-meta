using UnityEngine;

public class PlanterSpawner : MonoBehaviour
{
    public PlanterSpawnerScriptableObject MyConstants;

    [SerializeField] GameObject PlanterPrefab;
    [SerializeField] Transform TerrainTransform;

    private BoxCollider _planterCollider;
    private int _numberMaxX;
    private int _numberMaxZ;
    private float _initPosX;
    private float _initPosZ;
    private Vector3 _newPosition;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _planterCollider = PlanterPrefab.GetComponent<BoxCollider>();

        float terrainSizeX = (TerrainTransform.localScale.x * MyConstants.XTerrainSize);
        float terrainSizeZ = (TerrainTransform.localScale.z * MyConstants.ZTerrainSize);

        _initPosX = TerrainTransform.position.x - terrainSizeX / 2f + _planterCollider.size.x / 2f;
        _initPosZ = TerrainTransform.position.z - terrainSizeZ / 2f + _planterCollider.size.z / 2f;

        _newPosition = new Vector3(_initPosX, TerrainTransform.position.y, _initPosZ);

        _numberMaxX = (int)(terrainSizeX /_planterCollider.size.x);
        _numberMaxZ = (int)(terrainSizeZ / _planterCollider.size.z);

        for (int i = 0; i < _numberMaxX; i++)
        {
            for (int j = 0; j < _numberMaxZ; j++)
            { 
                Instantiate(PlanterPrefab, _newPosition, PlanterPrefab.transform.rotation, this.transform);
                
                _newPosition.z += _planterCollider.size.z;
            }
            _newPosition.x += _planterCollider.size.x;
            _newPosition.z = _initPosZ;
        }
    }
}
