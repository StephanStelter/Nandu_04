using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//KLASSE: Variablen für Resourcen Liste  
public class Res
{
    public ResourceType resourceName; //Name
    public int resourceAmount; //Menge
    public bool isBlocked; //Freigabe
    public bool isIdentified; //Freigabe
    public string playerName; //Player
    //public bool isCloseToVillage; //Freigabe

    //Konstruktor, um ein Objekt der Klasse zu erstellen
    public Res(ResourceType resourceName, int resourceAmount, bool isBlocked, string playerName, bool isIdentified)
    {
        this.resourceName = resourceName;
        this.resourceAmount = resourceAmount;
        this.isBlocked = isBlocked;
        this.isIdentified = isIdentified;
        this.playerName = playerName;
    }
}

//KLASSE: Player
public class Player
{
    // Statische Liste, um alle Spieler zu speichern
    public static List<Player> listOfPlayers = new List<Player>();

    // Statische Felder für die Spielerbezeichnungen
    public static readonly string None = nameof(PlayerSelecting.None);  // Kein Spieler
    public static readonly string PlayerOne = nameof(PlayerSelecting.PlayerOne);
    public static readonly string PlayerTwo = nameof(PlayerSelecting.PlayerTwo);

    public bool isActivePlayer;

    //Auswahl Player
    public enum PlayerSelecting
    {
        None,
        PlayerOne,
        PlayerTwo
    }

    // Feld für den Spielernamen
    public string playerName;

    // Konstruktor, um einen Spieler mit einem bestimmten Namen zu erstellen
    public Player(string name, bool isActivePlayer)
    {
        playerName = name;
        this.isActivePlayer = isActivePlayer;

        // Füge den Spieler zur Liste aller Spieler hinzu, wenn es sich nicht um "None" handelt
        if (playerName != None)
        {
            listOfPlayers.Add(this);
        }

        this.isActivePlayer = isActivePlayer;
    }
}

[System.Serializable]
public struct ResList
{
    public int Grain, Gold, Wood, Iron, Wool, Stone, Xp, Money;

    public ResList(int food, int gold, int wood, int iron, int stone, int wool, int xp, int money)
    {
        Grain = food;
        Gold = gold;
        Wood = wood;
        Iron = iron;
        Stone = stone;
        Wool = wool;
        Xp = xp;
        Money = money;
    }

    // ✅ Addition zweier ResourceVectors
    public static ResList operator +(ResList a, ResList b)
    {
        return new ResList(
            a.Grain + b.Grain,
            a.Gold + b.Gold,
            a.Wood + b.Wood,
            a.Iron + b.Iron,
            a.Stone + b.Stone,
            a.Wool + b.Wool,
            a.Xp + b.Xp,
            a.Money + b.Money
        );
    }

    // ✅ Subtraktion zweier ResourceVectors
    public static ResList operator -(ResList a, ResList b)
    {
        return new ResList(
            a.Grain - b.Grain,
            a.Gold - b.Gold,
            a.Wood - b.Wood,
            a.Iron - b.Iron,
            a.Stone - b.Stone,
            a.Wool - b.Wool,
            a.Xp - b.Xp,
            a.Money - b.Money
        );
    }

    // ✅ Multiplikation eines ResourceVectors mit einem Scalar
    public static ResList operator *(ResList a, int b)
    {
        return new ResList(
            a.Grain * b,
            a.Gold * b,
            a.Wood * b,
            a.Iron * b,
            a.Stone * b,
            a.Wool * b,
            a.Xp * b,
            a.Money * b
        );
    }

    // ✅ Multiplikation zweier ResourceVectors
    public static ResList operator ^(ResList a, ResList b)
    {
        return new ResList(
            a.Grain * b.Grain,
            a.Gold * b.Gold,
            a.Wood * b.Wood,
            a.Iron * b.Iron,
            a.Stone * b.Stone,
            a.Wool * b.Wool,
            a.Xp * b.Xp,
            a.Money * b.Money
        );
    }

    // ✅ Randomize anhand zweier ResourceVectors
    public static ResList operator &(ResList a, ResList b)
    {
        return new ResList(
            UnityEngine.Random.Range((int)a.Grain, (int)b.Grain + 1), // Grain
            UnityEngine.Random.Range((int)a.Gold, (int)b.Gold + 1),   // Gold
            UnityEngine.Random.Range((int)a.Wood, (int)b.Wood + 1),   // Wood
            UnityEngine.Random.Range((int)a.Iron, (int)b.Iron + 1),   // Iron
            UnityEngine.Random.Range((int)a.Stone, (int)b.Stone + 1), // Stone
            UnityEngine.Random.Range((int)a.Wool, (int)b.Wool + 1),   // Wool
            UnityEngine.Random.Range((int)a.Xp, (int)b.Xp + 1),       // Xp
            UnityEngine.Random.Range((int)a.Money, (int)b.Money + 1)  // Money
        );
    }

