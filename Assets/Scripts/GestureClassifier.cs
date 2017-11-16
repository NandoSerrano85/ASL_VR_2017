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

    private MulticlassSupportVectorMachine<Gaussian> multSVM;

    [SerializeField]
    private string modelName;

    private string modelPath;

    public bool ModelExists { get; set; }

    private void Start()
    {
        #if UNITY_EDITOR
            modelPath = string.Format(@"Assets/StreamingAssets/Model/{0}", modelName);
        #else
            string filePath = string.Format("{0}/{1}", Application.persistentDataPath, modelName);

            if(!File.Exists(filePath))
            {
                string pathToModel = Application.dataPath + "/StreamingAssets/Model/" + modelName;
	            File.Copy(pathToModel, filePath);
            }

            modelPath = filePath;
        #endif

        if (File.Exists(modelPath))
        {
            multSVM = loadModel();
            ModelExists = true;
        }
    }

    public IEnumerator trainClassifier()
    {
        List<FeatureVector> featureVectors = dataService.getAllFeatureVectors();

        double [][] inputs = new double[featureVectors.Count][];
        int [] outputs = new int[featureVectors.Count];

        createInputsAndOutputs(inputs, outputs, featureVectors);

        // Create a one-vs-one multi-class SVM learning algorithm 
        var teacher = new MulticlassSupportVectorLearning<Gaussian>()
        {
            Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
            {
                // Estimate a suitable guess for the Gaussian kernel's parameters.
                // This estimate can serve as a starting point for a grid search.
                UseKernelEstimation = true
            }
        };

        var machine = teacher.Learn(inputs, outputs);

        var calibration = new MulticlassSupportVectorLearning<Gaussian>()
        {
            Model = machine, // We will start with an existing machine

            // Configure the learning algorithm to use Platt's calibration
            Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
            {
                Model = param.Model // Start with an existing machine
            }
        };

        multSVM = calibration.Learn(inputs, outputs);

        saveModel();
        ModelExists = true;

        yield return null;
    }

    private void createInputsAndOutputs(double [][] inputs, int [] outputs, List<FeatureVector> featureVectors)
    {
        for (int i = 0; i < featureVectors.Count; i++)
        {
            inputs[i] = featureVectors[i].createInputVector();
            outputs[i] = featureVectors[i].GestureClassLabel;
        }
    }

    public string classifyGesture(double [] distanceVector)
    {
        int gestureClassLabel = multSVM.Decide(distanceVector);
        return dataService.classLabelToGesture(gestureClassLabel);
    }

    public void saveModel()
    {
        multSVM.Save(modelPath);
    }

    public MulticlassSupportVectorMachine<Gaussian> loadModel()
    {
        return Serializer.Load<MulticlassSupportVectorMachine<Gaussian>>(modelPath);
    }
}