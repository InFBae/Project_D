using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    PlayerStatusController playerStatusController;
    CollectedItemSceneUI collectedItemSceneUI;

    private void Awake()
    {
        playerStatusController = GetComponentInParent<PlayerStatusController>();
        collectedItemSceneUI = GetComponentInChildren<CollectedItemSceneUI>();
    }
    private void OnTriggerEnter(Collider other)
    {
        ICollectable collectable = other.GetComponent<ICollectable>();
        if (collectable != null)
        {
            collectable.Collect(playerStatusController);
            Destroy(other.gameObject);
        }
        
    }

}
