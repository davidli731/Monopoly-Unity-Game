using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Cards
{
    public string Name;
    public string Tag;
    public bool IsDebit; // +
    public bool IsCredit; // -
    public bool IsMove;
    public bool IsOther;
    public int DebitAmount;
    public int CreditAmount;
    public int MoveToIndexPosition;

    /*
     * IsOther boolean is needed to check for additional, specific card functions, 
     * i.e., go to nearest station, get out of jail free card, pay twice normal rent, etc.
     * */
    public Cards(
        string name,
        string tag,
        bool isDebit,
        bool isCredit,
        bool isMove,
        bool isOther,
        int debitAmount,
        int creditAmount,
        int moveToIndexPosition)
    {
        this.Name = name;
        this.Tag = tag;
        this.IsDebit = isDebit;
        this.IsCredit = isCredit;
        this.IsMove = isMove;
        this.IsOther = isOther;
        this.DebitAmount = debitAmount;
        this.CreditAmount = creditAmount;
        this.MoveToIndexPosition = moveToIndexPosition;
    }
}

public class LuckCards : MonoBehaviour
{

    System.Random rnd = new System.Random();

    // Chance cards
    public Cards[] ChanceCardList = new Cards[]
    {
        new Cards(
            "GO DIRECTLY TO JAIL, DO NOT PASS GO, DO NOT COLLECT $200",
            "Go to Jail",
            false,
            false,
            true,
            false,   // Don't collect $200 if pass go
            0,
            0,
            10),
        new Cards(
            "ADVANCE TO ILLINOIS AVE.",
             "Illinois Avenue",
            true,
            false,
            true,
            false,
            200,
            0,
            24),
        new Cards(
            "ADVANCE TO GO (COLLECT $200)",
            "GO",
            true,
            false,
            true,
            false,
            200,
            0,
            0),
        new Cards(
            "ADVANCE TOKEN TO NEAREST UTILITY. IF UNOWNED YOU MAY BUY IT FROM BANK. IF OWNED, THROW DICE AND PAY OWNER A TOTAL TEN TIMES THE AMOUNT THROWN",
             "Utility",
            false,
            false,
            true,
            true,   // move to nearest utility, pay 10* throw die count. Able to buy if unowned
            200,
            0,
            0),
        new Cards(
            "YOUR BUILDING AND LOAN MATURES, COLLECT $150",
            "Debit",
            true,
            false,
            false,
            false,
            150,
            0,
            0),
        new Cards(
            "ADVANCE TO ST. CHARLES PLACE. IF PASS GO, COLLECT $200",
             "St. Charles Place",
            false,
            false,
            true,
            false,
            200,
            0,
            11),
        new Cards(
            "TAKE A WALK ON THE BOARD WALK",
             "Boardwalk",
            false,
            false,
            true,
            false,
            0,
            0,
            39),
        new Cards(
            "ADVANCE TOKEN TO THE NEAREST RAILROAD AND PAY OWNER TWICE THE RENTAL TO WHICH HE IS OTHERWISE ENTITLED. IF RAILROAD IS UNOWNED, YOU MAY BUY IT FROM THE BANK",
             "Railroad",
            false,
            false,
            true,
            true,   // if owned, pay owner twice the usual rent, else player is able to buy rr
            0,
            0,
            0),
        new Cards(
            "TAKE A RIDE ON THE READING. IF YOU PASS GO, COLLECT $200",
            "Reading Railroad",
            true,
            false,
            true,
            false,
            200,    // Going to Reading RR from chance always passes go
            0,
            5),
        new Cards(
            "BANK PAYS YOU DIVIDEND OF $50",
            "Debit",
            true,
            false,
            false,
            false,
            50,
            0,
            0),
        new Cards(
            "THIS CARD MAY BE KEPT UNTIL NEEDED OR SOLD. GET OUT OF JAIL FREE",
            "Get Out of Jail Free",
            false,
            false,
            false,
            true,   // Can be kept, get out of jail free card
            0,
            0,
            0),
        new Cards(
            "MAKE GENERAL REPAIRS ON ALL YOUR PROPERTY. FOR EACH HOUSE PAY $25, FOR EACH HOTEL $100",
            "Credit",
            false,
            false,
            false,
            true,   // 25* for each house, 100* for each hotel
            0,
            0,
            0),
        new Cards(
            "PAY POOR TAX OF $15",
            "Credit",
            false,
            true,
            false,
            false,
            0,
            15,
            0),
        new Cards(
            "ADVANCE TOKEN TO THE NEAREST RAILROAD AND PAY OWNER TWICE THE RENTAL TO WHICH HE IS OTHERWISE ENTITLED. IF RAILROAD IS UNOWNED, YOU MAY BUY IT FROM THE BANK",
            "Railroad",
            false,
            false,
            false,
            true,   // if owned, pay owner twice the usual rent, else player is able to buy rr
            0,
            0,
            0),
        new Cards(
            "YOU HAVE BEEN ELECTED CHAIRMAN OF THE BOARD. PAY EACH PLAYER $50",
            "Credit",
            false,
            true,
            false,
            true,   // Other player receives $50
            0,
            50,
            0),
        new Cards(
            "GO BACK 3 SPACES",
            "Back 3",
            false,
            false,
            false,
            true,   // Move back 3 spaces from current position
            0,
            0,
            0),
    };

