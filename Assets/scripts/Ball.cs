using UnityEngine;

public class Ball : MonoBehaviour
{
    public GameManager gameManager;
    public bool readyToShoot = false;

    private float maxY = 10f;
    private float minScale = 0.1f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Tap detection (mouse or touch)
        if (readyToShoot && Input.GetMouseButtonDown(0))
        {
            readyToShoot = false;
            gameManager.powerMeter.StopAndShoot();
        }

        if (gameManager.isShot)
        {
            // Perspective scale
            float distFactor = transform.position.y / maxY;
            float scale = Mathf.Lerp(0.5f, minScale, Mathf.Clamp01(distFactor));
            transform.localScale = new Vector3(scale, scale, 1f);

            // Check stop (miss)
            if (rb.velocity.magnitude < 0.2f && transform.position.y > 1f)
            {
                gameManager.Miss();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hole"))
        {
            gameManager.Win();
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}