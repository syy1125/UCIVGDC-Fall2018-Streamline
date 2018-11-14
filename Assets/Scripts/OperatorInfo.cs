using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OperatorInfo {
    
   
    public static Selection GetTypeOf(Operator g)
    {
        switch(g.GetComponent<Operator>().OpName)
        {
            default:
                return Selection.NONE;
            case "Add":
                return Selection.ADD;
            case "Subtract":
                return Selection.SUB;
            case "Multiply":
                return Selection.MULT;
            case "Divide":
                return Selection.DIV;
            case "Constant":
                return Selection.CONSTANT;
            case "Importer":
                return Selection.IMPORTER;
            case "Output":
                return Selection.EXPORTER;
            case "Equality":
                return Selection.EQUALITY;
            case "LessThan":
                return Selection.LESSTHAN;
            case "Compare":
                return Selection.COMPARE;
        }

    }
    
	
}
