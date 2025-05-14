using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.IO;

//テクスチャのインポート設定
public class Texture2DToTile : EditorWindow
{
    [MenuItem("Assets/Texture2D To Tile")]
    static void OpenWindow()
    {
        GetWindow<Texture2DToTile>("テクスチャインポート設定");
    }

    private enum WindowState
    {
        InputWindow,
        ResultWindow
    }
    private WindowState windowState = WindowState.InputWindow;
    private Object[] objects;
    Object obj;
    private int currentObjectIndex = 0;

    void OnEnable()
    {
        // 選択したオブジェクトを全て取得
        objects = Selection.objects;
        // テクスチャの初期設定
        TextureInitialSettings(objects);
    }

    void OnGUI()
    {
        obj = objects[currentObjectIndex];

        // オブジェクトの名前を表示
        GUILayout.Label(objects[currentObjectIndex].name);

        switch (windowState)
        {
            case WindowState.InputWindow:
                DisplayInputWindow();
                break;
            case WindowState.ResultWindow:
                DisplayResultWindow();
                break;
            default:
                break;
        }
    }

    private void DisplayInputWindow()
    {
        if (obj == null) return;

        if (obj is Texture2D texture)   //オブジェクトがテクスチャか確認する
        {
            //テクスチャの描画
            DrawTexture(texture);

            GUILayout.Space(30);

            //決定ボタン
            if (GUILayout.Button("作成"))
            {
                // タイルを作成
                MakeTileBase(obj);

                // テクスチャアセットの移動
                windowState = WindowState.ResultWindow;
            }
        }
        else
        {
            windowState = WindowState.ResultWindow;
        }
    }

    private void DisplayResultWindow()
    {
        if (obj == null) return;

        if (obj is Texture2D texture)
        {
            // テクスチャの描画
            DrawTexture(texture);

            GUILayout.Label("正常に作成できました。");
            if (currentObjectIndex + 1 < objects.Length)
            {
                if (GUILayout.Button("次へ"))
                {
                    currentObjectIndex++;
                    windowState = WindowState.InputWindow;
                }
            }
            else
            {
                GUILayout.Label("全て作成しました。");
            }
        }
        else
        {
            GUILayout.Label("選択したオブジェクトはテクスチャではありません。");
        }
    }

    private void DrawTexture(Texture2D texture)
    {
        //最大サイズ
        float maxWidth = 300f;
        float maxHeight = 300f;
        //アスペクト比を計算
        float aspectRatio = (float)texture.width / (float)texture.height;
        //幅と高さをスケーリングしてアスペクト比を維持
        float newWidth = maxWidth;
        float newHeight = newWidth / aspectRatio;
        //高さが最大値を超える場合、最大高さに合わせてスケーリング
        if (newHeight > maxHeight)
        {
            newHeight = maxHeight;
            newWidth = newHeight * aspectRatio;
        }
        //描画エリアのサイズをテクスチャのオリジナルサイズに合わせる
        Rect rect = GUILayoutUtility.GetRect(newWidth, newHeight, GUILayout.ExpandWidth(false), GUILayout.ExpandHeight(false));
        //テクスチャを描画
        GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, true);
    }

    //共通設定
    private void TextureInitialSettings(Object[] objects)
    {
        foreach (var obj in objects)
        {
            //設定を変更
            string path = AssetDatabase.GetAssetPath(obj);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;

            if (ti != null)
            {
                ti.textureType = TextureImporterType.Sprite;
                ti.textureCompression = TextureImporterCompression.Uncompressed;
                ti.spriteImportMode = SpriteImportMode.Single;
                ti.spritePixelsPerUnit = 1;
                ti.mipmapEnabled = false;
                ti.filterMode = FilterMode.Point;
            }

            //設定を反映
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            AssetDatabase.SaveAssets();  //保存を明示的に呼び出す
        }
    }

    //個別のテクスチャ設定
    private void MakeTileBase(Object obj)
    {
        // ObjectからSpriteへ変換
        string path = AssetDatabase.GetAssetPath(obj);
        Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        // SpriteからTileへ変換
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = sprite;

        // Tileをアセットとして保存
        string tilePath = Path.ChangeExtension(path, ".asset");
        AssetDatabase.CreateAsset(tile, tilePath);
        AssetDatabase.SaveAssets();
    }
}
