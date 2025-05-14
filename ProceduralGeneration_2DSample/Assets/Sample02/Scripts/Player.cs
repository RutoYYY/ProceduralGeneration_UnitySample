using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Rigidbody2D))]         // Rigidbody2Dコンポーネントを必要とする
public class Player : MonoBehaviour
{
    [SerializeField, Tooltip("ジャンプする力")]
    private float jumpPower;
    private Rigidbody2D rigid;


    // Start is called before the first frame update
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // ジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    private void Jump()
    {
        rigid.velocity = new Vector2(rigid.velocity.x, 0);              // Y速度をリセット
        rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);    // ジャンプ
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);     // 障害物に当たったらステージを再ロード
    }
}
