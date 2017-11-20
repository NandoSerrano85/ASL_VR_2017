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

    private int maxGestureClassLabel;

    private void Awake()
    {
        #if UNITY_EDITOR
            databasePath = string.Format(@"Assets/StreamingAssets/Database/{0}", databaseName);
        #else
            string directoryPath = string.Format("{0}/Database", Application.persistentDataPath);
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
            string filePath = directoryPath + "/" + databaseName;

            if(!File.Exists(filePath))
            {
                string pathToDatabase = Application.dataPath + "/StreamingAssets/Database/" + databaseName;
	            File.Copy(pathToDatabase, filePath);
            }

            databasePath = filePath;
        #endif

        maxGestureClassLabel = getMaxGestureClassLabel();
    }

    // Returns every single feature vector that is on each row in the database.
    public List<FeatureVector> getAllFeatureVectors()
    {
        using (SQLiteConnection connection = new SQLiteConnection(databasePath))
        {
            return connection.Table<FeatureVector>().OrderBy(vector => vector.GestureClassLabel).ToList(); ;
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

    public int gestureToClassLabel(string gesture)
    {
        using (SQLiteConnection connection = new SQLiteConnection(databasePath))
        {
            FeatureVector featureVector = connection.Table<FeatureVector>()
                                          .Where(vector => vector.Gesture == gesture)
                                          .FirstOrDefault();

            if (featureVector != null)
                return featureVector.GestureClassLabel;
            else
                return ++maxGestureClassLabel;
        }
    }

    public string classLabelToGesture(int classLabel)
    {
        using (SQLiteConnection connection = new SQLiteConnection(databasePath))
        {
            FeatureVector featureVector = connection.Table<FeatureVector>()
                                          .Where(vector => vector.GestureClassLabel == classLabel)
                                          .FirstOrDefault();

            if (featureVector != null)
                return featureVector.Gesture;
            else
                return "Unknown Gesture";
        }
    }

    private int getMaxGestureClassLabel()
    {
        using (SQLiteConnection connection = new SQLiteConnection(databasePath))
        {
            if (connection.Table<FeatureVector>().FirstOrDefault() != null)
                return connection.Table<FeatureVector>().Max(vector => vector.GestureClassLabel);
            else
                return -1;
        }
    }
}
