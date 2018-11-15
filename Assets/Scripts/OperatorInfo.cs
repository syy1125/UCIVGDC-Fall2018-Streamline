using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class OperatorInfo {
    
   
    public static ComponentType GetTypeOf(Operator g)
    {
        switch(g.GetComponent<Operator>().OpName)
        {
            default:
                return ComponentType.NONE;
            case "Add":
                return ComponentType.ADD;
            case "Subtract":
                return ComponentType.SUB;
            case "Multiply":
                return ComponentType.MULT;
            case "Divide":
                return ComponentType.DIV;
            case "Constant":
                return ComponentType.CONSTANT;
            case "Importer":
                return ComponentType.IMPORTER;
            case "Output":
                return ComponentType.EXPORTER;
            case "Equality":
                return ComponentType.EQUALITY;
            case "LessThan":
                return ComponentType.LESSTHAN;
            case "Compare":
                return ComponentType.COMPARE;
        }

    }
    
	
}
