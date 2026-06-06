using UnityEngine;

/// <summary>
/// アタッチされたオブジェクトが有効な間、ゲーム時間を停止するスクリプト
/// </summary>
public class PauseWhileActive : MonoBehaviour
{
    /// <summary>
    /// オブジェクトが有効になった時に時間を停止
    /// </summary>
    private void OnEnable()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// 有効な間は時間停止を維持
    /// </summary>
    private void Update()
    {
        Time.timeScale = 0f;
    }

    /// <summary>
    /// オブジェクトが無効になった時に時間を再開
    /// </summary>
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}