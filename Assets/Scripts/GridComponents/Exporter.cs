public class Exporter : Operator
{

    private int[] _expectedOutput = {0};

    public int[] ExpectedOutput
    {
        get { return _expectedOutput;  }
        set
        {
            _expectedOutput = value;
            UpdateColumns();
        }
    }
    
    public ColArray outputColumn;
    public ColArray expectedOutputColumn;

    protected override void Start()
    {
        base.Start();
        OpName = "Output";
        
        GameController.outputColumns.Add(this);
    }

    private void UpdateColumns()
    {
        expectedOutputColumn.ClearNums();
        outputColumn.ClearNums();
        
        for (int i = 0; i < ExpectedOutput.Length; i++)
        {
             expectedOutputColumn.AddValue(ExpectedOutput[i]);
        }
    }

    public override void Step()
    {
        //Output only reads from num1 var
        GetFromReceiver();
        if(num1 != 0)
            outputColumn.AddValue(num1);
    }

    public void ResetState()
    {
        outputColumn.ClearNums();
    }
    public bool allCorrect()
    {
        return expectedOutputColumn.Matches(outputColumn);
    }
    public override string GetHint()
    {
        return "[ANY] -> [OUT]";

	}
    public override bool[] GetIOMask()
	{
		return new bool[3]{false,false,false};
	}
    public override string SaveString()
	{
		return "";
	}
}
