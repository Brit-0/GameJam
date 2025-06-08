using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

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
        "Olha s�! O falso milion�rio tentando virar dono da natureza. Que piada..."
    };

    public static List<string> chopWithoutTrees = new()
    {
        "Voc� acabou com todas as �rvores... Isso que � um prod�gio de verdade!"
    };

    public static string SelectDialog(List<string> dialogList)
    {
        return dialogList[Random.Range(0, dialogList.Count)];
    }
}
