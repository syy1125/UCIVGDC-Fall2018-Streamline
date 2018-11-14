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
    
    public enum WireType
    {
        RED, GREEN, BOTH
    }
    private static void LoadComponent(int[] values)
    {
        Grid grid = Grid.Instance;
        Vector2Int coords = new Vector2Int(values[1], values[2]);
        Operator op = null;
        switch ((Selection)values[0])
        {
            case Selection.GREENWIRE:
            case Selection.REDWIRE:
                Wire w = grid.SetGridComponent(coords, grid.wire).GetComponent<Wire>();
                w.Location = coords;
                w.HasGreen = ((WireType)values[3] == WireType.GREEN || (WireType)values[3] == WireType.BOTH);
                w.HasRed = ((WireType)values[3] == WireType.RED || (WireType)values[3] == WireType.BOTH);
                break;
            case Selection.CONSTANT:
                Constant c = grid.SetGridComponent(coords, grid.constant).GetComponent<Constant>();
                c.LoadConfig(values);
                break;
            case Selection.ADD:
                op = grid.SetGridComponent(coords, grid.addition).GetComponent<Operator>();
                op.LoadConfig(values);
                break;
            case Selection.SUB:
                op = grid.SetGridComponent(coords, grid.subtraction).GetComponent<Operator>();
                op.LoadConfig(values);
                break;
            case Selection.MULT:
                op = grid.SetGridComponent(coords, grid.multiplication).GetComponent<Operator>();
                op.LoadConfig(values);
                break;
            case Selection.DIV:
                op = grid.SetGridComponent(coords, grid.division).GetComponent<Operator>();
                op.LoadConfig(values);
                break;
            case Selection.EQUALITY:
                op = grid.SetGridComponent(coords, grid.equality).GetComponent<Operator>();
                op.LoadConfig(values);
                break;
            case Selection.LESSTHAN:
                op = grid.SetGridComponent(coords, grid.lessThan).GetComponent<Operator>();
                op.LoadConfig(values);
                break;
        }
    }
    public static Vector2Int GetDirection(int x)
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
    public static int GetDirection(Vector2Int v)
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
            result = g.GetComponent<Wire>().SaveString();
        }
        else if (grid.IsOperator(g))
        {
            result = g.GetComponent<Operator>().SaveString();
        }
        return result;
    }
}
