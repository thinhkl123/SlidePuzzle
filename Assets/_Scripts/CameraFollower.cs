using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smooth;

    private Vector3 distance;
    private bool canFollow = false;

    private void Awake()
    {
        //distance = target.position - transform.position;
    }

    private void Start()
    {
        
    }

    void Update()
    {
        if (target == null)
        {
            target = GameObject.Find("Player(Clone)").transform;
        }

        Follow();

    }

    void Follow()
    {
        Vector3 currentPosition = transform.position;

        Vector3 newPosiotion = target.position;
        newPosiotion.z = -10f;

        this.transform.position = Vector3.Lerp(currentPosition, newPosiotion, smooth * Time.deltaTime);
    }
}
