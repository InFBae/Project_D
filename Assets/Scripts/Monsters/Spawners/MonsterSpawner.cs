using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    protected string monsterName;
    protected Monster monster;

    protected virtual void Awake()
    {
        monster = GameManager.Resource.Load<Monster>($"Monster/{monsterName}");
    }
}
