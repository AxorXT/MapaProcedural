using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSmoothTime = 0.1f;
    public Transform cameraTransform;
    public float jumpForce = 5f;

    private Vector3 direction;
    private float currentVelocity;
    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        // Dirección según el input
        direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Si hay movimiento
        if (direction.magnitude >= 0.1f)
        {
            // Ángulo hacia el que rotar basado en la cámara
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref currentVelocity, rotationSmoothTime);

            // Rotar jugador suavemente
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // Dirección en el mundo
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;

            // Movimiento con Translate
            transform.Translate(moveDir.normalized * speed * Time.deltaTime, Space.World);
        }

        // Salto
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void OnCollisionStay(Collision collision)
    {
        // Detectar si está en el suelo
        isGrounded = true;
    }

    void OnCollisionExit(Collision collision)
    {
        isGrounded = false;
    }
}

