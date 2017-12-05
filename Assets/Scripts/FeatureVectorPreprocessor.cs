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
     * how many fingers are extended, the pinch strength and the grab strength of the hand,
     * the sphere radius of the hand and the angle from the wrist to the palm.
     */
    public FeatureVector createFeatureVector(Frame frame)
    {
        List<double> featureVectorList = new List<double>();

        calculatePalmToFingerDistances(featureVectorList, frame);

        calculateAdjacentFingerDistances(featureVectorList, frame);

        calculateHandToFingerDistances(featureVectorList, frame);

        double [] centered = Tools.Center(featureVectorList.ToArray<double>());
        List<double> standardizedVectorList = Tools.Standardize(centered).ToList();

        getMiscellaneousFeatures(standardizedVectorList, frame);

        FeatureVector featureVector = constructFeatureVector(standardizedVectorList);

        featureVector.NumExtendedFingers = getNumExtendedFingers(frame);

        return featureVector;
    }

    private FeatureVector constructFeatureVector(List<double> standardizedVectorList)
    {
        FeatureVector featureVector = new FeatureVector();

        Type type = featureVector.GetType();
        PropertyInfo[] properties = type.GetProperties();

        int propertyIndex = 0;

        /*
         * The minus is so we skip the last the properties
         * in the feature vector class. Those get assigned 
         * when the user creates the gesture. 
         */

        for (int i = 0; i < properties.Length - 3; i++)
        {
            PropertyInfo property = properties[i];

            if(property.Name != "ID")
            {
                property.SetValue(featureVector, standardizedVectorList[propertyIndex], null);
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
                Vector currentFinger = hand.Fingers[i].TipPosition - hand.PalmPosition;
                Vector previousFinger = hand.Fingers[i - 1].TipPosition - hand.PalmPosition;

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
    private void calculateHandToFingerDistances(List<double> featureVectorList, Frame frame)
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

                double distance = Math.Pow(curFingerMag, 2) + Math.Pow(handNormMag, 2) +
                                  - 2 * curFingerMag * handNormMag * angle;

                featureVectorList.Add(distance / hand.SphereRadius);
            }
        }
    }

    /*
     * featureVectorList[13] = RadiusSphere
     * featureVectorList[14] = PinchStrength
     * featureVectorList[15] = GrabStrength
     */
    private void getMiscellaneousFeatures(List<double> featureVectorList, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            int numExtendedFingers = 0;

            foreach (Finger finger in hand.Fingers)
            {
                if (finger.IsExtended)
                    numExtendedFingers++;
            }

            featureVectorList.Add(hand.SphereRadius);
            featureVectorList.Add(hand.PinchStrength);
            featureVectorList.Add(hand.GrabStrength);
        }
    }

    /*
    * featureVectorList[16] = NumExtendedFingers
    */
    private int getNumExtendedFingers(Frame frame)
    {
        int numExtendedFingers = 0;

        foreach (Hand hand in frame.Hands)
        {
            foreach (Finger finger in hand.Fingers)
            {
                if (finger.IsExtended)
                    numExtendedFingers++;
            }
        }

        return numExtendedFingers;
    }
}
