using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMainAreaScene : BaseScene
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

        if (exScene == "GameTitleScene" || exScene == "")
        {
            if (savePoints == null || savePoints.Count < 1)
            {
                player.transform.position = spawnPoints[0].position;
            }
            else
            {
                player.transform.position = savePoints[GameManager.Data.PlayerStatusData.savedSpawnPointIndex].position;
            }
        }
        else if (exScene == "Scenes/DungeonMaps/Corridor_Royal")
        {
            player.transform.position = spawnPoints[0].position;
        }
        else if (exScene == "Scenes/DungeonMaps/Corridor_BossArea")
        {
            player.transform.position = spawnPoints[1].position;
        }

        player.SetActive(true);

        progress = 1f;
    }
}
