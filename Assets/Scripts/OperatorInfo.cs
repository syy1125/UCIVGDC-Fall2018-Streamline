using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OperatorInfo {
    

    public static string GetOperatorHint(string opName)
    {
        switch (opName)
        {
            default:
                return "";
            case "Add":
                return "<Color=#00d2d6>In.1</Color> + <Color=#c9d300>In.2</Color> -> Out.1";
            case "Subtract":
                return "<Color=#00d2d6>In.1</Color> - <Color=#c9d300>In.2</Color> -> Out.1";
            case "Multiply":
                return "<Color=#00d2d6>In.1</Color> x <Color=#c9d300>In.2</Color> -> Out.1";
            case "Divide":
                return "<Color=#00d2d6>In.1</Color> / <Color=#c9d300>In.2</Color> -> Out.1";
            case "Constant":
                return "Value -> Out.1";
            case "Importer":
                return "[NEXT] -> [ANY]";
            case "Output":
                return "[ANY] -> [OUT]";
        }
    }
    public static bool[] GetIOMask(string opName)
    {
        switch (opName)
        {
            default:
                return new bool[] { true, true, true };
            case "Constant":
                return new bool[] { false, false, true };
            case "Output":
                return new bool[] { false, false, false };
            case "Importer":
                return new bool[] { false, false, false };
        }
    }
    
	
}
