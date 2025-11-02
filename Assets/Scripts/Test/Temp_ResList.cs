using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp_ResList : MonoBehaviour
{
    public void ResListTestMethode()
    {
        // Was ist eine ResList?______________________________________________________________________________________________________________________

        //- Eine Liste die für Jede Resource einen Wert speichert
        //- Eine Liste in der vorgefertigte Operatoren zur Verfügung stehen um das Handling so einfach wie möglich zu gestallten
        //- Eine Liste die durch ihre Struktur und Operatoren leicht verständlich ist
        //- Ein eigenerDatentyp der für Ressourcen verwendet wird

        //Beispiel der Basis-Struktur der ResList_____________________________________________________________________________________________________

        //public struct ResList
        //{
        //    public float Grain, Gold, Wood, Iron, Wool, Stone, Xp, Money;
        //}




        // Wie kann ich eine ResList erstellen?_______________________________________________________________________________________________________
        ResList exampleList1 = new ResList(0, 1, 2, 3, 4, 5, 6, 7);

        ResList exampleList2 = new ResList(7, 6, 5, 4, 3, 2, 1, 0);

        ResList exampleList3 = new ResList(0, 0, 0, 0, 0, 0, 0, 0);

        Debug.Log("Erstellte ResList: " + exampleList1);

        //Wie kann ich auf einzelne Ressourcen zugreifen?____________________________________________________________________________________________________
        // Zugriff über den Namen der Resource
        exampleList1.Gold = 0;

        Debug.Log("Geänderter Gold-Wert: " + exampleList1);

        exampleList2.Grain = exampleList2.Grain + 1;

        Debug.Log("Geänderter Grain-Wert: " + exampleList2);

        // Zugriff über den Index der Resource (0-7) mit Array-Syntax
        exampleList3[2] = 22;

        Debug.Log("Geänderter Wood-Wert: " + exampleList3);

        // Zugriff auf einzelne Resourcen und speichern in Variablen
        float stoneValue = exampleList2[4];
        float ironValue = exampleList2.Iron;

        Debug.Log("Steinwert und Eisenwert: " + stoneValue + " und " + ironValue);



        // Was kann ich mit einer ResList machen?____________________________________________________________________________________________________

        //- Addieren
        //- Subtrahieren
        //- Multiplizieren
        //- Zufällige Zahlen generieren
        //- Vergleichen ob eine ResList ausreichend gedeckt ist
        //- Ansprechen einzelner Ressourcen
        //- Einfaches Debuggen durch ToString Methode
        //- Und vieles mehr...


        //Wie kann ich ResLists Addieren?_____________________________________________________________________________________________________________
        ResList TotalAmount = new ResList(1, 1, 1, 1, 1, 1, 1, 1);

        ResList AdditionalAmount = new ResList(0, 2, 4, 6, 8, 10, 12, 14);

        // Addiere ResLists einfach mit Plus (+) Operator wie jede andere Zahl auch
        ResList result = TotalAmount + AdditionalAmount;

        Debug.Log("Ergebnis der Addition: " + result);




        // Wie kann ich ResLists SUbtraieren?_________________________________________________________________________________________________________
        ResList NegativeAmount = new ResList(0, 2, 0, 1, 1, 0, 0, 1);

        // Subtraieren Reslistes einfach mit Minus (-) Operator wie jede andere Zahl auch
        result = TotalAmount - NegativeAmount;

        Debug.Log("Ergebnis der Subtraktion: " + result);



        // Wie kann ich ResLists Multiplizieren?______________________________________________________________________________________________________
        // Es Gibt zwei varianten der Multiplikation

        // 1. Multiplikation mit einem Faktor
        int beispielFaktor = 2;        

        // Multipliaktion mit einem Faktor einfach mit Sternchen (*) Operator wie jede andere Zahl auch
        result = AdditionalAmount * beispielFaktor;

        Debug.Log("Ergebnis der Multiplikation mit Faktor: " + result);

        // 2. Multiplikation mit einer anderen ResList
        // Multiplikation mit einer anderen ResList einfach mit Zirkumflex (^) Operator eine Syntax-Eigenkreation
        ResList beispielFaktorList = new ResList(10, 10, 1, 1, 10, 10, 1, 100);

        result = AdditionalAmount ^ beispielFaktorList;

        Debug.Log("Ergebnis der Multiplikation mit ResLists: " + result);

        //Division ist bisher nicht vorgesehen aber grundsätzlich möglich durch Multiplikation mit dem Kehrwert (1/x) oder
        //auch noch erweiterbar.



        // Wie kann ich ResLists Vergleichen?_________________________________________________________________________________________________________
        ResList lowerAmount = new ResList(0, 1, 2, 3, 4, 5, 6, 7);

        ResList equalAmount = new ResList(1, 1, 1, 1, 1, 1, 1, 1);

        ResList higherAmount = new ResList(1, 2, 2, 3, 4, 5, 6, 7);

        // Ausreichendvorhanden?
        if (lowerAmount >= equalAmount)
        {
            Debug.Log("Reicht lowerAm. aus um equalAm. zu decken in jeder Resource?: Ja");
        }
        else
        {
            Debug.Log("Reicht lowerAm. aus um equalAm. zu decken in jeder Resource?: Nein");
        }

        if (higherAmount >= equalAmount)
        {
            Debug.Log("Reicht HighterAm. aus um equalAm. zu decken in jeder Resource?: Ja");
        }

        if (lowerAmount <= higherAmount)
        {
            Debug.Log("Ist lowerAm. kleiner/gleich als HighterAm. in jeder Resource?: Ja");
        }
        else
        {
            Debug.Log("Ist lowerAm. kleiner/gleich als HighterAm. in jeder Resource?: Nein");
        }



        // Zufällige Zahlen? Zufällig kein Problem!!
        ResList unterGrenze = new ResList(1, 1, 1, 1, 1, 1, 1, 1);

        ResList oberGrenze = new ResList(10, 10, 10, 10, 10, 10, 10, 10);

        // ZufallsOperation mit kaufmännischem Und (&) Operator eine Syntax-Eigenkreation
        result = unterGrenze & oberGrenze;

        Debug.Log("Ergebnis der Zufallszahlen: " + result);

        // Kombinierte Operationen sind auch möglich
        result = (unterGrenze * 4) & oberGrenze;

        Debug.Log("Ergebnis der Zufallszahlen in kombination: " + result);





        //Beispiele aus dem Spielkontext______________________________________________________________________________________________________________


        //    private void SetResourceValues(GameObject hex)
        //    {
        //        Biom biom = hex.GetComponent<Hex>().biom;

        //        ResList min = minValues[biom];
        //        ResList max = maxValues[biom];
        //        ResList randomized = min & max;

        //        Hex tile = hex.GetComponent<Hex>();
        //        tile.minValues = min;
        //        tile.maxValues = max;
        //        tile.actValues = randomized;
        //    }

        //    void InitTerrainResources()
        //    {
        //        // Berg
        //        minValues[Biom.Berg] = new ResList(0, 1, 0, 2, 2, 0, 0, 0);
        //        maxValues[Biom.Berg] = new ResList(1, 3, 1, 4, 4, 1, 0, 0);

        //        // Feld
        //        minValues[Biom.Feld] = new ResList(2, 0, 0, 0, 0, 1, 0, 0);
        //        maxValues[Biom.Feld] = new ResList(4, 1, 1, 1, 1, 2, 0, 0);

        //        // Hügelig
        //        minValues[Biom.Hügelig] = new ResList(1, 1, 1, 1, 1, 1, 0, 0);
        //        maxValues[Biom.Hügelig] = new ResList(3, 2, 2, 2, 2, 2, 0, 0);

        //        // Moor
        //        minValues[Biom.Moor] = new ResList(0, 0, 1, 0, 0, 1, 0, 0);
        //        maxValues[Biom.Moor] = new ResList(2, 1, 3, 1, 1, 3, 0, 0);

        //        // Wald
        //        minValues[Biom.Wald] = new ResList(1, 0, 2, 0, 1, 2, 0, 0);
        //        maxValues[Biom.Wald] = new ResList(2, 1, 4, 1, 2, 4, 0, 0);

        //        // Wiese
        //        minValues[Biom.Wiese] = new ResList(2, 0, 1, 0, 1, 2, 0, 0);
        //        maxValues[Biom.Wiese] = new ResList(3, 1, 2, 1, 2, 4, 0, 0);

        //        // Wüste
        //        minValues[Biom.Wüste] = new ResList(0, 1, 0, 0, 0, 0, 0, 0);
        //        maxValues[Biom.Wüste] = new ResList(1, 4, 1, 1, 1, 1, 0, 0);

        //        // Sea
        //        minValues[Biom.Sea] = new ResList(0, 0, 1, 0, 0, 2, 0, 0);
        //        maxValues[Biom.Sea] = new ResList(1, 1, 2, 1, 1, 4, 0, 0);
        //    }


        //HEX

        //    public ResList minValues;
        //    public ResList maxValues;
        //    public ResList actValues;
        //    public ResList cost;
        //    public ResList productionMultiplier;


}
}
