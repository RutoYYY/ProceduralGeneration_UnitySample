using UnityEngine;
using UnityEngine.Tilemaps;

public class Sample02StageManager : MonoBehaviour
{
    [SerializeField, Tooltip("障害物タイル")]
    private TileBase obstacleTile;

    [SerializeField, Tooltip("天井のタイルY座標")]
    private int tileCeilingPosY;
    [SerializeField, Tooltip("地面のタイルY座標")]
    private int tileGroundPosY;
    [SerializeField, Tooltip("設置をするタイルX座標")]
    private int tileSetPosX;
    [SerializeField, Tooltip("削除をするタイルX座標")]
    private int tileDeletePosX;

    [SerializeField, Tooltip("通れる穴の広さ"), Min(1)]
    private int gapSize;
    [SerializeField, Tooltip("障害物を設置する間隔"), Min(1)]
    private int setInterval;

    [SerializeField, Tooltip("移動スピード")]
    private float scrollSpeed;

    private Tilemap tilemap;
    private float accumulatedOffset = 0f;
    private float tileSize;
    private int intervalCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        // タイルマップの取得
        tilemap = GetComponent<Tilemap>();

        // タイルのサイズを取得
        tileSize = tilemap.layoutGrid.cellSize.x;
    }

    // Update is called once per frame
    void Update()
    {
        // ステージの移動
        float moveAmount = scrollSpeed * Time.deltaTime;        // 移動量の決定
        transform.position += Vector3.left * moveAmount;        // 移動
        accumulatedOffset += moveAmount;                        // 移動量を蓄積させる

        // 障害物設置判定
        if (accumulatedOffset >= tileSize)
        {
            accumulatedOffset -= tileSize;
            tileSetPosX++;
            tileDeletePosX++;
            SetGroundAndCeiling(tileSetPosX);

            intervalCount++;
            if (intervalCount >= setInterval)
            {
                intervalCount -= setInterval;

                // パイプを新しい列に追加
                SetPipe(tileSetPosX); // 画面右側に生成
                RemoveLeftmostColumn(tileDeletePosX); // 左端を消す
            }
        }
    }

    private void SetGroundAndCeiling(int x)
    {
        // 天井と地面のタイルを設置
        tilemap.SetTile(new Vector3Int(x, tileCeilingPosY, 0), obstacleTile);   // 天井
        tilemap.SetTile(new Vector3Int(x, tileGroundPosY, 0), obstacleTile);    // 地面
    }

    private void SetPipe(int x)
    {
        // 通れる穴の設定
        int gapY = Random.Range(tileGroundPosY + 1, tileCeilingPosY - gapSize + 1);     // 通れる穴の下端位置

        // 障害物の設置
        for (int y = tileGroundPosY + 1; y < tileCeilingPosY; y++)
        {
            if (y < gapY || y > gapY + gapSize - 1)
            {
                tilemap.SetTile(new Vector3Int(x, y, 0), obstacleTile);
            }
        }
    }

    private void RemoveLeftmostColumn(int x)
    {
        for (int y = tileGroundPosY; y <= tileCeilingPosY; y++)
        {
            tilemap.SetTile(new Vector3Int(x, y, 0), null);
        }
    }
}
