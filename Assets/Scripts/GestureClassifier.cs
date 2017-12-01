using System.Collections.Generic;
using Accord.MachineLearning.VectorMachines;
using UnityEngine;
using Accord.IO;
using System.IO;
using System.Collections;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using Accord.MachineLearning.Performance;
using Accord.Math.Optimization.Losses;
using Accord.Statistics.Analysis;
using Accord.MachineLearning;

public class GestureClassifier : MonoBehaviour
{
    [SerializeField]
    private DataService dataService;

    private MulticlassSupportVectorMachine<Gaussian> multiSVM;

    [SerializeField]
    private string modelName;

    private string modelPath;
    
    public bool ModelExists { get; set; }

    [SerializeField]
    private bool PeformValidationTests = false;

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

        var teacher = new MulticlassSupportVectorLearning<Gaussian>()
        {
            // Configure the learning algorithm to use SMO to train the
            //  underlying SVMs in each of the binary class subproblems.
            Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
            {
                // Estimate a suitable guess for the Gaussian kernel's parameters.
                // This estimate can serve as a starting point for a grid search.
                UseKernelEstimation = true
            }
        };

        var machine = teacher.Learn(inputs, outputs);

        // Create the multi-class learning algorithm for the machine
        var calibration = new MulticlassSupportVectorLearning<Gaussian>()
        {
            Model = machine, // We will start with an existing machine

            // Configure the learning algorithm to use Platt's calibration
            Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
            {
                Model = param.Model // Start with an existing machine
            }
        };

        multiSVM = calibration.Learn(inputs, outputs);

        if(PeformValidationTests)
        {
            splitSetValidation(inputs, outputs);
            crossValidation(inputs, outputs, 4);
        }

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

    private void splitSetValidation(double [][] inputs, int [] outputs)
    {
        var splitset = new SplitSetValidation<MulticlassSupportVectorMachine<Gaussian>, double[]>()
        {
            Learner = (s) => new MulticlassSupportVectorLearning<Gaussian>()
            {
                Learner = (m) => new SequentialMinimalOptimization<Gaussian>()
                {
                    UseComplexityHeuristic = true,
                }
            },

            Loss = (expected, actual, p) => new ZeroOneLoss(expected).Loss(actual),
        };

        var result = splitset.Learn(inputs, outputs);

        Debug.Log(result.Training.Value);
        Debug.Log(result.Validation.Value);
    }

    private void crossValidation(double[][] inputs, int[] outputs, int fold)
    {
        var crossValidation = new CrossValidation<MulticlassSupportVectorMachine<Gaussian>, double[]>()
        {
            K = fold,

            Learner = (s) => new MulticlassSupportVectorLearning<Gaussian>()
            {
                Learner = (m) => new SequentialMinimalOptimization<Gaussian>()
                {
                    UseComplexityHeuristic = true,
                }
            },

            Loss = (expected, actual, p) => new ZeroOneLoss(expected).Loss(actual),
        };

        var result = crossValidation.Learn(inputs, outputs);

        Debug.Log(result.Training.Mean);
        Debug.Log(result.Validation.Mean);

        GeneralConfusionMatrix gcm = result.ToConfusionMatrix(inputs, outputs);

        Debug.Log(gcm.Accuracy);
        Debug.Log(gcm.Error);
    }

    private void saveModel()
    {
        multiSVM.Save(modelPath);
    }

    private MulticlassSupportVectorMachine<Gaussian> loadModel()
    {
        return Serializer.Load<MulticlassSupportVectorMachine<Gaussian>>(modelPath);
    }
}