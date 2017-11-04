using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;

public class DataService
{
    private string databaseName = "Assets/Database/Gesture_Data.db";

    // This returns all the rows in the database.
    public List<FeatureVector> getAllFeatureVectors()
    {
        using (SQLiteConnection connection = new SQLiteConnection(databaseName))
        {
            return connection.Table<FeatureVector>().ToList();
        }
    }

    // Insert gesture in database
    public void InsertGesture(FeatureVector gesture)
    {
        using (SQLiteConnection connection = new SQLiteConnection(databaseName))
        {
            connection.Insert(gesture);
        }  
    }
}
