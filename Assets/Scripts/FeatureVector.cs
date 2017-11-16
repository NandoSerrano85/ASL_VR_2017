using SQLite4Unity3d;
using System;
using System.Reflection;
using System.Text;

public class FeatureVector
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public double PalmToThumb { get; set; }
    public double PalmToIndex { get; set; }
    public double PalmToMiddle { get; set; }
    public double PalmToRing { get; set; }
    public double PalmToPinky { get; set; }
    public double ThumbToIndex { get; set; }
    public double IndexToMiddle { get; set; }
    public double MiddleToRing { get; set; }
    public double RingToPinky { get; set; }
    public double ThumbToHandNormal { get; set; }
    public double IndexToHandNormal { get; set; }
    public double MiddleToHandNormal { get; set; }
    public double RingToHandNormal { get; set; }
    public double PinkyToHandNormal { get; set; }
    public double RadiusSphere { get; set; }
    public int NumOfFingers { get; set; }
    public string Gesture { get; set; }
    public int GestureClassLabel { get; set; }

    public double [] createInputVector()
    {
        return new double []
        {
            PalmToThumb, PalmToIndex, PalmToMiddle, PalmToRing, PalmToPinky,
            ThumbToIndex, IndexToMiddle, MiddleToRing, RingToPinky,
            ThumbToHandNormal, IndexToHandNormal, MiddleToHandNormal, RingToHandNormal, PinkyToHandNormal,
            RadiusSphere, NumOfFingers,
        };
    }

    public override string ToString()
    {
        var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.FlattenHierarchy;
        PropertyInfo[] properties = GetType().GetProperties(flags);

        StringBuilder sb = new StringBuilder();

        foreach (var property in properties)
        {
            object value = property.GetValue(this, null);
            sb.AppendFormat("{0}: {1}{2}", property.Name, value != null ? value : null, Environment.NewLine);
        }

        return sb.ToString();
    }
}