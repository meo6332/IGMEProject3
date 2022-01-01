using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    [SerializeField]
    float maximumSpeed = 0.1f;

    [SerializeField]
    float minSpeed = 0.01f;

    [SerializeField]
    Vector3 acceleration = Vector3.zero;

    [SerializeField]
    float accelerationRate = 0.01f;

    [SerializeField]
    float decelerationValue = 0.2f;

    private Vector3 direction = Vector3.zero;

    private Vector3 aimDirection = Vector3.zero;

    [SerializeField]
    Vector3 playerPosition = Vector3.zero;

    [SerializeField]
    Vector3 velocity = Vector3.zero;

    // Fields needed for player input
    [SerializeField]
    Vector2 moveInput;

    [SerializeField]
    Vector2 fireInput;

    [SerializeField]
    GameManager manager;

    [SerializeField]
    private float initialShootTimer = 30.0f;

    private float shootTimer;

    // Start is called before the first frame update
    void Start()
    {
        moveInput = Vector2.zero;
        fireInput = Vector2.zero;

        playerPosition = transform.position;

        manager = FindObjectOfType<GameManager>();

        shootTimer = initialShootTimer;
    }

    // Update is called once per frame
    void Update()
    {
        // The y values for input correspond to the x axis
        // The x values for input correspond to the z axis

        CheckMove();

        CheckAim();

        // Normalize the direction
        direction = direction.normalized;

        aimDirection = aimDirection.normalized;

        if (direction != Vector3.zero)
        {
            acceleration = direction * accelerationRate;

            velocity = velocity + (acceleration * Time.deltaTime);
        }
        else
        {
            velocity = velocity * (1 - (decelerationValue * Time.deltaTime));

            if (velocity.magnitude < minSpeed)
            {
                velocity = Vector3.zero;
            }
        }

        if (aimDirection != Vector3.zero)
        {
            // Shoot a bullet in the direction requested
            if(shootTimer < 0)
            {
                // If the shoot timer is less than zero, then we're able to fire
                GameObject bullet = manager.GetBullet();

                Projectile current = bullet.GetComponent<Projectile>();
                current.fireBullet(aimDirection, playerPosition);

                // Reset the bullet timer
                shootTimer = initialShootTimer;
            }
        }

        velocity = Vector3.ClampMagnitude(velocity, maximumSpeed);

        // Draw the vehicle at this rotation
        playerPosition = playerPosition + (velocity * Time.deltaTime);

        transform.position = playerPosition;

        // Rotate the vehicle to match our direction
        if (aimDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(aimDirection);
        }
        else if(direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
        acceleration = Vector3.zero;

        shootTimer -= Time.deltaTime;

        // Debug.Log(shootTimer);
    }


    private void CheckMove()
    {
        // Checks the current state of the direction for movement and updates direction accordingly 
        if (moveInput.x > 0)
        {
            // Greater than zero, move in the positive z direction
            direction.z = 1.0f;
        }
        else if (moveInput.x < 0)
        {
            // Less than zero, move in the negative z direction
            direction.z = -1.0f;
        }

        if (moveInput.y > 0)
        {
            // Greater than zero, move in the NEGATIVE x direction
            direction.x = -1.0f;
        }
        else if (moveInput.y < 0)
        {
            // Less than zero, move in the POSITIVE x direction
            direction.x = 1.0f;
        }

        if (moveInput.x == 0 && moveInput.y == 0)
        {
            // Set the direction to 0
            direction = Vector3.zero;
        }
    }

    private void CheckAim()
    {
        // Checks the current state of the direction for shooting and updates aim accordingly 
        if (fireInput.x > 0)
        {
            // Greater than zero, move in the positive z direction
            aimDirection.z = 1.0f;
        }
        else if (fireInput.x < 0)
        {
            // Less than zero, move in the negative z direction
            aimDirection.z = -1.0f;
        }

        if (fireInput.y > 0)
        {
            // Greater than zero, move in the NEGATIVE x direction
            aimDirection.x = -1.0f;
        }
        else if (fireInput.y < 0)
        {
            // Less than zero, move in the POSITIVE x direction
            aimDirection.x = 1.0f;
        }

        if (fireInput.x == 0 && fireInput.y == 0)
        {
            // Set the direction to 0
            aimDirection = Vector3.zero;
        }
    }


    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        Debug.Log("Move Input value: " + moveInput);
    }

    public void OnFire(InputValue value)
    {
        fireInput = value.Get<Vector2>();
        Debug.Log("Fire Input Value: " + fireInput);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerPosition, playerPosition + direction);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(playerPosition, playerPosition + aimDirection);

    }
}
