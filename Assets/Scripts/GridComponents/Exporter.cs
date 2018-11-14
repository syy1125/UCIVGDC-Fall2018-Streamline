public class Exporter : Operator {

    public int[] expectedOutput = { 0 };
    
    public ColArray outputColumn;
    public ColArray expectedOutputColumn;

    protected override void Start()
    {
        base.Start();
        OpName = "Output";
        for(int i = 0; i < expectedOutput.Length; i++)
        {
            expectedOutputColumn.AddValue(expectedOutput[i]);
        }
        GameController.outputColumns.Add(GetComponent<Exporter>());
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
}
