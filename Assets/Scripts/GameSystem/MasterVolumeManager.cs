using UnityEngine;

/// <summary>
/// 全シーン共通のマスターボリュームを管理するスクリプト
/// </summary>
public class MasterVolumeManager : MonoBehaviour
{
    /// <summary>
    /// MasterVolumeManagerの共有インスタンス
    /// </summary>
    public static MasterVolumeManager Instance;

    /// <summary>
    /// 音量保存用キー
    /// </summary>
    private const string VolumeKey = "MasterVolume";

    /// <summary>
    /// 現在のマスターボリューム
    /// </summary>
    public float masterVolume = 1f;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        masterVolume = PlayerPrefs.GetFloat(VolumeKey, 1f);
        AudioListener.volume = masterVolume;
    }

    /// <summary>
    /// マスターボリュームを設定
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        AudioListener.volume = masterVolume;

        PlayerPrefs.SetFloat(VolumeKey, masterVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 現在のマスターボリュームを取得
    /// </summary>
    /// <returns></returns>
    public float GetVolume()
    {
        return masterVolume;
    }
}