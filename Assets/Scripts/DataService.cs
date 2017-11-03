using SQLite4Unity3d;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;

public class DataService
{
    SQLiteConnection connection;
    string databaseName = "Assets/Database/Gesture_Data.db";
    // Creates connection to database
    public void CreateDatabase()
    {
        using (SQLiteConnection connection = new SQLiteConnection(databaseName))
        {
            connection.CreateTable<FeatureVector>();
        }
    }

    // This returns the entire Database
    public IEnumerable<FeatureVector> GetEntireDatabase()
    {
        using (SQLiteConnection connection = new SQLiteConnection(databaseName))
        {
            return connection.Table<FeatureVector>();
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
