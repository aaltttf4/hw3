using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class OwlMove : MonoBehaviour
{


    [SerializeField]
    Transform[] possibleTargets;

    [SerializeField]
    float lerpTimeMax;

    [SerializeField]
    AnimationCurve idleWalkCurve;

    [SerializeField]
    float hungerStep;

    [SerializeField]
    float life = 50f;
    Transform target = null; 
    Vector3 startPos = Vector3.zero;
    float lerpTime;
    List<GameObject> owlCount = new List<GameObject>();

    enum OwlStates
    {
        hunting,
        nesting,
        dying,
        flying
    }
    OwlStates state = OwlStates.flying;

    float hungerTime;
    //hunger stat
    float hungerVal = 3;

    List<GameObject> allFood = new List<GameObject>();
    GameObject touchingObj;

    void Start()
    {
        FindAllFood();
        hungerTime = hungerStep;
    }

    void Update()
    {
        switch (state)
        {
            case OwlStates.flying: 
                RunIdle();
                break;
            case OwlStates.hunting:
                RunEat();
                break;
            case OwlStates.nesting:
                RunNest();
                break;
            case OwlStates.dying:
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

        StepNeeds();

        if (hungerVal <= 0)
        {
            target = null;
            state = OwlStates.hunting;
        }
        if (life <= 0)
        {
            target = null;
            state = OwlStates.dying;
        }
        if (owlCount.Count >= 2 && hungerVal > 2 )
        {
            state = OwlStates.nesting;
        }
    }

    void RunEat() {
        if(target == null){ 
            target = FindNearest(allFood);
            startPos = transform.position;
            lerpTime = 0;
        } else {
            transform.position = Move();
            if (touchingObj != null)
            {
                if (touchingObj.tag == "snake")
                {
                    allFood.Remove(touchingObj);
                    hungerVal = 3;
                    Destroy(touchingObj);
                    touchingObj = null;
                    target = null;
                    state = OwlStates.flying;
                }
                else if (touchingObj.tag == "mice")
                {
                    allFood.Remove(touchingObj);
                    hungerVal = 3;
                    Destroy(touchingObj);
                    touchingObj = null;
                    target = null;
                    state = OwlStates.flying;
                }
            }
        }
    }

    void RunDeath()
    {
        Destroy(gameObject);
    }

    void RunNest()
    {
        if (target == null)
        {
            target = FindNearest(owlCount);
            startPos = transform.position;
            lerpTime = 0;
        }
        else
        {
            transform.position = Move();
            if (touchingObj != null)
            { 
                if (touchingObj.tag == "nest")
                { 
                    touchingObj = null;
                    target = null;
                    state = OwlStates.flying;
                }
            }
        }
    }

    void StepNeeds(){
        hungerTime -= Time.deltaTime;
        life -= Time.deltaTime;
        if (hungerTime <= 0)
        {
            hungerVal--;
            hungerTime = hungerStep;
        }
    }

    void FindAllFood()
    {
        allFood.AddRange(GameObject.FindGameObjectsWithTag("snake"));
        allFood.AddRange(GameObject.FindGameObjectsWithTag("mice"));
    }

    Transform FindNearest(List<GameObject> objsToFind){
        float minDist = Mathf.Infinity; //setting the min dist to a big number
        Transform nearest = null; //tracks the obj closest to us
        for(int i = 0; i < objsToFind.Count; i++){ //loop through the objects we're checking
            float dist = Vector3.Distance(transform.position, objsToFind[i].transform.position); //check the dist b/t the spider and the current obj
            if(dist < minDist){ //if the dist is less than our currently tracked min dist
                minDist = dist; //set the min dist to the new dist
                nearest = objsToFind[i].transform; //set the nearest obj var to this obj
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

    void OnTriggerExit2D(Collider2D col){
        if(col != null) { //if we stop touching something 
            if(col.gameObject == touchingObj) touchingObj = null; //AND that thing is being tracked, clear the touching tracking var
        }
    }
}
