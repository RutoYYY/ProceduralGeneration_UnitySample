using UnityEngine;
using UnityEngine.Tilemaps;

// 複数のタイルを管理するデータコンテナを作る
[CreateAssetMenu(fileName = "new TileData", menuName = "Tiles/TileData")]
public class TileCollection : ScriptableObject
{
    // 管理するタイル
    public TileBase[] tiles;
}