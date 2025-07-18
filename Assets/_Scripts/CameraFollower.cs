using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using CustomUtils;

public class CameraFollower : SingletonMono<CameraFollower>
{
    public Transform target;
    [SerializeField] private float smooth;

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
        newPosiotion.z = -10f;

        this.transform.position = Vector3.Lerp(currentPosition, newPosiotion, smooth * Time.deltaTime);
    }
}
