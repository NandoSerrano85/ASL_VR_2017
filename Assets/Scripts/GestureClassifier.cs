using System.Collections.Generic;
using Accord.MachineLearning.VectorMachines;
using UnityEngine;
using Accord.IO;
using System.IO;
using System.Collections;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;

public class GestureClassifier : MonoBehaviour
{
    [SerializeField]
    private DataService dataService;

    private MulticlassSupportVectorMachine<Gaussian> multiSVM;

    [SerializeField]
    private string modelName;

    private string modelPath;

    public bool ModelExists { get; set; }

    private void Start()
    {
    #if UNITY_EDITOR
        modelPath = string.Format(@"Assets/StreamingAssets/Model/{0}", modelName);
    #else
            string directoryPath = string.Format("{0}/Model", Application.persistentDataPath);
            
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            
            string filePath = directoryPath + "/" + modelName;

            if(!File.Exists(filePath))
            {
                string pathToModel = Application.dataPath + "/StreamingAssets/Model/" + modelName;
	            File.Copy(pathToModel, filePath);
            }

            modelPath = filePath;
    #endif

        if (File.Exists(modelPath))
        {
            multiSVM = loadModel();
            ModelExists = true;
        }
    }

    public IEnumerator trainClassifier()
    {
        List<FeatureVector> featureVectors = dataService.getAllFeatureVectors();

        double[][] inputs = new double[featureVectors.Count][];
        int[] outputs = new int[featureVectors.Count];

        createInputsAndOutputs(inputs, outputs, featureVectors);

        // Create the multi-class learning algorithm for the machine
        var teacher = new MulticlassSupportVectorLearning<Gaussian>();

        // Learn a machine
        multiSVM = teacher.Learn(inputs, outputs);

        saveModel();
        ModelExists = true;

        yield return null;
    }

    private void createInputsAndOutputs(double[][] inputs, int[] outputs, List<FeatureVector> featureVectors)
    {
        for (int i = 0; i < featureVectors.Count; i++)
        {
            inputs[i] = featureVectors[i].createInputVector();
            outputs[i] = featureVectors[i].GestureClassLabel;
        }
    }

    public string classifyGesture(double[] distanceVector)
    {
        int gestureClassLabel = multiSVM.Decide(distanceVector);
        return dataService.classLabelToGesture(gestureClassLabel);
    }

    public void saveModel()
    {
        multiSVM.Save(modelPath);
    }

    public MulticlassSupportVectorMachine<Gaussian> loadModel()
    {
        return Serializer.Load<MulticlassSupportVectorMachine<Gaussian>>(modelPath);
    }
}