public class Exporter : Operator {

    public int[] expectedOutput = { 0 };
    
    public ColArray outputColumn; //values will be distributed from left to right, looping
    private int colIndex = 0;

    protected override void Start()
    {
        base.Start();
        OpName = "Output";
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
