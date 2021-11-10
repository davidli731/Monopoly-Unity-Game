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
        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell House";
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
        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell Hotel";
    }

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

    private void MortgageBtnOnClick()
    {
        if (property.PropertyList[propertyIndex].Type == "property" && property.PropertyList[propertyIndex].Houses > 1)
        {
            // Button text = Sell House
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
        else if (property.PropertyList[propertyIndex].Type == "property" && property.PropertyList[propertyIndex].Houses == 1)
        {
            // Button text = Sell House -> Mortgage
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BuildCost / 2;
            property.PropertyList[propertyIndex].Houses -= 1;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage";
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
        else if (property.PropertyList[propertyIndex].Type == "property" && property.PropertyList[propertyIndex].IsHotel)
        {
            // Button text = Sell Hotel -> Mortgage
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BuildCost / 2;
            property.PropertyList[propertyIndex].IsHotel = false;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage";
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
        else if (!property.PropertyList[propertyIndex].IsMortgaged)
        {
            // Button text = Mortgage -> Lift Mortgage
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] += property.PropertyList[propertyIndex].BaseAmount / 2;
            property.PropertyList[propertyIndex].IsMortgaged = true;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Lift Mortgage ($" + (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f) + ")";
            if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] < (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f))
            {
                MortgageBtn.interactable = false;
                BuyHouseBtn.interactable = false;
            }
            BuyHouseBtn.interactable = false;
            SellPropertyBtn.interactable = false;
        }
        else if (property.PropertyList[propertyIndex].IsMortgaged)
        {
            // Button text = Lift Mortgage -> Mortgage
            Cash[property.PropertyList[propertyIndex].OwnedPlayer] -= (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f);
            property.PropertyList[propertyIndex].IsMortgaged = false;
            MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage";
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

    private void CloseBtnOnClick()
    {
        gameObject.SetActive(false);
    }

    public void Show(Property property, Properties selectedProperty, int[] Cash)
    {
        this.Cash = Cash;
        this.property = property;
        propertyIndex = property.GetPropertyIndex(selectedProperty.PositionIndex); ;

        MortgageBtn.interactable = true;
        BuyHouseBtn.interactable = true;

        if (property.PropertyList[propertyIndex].Type == "property")
        {
            BuyHouseBtn.gameObject.SetActive(true);
            BuyHotelBtn.gameObject.SetActive(true);
        }
        else
        {
            BuyHouseBtn.gameObject.SetActive(false);
            BuyHotelBtn.gameObject.SetActive(false);
        }

        setTitle();
        setRent();
        setButtons();
        gameObject.SetActive(true);
    }

    // Set title and colour of property card
    private void setTitle()
    {
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

    private void setRent()
    {
        setTextDescription();
        string text = "";

        // Set text on right side of card
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

    private void setTextDescription()
    {
        // Set property text on left side of card
        string text = "";

        int index = property.GetPropertyIndex(property.PropertyList[propertyIndex].PositionIndex);

        if (property.PropertyList[propertyIndex].Type != "utility")
        {
            RentTopText.text = "Rent $" + property.PropertyList[propertyIndex].Rent[0];
            if (property.PropertyList[index].Houses == 0 && !property.PropertyList[index].IsHotel && property.PropertyList[index].IsOwned)
            {
                RentTopText.text += " *";
            }

            // is property
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
                    if (property.PropertyList[index].Houses == i)
                    {
                        text += "*";
                    }
                    text += "\n\n";
                }
                text += "With Hotel ";
                if (property.PropertyList[index].IsHotel)
                {
                    text += "*";
                }
                text += "\n\nMortgage Value";
            }

            // is station
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

        // is utility
        else
        {
            RentTopText.text = "";

            text += "If one \"Utility\" is owned rent is 4 times amount shown on dice.\nIf both \"Utilities\" are owned rent is 10 times amount shown on dice.\n\nMortgage Value";
        }

        RentLeftText.text = text;
    }

    private void setButtons()
    {
        int liftMortgageAmount = (int)((property.PropertyList[propertyIndex].BaseAmount / 2) * 1.1f);

        if (property.PropertyList[propertyIndex].Type == "station" || property.PropertyList[propertyIndex].Type == "utility")
        {
            BuyHouseBtn.gameObject.SetActive(false);
            BuyHotelBtn.gameObject.SetActive(false);
            if (!property.PropertyList[propertyIndex].IsMortgaged)
            {
                MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage";
            }
            else
            {
                MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Lift Mortgage ($" + liftMortgageAmount + ")";
                if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= liftMortgageAmount)
                {
                    MortgageBtn.interactable = false;
                }
                else
                {
                    MortgageBtn.interactable = true;
                }
            }
        }

        // Selected property
        else
        {
            if (property.PropertyList[propertyIndex].Houses < 4)
            {
                if (property.PropertyList[propertyIndex].Type == "station" || property.PropertyList[propertyIndex].Type == "utility")
                {
                    BuyHouseBtn.gameObject.SetActive(true);
                    BuyHotelBtn.gameObject.SetActive(false);
                }
                else
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
            }
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


            if (property.PropertyList[propertyIndex].Houses >= 1)
            {
                MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell House";
            }
            else
            {
                if (!property.PropertyList[propertyIndex].IsMortgaged)
                {
                    if (property.PropertyList[propertyIndex].IsHotel)
                    {
                        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Sell Hotel";
                    }
                    else
                    {
                        MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Mortgage";
                    }
                }
                else
                {
                    if (Cash[property.PropertyList[propertyIndex].OwnedPlayer] >= liftMortgageAmount)
                    {
                        MortgageBtn.interactable = true;
                    }
                    else
                    {
                        MortgageBtn.interactable = false;
                    }
                    MortgageBtn.gameObject.GetComponentInChildren<Text>().text = "Lift Mortgage ($" + liftMortgageAmount + ")";
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
