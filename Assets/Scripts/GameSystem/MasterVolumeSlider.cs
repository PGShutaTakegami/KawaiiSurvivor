using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Sliderの値をマスターボリュームに反映するスクリプト
/// </summary>
public class MasterVolumeSlider : MonoBehaviour
{
    /// <summary>
    /// 音量調整用Slider
    /// </summary>
    private Slider slider;

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Awake()
    {
        slider = GetComponent<Slider>();
    }

    /// <summary>
    /// 有効化時にSliderへ現在音量を反映
    /// </summary>
    private void OnEnable()
    {
        if (slider == null)
        {
            slider = GetComponent<Slider>();
        }

        if (slider == null)
        {
            return;
        }

        if (MasterVolumeManager.Instance != null)
        {
            slider.value = MasterVolumeManager.Instance.GetVolume();
        }

        slider.onValueChanged.RemoveListener(SetVolume);
        slider.onValueChanged.AddListener(SetVolume);
    }

    /// <summary>
    /// 無効化時にイベントを解除
    /// </summary>
    private void OnDisable()
    {
        if (slider != null)
        {
            slider.onValueChanged.RemoveListener(SetVolume);
        }
    }

    /// <summary>
    /// Sliderの値をマスターボリュームに反映
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        if (MasterVolumeManager.Instance == null)
        {
            AudioListener.volume = volume;
            return;
        }

        MasterVolumeManager.Instance.SetVolume(volume);
    }
}