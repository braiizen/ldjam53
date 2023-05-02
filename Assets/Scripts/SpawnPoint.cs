using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] Object prefabToSpawn;
    public float respawnDelay = 10f;
    private bool isRespawning = false;

    private Vector3 initialPosition;
    private void Start()
    {
        initialPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !isRespawning)
        {
            StartCoroutine(RespawnCoroutine());
        }
    }

    private IEnumerator RespawnCoroutine()
    {
        isRespawning = true;
        yield return new WaitForSeconds(respawnDelay);
        Instantiate(prefabToSpawn, transform);
        isRespawning = false;
    }
}
