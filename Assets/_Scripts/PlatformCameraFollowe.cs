using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformCameraFollowe : MonoBehaviour
{
    public Transform target;
    [SerializeField] private float smooth;
    [SerializeField] private Vector2 boundX;

    private Vector3 distance;
    private bool canFollow = false;

    private void Start()
    {

    }

    void Update()
    {
        if (target != null)
        {
            Follow();
        }

    }

    void Follow()
    {
        Vector3 currentPosition = transform.position;

        Vector3 newPosiotion = target.position;
        newPosiotion.y = 0f;
        newPosiotion.x = Mathf.Clamp(newPosiotion.x, boundX.x, boundX.y);
        newPosiotion.z = -10f;

        this.transform.position = Vector3.Lerp(currentPosition, newPosiotion, smooth * Time.deltaTime);
    }
}
