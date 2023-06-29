using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerItemCollector : MonoBehaviour
{
    PlayerStatusController playerStatusController;

    private void Awake()
    {
        playerStatusController = GetComponentInParent<PlayerStatusController>();
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
