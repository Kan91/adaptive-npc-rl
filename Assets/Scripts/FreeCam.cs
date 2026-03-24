using UnityEngine;

public class FreeCam : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float lookSpeed = 2f;

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        float moveZ = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        float moveY = 0;

        if (Input.GetKey(KeyCode.E)) moveY += moveSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.Q)) moveY -= moveSpeed * Time.deltaTime;

        transform.Translate(new Vector3(moveX, moveY, moveZ));

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        transform.Rotate(Vector3.up, mouseX, Space.World);
        transform.Rotate(Vector3.left, mouseY);
    }
}
