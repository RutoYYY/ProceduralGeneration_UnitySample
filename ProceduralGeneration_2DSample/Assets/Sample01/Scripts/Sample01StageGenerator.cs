using UnityEngine;
using UnityEngine.Tilemaps;

public class Sample01StageGenerator : MonoBehaviour
{
    [SerializeField, Tooltip("生成するタイルデータ")]
    private TileCollection tileCollection;

    [SerializeField, Tooltip("生成する幅")]
    private int generateWidth;

    [SerializeField, Tooltip("生成する高さ")]
    private int generateHeight;

    [SerializeField, Tooltip("変化の激しさ"), Range(0f, 1f)]
    private float frequency;

    [SerializeField, Tooltip("シード値")]
    private uint seed;


    // インスペクター→スクリプト右クリックで項目が追加されます
    [ContextMenu("生成")]
    public void MakeTilemapStage()
    {
        // 自身のオブジェクトからTilemapを取得
        Tilemap tilemap = GetComponent<Tilemap>();

#if UNITY_EDITOR
        // Ctrl+Z対応
        UnityEditor.Undo.RegisterCompleteObjectUndo(tilemap, "Generate Tilemap");
#endif

        // タイルのクリア
        tilemap.ClearAllTiles();

        // タイルを設置
        for (int x = 0; x < generateWidth; x++)
        {
            for (int y = 0; y < generateHeight; y++)
            {
                // パーリンノイズから設置するタイルを選択
                float noise = Mathf.PerlinNoise(x * frequency + seed, y * frequency + seed);    // 0f〜1.0fを返す(ちょっと超えるかもしれない)
                noise = Mathf.Clamp01(noise);                                                   // 0f〜1.0fにする
                int length = tileCollection.tiles.Length;                                       // 登録しているタイルの長さ
                int tileNum = (int)(noise * length);                                            // 0〜{TileData.Length - 1}を返す

                // 設置するタイル座標
                Vector3Int pos = new Vector3Int(x, y, 0);
                // タイル設置
                tilemap.SetTile(pos, tileCollection.tiles[tileNum]);
            }
        }

#if UNITY_EDITOR
        // データの保存
        UnityEditor.EditorUtility.SetDirty(tilemap);
#endif
    }

    // インスペクター→スクリプト右クリックで項目が追加されます
    [ContextMenu("クリア")]
    public void StageAllClear()
    {
        // 自身のオブジェクトからTilemapを取得
        Tilemap tilemap = GetComponent<Tilemap>();

        // タイルのクリア
        tilemap.ClearAllTiles();
    }
}
