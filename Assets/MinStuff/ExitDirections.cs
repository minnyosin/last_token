using UnityEngine;
using TMPro;

public class ExitDirections : MonoBehaviour
{
    public TestTokenMachine machine;
    public TextMeshProUGUI instructions;
    public TextMeshPro terminalText;
    public Transform player;
    public Transform exit;
    int previousTokens;
    float signalTime;

    void Update()
    {
        int tokens = player.GetComponent<PlayerInteractor>().carriedTokens;
        if (tokens > previousTokens) { signalTime = 3; }
        previousTokens = tokens;
        signalTime -= Time.deltaTime;
        int distance = Mathf.RoundToInt(Vector3.Distance(player.position,exit.position));
        terminalText.text = "EXIT TERMINAL\n" + machine.DepositedTokens + " / 4 TOKENS\n[E] DEPOSIT";
        if (machine.ExitReady)
        {
            instructions.text = "EXIT OPEN - FOLLOW GREEN MARKERS\nEXIT: " + distance + " m";
            terminalText.text = "EXIT OPEN";
        }
        else
        {
            instructions.text = "COLLECT 4 TOKENS - RETURN TO GREEN EXIT\nEXIT: " + distance + " m";
            if (signalTime > 0)
            {
                instructions.text = "TOKEN SIGNAL - NEARBY ROBOTS INVESTIGATING\nKEEP MOVING!";
            }
        }
    }
}
