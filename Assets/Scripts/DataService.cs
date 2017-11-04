using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;

public class DataService
{
    private string databaseName = "Assets/Database/Gesture_Data.db";

    // Creates connection to database
    public void CreatesFeatureTable()
    {
        using (SQLiteConnection connection = new SQLiteConnection(databaseName))
        {
            connection.CreateTable<FeatureVector>();
        }
    }

    // This returns the entire Database
    public List<FeatureVector> GetEntireDatabase()
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
