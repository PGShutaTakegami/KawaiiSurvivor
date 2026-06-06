using UnityEngine;

/// <summary>
/// 経験値オーブの吸い込み・経験値加算・削除を管理するスクリプト
/// </summary>
public class XpController : MonoBehaviour
{
    [Header("設定")]
    /// <summary>
    /// このアイテムで獲得できる経験値量
    /// </summary>
    public int xpValue = 1;

    /// <summary>
    /// プレイヤーに吸い込まれる速度
    /// </summary>
    public float moveSpeed = 8f;

    /// <summary>
    /// 吸い込まれる対象のプレイヤー
    /// </summary>
    private Transform targetPlayer;

    /// <summary>
    /// 吸い込み中かどうか
    /// </summary>
    private bool isBeingAttracted = false;

    /// <summary>
    /// 吸い込み中ならプレイヤーへ移動
    /// </summary>
    private void Update()
    {
        if (isBeingAttracted && targetPlayer != null)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPlayer.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    /// <summary>
    /// ItemAreaで吸い込み開始、GetItemで経験値獲得
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("ItemArea"))
        {
            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                targetPlayer = player.transform;
                isBeingAttracted = true;
            }
        }

        if (collision.CompareTag("GetItem"))
        {
            PlayerController player = collision.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                player.AddXp(xpValue);
                Destroy(gameObject);
            }
        }
    }
}