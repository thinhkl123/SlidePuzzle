using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void OnEnter(PlayerController player);

    public abstract void OnExit(PlayerController player);

    public abstract void OnUpdate(PlayerController player);
}
