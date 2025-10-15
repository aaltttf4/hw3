using UnityEngine;

public class Egg : MonoBehaviour
{

    public enum EggTypes
    {
        spiderEgg,
        roachEgg,
        flyEgg
    }

    public EggTypes eggType;

    [SerializeField]
    Sprite spiderEggSprite, roachEggSprite, flyEggSprite;

    [SerializeField]
    GameObject spiderPrefab, roachPrefab, flyPrefab;

    float hatchTimer = 5f;
    float hatchFinish = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetComponent<SpriteRenderer>().sprite = SetupEgg();
    }

    // Update is called once per frame
    void Update()
    {
        hatchTimer -= Time.deltaTime;
        if (hatchTimer < hatchFinish)
        {
            Instantiate(HatchEgg(), transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    Sprite SetupEgg()
    {
        switch (eggType)
        {
            case EggTypes.spiderEgg:
                return spiderEggSprite;
            case EggTypes.roachEgg:
                return roachEggSprite;
            case EggTypes.flyEgg:
                return flyEggSprite;
            default:
                return null;
        }
    }

    GameObject HatchEgg()
    {
        switch (eggType)
        {
            case EggTypes.spiderEgg:
                return spiderPrefab;
            case EggTypes.roachEgg:
                return roachPrefab;
            case EggTypes.flyEgg:
                return flyPrefab;
            default:
                return null;
        }
    }
}
