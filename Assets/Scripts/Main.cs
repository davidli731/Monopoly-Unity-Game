using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class Main : MonoBehaviour, IPointerClickHandler
{

    // Transform and Rigidbody Variables
    [SerializeField] private GameObject Object;
    [SerializeField] private Rigidbody[] RigidbodyPlayer = new Rigidbody[2];
    [SerializeField] private Transform[] TransformPlayer = new Transform[2];
    [SerializeField] private Transform[] DieFace1 = new Transform[6];
    [SerializeField] private Transform[] DieFace2 = new Transform[6];
    [SerializeField] private Transform[] BoardLocation = new Transform[28];   // 28 total properties
    [SerializeField] private Transform[] Chance = new Transform[16];          // 16 chance cards
    [SerializeField] private Transform[] Community = new Transform[16];       // 16 community chest cards

    // Buttons Variables
    [SerializeField] private Button PlayBtn;
    [SerializeField] private Button JailFreeBtn;
    [SerializeField] private Button BailBtn;
    [SerializeField] private Button BuyBtn;
    [SerializeField] private Button ExitBtn;

    // Text Variables
    [SerializeField] private TextMeshProUGUI PropertyDetailsText;
    [SerializeField] private TextMeshProUGUI CashText;
    [SerializeField] private TextMeshProUGUI LogText;
    [SerializeField] private TextMeshProUGUI PropertiesText;

    // Panel
    [SerializeField] private PopupCard PropertyPopup;

    // Private variables
    private string propertyName = "";
    private string propertyPrice = "";
    private string logString = "";

    private int[] Cash = new int[2];
    private int[] rollValue = new int[2];
    private int[] doublesCounter = new int[2];
    private int[] jailTurnCounter = new int[2];
    private int[] currentIndex = new int[2];
    private int[] destinationIndex = new int[2];
    private int[] getOutOfJailFree = new int[2];
    private int currentPlayer;

    private float[] lastSqrMagnitude = new float[2];
    private float movementSpeed = 6f;

    private bool[] isInJail = new bool[2];
    private bool[] playerTurn = new bool[2];
    private bool[] inTransit = new bool[2];
    private bool[] rollForRent = new bool[2];
    private bool spaceKeyWasPressed;

    private Vector3[] destinationCoordinates = new Vector3[2];
    private Position position;
    private DieRoll dieRoll;
    private Positions destinationPosition;
    private Properties destinationProperty;
    private Properties selectedProperty;
    private Property property;
    private LuckCards luckCards;
    private Money money;

    // Start is called before the first frame update
    void Start()
    {
        position = Object.AddComponent<Position>();
        dieRoll = Object.AddComponent<DieRoll>();
        property = Object.AddComponent<Property>();
        luckCards = Object.AddComponent<LuckCards>();
        money = Object.AddComponent<Money>();

        PlayBtn.onClick.AddListener(PlayBtnOnClick);
        JailFreeBtn.onClick.AddListener(JailFreeBtnOnClick);
        BailBtn.onClick.AddListener(BailBtnOnClick);
        BuyBtn.onClick.AddListener(BuyBtnOnClick);
        BuyBtn.interactable = false;
        ExitBtn.onClick.AddListener(ExitBtnOnClick);

        logString = "Press the Play button or SPACE key to start.";

        playerTurn[0] = true;

        // Initialise start of game
        for (int i = 0; i < 2; i++)
        {
            currentIndex[i] = 0;
            destinationIndex[i] = 0;
            getOutOfJailFree[i] = 0;
            Cash[i] = 1500;
            lastSqrMagnitude[i] = Mathf.Infinity;
        }
    }

    // Update is called once per frame
    void Update()
    {
        triggerPlay();

        // Set property detail text
        string propertyDetails = "Property Name: " + propertyName + "\nProperty Price: " + propertyPrice;
        PropertyDetailsText.text = propertyDetails;

        // Set player cash count text
        CashText.text = "Cash\nPlayer 1: $" + Cash[0] + "\nPlayer 2: $" + Cash[1];

        // Check for jail options
        position.JailOptions(playerTurn, isInJail, getOutOfJailFree, JailFreeBtn, Cash, BailBtn);

        // Update log
        LogText.text = "Log:\n" + logString;

        checkForNegativeCash();
        showPropertiesText();
        checkTransit();
        checkRollForRent();
    }

    // Update for any physics aspects for the game
    private void FixedUpdate()
    {
        if (spaceKeyWasPressed)
        {
            resetCardVectors();
            getDieResult();
            getPlayer();

            // Reset space bar pressed for next turn
            spaceKeyWasPressed = false;
        }
    }

    // Pointer Click Event Handler
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(PropertiesText, Input.mousePosition, null);
            if (linkIndex > -1)
            {
                selectedProperty = property.FindPropertyByIndex(Int16.Parse(PropertiesText.textInfo.linkInfo[linkIndex].GetLinkID()));
                PropertyPopup.Show(property, selectedProperty, Cash);
            }
        }
    }

    // Play Button onClick function
    void PlayBtnOnClick()
    {
        spaceKeyWasPressed = true;
        if (GameObject.Find("PlayButton").GetComponentInChildren<Text>().text == "Play")
        {
            GameObject.Find("PlayButton").GetComponentInChildren<Text>().text = "Roll Dice";
        }
    }

    // Get out of Jail Button onClick function
    void JailFreeBtnOnClick()
    {
        if (playerTurn[0])
        {
            getOutOfJailFree[0]--;
            isInJail[0] = false;
            logString = "Player 1 used Get out of Jail card! ";
        }
        else if (playerTurn[1])
        {
            getOutOfJailFree[1]--;
            isInJail[1] = false;
            logString = "Player 2 used Get out of Jail card! ";
        }
        JailFreeBtn.interactable = false;
    }

    // Bail from jail Button onClick function
    void BailBtnOnClick()
    {
        if (playerTurn[0])
        {
            Cash[0] -= 50;
            isInJail[0] = false;
            logString = "Player 1 paid for bail! ";
        }
        else if (playerTurn[1])
        {
            Cash[1] -= 50;
            isInJail[1] = false;
            logString = "Player 2 paid for bail! ";
        }
        BailBtn.interactable = false;
    }

    // Buy property Button onClick function
    void BuyBtnOnClick()
    {
        int index = property.GetPropertyIndex(destinationIndex[currentPlayer]);
        property.PropertyList[index].IsOwned = true;
        property.PropertyList[index].OwnedPlayer = currentPlayer;
        if (Cash[currentPlayer] >= property.PropertyList[index].BaseAmount) Cash[currentPlayer] -= property.PropertyList[index].BaseAmount;

        GameObject.Find("BuyButton").GetComponentInChildren<Text>().text = "Buy";
        BuyBtn.interactable = false;

        logString = "Player " + (currentPlayer + 1) + " bought " + property.PropertyList[index].Name + "! ";
    }

    // Exit Button onClick function
    void ExitBtnOnClick()
    {
        Application.Quit();
    }

    // Trigger spaceKeyWasPressed boolean and change button text "Play" to "Roll Dice"
    private void triggerPlay()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            spaceKeyWasPressed = true;
            if (GameObject.Find("PlayButton").GetComponentInChildren<Text>().text == "Play")
            {
                GameObject.Find("PlayButton").GetComponentInChildren<Text>().text = "Roll Dice";
            }
        }
    }

    // Check for player negative cash
    private void checkForNegativeCash()
    {
        if (playerTurn[0] && money.CheckForNegativeCash(playerTurn[0], Cash[0]))
        {
            logString = "Player 1 has negative cash, sell houses, properties or mortgage to avoid bankruptcy! ";
        }
        if (playerTurn[1] && money.CheckForNegativeCash(playerTurn[1], Cash[1]))
        {
            logString = "Player 2 has negative cash, sell houses, properties or mortgage to avoid bankruptcy! ";
        }
    }

    // Print list of properties owned by players
    private void showPropertiesText()
    {
        // Set property summary for both players;
        string[] playerProperties = new string[2];
        playerProperties[0] = "";
        playerProperties[1] = "";
        foreach (Properties p in property.PropertyList)
        {
            if (p.IsOwned)
            {
                // All properties will be displayed in the format <link><color>propertyName</link></link>
                playerProperties[p.OwnedPlayer] += "<link=" + p.PositionIndex + "><color=" + p.Colour + ">" + p.Name + "</color></link>\n";
            }
        }
        PropertiesText.text = "Player 1's Properties:\n" + playerProperties[0] + "\nPlayer 2's Properties:\n" + playerProperties[1];
    }

    // Check whether piece is in transit, stop move if token reaches its destination
    private void checkTransit()
    {
        float sqrMagnitude;

        if (inTransit[0])
        {
            sqrMagnitude = (destinationCoordinates[0] - TransformPlayer[0].position).sqrMagnitude;
            if (sqrMagnitude > lastSqrMagnitude[0])
            {
                RigidbodyPlayer[0].velocity = Vector3.zero;
                lastSqrMagnitude[0] = Mathf.Infinity;
                inTransit[0] = false;
                actionsAfterMove(0);
            }
            else
            {
                lastSqrMagnitude[0] = sqrMagnitude;
            }
        }

        if (inTransit[1])
        {
            sqrMagnitude = (destinationCoordinates[1] - TransformPlayer[1].position).sqrMagnitude;
            if (sqrMagnitude > lastSqrMagnitude[1])
            {
                RigidbodyPlayer[1].velocity = Vector3.zero;
                lastSqrMagnitude[1] = Mathf.Infinity;
                inTransit[1] = false;
                actionsAfterMove(1);
            }
            else
            {
                lastSqrMagnitude[1] = sqrMagnitude;
            }
        }
    }

    // If a player lands on a chance, goes to the nearest utility and needs to roll for rent, give them an extra turn to roll
    private void checkRollForRent()
    {
        if (spaceKeyWasPressed)
        {
            if (rollForRent[0])
            {
                rollValue[0] = 0;
                rollValue[1] = 0;
                rollValue[0] += dieRoll.getDieResult(DieFace1);
                rollValue[1] += dieRoll.getDieResult(DieFace2);

                int rentAmount = 10 * (rollValue[0] + rollValue[1]);

                // Pay owner of property
                money.TransferCash(Cash, 0, 1, rentAmount);

                rollForRent[0] = false;
                playerTurn[0] = false;
                playerTurn[1] = true;

                spaceKeyWasPressed = false;
            }


            if (rollForRent[1])
            {
                rollValue[0] = 0;
                rollValue[1] = 0;
                rollValue[0] += dieRoll.getDieResult(DieFace1);
                rollValue[1] += dieRoll.getDieResult(DieFace2);

                int rentAmount = 10 * (rollValue[0] + rollValue[1]);

                // Pay owner of property
                money.TransferCash(Cash, 1, 0, rentAmount);

                rollForRent[1] = false;
                playerTurn[1] = false;
                playerTurn[0] = true;

                spaceKeyWasPressed = false;
            }
        }
    }

    // Reset all the card vectors from previous turns
    private void resetCardVectors()
    {
        foreach (Transform card in BoardLocation)
        {
            card.position = new Vector3(0, 0, -90);
        }

        foreach (Transform card in Chance)
        {
            card.position = new Vector3(0, 0, -50);
        }

        foreach (Transform card in Community)
        {
            card.position = new Vector3(0, 0, -50);
        }
    }

    // Get die results
    private void getDieResult()
    {
        rollValue[0] = 0;
        rollValue[1] = 0;
        rollValue[0] += dieRoll.getDieResult(DieFace1);
        rollValue[1] += dieRoll.getDieResult(DieFace2);
    }

    // Get current player
    private void getPlayer()
    {
        if (playerTurn[0])
        {
            // Player 1 move
            setPlayer(0, 1);
        }
        else if (playerTurn[1])
        {
            // Player 2 move
            setPlayer(1, 0);
        }
    }

    // Make player's turn, check who's turn it is and whether they are in jail
    private void setPlayer(int player, int otherPlayer)
    {
        if (playerTurn[player] && !isInJail[player])
        {
            // check for consecutive doubles
            if (rollValue[0] == rollValue[1])
            {
                doublesCounter[player]++;
                if (doublesCounter[player] == 3)
                {
                    playerTurn[player] = false;
                    playerTurn[otherPlayer] = true;

                    logString = "Player " + (player + 1) + " rolled 3 doubles in a row. Go to jail! ";

                }
                else
                {
                    logString = "Doubles rolled! Player " + (player + 1) + " gets an extra roll. ";
                }
            }
            else
            {
                doublesCounter[player] = 0;
                playerTurn[player] = false;
                playerTurn[otherPlayer] = true;

                logString = "";
            }

            destinationIndex[player] = currentIndex[player] + rollValue[0] + rollValue[1];
            playerMove(player);
        }

        // If player is in jail, roll double to get out or after 3 turns and pay $50 fine
        else if (playerTurn[player] && isInJail[player])
        {
            if (rollValue[0] == rollValue[1])
            {
                jailTurnCounter[player] = 0;
                isInJail[player] = false;

                logString = "Player " + (player + 1) + " broke out of jail with a double! ";

                destinationIndex[player] = currentIndex[player] + rollValue[0] + rollValue[1];
                playerMove(player);
            }
            else
            {
                jailTurnCounter[player]++;
                if (jailTurnCounter[player] == 3)
                {
                    jailTurnCounter[player] = 0;
                    Cash[player] -= 50;
                    isInJail[player] = false;

                    logString = "Player " + (player + 1) + " paid a $50 fine and got out of jail. ";

                    destinationIndex[player] = currentIndex[player] + rollValue[0] + rollValue[1];
                    playerMove(player);
                }
                else
                {
                    logString = "Player " + (player + 1) + " did not roll a double and is still in jail. " + (3 - jailTurnCounter[player]) + " more turns until freed. ";
                }
            }
            doublesCounter[player] = 0;
            playerTurn[player] = false;
            playerTurn[otherPlayer] = true;
        }
    }

    // Check for move and if they land on special squares, i.e. GO, pay tax, jail, etc.
    private void playerMove(int playerIndex)
    {

        // Pass GO
        if (destinationIndex[playerIndex] > 39)
        {
            destinationIndex[playerIndex] -= 40;
            Cash[playerIndex] += 200;

            logString = "Player " + (playerIndex + 1) + " passed GO, collect $200. ";
        }

        // Income Tax position
        else if (destinationIndex[playerIndex] == 4)
        {
            Cash[playerIndex] -= 200;

            logString = "Player " + (playerIndex + 1) + " paid a $200 tax. ";
        }

        // Luxury Tax position
        else if (destinationIndex[playerIndex] == 38)
        {
            Cash[playerIndex] -= 100;
            logString = "Player " + (playerIndex + 1) + " paid a $100 tax. ";
        }

        // Go to Jail if player rolled 3 doubles in a row
        if (doublesCounter[playerIndex] == 3)
        {

            logString = "Player " + (playerIndex + 1) + " got caught! ";

            destinationIndex[playerIndex] = 10;
            isInJail[playerIndex] = true;

            // Even if player rolls a double, end player's turn if they land in jail
            doublesCounter[playerIndex] = 0;
            if (playerIndex == 0)
            {
                playerTurn[0] = false;
                playerTurn[1] = true;
            }
            else
            {
                playerTurn[1] = false;
                playerTurn[0] = true;
            }
        }


        destinationPosition = position.FindPositionByIndex(destinationIndex[playerIndex]);
        logString += "Player " + (playerIndex + 1) + " went to " + destinationPosition.PositionName + ". ";

        // Move to destination point
        destinationCoordinates[playerIndex] = new Vector3(destinationPosition.X, destinationPosition.Y, destinationPosition.Z);
        RigidbodyPlayer[playerIndex].velocity = (destinationCoordinates[playerIndex] - TransformPlayer[playerIndex].position).normalized * movementSpeed;
        inTransit[playerIndex] = true;

        currentIndex[playerIndex] = destinationIndex[playerIndex];
    }

    // Actions after a move is made, i.e. buy property, landed on chance/community chest, etc
    private void actionsAfterMove(int playerIndex)
    {
        // display card, option to buy title deed if not already owned
        destinationProperty = property.FindPropertyByIndex(destinationIndex[playerIndex]);
        if ((destinationProperty.Type == "property" ||
            destinationProperty.Type == "station" ||
            destinationProperty.Type == "utility") &&
            !destinationProperty.IsOwned)
        {
            int index = property.GetPropertyIndex(destinationIndex[playerIndex]);
            if ((index != 0 && destinationIndex[playerIndex] != 1) || (index == 0 && destinationIndex[playerIndex] == 1))
            {
                BoardLocation[index].position = new Vector3(0, 0, 100);
                currentPlayer = playerIndex;
                if (Cash[playerIndex] >= destinationProperty.BaseAmount)
                {
                    BuyBtn.interactable = true;
                    GameObject.Find("BuyButton").GetComponentInChildren<Text>().text = "Buy ($" + destinationProperty.BaseAmount + ")";
                    logString += "You can buy or roll to pass. ";
                    propertyName = destinationProperty.Name;
                    propertyPrice = "$" + destinationProperty.BaseAmount;
                }
            }
        }
        else if (destinationProperty.IsOwned && destinationProperty.OwnedPlayer != playerIndex && !destinationProperty.IsMortgaged)
        {
            // Disable button if player is not an a purchaseable property
            BuyBtn.interactable = false;
            GameObject.Find("BuyButton").GetComponentInChildren<Text>().text = "Can't Buy";

            // Rent taken when on other player's property
            if (destinationProperty.Type == "property" && !destinationProperty.IsMortgaged)
            {
                if (!destinationProperty.IsHotel)
                {

                    // if the property is unimproved but the player owns all properties in the colour group, rent is doubled
                    if (destinationProperty.Houses == 0 && property.CheckForMonopoly(destinationProperty.Colour, destinationProperty.OwnedPlayer))
                    {
                        money.TransferCash(Cash, playerIndex, destinationProperty.OwnedPlayer, 2 * destinationProperty.Rent[destinationProperty.Houses]);
                    }
                    else
                    {
                        money.TransferCash(Cash, playerIndex, destinationProperty.OwnedPlayer, destinationProperty.Rent[destinationProperty.Houses]);
                    }
                }
                else
                {
                    money.TransferCash(Cash, playerIndex, destinationProperty.OwnedPlayer, destinationProperty.Rent[5]);
                }
            }

            // If on utility, times result from die roll
            else if (destinationProperty.Type == "utility" && !destinationProperty.IsMortgaged)
            {
                int utilitiesOwned = property.NumberOfUtilitiesOwned(destinationProperty.OwnedPlayer);
                money.TransferCash(Cash, playerIndex, destinationProperty.OwnedPlayer, destinationProperty.Rent[utilitiesOwned - 1] * (rollValue[0] + rollValue[1]));
            }

            // If on station, pay rent based on how many stations the other player has
            else if (destinationProperty.Type == "station" && !destinationProperty.IsMortgaged)
            {
                int stationsOwned = property.NumberOfStationsOwned(destinationProperty.OwnedPlayer);
                money.TransferCash(Cash, playerIndex, destinationProperty.OwnedPlayer, destinationProperty.Rent[stationsOwned - 1]);
            }
        }

        // Disable buy button if player is not on a property
        if (destinationIndex[playerIndex] == 7 ||
            destinationIndex[playerIndex] == 22 ||
            destinationIndex[playerIndex] == 36 ||
            destinationIndex[playerIndex] == 2 ||
            destinationIndex[playerIndex] == 17 ||
            destinationIndex[playerIndex] == 33 ||
            destinationIndex[playerIndex] == 4 ||
            destinationIndex[playerIndex] == 38 ||
            destinationIndex[playerIndex] % 10 == 0)
        {
            GameObject.Find("BuyButton").GetComponentInChildren<Text>().text = "Buy";
            BuyBtn.interactable = false;
            propertyName = "";
            propertyPrice = "";
        }

        if (destinationIndex[playerIndex] == 30)
        {
            isInJail[playerIndex] = true;
            doublesCounter[playerIndex] = 0;

            if (playerIndex == 0)
            {
                playerTurn[0] = false;
                playerTurn[1] = true;
            }
            else
            {
                playerTurn[1] = false;
                playerTurn[0] = true;
            }

            destinationIndex[playerIndex] = 10;
            playerMove(playerIndex);
        }

        // Chance positions
        if (destinationIndex[playerIndex] == 7 || destinationIndex[playerIndex] == 22 || destinationIndex[playerIndex] == 36)
        {

            int cardIndex = luckCards.GetRandomIndex();
            Cards selectedCard = luckCards.ChanceCardList[cardIndex];
            Chance[cardIndex].position = new Vector3(0, 0, 100);

            if (!selectedCard.IsOther)
            {
                basicLuckCards(selectedCard, playerIndex);
            }
            else
            {
                // ADVANCE TOKEN TO NEAREST UTILITY. IF UNOWNED YOU MAY BUY IT FROM BANK. IF OWNED, THROW DICE AND PAY OWNER A TOTAL TEN TIMES THE AMOUNT THROWN
                if (cardIndex == 3)
                {
                    currentIndex[playerIndex] = destinationIndex[playerIndex];
                    // Bottom Chance goes to Electric Company (Index 12)
                    if (currentIndex[playerIndex] == 7)
                    {
                        destinationIndex[playerIndex] = 12;
                    }
                    // Top Chance goes to Water Works (Index 28)
                    else if (currentIndex[playerIndex] == 22)
                    {
                        destinationIndex[playerIndex] = 28;
                    }
                    // Right Chance goes to Electric Company (Index 12) and passes GO
                    else if (currentIndex[playerIndex] == 36)
                    {
                        Cash[playerIndex] += 200;
                        destinationIndex[playerIndex] = 12;
                    }

                    playerMove(playerIndex);
                    destinationProperty = property.FindPropertyByIndex(destinationIndex[playerIndex]);

                    // if property is owned by other player, roll to determine rent
                    if (destinationProperty.IsOwned && destinationProperty.OwnedPlayer != playerIndex)
                    {
                        // Give player an extra "turn" to roll for rent
                        playerTurn[destinationProperty.OwnedPlayer] = false;
                        playerTurn[playerIndex] = true;
                        rollForRent[playerIndex] = true;
                    }
                }

                // ADVANCE TOKEN TO THE NEAREST RAILROAD AND PAY OWNER TWICE THE RENTAL TO WHICH HE IS OTHERWISE ENTITLED. IF RAILROAD IS UNOWNED, YOU MAY BUY IT FROM THE BANK
                // 2 in the set
                else if (cardIndex == 7 || cardIndex == 13)
                {
                    currentIndex[playerIndex] = destinationIndex[playerIndex];
                    // Bottom Chance goes to Pennsylvania RR (Index 15)
                    if (currentIndex[playerIndex] == 7)
                    {
                        destinationIndex[playerIndex] = 15;
                    }
                    // Top Chance goes to B.&O. RR (Index 25)
                    else if (currentIndex[playerIndex] == 22)
                    {
                        destinationIndex[playerIndex] = 25;
                    }
                    // Right Chance goes to Reading RR (Index 5) and passes GO
                    else if (currentIndex[playerIndex] == 36)
                    {
                        Cash[playerIndex] += 200;
                        destinationIndex[playerIndex] = 5;
                    }

                    playerMove(playerIndex);
                    destinationProperty = property.FindPropertyByIndex(destinationIndex[playerIndex]);

                    // if property is owned by other player, pay 2* usual rent
                    if (destinationProperty.IsOwned && destinationProperty.OwnedPlayer != playerIndex && !destinationProperty.IsMortgaged)
                    {
                        money.TransferCash(Cash, playerIndex, destinationProperty.OwnedPlayer, 2 * destinationProperty.Rent[property.NumberOfStationsOwned(destinationProperty.OwnedPlayer) - 1]);
                    }
                }

                // THIS CARD MAY BE KEPT UNTIL NEEDED OR SOLD. GET OUT OF JAIL FREE
                else if (cardIndex == 10)
                {
                    getOutOfJailFree[playerIndex]++;
                }

                // MAKE GENERAL REPAIRS ON ALL YOUR PROPERTY. FOR EACH HOUSE PAY $25, FOR EACH HOTEL $100
                else if (cardIndex == 11)
                {
                    int houseCount = property.GetHouseCount(playerIndex);
                    int hotelCount = property.GetHotelCount(playerIndex);
                    Cash[playerIndex] -= (25 * houseCount) + (100 * hotelCount);
                }

                // YOU HAVE BEEN ELECTED CHAIRMAN OF THE BOARD. PAY EACH PLAYER $50
                else if (cardIndex == 14)
                {
                    int otherPlayerIndex;
                    if (playerIndex == 0)
                    {
                        otherPlayerIndex = 1;
                    }
                    else
                    {
                        otherPlayerIndex = 0;
                    }
                    money.TransferCash(Cash, playerIndex, otherPlayerIndex, 50);
                }

                // GO BACK 3 SPACES
                else if (cardIndex == 15)
                {
                    // Since the closest chance from GO is 7 positions away, it won't go back past GO
                    currentIndex[playerIndex] = destinationIndex[playerIndex];
                    destinationIndex[playerIndex] -= 3;
                    playerMove(playerIndex);
                }
            }
        }

        // Community Card positions
        if (destinationIndex[playerIndex] == 2 || destinationIndex[playerIndex] == 17 || destinationIndex[playerIndex] == 33)
        {

            int cardIndex = luckCards.GetRandomIndex();
            Cards selectedCard = luckCards.CommunityCardList[cardIndex];
            Community[cardIndex].position = new Vector3(0, 0, 100);

            if (!selectedCard.IsOther)
            {
                basicLuckCards(selectedCard, playerIndex);
            }
            else
            {
                // GRAND OPERA OPENING. COLLECT $50 FROM EVERY PLAYER
                if (cardIndex == 6)
                {
                    int otherPlayerIndex;
                    if (playerIndex == 0)
                    {
                        otherPlayerIndex = 1;
                    }
                    else
                    {
                        otherPlayerIndex = 0;
                    }
                    money.TransferCash(Cash, otherPlayerIndex, playerIndex, 50);
                }

                // YOU ARE ASSESSED FOR STREET REAPAIRS. $40 PER HOUSE, $115 PER HOTEL
                else if (cardIndex == 7)
                {
                    int houseCount = property.GetHouseCount(playerIndex);
                    int hotelCount = property.GetHotelCount(playerIndex);
                    Cash[playerIndex] -= (40 * houseCount) + (115 * hotelCount);
                }

                // GET OUT OF JAIL, FREE. THIS CARD MAY BE KEPT UNTIL NEEDED OR SOLD
                else if (cardIndex == 10)
                {
                    getOutOfJailFree[playerIndex]++;
                }
            }
        }
    }

    // This is for simple chance/community cards where the features are simple, i.e. collect/give xxx amount, move to location, etc.
    private void basicLuckCards(Cards selectedCard, int playerIndex)
    {
        if (selectedCard.IsDebit && !selectedCard.IsMove)
        {
            Cash[playerIndex] += selectedCard.DebitAmount;
        }
        else if (selectedCard.IsDebit && selectedCard.IsMove)
        {
            // If the move to position is less than current position, then it must go through GO for that position
            if (selectedCard.MoveToIndexPosition < currentIndex[playerIndex])
            {
                Cash[playerIndex] += selectedCard.DebitAmount;
            }
            destinationIndex[playerIndex] = selectedCard.MoveToIndexPosition;

            // Even if player rolls a double, end player's turn if they land in jail
            doublesCounter[playerIndex] = 0;
            if (playerIndex == 0)
            {
                playerTurn[0] = false;
                playerTurn[1] = true;
            }
            else
            {
                playerTurn[1] = false;
                playerTurn[0] = true;
            }
            playerMove(playerIndex);
        }
        else if (!selectedCard.IsDebit && selectedCard.IsMove)
        {
            if (selectedCard.Tag == "Go to Jail")
            {
                isInJail[playerIndex] = true;
                doublesCounter[playerIndex] = 0;

                if (playerIndex == 0)
                {
                    playerTurn[0] = false;
                    playerTurn[1] = true;
                }
                else
                {
                    playerTurn[1] = false;
                    playerTurn[0] = true;
                }
            }
            destinationIndex[playerIndex] = selectedCard.MoveToIndexPosition;
            playerMove(playerIndex);
        }
        else if (selectedCard.IsCredit)
        {
            Cash[playerIndex] -= selectedCard.CreditAmount;
        }
    }
}