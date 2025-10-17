using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    Transform topleftLimit, bottomrightLimit;

    [SerializeField]
    GameObject seed;

    [SerializeField]
    Transform[] miceTargets;
    [SerializeField]
    Transform[] snakeTargets;
    [SerializeField]
    Transform[] owlTargets;

    [SerializeField]
    GameObject mouse, owl, snake;

    float miceSpawnTimer = 5;
    float owlSpawnTimer = 20;
    float snakeSpawnTimer = 10;

    int startCreatureNum = 3;

    float seedTimer = 7;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        CreateStartCreatures();
    }

    // Update is called once per frame
    void Update()
    {
        seedTimer -= Time.deltaTime;
        miceSpawnTimer -= Time.deltaTime;
        owlSpawnTimer -= Time.deltaTime;
        snakeSpawnTimer -= Time.deltaTime;

        if (miceSpawnTimer <= 0)
        {
            Vector3 startPos = new Vector3(Random.Range(topleftLimit.position.x, bottomrightLimit.position.x),
                                            Random.Range(bottomrightLimit.position.y, topleftLimit.position.y));
            GameObject newCreature = Instantiate(mouse, startPos, Quaternion.identity);
            MiceMove mouseScript = newCreature.GetComponent<MiceMove>();
            mouseScript.possibleTargets = miceTargets;
            miceSpawnTimer = 5;
        }
        if (snakeSpawnTimer <= 0)
        {
            Vector3 startPos = new Vector3(Random.Range(topleftLimit.position.x, bottomrightLimit.position.x),
                                            Random.Range(bottomrightLimit.position.y, topleftLimit.position.y));
            GameObject newCreature = Instantiate(snake, startPos, Quaternion.identity);
            SnakeMove mouseScript = newCreature.GetComponent<SnakeMove>();
            mouseScript.possibleTargets = snakeTargets;
            snakeSpawnTimer = 10;
        }
        if (owlSpawnTimer <= 0)
        {
            Vector3 startPos = new Vector3(Random.Range(topleftLimit.position.x, bottomrightLimit.position.x),
                                            Random.Range(bottomrightLimit.position.y + 4, topleftLimit.position.y + 4));
            GameObject newCreature = Instantiate(owl, startPos, Quaternion.identity);
            OwlMove owlScript = newCreature.GetComponent<OwlMove>();
            owlScript.possibleTargets = owlTargets;
            owlSpawnTimer = 20;
        }
        if (seedTimer <= 0)
        {
            CreateStartCreatures(); 
            seedTimer = 7;
        }
    }

    void CreateStartCreatures()
    {
        for (int i = 0; i < startCreatureNum; i++)
        {
            Vector3 startPos = new Vector3(Random.Range(topleftLimit.position.x, bottomrightLimit.position.x),
                                            Random.Range(bottomrightLimit.position.y, topleftLimit.position.y));
            GameObject newCreature = Instantiate(seed, startPos, Quaternion.identity);
        }
    }
}
