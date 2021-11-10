using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Properties
{
    public string Colour;
    public string Type;
    public string Name;
    public int PositionIndex;
    public int BaseAmount;
    public int[] Rent;
    public bool IsOwned;
    public int OwnedPlayer;
    public int Houses;
    public int BuildCost;
    public bool IsHotel;
    public bool IsMortgaged;

    // Constructor
    public Properties(
        string colour,
        string type,
        string name,
        int positionIndex,
        int baseAmount,
        int[] rent,
        bool isOwned,
        int ownedPlayer,
        int houses,
        int buildCost,
        bool isHotel,
        bool isMortgaged)
    {
        this.Colour = colour;
        this.Type = type;
        this.Name = name;
        this.Name = name;
        this.PositionIndex = positionIndex;
        this.BaseAmount = baseAmount;
        this.Rent = rent;
        this.IsOwned = isOwned;
        this.OwnedPlayer = ownedPlayer;
        this.Houses = houses;
        this.BuildCost = buildCost;
        this.IsHotel = isHotel;
        this.IsMortgaged = isMortgaged;
    }
}


public class Property : MonoBehaviour
{
    // Initialise all properties, stations and utilities
    // Although Mediteranean Avenue and Baltic Avenue are brown on board, they will be considered as dark purple for rules
    public Properties[] PropertyList = new Properties[]
{
        new Properties(
            "#0e3578",
            "property",
            "Mediteranean Avenue",
            1,
            60,
            new int[] {2, 10, 30, 90, 160, 250},
            false,
            -1,
            0,
            50,
            false,
            false),
        new Properties(
            "#0e3578",
            "property",
            "Baltic Avenue",
            3,
            60,
             new int[] {4, 20, 60, 180, 320, 450},
            false,
            -1,
            0,
            50,
            false,
            false),
        new Properties(
            "#bbeaf0",
            "property",
            "Oriental Avenue",
            6,
            100,
             new int[] {6, 30, 90, 270, 400, 550},
            false,
            -1,
            0,
            50,
            false,
            false),
        new Properties(
            "#bbeaf0",
            "property",
            "Vermont Avenue",
            8,
            100,
             new int[] {6, 30, 90, 270, 400, 550},
            false,
            -1,
            0,
            50,
            false,
            false),
        new Properties(
            "#bbeaf0",
            "property",
            "Connecticut Avenue",
            9,
            120,
             new int[] {8, 40, 100, 300, 450, 600},
            false,
            -1,
            0,
            50,
            false,
            false),
        new Properties(
            "#c12a87",
            "property",
            "St. Charles Place",
            11,
            140,
             new int[] {10, 50, 150, 450, 625, 750},
            false,
            -1,
            0,
            100,
            false,
            false),
        new Properties(
            "#c12a87",
            "property",
            "States Avenue",
            13,
            140,
             new int[] {10, 50, 150, 450, 625, 750},
            false,
            -1,
            0,
            100,
            false,
            false),
        new Properties(
            "#c12a87",
            "property",
            "Virginia Avenue",
            14,
            160,
             new int[] {12, 60, 180, 500, 700, 900},
            false,
            -1,
            0,
            100,
            false,
            false),
        new Properties(
            "#f9bb06",
            "property",
            "St. James Place",
            16,
            180,
             new int[] {14, 70, 200, 550, 750, 950},
            false,
            -1,
            0,
            100,
            false,
            false),
        new Properties(
            "#f9bb06",
            "property",
            "Tennessee Avenue",
            18,
            180,
             new int[] {14, 70, 200, 550, 750, 950},
            false,
            -1,
            0,
            100,
            false,
            false),
        new Properties(
            "#f9bb06",
            "property",
            "New York Avenue",
            19,
            200,
             new int[] {16, 80, 220, 600, 800, 1000},
            false,
            -1,
            0,
            100,
            false,
            false),
        new Properties(
            "#f03316",
            "property",
            "Kentucky Avenue",
            21,
            220,
             new int[] {18, 90, 250, 700, 875, 1050},
            false,
            -1,
            0,
            150,
            false,
            false),
        new Properties(
            "#f03316",
            "property",
            "Indiana Avenue",
            23,
            220,
             new int[] {18, 90, 250, 700, 875, 1050},
            false,
            -1,
            0,
            150,
            false,
            false),
        new Properties(
            "#f03316",
            "property",
            "Illinois Avenue",
            24,
            240,
             new int[] {20, 100, 300, 750, 925, 1100},
            false,
            -1,
            0,
            150,
            false,
            false),
        new Properties(
            "#feff01",
            "property",
            "Atlantic Avenue",
            26,
            260,
             new int[] {22, 110, 330, 800, 975, 1150},
            false,
            -1,
            0,
            150,
            false,
            false),
        new Properties(
            "#feff01",
            "property",
            "Ventnor Avenue",
            27,
            260,
             new int[] {22, 110, 330, 800, 975, 1150},
            false,
            -1,
            0,
            150,
            false,
            false),
        new Properties(
            "#feff01",
            "property",
            "Marvin Gardens",
            29,
            280,
             new int[] {24, 120, 360, 850, 1025, 1200},
            false,
            -1,
            0,
            150,
            false,
            false),
        new Properties(
            "#69d58a",
            "property",
            "Pacific Avenue",
            31,
            300,
             new int[] {26, 130, 390, 900, 1100, 1275},
            false,
            -1,
            0,
            200,
            false,
            false),
        new Properties(
            "#69d58a",
            "property",
            "North Carolina Avenue",
            32,
            300,
             new int[] {26, 130, 390, 900, 1100, 1275},
            false,
            -1,
            0,
            200,
            false,
            false),
        new Properties(
            "#69d58a",
            "property",
            "Pennsylvania Avenue",
            34,
            320,
             new int[] {28, 150, 450, 1000, 1200, 1400},
            false,
            -1,
            0,
            200,
            false,
            false),
        new Properties(
            "#00529e",
            "property",
            "Park Place",
            37,
            350,
             new int[] {35, 175, 500, 1100, 1300, 1500},
            false,
            -1,
            0,
            200,
            false,
            false),
        new Properties(
            "#00529e",
            "property",
            "Boardwalk",
            39,
            400,
             new int[] {50, 200, 600, 1400, 1700, 2000},
            false,
            -1,
            0,
            200,
            false,
            false),
        new Properties(
            "#ffffff",
            "station",
            "Reading Railroad",
            5,
            200,
             new int[] {25, 50, 100, 200},
            false,
            -1,
            0,
            0,
            false,
            false),
        new Properties(
            "#ffffff",
            "station",
            "Pennsylvania Railroad",
            15,
            200,
             new int[] {25, 50, 100, 200},
            false,
            -1,
            0,
            0,
            false,
            false),
        new Properties(
            "#ffffff",
            "station",
            "B. & O. Railroad",
            25,
            200,
             new int[] {25, 50, 100, 200},
            false,
            -1,
            0,
            0,
            false,
            false),
        new Properties(
            "#ffffff",
            "station",
            "Short Line Railroad",
            35,
            200,
             new int[] {25, 50, 100, 200},
            false,
            -1,
            0,
            0,
            false,
            false),
        new Properties(
            "#ffffff",
            "utility",
            "Electric Company",
            12,
            150,
             new int[] {4, 10}, // times amount based on die roll
            false,
            -1,
            0,
            0,
            false,
            false),
        new Properties(
            "#ffffff",
            "utility",
            "Water Works",
            28,
            260,
             new int[] {4, 10}, // times amount based on die roll
            false,
            -1,
            0,
            150,
            false,
            false)
};

