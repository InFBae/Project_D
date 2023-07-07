using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorridorBossAreaScene : BaseScene
{
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] List<Transform> savePoints;
    [SerializeField] GameObject player;
    protected override IEnumerator LoadingRoutine(string exScene)
    {
        // UI 로딩
        GameManager.UI.SceneLoadInit();
        GameManager.Pool.SceneLoadInit();
        progress = 0.5f;
        yield return null;

        // 플레이어 위치 이동
        player.SetActive(false);

        if (exScene == "GameTitleScene")
        {
            if (savePoints == null)
            {
                player.transform.position = spawnPoints[0].position;
            }
            else
            {
                player.transform.position = savePoints[GameManager.Data.PlayerStatusData.savedSpawnPointIndex].position;
            }            
        }
        else if (exScene == "Scenes/DungeonMaps/Room_MainArea")
        {
            player.transform.position = spawnPoints[0].position;
        }
        else if (exScene == "Scenes/DungeonMaps/Room_BossArea")
        {
            player.transform.position = spawnPoints[1].position;
            GameManager.Sound.Play("DefaultBGM", SoundManager.Sound.BGM);
        }

        player.SetActive(true);

        progress = 1f;
    }
}


