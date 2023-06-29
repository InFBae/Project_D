using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomKitchenScene : BaseScene
{
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] GameObject player;
    protected override IEnumerator LoadingRoutine(string exScene)
    {
        // 플레이어 위치 이동
        if (exScene == "GameTitleScene")
        {
            if (GameManager.Data.PlayerStatusData.savedSpawnPoint == null)
            {
                GameManager.Data.PlayerTransform = spawnPoints[0];
            }
            else
            {
                GameManager.Data.PlayerTransform = GameManager.Data.PlayerStatusData.savedSpawnPoint;
            }
        }
        else if (exScene == "Scenes/DungeonMaps/Corridor_Kitchen")
        {
            GameManager.Data.PlayerTransform = spawnPoints[0];
        }

        player.SetActive(false);

        player.transform.position = GameManager.Data.PlayerTransform.position;

        player.SetActive(true);


        progress = 0.5f;
        yield return null;
        // 씬의 몬스터 리스폰

        progress = 1f;

    }

    private void OnDestroy()
    {
        // TODO 모든 몬스터 삭제
    }
}