    // Finding a position by name
    public Properties FindPropertyByName(string propertyName)
    {
        foreach (Properties property in PropertyList)
        {
            if (property.Name == name)
            {
                return property;
            }
        }
        return PropertyList[0];
    }

    // Find the property by the location index value
    public Properties FindPropertyByIndex(int index)
    {
        foreach (Properties property in PropertyList)
        {
            if (property.PositionIndex == index)
            {
                return property;
            }
        }
        return PropertyList[0];
    }

    // Get array index of PropertyList from location index
    public int GetPropertyIndex(int index)
    {
        for (int i = 0; i < PropertyList.Length; i++)
        {
            if (PropertyList[i].PositionIndex == index)
            {
                return i;
            }
        }
        return 0;
    }

    // Check player's number of station owned
    public int NumberOfStationsOwned(int playerIndex)
    {
        int count = 0;
        foreach (Properties property in PropertyList)
        {
            if (property.Type == "station" && property.OwnedPlayer == playerIndex) count++;
        }
        return count;
    }

    // Check player's number of station owned
    public int NumberOfUtilitiesOwned(int playerIndex)
    {
        int count = 0;
        foreach (Properties property in PropertyList)
        {
            if (property.Type == "utility" && property.OwnedPlayer == playerIndex) count++;
        }
        return count;
    }

    // Check player's total number of houses owned
    public int GetHouseCount(int playerIndex)
    {
        int count = 0;
        foreach (Properties property in PropertyList)
        {
            if (property.OwnedPlayer == playerIndex) count += property.Houses;
        }
        return count;
    }

    // Check player's total number of hotels owned
    public int GetHotelCount(int playerIndex)
    {
        int count = 0;
        foreach (Properties property in PropertyList)
        {
            if (property.OwnedPlayer == playerIndex && property.IsHotel) count++;
        }
        return count;
    }

    // Check if player is able to buy hotels
    public bool CheckForMonopoly(string colour, int player)
    {
        int targetValue;

        // Dark purple and dark blue have 2 properties each, and the rest have 3
        if (colour == "#0e3578" || colour == "#00529e")
        {
            targetValue = 2;
        }
        else
        {
            targetValue = 3;
        }

        int fullHouseCount = 0;
        foreach (Properties property in PropertyList)
        {
            if ((property.Houses == 4 || property.IsHotel) && property.Colour == colour && property.OwnedPlayer == player)
            {
                fullHouseCount++;
            }
        }

        if (targetValue == fullHouseCount) return true;
        return false;
    }
}
