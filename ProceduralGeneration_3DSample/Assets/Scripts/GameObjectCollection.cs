using System.Collections.Generic;
using UnityEngine;

// 複数のキューブを管理するデータコンテナを作る
[CreateAssetMenu(fileName = "new GameObjectData", menuName = "Datas/GameObjectData")]
public class GameObjectCollection : ScriptableObject
{
    // 内部では通常のListで管理（インスペクタに表示される）
    [SerializeField]
    private List<GameObject> objects;

    // 外部からは配列でアクセスできる
    public GameObject[] Objects => objects.ToArray();
}