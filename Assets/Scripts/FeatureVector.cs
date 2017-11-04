using SQLite4Unity3d;
using System.Collections.Generic;

public class FeatureVector
{
    [PrimaryKey, AutoIncrement]
    public int ID { get; set; }
    public float PalmToThumb { get; set; }
    public float PalmToIndex { get; set; }
    public float PalmToMiddle { get; set; }
    public float PalmToRing { get; set; }
    public float PalmToPinky { get; set; }
    public string Gesture { get; set; }

    public List<float> createDistanceVector()
    {
        return new List<float>()
        {
            PalmToThumb,
            PalmToIndex,
            PalmToMiddle,
            PalmToRing,
            PalmToPinky
        };
    }

    public override string ToString()
    {
        return string.Format("[FeatureVector: ID={0}, PalmToThumb={1}, PalmToIndex={2}, PalmToMiddle={3}, PalmToRing={4}, PalmToPinky={5}, Gesture={6}]",
                             ID, PalmToThumb, PalmToIndex, PalmToMiddle, PalmToRing, PalmToPinky, Gesture);
    }
}