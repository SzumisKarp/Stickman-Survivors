using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class PoisonPuddle : MonoBehaviour
{
    [Header("Poison Settings")]
    [Tooltip("Ile sekund ¿yje plama")]
    public float duration = 5f;
    [Tooltip("Ile obra¿eñ zadaje jedna plama")]
    public int damage = 1;
    [Tooltip("Co ile sekund zadaje obra¿enia")]
    public float damageInterval = 1f;

    private Coroutine _damageRoutine;

    void Start()
    {
        // automatycznie zniszcz plamê po czasie
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var ph = other.GetComponent<PlayerHealth>();
        if (ph != null && _damageRoutine == null)
        {
            // od razu zadaj dmg i uruchom coroutine
            ph.TakeDamage(damage);
            _damageRoutine = StartCoroutine(DamageOverTime(ph));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        var ph = other.GetComponent<PlayerHealth>();
        if (ph != null && _damageRoutine != null)
        {
            StopCoroutine(_damageRoutine);
            _damageRoutine = null;
        }
    }

    private IEnumerator DamageOverTime(PlayerHealth ph)
    {
        // pêtlij dopóki jesteœmy w triggerze
        while (true)
        {
            yield return new WaitForSeconds(damageInterval);
            ph.TakeDamage(damage);
        }
    }
}
