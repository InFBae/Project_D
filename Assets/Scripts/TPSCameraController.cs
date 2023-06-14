using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

public class TPSCameraController : MonoBehaviour
{
    [SerializeField] private Transform cameraRoot;
    [SerializeField] private Transform lookPoint;
    [SerializeField] private float mouseSensitivity;
    

    private Vector2 lookDelta;
    private float xRotation;
    private float yRotation;


    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
    }


    private void Update()
    {
        Rotate();
    }

    private void LateUpdate()
    {
        Look();
        ObstacleCheck();
    }

    private void Rotate()
    {
        lookPoint.position = Camera.main.transform.position + Camera.main.transform.forward * 20f;
        Vector3 point = lookPoint.position;
        point.y = transform.position.y;
        transform.LookAt(point);
    }

    private void Look()
    {
        yRotation += lookDelta.x * mouseSensitivity * Time.deltaTime;
        xRotation -= lookDelta.y * mouseSensitivity * Time.deltaTime;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraRoot.rotation = Quaternion.Euler(xRotation, yRotation, 0);    
    }

    public void ObstacleCheck()
    {
        Vector3 direction = (cameraRoot.position - Camera.main.transform.position).normalized;
        float distance = Vector3.Distance(Camera.main.transform.position, cameraRoot.position);

        RaycastHit[] hits = Physics.RaycastAll(Camera.main.transform.position, direction, distance, 1 << LayerMask.NameToLayer("Environment"));

        for (int i = 0; i < hits.Length; i++)
        {
            TransparentObject[] obj = hits[i].transform.GetComponentsInChildren<TransparentObject>();
            for (int j = 0; j < obj.Length; j++)
            {
                obj[j]?.BecomeTransparent();
            }
        }
        
    }

    private void OnLook(InputValue value)
    {
        lookDelta = value.Get<Vector2>();
    }
}
