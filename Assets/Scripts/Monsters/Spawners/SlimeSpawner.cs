using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimeSpawner : MonsterSpawner
{
    protected override void Awake()
    {
        monsterName = "Slime";
        base.Awake();

        Slime slime = GameManager.Pool.Get<Monster>(monster, transform.position, transform.rotation, transform) as Slime;

        slime.Regen();
    }
}
