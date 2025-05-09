using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Maintains a pool of homing drones that automatically respawn after they explode.
/// Attach to an empty GameObject parented under the Player.
/// </summary>
public class FlyingDroneUpgrade : MonoBehaviour
{
    public static FlyingDroneUpgrade Instance { get; private set; }

    [Header("Setup")]
    public GameObject dronePrefab;
    public Transform player;                   // optional – grabbed automatically if left null
    public float droneSpeed = 8f;

    [Header("Current level (1‒3)")]
    public int currentLevel = 0;
    public int maxLevel = 3;

    // level tables
    private readonly int[] droneCount = { 3, 4, 5 };
    private readonly int[] droneDamage = { 1, 2, 3 };
    private readonly float[] respawnDelay = { 4f, 3.5f, 4f };

    private readonly List<DroneController> _active = new List<DroneController>();

    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;

        if (player == null && PlayerController.Instance != null)
            player = PlayerController.Instance.transform;
    }

    public void ApplyUpgrade()
    {
        if (currentLevel >= maxLevel) return;
        currentLevel++;
        RebuildFleet();
    }

    private void RebuildFleet()
    {
        // kill existing drones
        foreach (var d in _active)
            if (d) Destroy(d.gameObject);
        _active.Clear();

        // spawn new set
        for (int i = 0; i < droneCount[currentLevel - 1]; i++)
            SpawnDrone();
    }

    private void SpawnDrone()
    {
        if (!dronePrefab || !player) return;

        var go = Instantiate(dronePrefab,
                             player.position + (Vector3)Random.insideUnitCircle * .5f,
                             Quaternion.identity,
                             transform);               // parented under upgrade GO
        var ctrl = go.GetComponent<DroneController>();
        ctrl.Initialize(droneDamage[currentLevel - 1],
                        droneSpeed,
                        () => StartCoroutine(RespawnRoutine()));
        _active.Add(ctrl);
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay[currentLevel - 1]);
        SpawnDrone();
    }
}
