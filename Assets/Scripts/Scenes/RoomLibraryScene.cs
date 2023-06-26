using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomLibraryScene : BaseScene
{
    [SerializeField] List<Transform> spawnPoints;
    [SerializeField] GameObject player;
    protected override IEnumerator LoadingRoutine(string exScene)
    {
        // �÷��̾� ��ġ �̵�
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
        else if (exScene == "Corridor_Kitchen")
        {
            GameManager.Data.PlayerTransform = spawnPoints[1];
        }
        else if (exScene == "Corridor_Royal")
        {
            GameManager.Data.PlayerTransform = spawnPoints[2];
        }
        
        player.transform.position = GameManager.Data.PlayerTransform.position;
        player.transform.rotation = GameManager.Data.PlayerTransform.rotation;
        player.SetActive(true);

        progress = 0.5f;
        yield return null;
        // ���� ���� ������

        progress = 1f;
        
    }

    private void OnDestroy()
    {
        // TODO ��� ���� ����
    }
}
