using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

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

    public static bool IsLookingTarget(this Monster monster)
    {
        Vector3 targetDir = (monster.Target.transform.position - monster.transform.position).normalized;

        if (Vector3.Dot(monster.transform.forward, targetDir) > 0.99f)
        {
            return true;
        }
        else return false;
    }
}
