using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static bool IsValid(this GameObject obj)
    {
        return obj != null && obj.activeInHierarchy;
    }

    public static bool IsValid(this Component component)
    {
        return component != null && component.gameObject.activeInHierarchy;
    }

    public static int IsMoving(this PlayerStateController playerStateController)
    {
        if (playerStateController.MoveDir.sqrMagnitude > 0.1)
            return 1;
        else return 0;
    }
}
