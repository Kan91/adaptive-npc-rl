using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform shootPoint; // Position, wo die Kugel spawnt
    public float shootForce = 10f;
    public float shootCooldown = 1f;

    
    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    
    private float nextShootTime;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true; // verhindert Umkippen
        Cursor.lockState = CursorLockMode.Locked; // Maus im Fenster fixieren
        Cursor.visible = false; // Mauszeiger unsichtbar
    }

    void Update()
    {
        Move();
        RotateWithMouse();
        if (Input.GetMouseButtonDown(0) && Time.time >= nextShootTime)
        {
            Shoot();
            nextShootTime = Time.time + shootCooldown;
        }
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        rb.MovePosition(rb.position + move.normalized * moveSpeed * Time.deltaTime);
    }
    void RotateWithMouse()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);
    }

    void Shoot()
    {
        if (projectilePrefab != null && shootPoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);
            Rigidbody projRb = proj.GetComponent<Rigidbody>();
            projRb.AddForce(shootPoint.forward * shootForce, ForceMode.Impulse);
        }
    }
}


