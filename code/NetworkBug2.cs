using System.Linq;
using System.Collections.Generic;
using Sandbox;

namespace Cinema;

public partial class MyObject : BaseNetworkable
{
    [Net]
    public string Value1 { get; set; }

    [Net]
    public string Value2 { get; set; }

    public override string ToString()
    {
        return $"MyObject: {Value1} ({Value2})";
    }

}

public partial class NetworkBugTest : Entity
{
    [Net]
    public IList<MyObject> MyList { get; set; }

    [Net]
    public MyObject ObjectReference { get; set; }

    private int NumTicks = 0;

    public override void Spawn()
    {
        Transmit = TransmitType.Always;

        ObjectReference = null;
    }

    [GameEvent.Tick.Server]
    protected void TickServer()
    {
        ++NumTicks;

        if (NumTicks == 1)
        {
            var testObject = new MyObject()
            {
                Value1 = "Hello",
                Value2 = "World"
            };
            MyList.Add(testObject);
            //ObjectReference = testObject; // Comment me out, bug goes away
        }

        if (NumTicks == 5)
        {
            MyList.RemoveAt(0);
            return;
        }

        if (NumTicks < 20)
            Log.Info(MyList.Count);
    }


    [GameEvent.Tick.Client]
    protected void TickClient()
    {
        ++NumTicks;

        if (NumTicks > 20)
            return;

        Log.Info(MyList.Count);
        if (MyList.Count > 0)
        {
            Log.Info(MyList.FirstOrDefault());
        }

    }
}
