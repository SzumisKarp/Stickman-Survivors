using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Ruch gracza")]
    public float speed = 5f;
    public Rigidbody2D myRigidbody;

    void Awake()
    {
        if (myRigidbody == null)
            myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");
        movement = new Vector2(moveX, moveY);
    }

    private Vector2 movement;

    void FixedUpdate()
    {
        Vector2 newPos = myRigidbody.position + movement * speed * Time.fixedDeltaTime;
        myRigidbody.MovePosition(newPos);
    }
}
