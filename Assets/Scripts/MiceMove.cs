using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class MiceMove : MonoBehaviour
{


    [SerializeField]
    public Transform[] possibleTargets;
    [SerializeField] Sprite deadMice;

    [SerializeField]
    float lerpTimeMax;

    [SerializeField]
    AnimationCurve idleWalkCurve;
    [SerializeField]
    float deathTimer = 1;
    [SerializeField] 
    GameObject Mice;

    [SerializeField]
    float hungerStep;

    [SerializeField]
    float life = 5f;
    Transform target = null; 
    Vector3 startPos = Vector3.zero;
    float lerpTime;
    List<GameObject> miceCount = new List<GameObject>();
    float mateTimer = 2;

    enum MiceStates
    {
        eating,
        mating,
        dying,
        idle
    }
    [SerializeField]
    MiceStates state = MiceStates.idle;

    float hungerTime;
    //hunger stat
    float hungerVal = 2;

    List<GameObject> allFood = new List<GameObject>();
    GameObject touchingObj;

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
            case MiceStates.idle: 
                RunIdle();
                break;
            case MiceStates.eating:
                RunEat();
                break;
            case MiceStates.mating:
                RunMate();
                break;
            case MiceStates.dying:
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
            state = MiceStates.eating;
        }
        if (life <= 0)
        {
            target = null;
            state = MiceStates.dying;
        }
        /*if (mateTimer <= 0)
        {
            Debug.Log("mating");
            target = null;
            state = MiceStates.mating;
        }*/
    }

    void RunEat() {
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
                if (touchingObj.tag == "seed")
                {
                    allFood.Remove(touchingObj);
                    hungerVal = 3; 
                    Destroy(touchingObj);
                    touchingObj = null;
                    target = null;
                    state = MiceStates.idle;
                }
            }
        }
    }

    void RunDeath()
    {
        deathTimer -= Time.deltaTime;
        this.gameObject.GetComponent<SpriteRenderer>().sprite = deadMice;
        if (deathTimer <= 0)
        {
            Destroy(gameObject);
        }
    }

    void RunMate()
    {
        if (target == null)
        {
            target = FindNearest(miceCount);
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
                    GameObject newMice = Instantiate(Mice, transform.position, Quaternion.identity); 
                    touchingObj = null;
                    target = null;
                    mateTimer = 2;
                    state = MiceStates.idle;
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
        allFood.AddRange(GameObject.FindGameObjectsWithTag("seed"));
    }
    void FindAllMice()
    {
        miceCount.AddRange(GameObject.FindGameObjectsWithTag("mice"));
    }

    Transform FindNearest(List<GameObject> objsToFind){
        float minDist = Mathf.Infinity; //setting the min dist to a big number
        Transform nearest = null; //tracks the obj closest to us
        for(int i = 0; i < objsToFind.Count; i++){ //loop through the objects we're checking
        if (objsToFind[i] != null){
            float dist = Vector3.Distance(transform.position, objsToFind[i].transform.position); //check the dist b/t the spider and the current obj
            if (dist < minDist){ //if the dist is less than our currently tracked min dist
                minDist = dist; //set the min dist to the new dist
                nearest = objsToFind[i].transform; //set the nearest obj var to this obj
            }
        }
        }
        return nearest; //return the closest obj
    }

    Vector3 Move(){
        lerpTime += Time.deltaTime; //increase progress by delta time (time b/t frames)
        float percent = idleWalkCurve.Evaluate(lerpTime/lerpTimeMax); //from progress on curve
        Vector3 newPos = Vector3.LerpUnclamped(startPos, target.position, percent); //find current lerped position
        return newPos; //return the new position
    }

    void OnTriggerEnter2D(Collider2D col){
        if(col != null) touchingObj = col.gameObject; //if we touch something, set the var to whatever that thing is
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col != null)
        { //if we stop touching something 
            if (col.gameObject == touchingObj) touchingObj = null; //AND that thing is being tracked, clear the touching tracking var
        }
    }
}
