using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNormalState : PlayerBaseState
{
    public override void OnEnter(PlayerController player)
    {
        
    }

    public override void OnExit(PlayerController player)
    {
        player.rb.velocity = Vector2.zero;
        player.animator.SetFloat("Speed", 0f);
        player.animator.SetBool("Fall", false);
        player.animator.SetBool("Jump", false);
    }

    public override void OnUpdate(PlayerController player)
    {
        Move(player);
        Jump(player);
    }

    private void Move(PlayerController player)
    {
        player.rb.velocity = new Vector2(player.input.horizontal * player.moveSpeed, player.rb.velocity.y);

        if (player.isGround())
        {
            player.animator.SetFloat("Speed", Mathf.Abs(player.input.horizontal));
        }
        else
        {
            player.animator.SetFloat("Speed", 0f);
        }

        if (player.input.horizontal < 0)
        {
            player.transform.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (player.input.horizontal > 0)
        {
            player.transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    public void Jump(PlayerController player)
    {
        if (player.rb.velocity.y > 0)
        {
            player.animator.SetBool("Jump", true);
            player.animator.SetBool("Fall", false);
        }
        else
        {
            player.animator.SetBool("Jump", false);
            player.animator.SetBool("Fall", true);
        }

        if (player.isGround()) player.animator.SetBool("Fall", false);

        if (player.input.isJump && player.isGround())
        {
            player.animator.SetTrigger("StartJump");
            player.rb.velocity = new Vector2(player.rb.velocity.x, player.jumpForce);
        }
    }
}
