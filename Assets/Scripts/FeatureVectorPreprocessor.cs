using Accord.Statistics;
using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class FeatureVectorPreprocessor
{
    /*
     * This function creates a vector containing the distance between the palm of the hand 
     * and tip of each finger. It also constructs the distances between adjacent fingers,
     * how many fingers are extended, the pinch strength and the grab strength of the hand.
     */
    public FeatureVector createFeatureVector(Frame frame)
    {
        List<double> featureVectorList = new List<double>();

        calculatePalmToFingerDistances(featureVectorList, frame);

        calculateAdjacentFingerDistances(featureVectorList, frame);

        calculateHandToFingerAngles(featureVectorList, frame);

        double [] centered = Tools.Center(featureVectorList.ToArray<double>());
        double[] standardizedVector = Tools.Standardize(centered);

        FeatureVector featureVector = constructFeatureVector(standardizedVector);

        getMiscellaneousFeatures(featureVector, frame);

        return featureVector;
    }

    private FeatureVector constructFeatureVector(double [] featureVectorList)
    {
        FeatureVector featureVector = new FeatureVector();

        Type type = featureVector.GetType();
        PropertyInfo[] properties = type.GetProperties();

        int propertyIndex = 0;

        for (int i = 0; i < properties.Length; i++)
        {
            PropertyInfo property = properties[i];

            if(property.Name != "ID" && property.Name != "Gesture" && 
               property.Name != "GestureClassLabel" && property.Name != "NumExtendedFingers" &&
               property.Name != "PinchStrength" && property.Name != "GrabStrength")
            {
                property.SetValue(featureVector, featureVectorList[propertyIndex], null);
                propertyIndex++;
            }
        }

        return featureVector;
    }

    /*
     * featureVectorList[0] = PalmToThumb Distance
     * featureVectorList[1] = PalmToIndex Distance
     * featureVectorList[2] = PalmToMiddle Distance
     * featureVectorList[3] = PalmToRing Distance
     * featureVectorList[4] = PalmToPinky Distance
     */
    private void calculatePalmToFingerDistances(List<double> featureVectorList, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            foreach (Finger finger in hand.Fingers)
            {
                featureVectorList.Add(finger.TipPosition.DistanceTo(hand.PalmPosition) / hand.SphereRadius);
            }
        }
    }

    /* 
     * featureVectorList[5] = PinkyToRing Distance
     * featureVectorList[6] = RingToMiddle Distance
     * featureVectorList[7] = MiddleToIndex Distance
     * featureVectorList[8] = IndexToThumb Distance
     */

    private void calculateAdjacentFingerDistances(List<double> featureVectorList, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            for (int i = hand.Fingers.Count - 1; i > 0; i--)
            {
                Vector currentFinger = hand.Fingers[i].TipPosition;
                Vector previousFinger = hand.Fingers[i - 1].TipPosition;

                featureVectorList.Add(currentFinger.DistanceTo(previousFinger) / hand.SphereRadius);
            }
        }
    }

    /*
     * featureVectorList[9] = ThumbToHandNormal Distance
     * featureVectorList[10] = IndexToHandNormal Distance
     * featureVectorList[11] = MiddleToHandNormal Distance
     * featureVectorList[12] = RingToHandNormal Distance
     * featureVectorList[13] = PinkyToHandNormal Distance
     */
    private void calculateHandToFingerAngles(List<double> featureVectorList, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            Vector handNorm = hand.PalmNormal;

            foreach (Finger finger in hand.Fingers)
            {
                Vector curFinger = finger.Direction;

                double angle = curFinger.AngleTo(handNorm);

                float curFingerMag = curFinger.Magnitude;
                float handNormMag = handNorm.Magnitude;

                double distance = Math.Pow(curFingerMag, 2) + Math.Pow(handNormMag, 2.0) +
                                  - 2 * curFingerMag * handNormMag * angle;

                featureVectorList.Add(distance / hand.SphereRadius);
            }
        }
    }

    /*
     * featureVectorList[14] = NumFingersExtended
     * featureVectorList[15] = PinchStrength
     * featureVectorList[16] = GrabStrength
     */

    private void getMiscellaneousFeatures(FeatureVector featureVector, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            int numExtendedFingers = 0;

            foreach (Finger finger in hand.Fingers)
            {
                if (finger.IsExtended)
                    numExtendedFingers++;
            }

            featureVector.NumExtendedFingers = numExtendedFingers;
            featureVector.PinchStrength = hand.PinchStrength;
            featureVector.GrabStrength = hand.GrabStrength;
        }
    }
}
