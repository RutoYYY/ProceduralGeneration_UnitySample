using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SampleScene02Generator : MonoBehaviour
{
    [SerializeField, Tooltip("生成するキューブデータ")]
    private GameObjectCollection cubeCollection;
    [SerializeField, Tooltip("草原地帯に生成するデータ")]
    private GameObjectCollection grassCollection;
    [SerializeField, Tooltip("草原地帯に生成するデータ")]
    private GameObjectCollection treeCollection;

    [SerializeField, Tooltip("生成する最大の高さ"), Min(1)]
    private int maxHeight;

    [SerializeField, Tooltip("変化の激しさ"), Range(0f, 1f)]
    private float frequency;

    [SerializeField, Tooltip("シード値")]
    private uint seed;

    [SerializeField, Tooltip("描画チャンク距離(半径)")]
    private uint drawChunkDistance;

    private Chunk[,] chunks;

    [SerializeField]
    private GameObject playerObj;

    private const int chunkWidth = 16;

    void Start()
    {
        InitChunks();
    }

    void Update()
    {
        CheckPlayerOutsideChunk();

        if (Input.GetKeyDown(KeyCode.R))
        {
            seed = seed + 1;
            RegenerateChunks();
        }
    }

    private void RegenerateChunks()
    {
        // 既存のオブジェクトを削除
        foreach (var chunk in chunks)
        {
            StartCoroutine(ClearChunkObjects(chunk));
        }

        // チャンクを再生成
        InitChunks();
    }

    private void InitChunks()
    {
        chunks = new Chunk[(int)drawChunkDistance * 2 + 1, (int)drawChunkDistance * 2 + 1];

        Vector2Int playerChunkPos = new Vector2Int(Mathf.FloorToInt(playerObj.transform.position.x / chunkWidth) * chunkWidth, Mathf.FloorToInt(playerObj.transform.position.z / chunkWidth) * chunkWidth);
        for (int x = 0; x < chunks.GetLength(0); x++)
        {
            for (int y = 0; y < chunks.GetLength(1); y++)
            {
                Vector2Int chunkPos = playerChunkPos + new Vector2Int((int)((x - drawChunkDistance) * chunkWidth), (int)((y - drawChunkDistance) * chunkWidth));
                chunks[x, y] = new Chunk(chunkPos, chunkPos + new Vector2Int(chunkWidth - 1, chunkWidth - 1));
                StartCoroutine(GenerateChunkObjects(chunks[x, y]));
            }
        }
    }

    private void CheckPlayerOutsideChunk()
    {
        int value = chunks[drawChunkDistance, drawChunkDistance].IsPlayerOutside(playerObj.transform.position);

        if (value == 0) return;

        switch (value)
        {
            case 1:
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    StartCoroutine(ClearChunkObjects(chunks[0, y]));
                }

                for (int x = 0; x < chunks.GetLength(0) - 1; x++)
                {
                    for (int y = 0; y < chunks.GetLength(1); y++)
                    {
                        chunks[x, y] = chunks[x + 1, y];
                    }
                }

                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    int x = chunks.GetLength(0) - 1;
                    Vector2Int chunkPos = chunks[x - 1, y].bottomLeft + new Vector2Int(chunkWidth - 1, 0);
                    chunks[x, y] = new Chunk(chunkPos, chunkPos + new Vector2Int(chunkWidth - 1, chunkWidth - 1));
                    StartCoroutine(GenerateChunkObjects(chunks[x, y]));
                }
                break;
            case 2:
                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    StartCoroutine(ClearChunkObjects(chunks[x, chunks.GetLength(1) - 1]));
                }

                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    for (int y = chunks.GetLength(1) - 1; y > 0; y--)
                    {
                        chunks[x, y] = chunks[x, y - 1];
                    }
                }

                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    int y = 0;
                    Vector2Int chunkPos = chunks[x, y + 1].bottomLeft - new Vector2Int(0, chunkWidth - 1);
                    chunks[x, y] = new Chunk(chunkPos, chunkPos + new Vector2Int(chunkWidth - 1, chunkWidth - 1));
                    StartCoroutine(GenerateChunkObjects(chunks[x, 0]));
                }
                break;
            case 3:
                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    StartCoroutine(ClearChunkObjects(chunks[chunks.GetLength(0) - 1, y]));
                }

                for (int x = chunks.GetLength(1) - 1; x > 0; x--)
                {
                    for (int y = 0; y < chunks.GetLength(1); y++)
                    {
                        chunks[x, y] = chunks[x - 1, y];
                    }
                }

                for (int y = 0; y < chunks.GetLength(1); y++)
                {
                    int x = 0;
                    Vector2Int chunkPos = chunks[x + 1, y].bottomLeft - new Vector2Int(chunkWidth - 1, 0);
                    chunks[x, y] = new Chunk(chunkPos, chunkPos + new Vector2Int(chunkWidth - 1, chunkWidth - 1));
                    StartCoroutine(GenerateChunkObjects(chunks[0, y]));
                }
                break;
            case 4:
                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    StartCoroutine(ClearChunkObjects(chunks[x, 0]));
                }

                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    for (int y = 0; y < chunks.GetLength(1) - 1; y++)
                    {
                        chunks[x, y] = chunks[x, y + 1];
                    }
                }

                for (int x = 0; x < chunks.GetLength(0); x++)
                {
                    int y = chunks.GetLength(1) - 1;
                    Vector2Int chunkPos = chunks[x, y - 1].bottomLeft + new Vector2Int(0, chunkWidth - 1);
                    chunks[x, y] = new Chunk(chunkPos, chunkPos + new Vector2Int(chunkWidth - 1, chunkWidth - 1));
                    StartCoroutine(GenerateChunkObjects(chunks[x, chunks.GetLength(1) - 1]));
                }
                break;
            default:
                break;
        }
    }

    private IEnumerator GenerateChunkObjects(Chunk chunk)
    {
        Dictionary<Material, List<CombineInstance>> materialCombineInstances = new Dictionary<Material, List<CombineInstance>>();

        for (int x = chunk.bottomLeft.x; x <= chunk.topRight.x; x++)
        {
            for (int z = chunk.bottomLeft.y; z <= chunk.topRight.y; z++)
            {
                // パーリンノイズから高さを算出
                float noise = Mathf.PerlinNoise(x * frequency + seed, z * frequency + seed);    // 0f〜1.0fを返す(ちょっと超えるかもしれない)
                noise = Mathf.Clamp01(noise);                                                   // 0f〜1.0fにする
                int y = (int)(noise * maxHeight);                                               // 高さを計算

                // 設置するキューブ座標
                Vector3 cubePos = new Vector3(x, y, z);

                // 高さからキューブを選択する
                GameObject generateObj;
                float proportion = (float)y / (float)maxHeight;
                if (proportion <= 0.3f)         // 海
                {
                    generateObj = cubeCollection.Objects[0];
                }
                else if (proportion <= 0.6f)    // 草原
                {
                    generateObj = cubeCollection.Objects[1];

                    // 草や木を生成するか決める
                    bool spawn = false;
                    spawn = TrySpawn(1.0f, treeCollection.Objects, cubePos + new Vector3(0, 0.5f, 0), chunk);
                    if (!spawn) TrySpawn(50.0f, grassCollection.Objects, cubePos + new Vector3(0, 0.5f, 0), chunk);
                }
                else                            // 岩山
                {
                    generateObj = cubeCollection.Objects[2];
                }

                Mesh mesh = generateObj.GetComponent<MeshFilter>().sharedMesh;
                Material mat = generateObj.GetComponent<Renderer>().sharedMaterial;

                if (!materialCombineInstances.ContainsKey(mat))
                    materialCombineInstances[mat] = new List<CombineInstance>();

                CombineInstance ci = new CombineInstance();
                ci.mesh = mesh;
                ci.transform = Matrix4x4.TRS(cubePos, Quaternion.identity, Vector3.one);
                materialCombineInstances[mat].Add(ci);
            }
        }

        foreach (var pair in materialCombineInstances)
        {
            CombineMeshes(chunk, pair.Key, pair.Value.ToArray());
        }

        yield break;
    }

    void CombineMeshes(Chunk chunk, Material material, CombineInstance[] combine)
    {
        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        GameObject newObj = new GameObject($"{chunk.bottomLeft.x}_{chunk.bottomLeft.y}");
        newObj.transform.parent = transform;
        newObj.transform.position = Vector3.zero;

        var meshFilter = newObj.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = combinedMesh;

        var meshRenderer = newObj.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = material;

        newObj.AddComponent<MeshCollider>();

        newObj.isStatic = true;

        chunk.objects.Add(newObj);
    }


    // 確率でオブジェクトを生成する
    private bool TrySpawn(float probability, GameObject[] gameObjects, Vector3 position, Chunk chunk)
    {
        // 指定した確率未満なら生成
        if (Random.value * 100f < probability)
        {
            //　生成するオブジェクトの選択
            GameObject gameObject = RandomObject(gameObjects);
            // 生成
            GameObject newObj = InstantiateObject(gameObject, position);
            chunk.objects.Add(newObj);

            return true;
        }

        return false;
    }

    // 確率でオブジェクトを生成する
    private GameObject RandomObject(GameObject[] gameObjects)
    {
        return gameObjects[Random.Range(0, gameObjects.Length)];
    }

    private GameObject InstantiateObject(GameObject gameObject, Vector3 position)
    {
        GameObject newObj = Instantiate(gameObject, position, transform.rotation);  // キューブを生成
        newObj.name = gameObject.name;                                              // (Clone)を表示させないようにする
        newObj.transform.SetParent(transform);                                      // ステージの子オブジェクトに設定
        newObj.hideFlags = HideFlags.HideInHierarchy;                               // 大量に生成されるため、ヒエラルキ上では見えないようにする

        return newObj;
    }

    private IEnumerator ClearChunkObjects(Chunk chunk)
    {
        foreach (var obj in chunk.objects)
        {
            if (obj == null) continue;

            Destroy(obj);
        }

        yield break;
    }
}

public class Chunk
{
    public Vector2Int bottomLeft;      // 左手前
    public Vector2Int topRight;        // 右奥

    public List<GameObject> objects;

    public Chunk(Vector2Int bottomLeftVec, Vector2Int topRightVec)
    {
        // チャンク座標の設定
        bottomLeft = bottomLeftVec;
        topRight = topRightVec;

        objects = new List<GameObject>();
    }

    public int IsPlayerOutside(Vector3 playerPos)
    {
        Vector3Int playerPosInt = Vector3Int.FloorToInt(playerPos);
        // Debug.Log(bottomLeft);

        if (playerPosInt.x > topRight.x)
        {
            return 1;
        }
        if (playerPosInt.z < bottomLeft.y)
        {
            return 2;
        }
        if (playerPosInt.x < bottomLeft.x)
        {
            return 3;
        }
        if (playerPosInt.z > topRight.y)
        {
            return 4;
        }

        return 0;
    }
}
