using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Ruch gracza")]
    [Tooltip("Prêdkoœæ poruszania siê gracza w jednostkach na sekundê")]
    public float speed = 5f;
    [Tooltip("Rigidbody2D gracza")]
    public Rigidbody2D myRigidbody;

    private Vector2 input;
    private Vector2 movement;

    void Awake()
    {
        // singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Pobranie komponentu, jeœli nie zosta³ ustawiony w Inspektorze
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();

        // Ustawienia fizyki dla top-downa
        myRigidbody.gravityScale = 0f;
        myRigidbody.freezeRotation = true;
        myRigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        myRigidbody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        // Wejœcie bez wyg³adzania – od razu -1, 0 lub +1
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Znormalizowane do sta³ej prêdkoœci w dowolnym kierunku
        movement = input.normalized * speed;
        // Ustawiamy prêdkoœæ – silnik fizyki zadba o kolizje
        myRigidbody.velocity = movement;
    }
}
