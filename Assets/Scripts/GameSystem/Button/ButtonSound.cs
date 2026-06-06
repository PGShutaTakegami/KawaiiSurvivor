using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ボタンを押した時に指定したAudioClipを再生するスクリプト
/// </summary>
public class ButtonSound : MonoBehaviour
{
    [Header("サウンド設定")]
    /// <summary>
    /// 再生するサウンド
    /// </summary>
    public AudioClip soundClip;

    /// <summary>
    /// 音量
    /// </summary>
    public float volume = 1f;

    /// <summary>
    /// Buttonコンポーネント
    /// </summary>
    private Button button;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    /// <summary>
    /// 有効化時にクリックイベントを登録
    /// </summary>
    private void OnEnable()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (button != null)
        {
            button.onClick.RemoveListener(PlaySound);
            button.onClick.AddListener(PlaySound);
        }
    }

    /// <summary>
    /// 無効化時にクリックイベントを解除
    /// </summary>
    private void OnDisable()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(PlaySound);
        }
    }

    /// <summary>
    /// ボタンを押した時にサウンドを再生
    /// </summary>
    public void PlaySound()
    {
        if (soundClip == null)
        {
            return;
        }

        Vector3 playPosition = Vector3.zero;

        if (Camera.main != null)
        {
            playPosition = Camera.main.transform.position;
        }

        AudioSource.PlayClipAtPoint(soundClip, playPosition, volume);
    }
}