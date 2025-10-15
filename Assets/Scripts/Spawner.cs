using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Spawner : MonoBehaviour
{


    [SerializeField]
    List<GameObject> creaturePrefabs;

    [SerializeField]
    Transform topleftLimit, bottomrightLimit;

    [SerializeField]
    int startCreatureNum;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateStartCreatures();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateStartCreatures()
    {
        for (int i = 0; i < startCreatureNum; i++)
        {
            int rand = Random.Range(0, creaturePrefabs.Count);
            Vector3 startPos = new Vector3(Random.Range(topleftLimit.position.x, bottomrightLimit.position.x),
                                            Random.Range(bottomrightLimit.position.y, topleftLimit.position.y));
            GameObject newCreature = Instantiate(creaturePrefabs[rand], startPos, Quaternion.identity);
        }
    }
}
