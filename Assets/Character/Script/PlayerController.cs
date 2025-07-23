using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //State Machine
    public PlayerBaseState currentState;
    public PlayerNormalState NormalState = new PlayerNormalState();

    public Rigidbody2D rb;
    public Animator animator;
    public PlayerInput input;
    public Collider2D col;

    [Header(" Collision ")]
    [SerializeField] private float collisionRadius = 0.25f;
    public Vector2 bottomOffest, rightOffset, leftOffset;

    //Grab Edge
    public float redXOffset, redYOffset, redXSize, redYSize;
    public float greenXOffset, greenYOffset, greenXSize, greenYSize;
    public bool greenBox, redBox;

    [Header(" Move ")]
    public float moveSpeed = 5f;
    public bool canMove = true;

    [Header(" Jump ")]
    public LayerMask groundLayer;
    public float jumpForce;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        input = GetComponent<PlayerInput>();
        animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        SwitchToState(NormalState);
    }

    private void Update()
    {
        currentState.OnUpdate(this);
    }

    public void SwitchToState(PlayerBaseState newState)
    {
        if (currentState != null)
        {
            currentState.OnExit(this);
        }

        currentState = newState;

        if (currentState != null)
        {
            currentState.OnEnter(this);
        }
    }

    public bool isGround()
    {
        //RaycastHit hit;
        return Physics2D.Raycast(transform.position, Vector2.down, 1.65f, groundLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Door"))
        {
            LoadingManager.instance.LoadScene("Level2");
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * 1.65f);

        Gizmos.color = Color.red;
    }
}
