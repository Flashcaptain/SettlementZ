using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControler : MonoBehaviour
{
    public static PlayerControler _playerControler;

    [SerializeField]
    private Rigidbody _rigidbody;

    [SerializeField]
    private float _speed;

    [SerializeField]
    private float _rotationSpeed;

    void Start()
    {
        _playerControler = this;
        DontDestroyOnLoad(this);
    }

    void Update()
    {
        Movement();
        Rotation();
    }

    void Movement()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);

        _rigidbody.AddRelativeForce((movement * _speed) * Time.deltaTime);
    }

    void Rotation()
    {
        float rotateKeys = Input.GetAxis("Rotation");
        float rotate = rotateKeys;

        if (!GridSystem._gridSystem._inBuildMenu && !GridSystem._gridSystem._inInventory && !GridSystem._gridSystem._inManagementMenu)
        {
            float rotateMouse = Input.GetAxis("Mouse X");
            rotate = +rotateMouse;
        }
        transform.Rotate(new Vector3(0, (rotate) * _rotationSpeed, 0) * Time.deltaTime);
    }
}
