using System.Collections.Generic;
using UnityEngine;

public static class CapitalistDialog
{
    public static List<string> buyMachine = new() 
    { 
        
    };

    public static List<string> failedBuyMachine = new()
    {
        "Ora ora... Sem tost�es e ainda quer negociar",
        "Por um s� momento pensei que voc� pudesse ser �til. Que desperd�cio...",
        "Volte quando suas moedas valerem mais do que meu tempo!"
    };

    public static List<string> failedBuyHelper = new()
    {
        "Olha s�! O falso milhon�rio tentando virar dono da natureza. Que piada..."
    };

    public static string SelectDialog(List<string> dialogList)
    {
        return dialogList[Random.Range(0, dialogList.Count)];
    }
}
