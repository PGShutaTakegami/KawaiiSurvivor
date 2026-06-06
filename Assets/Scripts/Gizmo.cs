using UnityEngine;

public class Gizmo : MonoBehaviour
{
    // 弦の始点と終点
    public Transform pointStart;
    public Transform pointEnd;

    // シーンビューにギズモを描画するメソッド
    private void OnDrawGizmosSelected()
    {
        // 始点と終点が設定されているか確認
        if (pointStart != null && pointEnd != null)
        {
            // ギズモの色を赤に設定
            Gizmos.color = Color.red;

            // 2点を結ぶ線（弦）を描画
            Gizmos.DrawLine(pointStart.position, pointEnd.position);
        }
    }
}