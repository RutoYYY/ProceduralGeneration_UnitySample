using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SampleScene03Generator : MonoBehaviour
{
    [SerializeField, Tooltip("生成する通路部屋のデータ")]
    private GameObjectCollection pathRoomCollection;
    [SerializeField, Tooltip("生成するスタート部屋のデータ")]
    private GameObjectCollection startRoomCollection;
    [SerializeField, Tooltip("生成するゴール部屋のデータ")]
    private GameObjectCollection goalRoomCollection;

    private const float roomWidth = 70f;
    [SerializeField, Min(1)]
    private int mapWidth;
    [SerializeField, Min(1)]
    private int sideRoomMin;
    [SerializeField, Min(1)]
    private int sideRoomMax;
    private int[,] map;

    // インスペクター→スクリプト右クリックで項目が追加されます
    [ContextMenu("生成")]
    public void MakeMap()
    {
        map = new int[mapWidth, mapWidth];

        // マップの削除
        ClearMap();

        // マップの生成
        GenerateMap();
    }

    private void GenerateMap()
    {
        // スタートとゴールをランダムに決定
        Vector2Int start = Vector2Int.zero;
        Vector2Int goal = Vector2Int.zero;
        while (start == goal)
        {
            start = new Vector2Int(Random.Range(0, map.GetLength(1)), Random.Range(0, map.GetLength(0)));
            goal = new Vector2Int(Random.Range(0, map.GetLength(1)), Random.Range(0, map.GetLength(0)));
        }
        map[start.y, start.x] = 1;
        map[goal.y, goal.x] = 2;

        Vector2Int current = start;

        // 最短経路を生成
        while (true)
        {
            if (current.x == goal.x)
            {
                current.y += current.y < goal.y ? 1 : -1;
            }
            else if (current.y == goal.y)
            {
                current.x += current.x < goal.x ? 1 : -1;
            }
            else
            {
                int dir = Random.Range(0, 2);
                switch (dir)
                {
                    case 0:
                        current.x += current.x < goal.x ? 1 : -1;
                        break;
                    case 1:
                        current.y += current.y < goal.y ? 1 : -1;
                        break;
                    default:
                        break;
                }
            }

            if (current == goal) break;

            map[current.y, current.x] = 3;
        }

        // 寄り道を生成
        int sideRoomNum = Random.Range(sideRoomMin, sideRoomMax + 1);
        for (int i = 0; i < sideRoomNum; i++)
        {
            while (true)
            {
                int x = Random.Range(0, map.GetLength(1));
                int y = Random.Range(0, map.GetLength(0));

                if (map[y, x] != 0) continue;

                if ((y + 1 < map.GetLength(0) && map[y + 1, x] != 0) ||
                (x + 1 < map.GetLength(1) && map[y, x + 1] != 0) ||
                (y - 1 >= 0 && map[y - 1, x] != 0) ||
                (x - 1 >= 0 && map[y, x - 1] != 0))
                {
                    map[y, x] = 3;
                    break;
                }
            }
        }

        // オブジェクトの生成
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                GameObject newObj;

                switch (map[y, x])
                {
                    case 1:
                        // スタート部屋
                        newObj = InstantiateObject(startRoomCollection.Objects[Random.Range(0, startRoomCollection.Objects.Length)], new Vector3(x * roomWidth, 0, y * roomWidth));

                        // ドア(通れる穴)の設置
                        DoorInstallation(x, y, newObj);
                        break;
                    case 2:
                        // ゴール部屋
                        newObj = InstantiateObject(goalRoomCollection.Objects[Random.Range(0, goalRoomCollection.Objects.Length)], new Vector3(x * roomWidth, 0, y * roomWidth));

                        // ドア(通れる穴)の設置
                        DoorInstallation(x, y, newObj);
                        break;
                    case 3:
                        // 通路部屋
                        newObj = InstantiateObject(pathRoomCollection.Objects[Random.Range(0, pathRoomCollection.Objects.Length)], new Vector3(x * roomWidth, 0, y * roomWidth));

                        // ドア(通れる穴)の設置
                        DoorInstallation(x, y, newObj);
                        break;
                    default:
                        break;
                }
            }
        }

#if UNITY_EDITOR
        // データの保存
        EditorUtility.SetDirty(this.gameObject);
#endif


        void DoorInstallation(int x, int y, GameObject gameObject)
        {
            if (y + 1 < map.GetLength(0) && map[y + 1, x] != 0)
            {
                gameObject.transform.Find("Wall_North").gameObject.SetActive(false);
                gameObject.transform.Find("Wall_North_Door").gameObject.SetActive(true);
            }
            if (x + 1 < map.GetLength(1) && map[y, x + 1] != 0)
            {
                gameObject.transform.Find("Wall_East").gameObject.SetActive(false);
                gameObject.transform.Find("Wall_East_Door").gameObject.SetActive(true);
            }
            if (y - 1 >= 0 && map[y - 1, x] != 0)
            {
                gameObject.transform.Find("Wall_South").gameObject.SetActive(false);
                gameObject.transform.Find("Wall_South_Door").gameObject.SetActive(true);
            }
            if (x - 1 >= 0 && map[y, x - 1] != 0)
            {
                gameObject.transform.Find("Wall_West").gameObject.SetActive(false);
                gameObject.transform.Find("Wall_West_Door").gameObject.SetActive(true);
            }
        }
    }

    [ContextMenu("マップのクリア")]
    private void ClearMap()
    {
        // 自身のTransformから全ての子オブジェクトを削除
        foreach (Transform child in transform)
        {
#if UNITY_EDITOR
            // ループ時に削除してしまうとエラーが発生するので、次のフレームで呼び出す
            EditorApplication.delayCall += () =>
            {
                DestroyImmediate(child.gameObject);
            };
#else
            // Destroy関数はフレームの終了時に行うので問題なし
            Destroy(child.gameObject);
#endif
        }

        // map配列のクリア
        for (int y = 0; y < map.GetLength(0); y++)
        {
            for (int x = 0; x < map.GetLength(1); x++)
            {
                map[y, x] = 0;
            }
        }
    }

    private GameObject InstantiateObject(GameObject gameObject, Vector3 position)
    {
        GameObject newObj = Instantiate(gameObject, position, transform.rotation);  // キューブを生成
        newObj.name = gameObject.name;                                              // (Clone)を表示させないようにする
        newObj.transform.SetParent(transform);                                      // ステージの子オブジェクトに設定
        // newObj.hideFlags = HideFlags.HideInHierarchy;                               // 大量に生成されるため、ヒエラルキ上では見えないようにする

        return newObj;
    }
}
