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

        Transform spawnPoint;

        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position, out hit, 1.0f, 1);
        Goblin goblin = GameManager.Pool.Get(monster, hit.position, Quaternion.identity) as Goblin;
        //goblin.spawnPoint = hit.;

        goblin.Regen();
        
    }
}
