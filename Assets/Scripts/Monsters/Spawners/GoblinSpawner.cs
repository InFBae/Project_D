using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GoblinSpawner : MonsterSpawner
{
    protected override void Awake()
    {
        monsterName = "Goblin";
        base.Awake();

        Goblin goblin = GameManager.Pool.Get(monster, transform.position, transform.rotation) as Goblin;
        goblin.spawnPoint = transform;

        goblin.Regen();
        
    }
}
