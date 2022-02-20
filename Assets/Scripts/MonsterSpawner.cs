using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject[] monsterReference;
    private GameObject spawnedMonster;

    [SerializeField]
    private Transform leftPos, rightPos;

    private int randomIndex;
    private int randomSide;
    private int speedInc;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnMonsters());
    }

    IEnumerator SpawnMonsters() {
        while (true) {

            yield return new WaitForSeconds(Random.Range(1, 5));

            randomIndex = Random.Range(0, monsterReference.Length);
            randomSide = Random.Range(0, 2);
            spawnedMonster = Instantiate(monsterReference[randomIndex]);
            
            speedInc = (spawnedMonster.ToString().Contains("Skeleton")) ? Random.Range(3, 5) : (spawnedMonster.ToString().Contains("Goblin")) ? Random.Range(8,10) : (spawnedMonster.ToString().Contains("Mushroom")) ? Random.Range(6,8) : (spawnedMonster.ToString().Contains("Eye")) ? Random.Range(5,8) :  Random.Range(5,8);

            // left side
            if (randomSide == 0)
            {
                spawnedMonster.transform.position = leftPos.position;
                spawnedMonster.GetComponent<Monster>().speed = speedInc;
            }
            else
            {
                // right side
                spawnedMonster.transform.position = rightPos.position;
                spawnedMonster.GetComponent<Monster>().speed = -speedInc;
                spawnedMonster.transform.localScale = new Vector3(-spawnedMonster.transform.localScale.y, spawnedMonster.transform.localScale.y, spawnedMonster.transform.localScale.z);
            }
        } // while loop
    }
} // class






























