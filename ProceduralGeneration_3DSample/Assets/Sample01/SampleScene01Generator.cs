using UnityEngine;

public class SampleScene01Generator : MonoBehaviour
{
    [SerializeField, Tooltip("生成するTerrainのサイズ")]
    private Vector3 terrainSize;
    [SerializeField, Tooltip("変化の激しさ"), Range(0f, 1f)]
    private float frequency;
    [SerializeField, Tooltip("シード値")]
    private uint seed;

    private float perlinHeight;


    [ContextMenu("生成")]
    private void makeGround()
    {
        // TerrainDataを取得
        TerrainData terrainData = GetComponent<Terrain>().terrainData;

        // Terrainのサイズを設定
        terrainData.size = terrainSize;

        // 高さマップの初期化
        float[,] heights = new float[terrainData.heightmapResolution, terrainData.heightmapResolution];

        // 各ピクセルに対して高さを決定
        for (int x = 0; x < terrainData.heightmapResolution; x++)
        {
            for (int y = 0; y < terrainData.heightmapResolution; y++)
            {
                // パーリンノイズから高さのベースを算出
                perlinHeight = Mathf.PerlinNoise(x * frequency + seed, y * frequency + seed);

                // 高さ情報を格納
                heights[x, y] = perlinHeight;
            }
        }

        // Terrainの高さを反映
        terrainData.SetHeights(0, 0, heights);
    }
}
