using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PopupCard : MonoBehaviour
{

    [SerializeField] private Image CardColour;
    [SerializeField] private TextMeshProUGUI TitleText;
    [SerializeField] private TextMeshProUGUI RentTopText;
    [SerializeField] private TextMeshProUGUI RentLeftText;
    [SerializeField] private TextMeshProUGUI RentValuesText;
    [SerializeField] private Button BuyHouseBtn;
    [SerializeField] private Button BuyHotelBtn;
    [SerializeField] private Button SellPropertyBtn;
    [SerializeField] private Button MortgageBtn;
    [SerializeField] private Button CloseBtn;

    Property property;
    int[] Cash;
    int propertyIndex;

    private static PopupCard instance;

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);

        BuyHouseBtn.onClick.AddListener(BuyHouseBtnOnClick);
        BuyHotelBtn.onClick.AddListener(BuyHotelBtnOnClick);
        SellPropertyBtn.onClick.AddListener(SellPropertyOnClick);
        MortgageBtn.onClick.AddListener(MortgageBtnOnClick);
        CloseBtn.onClick.AddListener(CloseBtnOnClick);
    }

    // Buy houses
    private void BuyHouseBtnOnClick()
    {
        property.PropertyList[propertyIndex].Houses++;
        Cash[property.PropertyList[propertyIndex].OwnedPlayer] -= property.PropertyList[propertyIndex].BuildCost;

        if (property.CheckForMonopoly(property.PropertyList[propertyIndex].Colour, property.PropertyList[propertyIndex].OwnedPlayer) &&
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= property.PropertyList[propertyIndex].BuildCost)
        {
            BuyHotelBtn.interactable = true;
        }

        setTextDescription();
        if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= property.PropertyList[propertyIndex].BuildCost)
        {
            BuyHouseBtn.interactable = true;
        }
        else
        {
            BuyHouseBtn.interactable = false;
        }

        if (property.PropertyList[propertyIndex].Houses == 4 || property.PropertyList[propertyIndex].IsHotel)
        {
            BuyHouseBtn.interactable = false;
        }
        SellPropertyBtn.interactable = false;
        MortgageBtn.interactable = true;
        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell House ($" + (property.PropertyList[propertyIndex].BuildCost / 2) + ")";
    }

    // Can only buy hotels when all properties in a colour group are owned and have all 4 houses on each
    private void BuyHotelBtnOnClick()
    {
        property.PropertyList[propertyIndex].Houses = 0;
        property.PropertyList[propertyIndex].IsHotel = true;
        Cash[property.PropertyList[propertyIndex].OwnedPlayer] -= property.PropertyList[propertyIndex].BuildCost;

        setTextDescription();
        BuyHotelBtn.interactable = false;
        SellPropertyBtn.interactable = false;
        MortgageBtn.interactable = true;
        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell Hotel ($" + (property.PropertyList[propertyIndex].BuildCost / 2) + ")";
    }

    // Sell property button
    private void SellPropertyOnClick()
    {
        Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BaseAmount / 2;
        property.PropertyList[propertyIndex].IsOwned = false;
        property.PropertyList[propertyIndex].OwnedPlayer = -1;
        setTextDescription();
        BuyHouseBtn.interactable = false;
        SellPropertyBtn.interactable = false;
        MortgageBtn.interactable = false;
    }

    // Mortgage/Sell houses/hotel button
    private void MortgageBtnOnClick()
    {
        // Button text on click is = Sell House if they have at least 2 houses no a property
        if (property.PropertyList[propertyIndex].Type == "property" && property.PropertyList[propertyIndex].Houses > 1)
        {
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BuildCost / 2;
            property.PropertyList[propertyIndex].Houses -= 1;
            if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= (property.PropertyList[propertyIndex].BuildCost))
            {
                BuyHouseBtn.interactable = true;
            }
            else
            {
                BuyHouseBtn.interactable = false;
            }
            BuyHotelBtn.interactable = false;
        }

        // Button text on click goes from Sell House -> Mortgage
        else if (property.PropertyList[propertyIndex].Type == "property" && property.PropertyList[propertyIndex].Houses == 1)
        {
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BuildCost / 2;
            property.PropertyList[propertyIndex].Houses -= 1;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage ($" + (property.PropertyList[propertyIndex].BaseAmount / 2) + ")";

            // if the player has enough cash, they can choose to build houses
            if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= (property.PropertyList[propertyIndex].BuildCost))
            {
                BuyHouseBtn.interactable = true;
            }
            else
            {
                BuyHouseBtn.interactable = false;
            }
            SellPropertyBtn.interactable = true;
        }

        // Button text on click goes from Sell Hotel -> Mortgage
        else if (property.PropertyList[propertyIndex].Type == "property" && property.PropertyList[propertyIndex].IsHotel)
        {
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BuildCost / 2;
            property.PropertyList[propertyIndex].IsHotel = false;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage ($" + (property.PropertyList[propertyIndex].BaseAmount / 2) + ")";

            // if player has enough cash, they may buy houses after selling a hotel and becoming an unimproved property
            if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= (property.PropertyList[propertyIndex].BuildCost))
            {
                BuyHouseBtn.interactable = true;
            }
            else
            {
                BuyHouseBtn.interactable = false;
            }
            SellPropertyBtn.interactable = true;
        }

        // Button text on click goes from Mortgage -> Lift Mortgage
        else if (!property.PropertyList[propertyIndex].IsMortgaged)
        {
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BaseAmount / 2;
            property.PropertyList[propertyIndex].IsMortgaged = true;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Lift Mortgage ($" + (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f) + ")";

            // disable buying houses option if the player doesn't have enough cash
            if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] < (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f))
            {
                MortgageBtn.interactable = false;
                BuyHouseBtn.interactable = false;
            }
            BuyHouseBtn.interactable = false;
            SellPropertyBtn.interactable = false;
        }

        // Button text on click goes from Lift Mortgage -> Mortgage
        else if (property.PropertyList[propertyIndex].IsMortgaged)
        {
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] -= (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f);
            property.PropertyList[propertyIndex].IsMortgaged = false;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage ($" + (property.PropertyList[propertyIndex].BaseAmount / 2) + ")";

            // Enable/disable buy houses option if they have the cash
            if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= (property.PropertyList[propertyIndex].BuildCost))
            {
                BuyHouseBtn.interactable = true;
            }
            else
            {
                BuyHouseBtn.interactable = false;
            }
            SellPropertyBtn.interactable = true;
        }
        setTextDescription();
    }

    // Close title deeds/popup card
    private void CloseBtnOnClick()
    {
        gameObject.SetActive(false);
    }

    // Show card when they are clicked
    public void Show(Property property, Properties selectedProperty, int[] Cash)
    {
        this.Cash = Cash;
        this.property = property;
        propertyIndex = property.GetPropertyIndex(selectedProperty.PositionIndex); ;

        MortgageBtn.interactable = true;
        BuyHouseBtn.interactable = true;

        setTitle();
        setRent();
        setButtons();
        gameObject.SetActive(true);

        BuyHouseBtn.gameObject.GetComponentInChildren<Text>().text = "Buy House ($" + property.PropertyList[propertyIndex].BuildCost + ")";
        BuyHotelBtn.gameObject.GetComponentInChildren<Text>().text = "Buy Hotel ($" + property.PropertyList[propertyIndex].BuildCost + ")";
        SellPropertyBtn.gameObject.GetComponentInChildren<Text>().text = "Sell Property ($" + (property.PropertyList[propertyIndex].BaseAmount / 2) + ")";
    }

    // Set title and colour of property card
    private void setTitle()
    {
        // If the colour group of the property is dark purple (#0e3578) or dark blue (#00529e), set text colour to white, else black
        if (property.PropertyList[propertyIndex].Colour == "#0e3578" || property.PropertyList[propertyIndex].Colour == "#00529e")
        {
            TitleText.color = Color.white;
        }
        else
        {
            TitleText.color = Color.black;
        }

        Color newColor;
        ColorUtility.TryParseHtmlString(property.PropertyList[propertyIndex].Colour, out newColor);
        CardColour.color = newColor;

        TitleText.text = "TITLE DEED\n" + property.PropertyList[propertyIndex].Name;
    }

    // Set the rent values
    private void setRent()
    {
        setTextDescription();
        string text = "";

        // Set the right aligned value text for each property
        if (property.PropertyList[propertyIndex].Type == "property" || property.PropertyList[propertyIndex].Type == "station")
        {
            for (int i = 1; i < property.PropertyList[propertyIndex].Rent.Length; i++)
            {
                text += "$" + property.PropertyList[propertyIndex].Rent[i] + "\n\n";
            }
        }
        else
        {
            for (int i = 0; i < 5; i++) text += "\n";
        }
        text += "$" + (0.5f * property.PropertyList[propertyIndex].BaseAmount);
        RentValuesText.text = text;
    }

    // Set the description text for the title deed cards 
    private void setTextDescription()
    {
        // Set property text on left side of card
        string text = "";

        if (property.PropertyList[propertyIndex].Type != "utility")
        {

            // if property is unimproved, set the subtitle text
            RentTopText.text = "Rent $" + property.PropertyList[propertyIndex].Rent[0];
            if (property.PropertyList[propertyIndex].Type == "property" &&
                property.PropertyList[propertyIndex].Houses == 0 &&
                !property.PropertyList[propertyIndex].IsHotel &&
                property.PropertyList[propertyIndex].IsOwned)
            {
                RentTopText.text += " *";
            }
            else if (property.PropertyList[propertyIndex].Type == "station" &&
              property.NumberOfStationsOwned(property.PropertyList[propertyIndex].OwnedPlayer) == 1)
            {
                RentTopText.text += " *";
            }

            // if selected property is a street property, set body text
            if (property.PropertyList[propertyIndex].Type == "property")
            {
                for (int i = 1; i <= 4; i++)
                {
                    text += "With " + i + " House";
                    if (i == 1)
                    {
                        text += " ";
                    }
                    else
                    {
                        text += "s ";
                    }
                    if (property.PropertyList[propertyIndex].Houses == i)
                    {
                        text += "*";
                    }
                    text += "\n\n";
                }
                text += "With Hotel ";
                if (property.PropertyList[propertyIndex].IsHotel)
                {
                    text += "*";
                }
                text += "\n\nMortgage Value";
            }

            // if selected property is a station, set body text
            else if (property.PropertyList[propertyIndex].Type == "station")
            {
                int numberOfStationsOwned = property.NumberOfStationsOwned(property.PropertyList[propertyIndex].OwnedPlayer);
                for (int i = 2; i <= 4; i++)
                {
                    text += "If " + i + " Railroads are owned ";

                    if (numberOfStationsOwned == i)
                    {
                        text += "*";

                    }

                    text += "\n\n";
                }
                text += "Mortgage Value ";
            }
        }

        // if selected property is a utility, set body text
        else
        {
            RentTopText.text = "";

            text += "If one \"Utility\" is owned rent is 4 times amount shown on dice.\nIf both \"Utilities\" are owned rent is 10 times amount shown on dice.\n\nMortgage Value";
        }

        RentLeftText.text = text;
    }


    // Enable or disable buttons depending on the player's cash level, and follow monopoly's building rules
    private void setButtons()
    {
        int liftMortgageAmount = (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f);

        // Disable buying of houses and hotels if property selected is a station or utility
        if (property.PropertyList[propertyIndex].Type == "station" || property.PropertyList[propertyIndex].Type == "utility")
        {
            BuyHouseBtn.gameObject.SetActive(false);
            BuyHotelBtn.gameObject.SetActive(false);

            // Setting mortgage rules
            if (!property.PropertyList[propertyIndex].IsMortgaged)
            {
                MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage ($" + (property.PropertyList[propertyIndex].BaseAmount / 2) + ")";
            }
            else
            {
                MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Lift Mortgage ($" + liftMortgageAmount + ")";
                if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= liftMortgageAmount)
                {
                    MortgageBtn.interactable = true;
                }
                else
                {
                    MortgageBtn.interactable = false;
                }
            }
        }


        // if the property selected is a street property and player has enough cash, allow buying of houses
        else
        {
            BuyHouseBtn.gameObject.SetActive(true);
            BuyHotelBtn.gameObject.SetActive(true);

            if (property.PropertyList[propertyIndex].Houses < 4)
            {
                if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= (property.PropertyList[propertyIndex].BuildCost) &&
                        !property.PropertyList[propertyIndex].IsHotel)
                {
                    BuyHouseBtn.interactable = true;
                }
                else
                {
                    BuyHouseBtn.interactable = false;
                }
                BuyHotelBtn.interactable = false;
            }

            /* if player has 4 houses and has a monopoly (all properties within a colour group owned and 4 houses for each), 
             * they may buy a hotel if they have enough cash
             */
            else if (property.PropertyList[propertyIndex].Houses == 4)
            {
                if (property.CheckForMonopoly(property.PropertyList[propertyIndex].Colour, property.PropertyList[propertyIndex].OwnedPlayer))
                {
                    if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= (property.PropertyList[propertyIndex].BuildCost))
                    {
                        BuyHotelBtn.interactable = true;
                    }
                }
                else
                {
                    BuyHotelBtn.interactable = false;
                }
                BuyHouseBtn.interactable = false;
            }

            // If player has houses, allow selling of houses
            if (property.PropertyList[propertyIndex].Houses >= 1)
            {
                MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell House ($" + (property.PropertyList[propertyIndex].BuildCost / 2) + ")";
            }
            else
            {

                // If property is not mortgaged, allow selling of hotel if hotel is owned
                if (!property.PropertyList[propertyIndex].IsMortgaged)
                {
                    if (property.PropertyList[propertyIndex].IsHotel)
                    {
                        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell Hotel ($" + (property.PropertyList[propertyIndex].BuildCost / 2) + ")";
                    }
                    else
                    {
                        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage ($" + (property.PropertyList[propertyIndex].BaseAmount / 2) + ")";
                    }
                }
                else
                {

                    // If property is mortgaged, disable buying houses option, player can lift mortgage if they have the cash
                    if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= liftMortgageAmount)
                    {
                        MortgageBtn.interactable = true;
                    }
                    else
                    {
                        MortgageBtn.interactable = false;
                    }
                    MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Lift Mortgage ($" + liftMortgageAmount + ")";
                    BuyHouseBtn.interactable = false;
                }
            }
        }

        // Properties can only be sold if it has no buildings on it and is not mortgaged
        if (property.PropertyList[propertyIndex].Houses == 0 && !property.PropertyList[propertyIndex].IsHotel && !property.PropertyList[propertyIndex].IsMortgaged)
        {
            SellPropertyBtn.interactable = true;
        }
        else
        {
            SellPropertyBtn.interactable = false;
        }
    }
}
