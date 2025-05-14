using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(menuName = "Custom Tiles/Animation Tile")]
public class AnimationTile : TileBase
{
    public Sprite[] animatedSprites;
    public float minSpeed = 1f;
    public float maxSpeed = 1f;
    public float animationStartTime = 0f;

    public Tile.ColliderType colliderType = Tile.ColliderType.None;

    // タイルの静的な見た目（初期状態）
    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        // タイルの見た目
        tileData.sprite = animatedSprites != null && animatedSprites.Length > 0 ? animatedSprites[0] : null;
        // コライダータイプ
        tileData.colliderType = colliderType;
        // スクリプトで明示的に設定した内容をそのまま使う
        tileData.flags = TileFlags.LockAll;
    }

    // アニメーションをさせるための関数
    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        // アニメーションを必要としないならfalseを返す
        if (animatedSprites == null || animatedSprites.Length == 0)
            return false;

        // アニメーションさせるスプライトの設定
        tileAnimationData.animatedSprites = animatedSprites;
        // アニメーションスピード
        tileAnimationData.animationSpeed = Random.Range(minSpeed, maxSpeed);
        // アニメーションのスタート時間
        tileAnimationData.animationStartTime = animationStartTime;
        return true;
    }
}
