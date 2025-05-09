using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform))]
public class ExplosiveOrbsUpgrade : MonoBehaviour
{
    public static ExplosiveOrbsUpgrade Instance { get; private set; }

    [Header("Orb Settings")]
    [Tooltip("Prefab should have a trigger collider + OrbController attached")]
    public GameObject orbPrefab;
    [Tooltip("Orbit radius in world units")]
    public float radius = 1.5f;
    [Tooltip("Base orbit speed in degrees/sec (level 1)")]
    public float baseOrbitSpeed = 120f;
    [Tooltip("Damage dealt on enemy contact per orb")]
    public int baseDamage = 1;

    [Header("Level Settings")]
    [Tooltip("Current upgrade level (1–3)")]
    public int currentLevel = 0;
    public int maxLevel = 3;

    private List<GameObject> _orbs = new List<GameObject>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    /// <summary>
    /// Call this when the player selects the Explosive Orbs upgrade.
    /// </summary>
    public void ApplyUpgrade()
    {
        if (currentLevel >= maxLevel) return;
        currentLevel++;
        RebuildOrbs();
    }

    private void RebuildOrbs()
    {
        // remove old orbs
        foreach (var o in _orbs) Destroy(o);
        _orbs.Clear();

        // spawn new orbs evenly spaced
        float angleStep = 360f / currentLevel;
        float speed = baseOrbitSpeed * SpeedMultiplierForLevel(currentLevel);

        for (int i = 0; i < currentLevel; i++)
        {
            float startAngle = i * angleStep;
            GameObject orb = Instantiate(orbPrefab, transform);
            var ctrl = orb.GetComponent<OrbController>();
            ctrl.Initialize(transform, radius, startAngle, speed, baseDamage);
            _orbs.Add(orb);
        }
    }

    private float SpeedMultiplierForLevel(int lvl)
    {
        switch (lvl)
        {
            case 2: return 1.25f;
            case 3: return 1.75f;
            default: return 1f;
        }
    }
}
