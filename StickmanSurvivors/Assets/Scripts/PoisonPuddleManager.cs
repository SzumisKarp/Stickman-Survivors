using UnityEngine;

public class PoisonPuddleManager : MonoBehaviour
{
    public static PoisonPuddleManager Instance { get; private set; }

    // ten Transform bêdzie rootem dla wszystkich plam
    public Transform puddlesParent;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // stwórz pusty GameObject, jeœli jeszcze go nie ma
        var go = new GameObject("PoisonPuddles");
        puddlesParent = go.transform;
    }

    /// <summary>
    /// Wywo³aj, by stworzyæ now¹ plamê.
    /// </summary>
    public void SpawnPuddle(GameObject puddlePrefab, Vector3 position)
    {
        Instantiate(puddlePrefab,
                    position,
                    Quaternion.identity,
                    puddlesParent);
    }
}
