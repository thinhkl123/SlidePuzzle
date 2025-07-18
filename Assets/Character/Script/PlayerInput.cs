using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public float horizontal;
    public float vertical;
    public bool isJump;

    private void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        isJump = Input.GetKeyDown(KeyCode.Space);
    }
}
