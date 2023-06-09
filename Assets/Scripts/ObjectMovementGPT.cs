using UnityEngine;

public class ObjectMovementGPT : MonoBehaviour
{
    // Parameters of the linear system
    public float Kp = 2.0f;      // Process gain
    public float Taus = 1.0f;    // Second order time constant
    public float Zeta = 1.0f;    // Damping factor
    public Transform target;
    private Vector3 initialPosition;
    private Vector3 targetPosition => target.position;

    // Variables for the linear system
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;

    // Start is called before the first frame update
    void Start()
    {
        // Set initial and target positions
        initialPosition = transform.position;

        // Initialize variables
        position = initialPosition;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate error
        Vector3 error = targetPosition - position;

        // Calculate control input
        Vector3 controlInput = (Kp * error - 2 * Zeta * Taus * velocity) / (Taus * Taus);

        // Calculate acceleration
        acceleration = controlInput;

        // Update velocity and position using Euler's method
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;

        // Update object's position
        transform.position = position;
    }
}