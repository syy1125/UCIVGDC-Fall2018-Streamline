public class Exporter : Operator {

    public int[] expectedOutput = { 0 };
    
    public ColArray outputColumn;
    public ColArray expectedOutputColumn;
    private int colIndex = 0;

    protected override void Start()
    {
        base.Start();
        OpName = "Output";
        for(int i = 0; i < expectedOutput.Length; i++)
        {
            expectedOutputColumn.AddValue(expectedOutput[i]);
        }
    }

    public override void Step()
    {
        //Output only reads from num1 var
        GetFromReceiver();
        outputColumn.AddValue(num1);
        colIndex++;
    }

    public void ResetState()
    {
        colIndex++;
        outputColumn.ClearNums();
    }
}
