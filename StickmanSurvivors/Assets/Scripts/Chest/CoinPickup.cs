using UnityEngine;

/// <summary> MonoBehaviour dla pojedynczej monety (Trigger2D). </summary>
[RequireComponent(typeof(Collider2D))]
public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        CurrencySystem.Instance.AddCoins(1);
        Destroy(gameObject);
    }
}
