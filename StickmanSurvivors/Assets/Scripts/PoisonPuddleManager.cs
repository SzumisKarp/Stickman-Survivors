using UnityEngine;

public class PoisonPuddleManager : MonoBehaviour
{
    public static PoisonPuddleManager Instance { get; private set; }

    // ten Transform b�dzie rootem dla wszystkich plam
    public Transform puddlesParent;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        // stw�rz pusty GameObject, je�li jeszcze go nie ma
        var go = new GameObject("PoisonPuddles");
        puddlesParent = go.transform;
    }

    /// <summary>
    /// Wywo�aj, by stworzy� now� plam�.
    /// </summary>
    public void SpawnPuddle(GameObject puddlePrefab, Vector3 position)
    {
        Instantiate(puddlePrefab,
                    position,
                    Quaternion.identity,
                    puddlesParent);
    }
}