    // Community Cards
    public Cards[] CommunityCardList = new Cards[]
    {
        new Cards(
            "LIFE INSURANCE MATURES, COLLECT $100",
            "Debit",
            true,
            false,
            false,
            false,
            100,
            0,
            0),
        new Cards(
            "DOCTOR'S FREE, PAY $50",
             "Credit",
            false,
            true,
            false,
            false,
            0,
            50,
            0),
        new Cards(
            "PAY SCHOOL TAX OF $150",
            "Credit",
            true,
            false,
            false,
            false,
            0,
            150,
            0),
        new Cards(
            "XMAS FUND MATURES, COLLECT $100",
             "Debit",
            true,
            false,
            false,
            false,
            100,
            0,
            0),
        new Cards(
            "GO TO JAIL. GO DIRECTLY TO JAIL. DO NOT PASS GO. DO NOT COLLECT $200",
            "Go to Jail",
            false,
            false,
            true,
            false,   // Don't collect $200 if pass go
            0,
            0,
            10),
        new Cards(
            "FROM SALE OF STOCK, YOU GET $45",
             "Debit",
            true,
            false,
            false,
            false,
            45,
            0,
            0),
        new Cards(
            "GRAND OPERA OPENING. COLLECT $50 FROM EVERY PLAYER",
             "Debit",
            true,
            false,
            false,
            true,
            50, // Get $50 from other player
            0,
            0),
        new Cards(
            "YOU ARE ASSESSED FOR STREET REAPAIRS. $40 PER HOUSE, $115 PER HOTEL",
             "Credit",
            false,
            true,
            false,
            true,   // 40* each house, 115* each hotel
            0,
            0,
            0),
        new Cards(
            "INCOME TAX REFUND, COLLECT $20",
            "Debit",
            true,
            false,
            false,
            false,
            20,
            0,
            0),
        new Cards(
            "BANK ERROR IN YOUR FAVOR, COLLECT $200",
            "Debit",
            true,
            false,
            false,
            false,
            200,
            0,
            0),
        new Cards(
            "GET OUT OF JAIL, FREE. THIS CARD MAY BE KEPT UNTIL NEEDED OR SOLD",
            "Get Out of Jail Free",
            false,
            false,
            false,
            true,   // Can be kept, get out of jail free card
            0,
            0,
            0),
        new Cards(
            "YOU HAVE WON SECOND PRIZE IN A BEAUTY CONTEST, COLLECT $10",
            "Debit",
            true,
            false,
            false,
            false,
            10,
            0,
            0),
        new Cards(
            "YOU INHERIT $100",
            "Debit",
            true,
            false,
            false,
            false,
            100,
            0,
            0),
        new Cards(
            "ADVANCE TO GO (COLLECT $200)",
            "GO",
            true,
            false,
            true,
            false,
            200,
            0,
            0),
        new Cards(
            "PAY HOSPITAL $100",
            "Credit",
            false,
            true,
            false,
            false,
            0,
            100,
            0),
        new Cards(
            "RECEIVE FOR SERVICES, $25",
            "Debit",
            true,
            false,
            false,
            false,
            25,
            0,
            0),
    };

    // Get random number to decide what card to select
    public int GetRandomIndex()
    {
        return rnd.Next(0, 15);
    }
}
