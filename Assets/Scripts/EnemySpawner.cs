using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    [SerializeField] float spawnrate;
    [SerializeField] Transform[] spawnpoints;
    [SerializeField] int numOfBlinks = 10;
    [SerializeField] float blinkDelayTime = 1f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemy());
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }
    IEnumerator SpawnEnemy()
    {
        while (true)
        {
            Transform randomSpawnpoint = spawnpoints[Random.Range(0, spawnpoints.Length - 1)];
            for (int i = 0; i <numOfBlinks; i++)
            {
                randomSpawnpoint.gameObject.GetComponent<SpriteRenderer>().enabled = true;
                yield return new WaitForSecondsRealtime(blinkDelayTime / numOfBlinks / 2);
                randomSpawnpoint.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                yield return new WaitForSecondsRealtime(blinkDelayTime / numOfBlinks / 2);
            }
            Instantiate(enemy, randomSpawnpoint.position, transform.rotation);
            yield return new WaitForSecondsRealtime(spawnrate-blinkDelayTime);
        }
    }
}
