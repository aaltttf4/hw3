using UnityEngine;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;

public class SnakeMove : MonoBehaviour
{


    [SerializeField]public Transform[] possibleTargets;

    [SerializeField] float lerpTimeMax;

    [SerializeField] GameObject Snake;

    [SerializeField]
    AnimationCurve idleWalkCurve;
    [SerializeField]float hungerStep;
    Transform target = null;
    Vector3 startPos = Vector3.zero;
    float lerpTime;
    [SerializeField]
    float deathTimer = 1;

    [SerializeField]float life = 20f;
    enum SnakeStates
    {
        eating,
        mating,
        dying,
        idling
    }

    [SerializeField] SnakeStates state = SnakeStates.idling;
    [SerializeField] float hungerTime;
    [SerializeField] Sprite deadSnake;
    //hunger stat
    [SerializeField] float hungerVal = 5;
    List<GameObject> allFood = new List<GameObject>();
    GameObject touchingObj;
    List<GameObject> snakeTag = new List<GameObject>();
    float mateTimer = 10;

    void Start()
    {
        FindAllFood();
        hungerTime = hungerStep; 
    }

    void Update()
    {
        StepNeeds();
        
        switch (state)
        {
            case SnakeStates.idling:
                RunIdle();
                break;
            case SnakeStates.eating:
                RunEat(); 
                break;
            case SnakeStates.mating:
                RunMate();
                break;
            case SnakeStates.dying:
                RunDeath();
                break;
            default:
                break;
        }
    }

    void RunIdle()
    {
        int newTarget = Random.Range(0, possibleTargets.Length);
        Vector3 currentPosition = transform.position;
        if (target == null)
        {
            target = possibleTargets[newTarget];
            startPos = transform.position;
            lerpTime = 0;
        }
        else
        {
            transform.position = Move();
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            newTarget = Random.Range(0, possibleTargets.Length);
            target = possibleTargets[newTarget];
            startPos = transform.position;
            lerpTime = 0;
        }

        if (hungerVal <= 0)
        {
            target = null;
            state = SnakeStates.eating;
        }
        if (life <= 0)
        {
            target = null;
            state = SnakeStates.dying;
        }
        if (snakeTag.Count >= 2 && hungerVal > 2 && mateTimer <= 0)
        {
            target = null;
            state = SnakeStates.mating;
        }
    }

    void RunEat()
    {
        if (target == null)
        {
            target = FindNearest(allFood);
            startPos = transform.position;
            lerpTime = 0;
        }
        else
        {
            transform.position = Move();
            if (touchingObj != null)
            { 
                if (touchingObj.tag == "mice")
                { 
                    allFood.Remove(touchingObj);
                    hungerVal = 5;
                    Destroy(touchingObj);
                    touchingObj = null;
                    target = null;
                    state = SnakeStates.idling;
                }
            }
        }
    }

    void RunDeath()
    {
        deathTimer -= Time.deltaTime;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = deadSnake;
        if (deathTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    void RunMate()
    {
        if (target == null)
        {
            target = FindNearest(snakeTag);
            startPos = transform.position;
            lerpTime = 0;
        }
        else
        {
            transform.position = Move();
            if (touchingObj != null)
            { 
                if (touchingObj.tag == "snake")
                {
                    GameObject newSnake = Instantiate(Snake, transform.position, Quaternion.identity);
                    SnakeMove snakeScript = newSnake.GetComponent<SnakeMove>();
                    snakeScript.possibleTargets = this.possibleTargets;
                    touchingObj = null;
                    target = null;
                    mateTimer = 10;
                    state = SnakeStates.idling;
                }
            }
        }
    }

    void StepNeeds(){
        hungerTime -= Time.deltaTime;
        life -= Time.deltaTime;
        mateTimer -= Time.deltaTime;
        if (hungerTime <= 0)
        {
            hungerVal--;
            hungerTime = hungerStep;
        }
    }

    void FindAllFood()
    {
        allFood.AddRange(GameObject.FindGameObjectsWithTag("mice"));
    }

    void FindSnake()
    {
        snakeTag.AddRange(GameObject.FindGameObjectsWithTag("snake"));
    }

    Transform FindNearest(List<GameObject> objsToFind){
        float minDist = Mathf.Infinity; 
        Transform nearest = null; 
        for(int i = 0; i < objsToFind.Count; i++){
            if (objsToFind[i] != null){
            float dist = Vector3.Distance(transform.position, objsToFind[i].transform.position);
            if(dist < minDist){ 
                minDist = dist;
                nearest = objsToFind[i].transform;
            }
            }
        }
        return nearest;
    }

    Vector3 Move(){
        lerpTime += Time.deltaTime;
        float percent = idleWalkCurve.Evaluate(lerpTime/lerpTimeMax);
        Vector3 newPos = Vector3.LerpUnclamped(startPos, target.position, percent);
        return newPos;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col != null) touchingObj = col.gameObject;
    }

    void OnTriggerExit2D(Collider2D col){
        if (col != null)
        { 
            if (col.gameObject == touchingObj) touchingObj = null;
        }
    }
}
