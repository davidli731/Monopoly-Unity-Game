using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Money : MonoBehaviour
{
    // Transfer money (pay rent) from one player to another
    public void TransferCash(int[] Cash, int from, int to, int amount)
    {
        Cash[from] -= amount;
        Cash[to] += amount;
    }

    // Check for negative cash
    public bool CheckForNegativeCash(bool playerTurn, int Cash)
    {
        if (playerTurn && Cash < 0) return true;
        return false;
    }
}
