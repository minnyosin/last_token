using UnityEngine;

public class TokenTest : MonoBehaviour
{

    public GameObject[] tokens;
    public int carriedTokens;
    public Transform machine;
    public GameObject exitDoor;
    public int depositedTokens;
    public int requiredTokens = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject coin in tokens)
        {
            if (coin == null || !coin.activeSelf)
            {
                continue;
            }

            Vector3 PlayerPosition = transform.position;
            Vector3 coinPosition = coin.transform.position;

            coinPosition.y = PlayerPosition.y;
            if (Vector3.Distance(PlayerPosition, coinPosition) <= 1.2f)
            {
                carriedTokens++;
                coin.SetActive(false);
                Debug.Log("Carrying: " + carriedTokens);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            TryDepositTokens();
        }


    }
    void TryDepositTokens()
    {
        if (machine == null || exitDoor == null)
        {
            return;
        }
        Vector3 machinePosition = machine.position;
        machinePosition.y = transform.position.y;

        if (Vector3.Distance(transform.position, machinePosition) > 2f)
        {
            return;
        }
        if (carriedTokens == 0)
        {
            return;
        }
        depositedTokens += carriedTokens;
        carriedTokens = 0;

        Debug.Log("Machine has " + depositedTokens + " / " + requiredTokens);

        if (depositedTokens >= requiredTokens)
        {
            exitDoor.SetActive(false);
        }
    }
}
