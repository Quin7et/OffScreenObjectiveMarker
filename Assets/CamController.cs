using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamController : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 1f;
    [SerializeField] float flySpeed = 5f;

    float _cameraPitch;
    float _cameraYaw;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMouseLook();
        UpdateMovement();
        
    }

    void UpdateMouseLook()
    {
        // calculate new rotation
        _cameraPitch -= Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        _cameraPitch = Mathf.Clamp(_cameraPitch, -89f, 89f);
        _cameraYaw += Input.GetAxisRaw("Mouse X") * mouseSensitivity;

        // rotate view
        transform.localEulerAngles = Vector3.right * _cameraPitch + Vector3.up * _cameraYaw;
    }

    void UpdateMovement()
    {
        var translateVector = flySpeed * Time.deltaTime * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        transform.Translate(translateVector);

        float up = Input.GetKey(KeyCode.Space) ? 1 : 0;
        up += Input.GetKey(KeyCode.LeftControl) ? -1 : 0;
        transform.position += up * flySpeed * Time.deltaTime * Vector3.up;
    }
}
