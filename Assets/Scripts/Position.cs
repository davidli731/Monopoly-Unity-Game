using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct Positions
{
    public string PositionName;
    public float X;
    public float Y;
    public float Z;

    // Constructor
    public Positions(string positionName, float x, float y, float z)
    {
        this.PositionName = positionName;
        this.X = x;
        this.Y = y;
        this.Z = z;
    }
}

public class Position : MonoBehaviour
{
    // Hardcoded centre positions for each spot on the spot
    public Positions[] Pos = new Positions[]
    {
        new Positions ( "GO", 6.58f, -6.5f, 0 ),
        new Positions("Mediteranean Avenue", 4.9f, -6.5f, 0),
        new Positions("Community Chest", 3.65f, -6.5f, 0),
        new Positions("Baltic Avenue", 2.43f, -6.5f, 0),
        new Positions("Income Tax", 1.15f, -6.5f, 0),
        new Positions("Reading Railroad", -0.05f, -6.5f, 0),
        new Positions("Oriental Avenue", -1.24f, -6.5f, 0),
        new Positions("Chance", -2.46f, -6.5f, 0),
        new Positions("Vermont Avenue", -3.66f, -6.5f, 0),
        new Positions("Connecticut Avenue", -4.88f, -6.5f, 0),
        new Positions("Jail", -6.45f, -6.5f, 0),
        new Positions("St. Charles Place", -6.45f, -4.81f, 0),
        new Positions("Electric Company", -6.45f, -3.52f, 0),
        new Positions("States Avenue", -6.45f, -2.34f, 0),
        new Positions("Virginia Avenue", -6.45f, -1.15f, 0),
        new Positions("Pennsylvania Railroad", -6.45f, 0.08f, 0),
        new Positions("St. James Place", -6.45f, 1.28f, 0),
        new Positions("Community Chest", -6.45f, 2.53f, 0),
        new Positions("Tennessee Avenue", -6.45f, 3.75f, 0),
        new Positions("New York Avenue", -6.45f, 5.06f, 0),
        new Positions("Free Parking", -6.45f, 6.59f, 0),
        new Positions("Kentucky Avenue", -4.98f, 6.59f, 0),
        new Positions("Chance", -3.7f, 6.59f, 0),
        new Positions("Indiana Avenue", -2.49f, 6.59f, 0),
        new Positions("Illinois Avenue", -1.27f, 6.59f, 0),
        new Positions("B. & O. Railroad", -0.04f, 6.59f, 0),
        new Positions("Atlantic Avenue", 1.23f, 6.59f, 0),
        new Positions("Ventnor Avenue", 2.45f, 6.59f, 0),
        new Positions("Water Works", 3.7f, 6.59f, 0),
        new Positions("Marvin Gardens", 4.89f, 6.59f, 0),
        new Positions("Go To Jail", 6.65f, 6.59f, 0),
        new Positions("Pacific Avenue", 6.65f, 5.04f, 0),
        new Positions("North Carolina Avenue", 6.65f, 3.78f, 0),
        new Positions("Community Chest", 6.65f, 2.56f, 0),
        new Positions("Pennsylvania Avenue", 6.65f, 1.37f, 0),
        new Positions("Short Line", 6.65f, 0.15f, 0),
        new Positions("Chance", 6.65f, -1.12f, 0),
        new Positions("Park Place", 6.65f, -2.32f, 0),
        new Positions("Luxury Tax", 6.65f, -3.56f, 0),
        new Positions("Boardwalk", 6.65f, -4.79f, 0),
    };

    private Positions currentPosition;

    // Start is called before the first frame update
    void Start()
    {
        currentPosition = Pos[0];
    }

    // Finding a position by name
    public Positions FindPositionByName(string positionName)
    {
        foreach (Positions position in Pos)
        {
            if (position.PositionName == positionName)
            {
                return position;
            }
        }
        return Pos[0];
    }

    // Find position by the array index
    public Positions FindPositionByIndex(int index)
    {
        return Pos[index];
    }

    // Get position index of array from name
    public int GetPositionIndex(string positionName)
    {
        for (int i = 0; i < Pos.Length; i++)
        {
            if (Pos[i].PositionName == positionName)
            {
                return i;
            }
        }
        return -1;
    }

    // Enable Bail and Jail free buttons if criteria is met
    public void JailOptions(bool[] playerTurn, bool[] isInJail, int[] getOutOfJailFree, Button JailFreeBtn, int[] Cash, Button BailBtn)
    {
        if (playerTurn[0] && isInJail[0])
        {
            if (getOutOfJailFree[0] > 0)
            {
                JailFreeBtn.interactable = true;
            }
            if (Cash[0] >= 50)
            {
                BailBtn.interactable = true;
            }
        }
        else if (playerTurn[1] && isInJail[1])
        {
            if (getOutOfJailFree[1] > 0)
            {
                JailFreeBtn.interactable = true;
            }
            if (Cash[1] >= 50)
            {
                BailBtn.interactable = true;
            }
        }
        else
        {
            JailFreeBtn.interactable = false;
            BailBtn.interactable = false;
        }
    }
}