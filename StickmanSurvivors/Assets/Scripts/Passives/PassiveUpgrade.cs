using UnityEngine;

[CreateAssetMenu(menuName = "StickmanSurvivors/Passive Upgrade")]
public class PassiveUpgrade : ScriptableObject
{
    public string id;
    public string title;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Pricing (per level)")]
    public int[] price = { 50, 100, 200 };
    public int maxLevel = 3;
}
