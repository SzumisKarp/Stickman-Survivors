using System.Collections;
using UnityEngine;

/// <summary>
/// Skrzynia – po ‘otwarciu’ instancjonuje losową liczbę monet
/// i ustawia im początkowy pęd, żeby “wyskoczyły”.
/// </summary>
[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class Chest : MonoBehaviour
{
    [Header("Drop ustawienia")]
    [Tooltip("Prefab pojedynczej monety (z CoinPickup.cs)")]
    public GameObject coinPrefab;
    public int minCoins = 5;
    public int maxCoins = 15;
    public float scatterForce = 2f;

    [Header("Opcjonalne")]
    public Transform coinParent;

    private bool _opened = false;

    // Wywołuj np. z animacji / OnTrigger / OnMouseDown
    public void Open()
    {
        if (_opened) return;
        _opened = true;

        int amount = Random.Range(minCoins, maxCoins + 1);
        StartCoroutine(SpawnCoins(amount));

        // TODO: dźwięk / animacja otwierania
        GetComponent<SpriteRenderer>().color = Color.gray;
    }
    void Awake()
    {
        //automatycznie wyszukaj "Coins", jeśli nie przypięto ręcznie
        if (coinParent == null)
        {
            var go = GameObject.Find("Coins");
            if (go != null) coinParent = go.transform;
        }
    }

    private IEnumerator SpawnCoins(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            var go = Instantiate(coinPrefab, transform.position, Quaternion.identity, coinParent);
            var rb = go.GetComponent<Rigidbody2D>();   // opcjonalnie
            if (rb != null)
            {
                Vector2 dir = Random.insideUnitCircle.normalized;
                rb.AddForce(dir * scatterForce, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(0.02f);   // drobny odstęp dla efektu
        }

        // Usuń pustą skrzynię
        Destroy(gameObject);
    }

    // Przykładowe otwarcie po kolizji z graczem
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Player"))
            Open();
    }
}
