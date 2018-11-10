using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public static class SaveData {


    /*
     *         C:/Users/[USER]/AppData/LocalLow/DefaultCompany/Streamline
     * Data format:
     * Tab separated values (tsv)
     * Each level has separate save file, which is "SaveData" + levelNumber
     * operators:   [Component Type],[X],[Y],[Input1Direction],[Input2Direction],[OutputDirection]
     * Constant:    [Component Type],[X],[Y],[OutputDirection],[Value]
     * Wires:       [Component Type],[X],[Y],[Enum--RED,GREEN,BOTH]
     */
    public static void LoadData(string levelName, int solutionNum)
    {
        string path = Application.persistentDataPath;
        path += "\\SaveData" + levelName + solutionNum.ToString() + ".txt";
        if (File.Exists(path))
        {
            Debug.Log("Loading Data from " + path);
            StreamReader streamReader = new StreamReader(path);
            while (!streamReader.EndOfStream)
            {
                string line = streamReader.ReadLine();
                string[] data = line.Split('\t');
                int[] values = new int[data.Length];
                for (int i = 0; i < data.Length; i++)
                {
                    values[i] = int.Parse(data[i]);
                }
                LoadComponent(values);


            }
            streamReader.Close();
        } else
        {
            Debug.Log("Save File not Found: " + path);

        }
    }
    private enum Components
    {
        WIRE, ADD, SUB, MULT, DIV, CONSTANT
    }
    private enum WireType
    {
        RED, GREEN, BOTH
    }
    private static void LoadComponent(int[] values)
    {
        Grid grid = Grid.Instance;
        Vector2Int coords = new Vector2Int(values[1], values[2]);
        Transform op = null;
        switch ((Components)values[0])
        {
            case Components.WIRE:
                Wire w = grid.SetGridComponent(coords, grid.wire).GetComponent<Wire>();
                w.Location = coords;
                w.HasGreen = ((WireType)values[3] == WireType.GREEN || (WireType)values[3] == WireType.BOTH);
                w.HasRed = ((WireType)values[3] == WireType.RED || (WireType)values[3] == WireType.BOTH);
                break;
            case Components.CONSTANT:
                Transform c = grid.SetGridComponent(coords, grid.constant).transform;
                c.GetComponent<Transmitter>().OutputDirection = GetDirection(values[3]);
                c.GetComponent<Constant>().number = values[4];
                break;
            case Components.ADD:
                op = grid.SetGridComponent(coords, grid.addition).transform;
                op.GetComponent<Receiver>().InputDirection1 = GetDirection(values[3]);
                op.GetComponent<Receiver>().InputDirection2 = GetDirection(values[4]);
                op.GetComponent<Transmitter>().OutputDirection = GetDirection(values[5]);
                break;
            case Components.SUB:
                op = grid.SetGridComponent(coords, grid.subtraction).transform;
                op.GetComponent<Receiver>().InputDirection1 = GetDirection(values[3]);
                op.GetComponent<Receiver>().InputDirection2 = GetDirection(values[4]);
                op.GetComponent<Transmitter>().OutputDirection = GetDirection(values[5]);
                break;
            case Components.MULT:
                op = grid.SetGridComponent(coords, grid.multiplication).transform;
                op.GetComponent<Receiver>().InputDirection1 = GetDirection(values[3]);
                op.GetComponent<Receiver>().InputDirection2 = GetDirection(values[4]);
                op.GetComponent<Transmitter>().OutputDirection = GetDirection(values[5]);
                break;
            case Components.DIV:
                op = grid.SetGridComponent(coords, grid.division).transform;
                op.GetComponent<Receiver>().InputDirection1 = GetDirection(values[3]);
                op.GetComponent<Receiver>().InputDirection2 = GetDirection(values[4]);
                op.GetComponent<Transmitter>().OutputDirection = GetDirection(values[5]);
                break;
        }
    }
    private static Vector2Int GetDirection(int x)
    {
        switch (x)
        {
            case 0:
                return Vector2Int.right;
            case 1:
                return Vector2Int.up;
            case 2:
                return Vector2Int.left;
            case 3:
                return Vector2Int.down;
            default:
                return Vector2Int.right;
        }
    }
    private static int GetDirection(Vector2Int v)
    {
        if (v == Vector2Int.right)
            return 0;
        if (v == Vector2Int.up)
            return 1;
        if (v == Vector2Int.left)
            return 2;
        return 3;
    }
    public static void WriteData(string levelName, int solutionNum)
    {
        //C:/Users/[USER]/AppData/LocalLow/Streamline/Streamline
        string path = Application.persistentDataPath;
        path += "\\SaveData" + levelName + solutionNum.ToString() + ".txt";
        System.IO.File.WriteAllText(path, string.Empty);
        Debug.Log("Saving Data to " + path);
        StreamWriter writer = new StreamWriter(path,false);
        GameObject g = null;
       
        for(int i = 0; i < Grid.Instance.Width; i++)
        {
            for(int j = 0; j < Grid.Instance.Height; j++)
            {
                g = Grid.Instance.GetGridComponent(i, j);
                string componentData = TranslateComponent(i, j, g);
                if (!componentData.Equals(""))
                    writer.WriteLine(componentData);
            }
        }
        writer.Close();  
    }
    public static string TranslateComponent(int x, int y, GameObject g)
    {
        if (g == null)
            return "";
        string result = "";
        Grid grid = Grid.Instance;
        if (grid.IsWire(g))
        {
            Wire wire = g.GetComponent<Wire>();
            result += "" + (int)Components.WIRE + "\t";
            result += "" + x + "\t";
            result += "" + y + "\t";
            if (wire.HasGreen && wire.HasRed)
                result += "" + (int)WireType.BOTH;
            else if(wire.HasGreen)
                result += "" + (int)WireType.GREEN;
            else
                result += "" + (int)WireType.RED;

        }
        else if (grid.IsOperator(g))
        {
            switch (g.GetComponent<Operator>().OpName)
            {
                case "Add":
                    result += ((int)Components.ADD).ToString() + "\t";
                    break;
                case "Subtract":
                    result += ((int)Components.SUB).ToString() + "\t";
                    break;
                case "Multiply":
                    result += ((int)Components.MULT).ToString() + "\t";
                    break;
                case "Divide":
                    result += ((int)Components.DIV).ToString() + "\t";
                    break;
                case "Constant":
                    result += ((int)Components.CONSTANT).ToString() + "\t";
                    break;
                default:
                    //Do not save importers and exporters
                    return "";
            }
            result += x.ToString() + "\t";
            result += y.ToString() + "\t";
            if (g.GetComponent<Constant>() != null)
            {
                result += GetDirection(g.GetComponent<Transmitter>().OutputDirection).ToString() + "\t";
                result += g.GetComponent<Constant>().number.ToString();
            }
            else
            {
                result += GetDirection(g.GetComponent<Receiver>().InputDirection1).ToString() + "\t";
                result += GetDirection(g.GetComponent<Receiver>().InputDirection2).ToString() + "\t";
                result += GetDirection(g.GetComponent<Transmitter>().OutputDirection).ToString();
            }
        }
        return result;
    }
}
