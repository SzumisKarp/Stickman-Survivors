using UnityEngine;
using UnityEngine.UI;

public class ExpBarUI : MonoBehaviour
{
    public Slider expSlider;

    void Start()
    {
        if (expSlider == null)
            expSlider = GetComponent<Slider>();
    }

    void Update()
    {
        if (LevelSystem.Instance != null)
        {
            expSlider.maxValue = LevelSystem.Instance.expToNext;
            expSlider.value = LevelSystem.Instance.exp;
        }
    }
}
