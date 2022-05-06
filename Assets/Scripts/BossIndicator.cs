using UnityEngine;

public class BossIndicator : MonoBehaviour
{
    [SerializeField] private int size;
    [SerializeField] private ScriptableFloat level;
    [SerializeField] private GameObject bossRing;
    [SerializeField] private bool bossAlive;
    // Start is called before the first frame update
    void Start()
    {
        bossAlive = true;
        size = 30 + Mathf.FloorToInt(level.value * 5);
    }
    //TODO: Check if boss is alive
    public void Shrink()
    {
        if (bossAlive)
        {
            float ringOffset = Random.Range(0f, (size));
            Vector3 bossPosition = GameObject.Find("Boss").transform.position;

            //Limit ring size
            if (size - 5 > 5 && bossAlive)
            {
                size -= 5;
                //Set boss ring position close to boss' location, getting more accurate after every kill
                bossRing.transform.localScale = new Vector3(size, size, 1);
                bossRing.transform.position = new Vector3(bossPosition.x + ringOffset, bossPosition.y + ringOffset, -20);
            }
            //Only draw ring when desired size is reached
            if (size == 30)
            {
                bossRing.SetActive(true);

            }
        } else
        {
            bossRing.SetActive(false);
        }
    }

    public void Disable()
    {
        bossAlive = false;
    }
}