    // ✅ Sufficientycheck zweier ResourceVectors
    public static bool operator >=(ResList a, ResList b)
    {
        return a.Grain >= b.Grain && a.Gold >= b.Gold && a.Wood >= b.Wood && a.Iron >= b.Iron && a.Wool >= b.Wool && a.Stone >= b.Stone && a.Xp >= b.Xp && a.Money >= b.Money;
    }

    // ✅ Sufficientycheck zweier ResourceVectors
    public static bool operator <=(ResList a, ResList b)
    {
        return a.Grain <= b.Grain && a.Gold <= b.Gold && a.Wood <= b.Wood && a.Iron <= b.Iron && a.Wool <= b.Wool && a.Stone <= b.Stone && a.Xp <= b.Xp && a.Money <= b.Money;

    }
    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => Grain,
                1 => Gold,
                2 => Wood,
                3 => Iron,
                4 => Stone,
                5 => Wool,
                6 => Xp,
                7 => Money,
                _ => throw new IndexOutOfRangeException("Index muss zwischen 0 und 7 liegen.")
            };
        }
        set
        {
            switch (index)
            {
                case 0: Grain = value; break;
                case 1: Gold = value; break;
                case 2: Wood = value; break;
                case 3: Iron = value; break;
                case 4: Stone = value; break;
                case 5: Wool = value; break;
                case 6: Xp = value; break;
                case 7: Money = value; break;
                default: throw new IndexOutOfRangeException("Index muss zwischen 0 und 7 liegen.");
            }
        }
    }

    // ✅ Optionale ToString-Methode für Debug-Ausgabe
    public override string ToString()
    {
        return $"Grain: {Grain}, Gold: {Gold}, Wood: {Wood}, Iron: {Iron}, Stone: {Stone}, Wool: {Wool}, Xp: {Xp}, Money: {Money}";
    }
}

[System.Serializable]
public struct VictoryVector
{
    public float CraftTech; // Handwerk & Technik
    public float Faith;     // Religion
    public float Popu;      // Bevölkerung
    public float Mili;      // Militär
    public float Politics;  // Politik
    public float Trade;     // Wirtschaft / Handel

    public VictoryVector(float craftTech, float faith, float population, float military, float politics, float trade)
    {
        CraftTech = craftTech;
        Faith = faith;
        Popu = population;
        Mili = military;
        Politics = politics;
        Trade = trade;
    }

    public float this[int index]
    {
        get
        {
            return index switch
            {
                0 => CraftTech,
                1 => Faith,
                2 => Popu,
                3 => Mili,
                4 => Politics,
                5 => Trade,
                _ => throw new IndexOutOfRangeException("Index muss zwischen 0 und 5 liegen.")
            };
        }
        set
        {
            switch (index)
            {
                case 0: CraftTech = value; break;
                case 1: Faith = value; break;
                case 2: Popu = value; break;
                case 3: Mili = value; break;
                case 4: Politics = value; break;
                case 5: Trade = value; break;
                default: throw new IndexOutOfRangeException("Index muss zwischen 0 und 5 liegen.");
            }
        }
    }

    // Addition zweier VictoryVectors
    public static VictoryVector operator +(VictoryVector a, VictoryVector b)
    {
        return new VictoryVector(
            a.CraftTech + b.CraftTech,
            a.Faith + b.Faith,
            a.Popu + b.Popu,
            a.Mili + b.Mili,
            a.Politics + b.Politics,
            a.Trade + b.Trade
        );
    }

    // Subtraktion zweier VictoryVectors
    public static VictoryVector operator -(VictoryVector a, VictoryVector b)
    {
        return new VictoryVector(
            a.CraftTech - b.CraftTech,
            a.Faith - b.Faith,
            a.Popu - b.Popu,
            a.Mili - b.Mili,
            a.Politics - b.Politics,
            a.Trade - b.Trade
        );
    }

    // Debug-Ausgabe
    public override string ToString()
    {
        return $"CraftTech: {CraftTech}, Faith: {Faith}, Popu: {Popu}, Mili: {Mili}, Politics: {Politics}, Trade: {Trade}";
    }
}


