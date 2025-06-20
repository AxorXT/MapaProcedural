using System.Collections.Generic;
using UnityEngine;

public class MapaProcedural : MonoBehaviour
{
    [Header("Configuración del Terreno")]
    public GameObject[] terrainPrefabs;
    public float blockSize = 10f;
    public int viewRadius = 3;

    [Header("Objetos a Spawnear")]
    public GameObject[] objectPrefabs;
    public int minObjectsPerBlock = 0;
    public int maxObjectsPerBlock = 3;

    [Header("Jugador")]
    public Transform player;

    // Cada bloque con sus objetos hijos
    [SerializeField]private Dictionary<Vector2Int, BlockData> spawnedBlocks = new Dictionary<Vector2Int, BlockData>();

    void Start()
    {
        UpdateTerrain();
    }

    void Update()
    {
        UpdateTerrain();
    }

    void UpdateTerrain()
    {
        Vector2Int playerCoord = WorldToGrid(player.position);

        // Generar bloques alrededor del jugador
        for (int x = -viewRadius; x <= viewRadius; x++)
        {
            for (int z = -viewRadius; z <= viewRadius; z++)
            {
                Vector2Int coord = new Vector2Int(playerCoord.x + x, playerCoord.y + z);
                if (!spawnedBlocks.ContainsKey(coord))
                {
                    SpawnBlockAt(coord);
                }
            }
        }

        // Eliminar bloques lejanos
        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (var kvp in spawnedBlocks)
        {
            float distX = Mathf.Abs(kvp.Key.x - playerCoord.x);
            float distZ = Mathf.Abs(kvp.Key.y - playerCoord.y);
            if (distX > viewRadius + 1 || distZ > viewRadius + 1)
            {
                DestroyBlock(kvp.Key);
                toRemove.Add(kvp.Key);
            }
        }
        foreach (var key in toRemove)
        {
            spawnedBlocks.Remove(key);
        }
    }

    void SpawnBlockAt(Vector2Int coord)
    {
        Vector3 position = new Vector3(coord.x * blockSize, 0, coord.y * blockSize);
        GameObject terrainPrefab = terrainPrefabs[Random.Range(0, terrainPrefabs.Length)];
        GameObject block = Instantiate(terrainPrefab, position, Quaternion.identity);

        BlockData blockData = new BlockData(block);

        int objectsToSpawn = Random.Range(minObjectsPerBlock, maxObjectsPerBlock + 1);
        for (int i = 0; i < objectsToSpawn; i++)
        {
            GameObject objPrefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
            GameObject obj = Instantiate(objPrefab, block.transform);

            float localX = Random.Range(1f, blockSize - 1f);
            float localZ = Random.Range(1f, blockSize - 1f);

            
            float alturaY = 0.5f;
            Vector3 objLocalPos = new Vector3(localX, alturaY, localZ);
            obj.transform.localPosition = objLocalPos;

            blockData.spawnedObjects.Add(obj);
        }

        spawnedBlocks.Add(coord, blockData);
    }

    void DestroyBlock(Vector2Int coord)
    {
        if (spawnedBlocks.TryGetValue(coord, out BlockData blockData))
        {
            // Destruir objetos hijos
            foreach (GameObject obj in blockData.spawnedObjects)
            {
                Destroy(obj);
            }

            // Destruir el bloque
            Destroy(blockData.blockGameObject);
        }
    }

    Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / blockSize);
        int z = Mathf.RoundToInt(worldPos.z / blockSize);
        return new Vector2Int(x, z);
    }

    // Clase para guardar datos de cada bloque
    private class BlockData
    {
        public GameObject blockGameObject;
        public List<GameObject> spawnedObjects = new List<GameObject>();

        public BlockData(GameObject block)
        {
            blockGameObject = block;
        }
    }
}