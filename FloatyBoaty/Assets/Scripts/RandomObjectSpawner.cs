using UnityEngine;
using System.Collections;
using EasyButtons;

public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject[] spawnObjects;
    public GameObject[] spawnPoints;
    public float RespawnTime = 5f;
    private GameObject item;

    void Start()
    {
        Spawn();
    }
    [Button]
    private void Spawn()
    {
        int spawnObjectIndex = Random.Range(0, spawnObjects.Length);
        int spawnPointIndex = Random.Range(0, spawnPoints.Length);

        item = GameObject.Instantiate(spawnObjects[spawnObjectIndex]) as GameObject;

        item.transform.parent = spawnPoints[spawnPointIndex].transform;
        item.transform.position = spawnPoints[spawnPointIndex].transform.position;
        item.transform.rotation = spawnPoints[spawnPointIndex].transform.rotation;
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(RespawnTime);
        Spawn();
    }
}