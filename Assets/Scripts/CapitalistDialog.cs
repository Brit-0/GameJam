using System.Collections.Generic;
using UnityEngine;

public static class CapitalistDialog
{
    public static List<string> buyMachine = new() 
    { 
        
    };

    public static List<string> failedBuyMachine = new()
    {
        "Ora ora... Sem tostões e ainda quer negociar",
        "Por um só momento pensei que você pudesse ser útil. Que desperdício...",
        "Volte quando suas moedas valerem mais do que meu tempo!"
    };

    public static List<string> failedBuyHelper = new()
    {
        "Olha só! O falso milhonário tentando virar dono da natureza. Que piada..."
    };

    public static string SelectDialog(List<string> dialogList)
    {
        return dialogList[Random.Range(0, dialogList.Count)];
    }
}
