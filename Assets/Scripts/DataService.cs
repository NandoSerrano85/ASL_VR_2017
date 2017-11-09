using SQLite4Unity3d;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if !UNITY_EDITOR
using System.Collections;
using System.IO;
#endif

public class DataService : MonoBehaviour
{
    [SerializeField]
    private string databaseName;

    private string databasePath;

    private void Awake()
    {
        #if UNITY_EDITOR
            databasePath = string.Format(@"Assets/StreamingAssets/Database/{0}", databaseName);
        #else
            string filePath = string.Format("{0}/{1}", Application.persistentDataPath, databaseName);

            if(!File.Exists(filePath))
            {
                string pathToDatabase = Application.dataPath + "/StreamingAssets/Database/" + databaseName;
	            File.Copy(pathToDatabase, filePath);
            }

            databasePath = filePath;
        #endif
    }

    // Returns every single feature vector that is on each row in the database.
    public List<FeatureVector> getAllFeatureVectors()
    {
        using (SQLiteConnection connection = new SQLiteConnection(databasePath))
        {
            return connection.Table<FeatureVector>().ToList();
        }
    }

    // Insert feature vector in the database
    public void InsertGesture(FeatureVector gesture)
    {
        using (SQLiteConnection connection = new SQLiteConnection(databasePath))
        {
            connection.Insert(gesture);
        }  
    }
}
