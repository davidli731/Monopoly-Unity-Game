using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieRoll : MonoBehaviour
{
    System.Random rnd = new System.Random();

    // Roll the die
    public int getDieResult(Transform[] dieFace)
    {
        // Reset all previous passes
        foreach (Transform die in dieFace)
        {
            die.position = new Vector3(die.position.x, die.position.y, -30);
        }
        // roll die effect
        int num = rnd.Next(1, 6);
        dieFace[num].position = new Vector3(dieFace[num].position.x, dieFace[num - 1].position.y, 0);
        return num + 1;
    }
}