﻿using Accord.Statistics;
using Leap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class FeatureVectorPreprocessor
{
    /*
     * This function creates the distance between the palm of the hand and tip of each finger. It
     * also constructs the angles between adjacent fingers, angles between normalized distance
     * vectors, the curvature of the hand and the number of fingers.
     */
    public FeatureVector createFeatureVector(Frame frame)
    {
        List<double> featureVectorList = new List<double>();

        calculateDistances(featureVectorList, frame);

        calculateAdjacentFingerAngles(featureVectorList, frame);

        calculateHandToFingerAngles(featureVectorList, frame);

        double [] centered = Tools.Center(featureVectorList.ToArray<double>());
        double[] standard = Tools.Standardize(centered);

        FeatureVector featureVector = constructFeatureVector(standard);

        featureVector.NumOfFingers = getTotalNumberOfFingers(frame);

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
               property.Name != "GestureClassLabel" && property.Name != "NumOfFingers")
            {
                property.SetValue(featureVector, featureVectorList[propertyIndex], null);
                propertyIndex++;
            }
        }

        return featureVector;
    }

    /*
     * featureVectorList[0] = PalmToThumb Distance Vector
     * featureVectorList[1] = PalmToIndex Distance Finger Vector
     * featureVectorList[2] = PalmToMiddle Distance Finger Vector
     * featureVectorList[3] = PalmToRing Distance Finger Vector
     * featureVectorList[4] = PalmToPinky Distance Finger Vector
     */
    private void calculateDistances(List<double> featureVectorList, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            foreach (Finger finger in hand.Fingers)
            {
                featureVectorList.Add(finger.TipPosition.DistanceTo(hand.PalmPosition));
            }
        }
    }

    /*
     * Calculate the angles between adajcent fingers.
     * 
     * featureVectorList[5] = ThumbToIndex Angle
     * featureVectorList[6] = IndexToMiddle Angle
     * featureVectorList[7] = MiddleToRing Angle
     * featureVectorList[8] = RingToPinky Angle
     */
    private void calculateAdjacentFingerAngles(List<double> featureVectorList, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            for (int i = 1; i < hand.Fingers.Count; i++)
            {
                Vector currentFinger = (hand.Fingers[i].TipPosition - hand.PalmPosition).Normalized;
                Vector previousFinger = (hand.Fingers[i - 1].TipPosition - hand.PalmPosition).Normalized;

                featureVectorList.Add(previousFinger.Dot(currentFinger));
            }
        }
    }

    /*
     * Calculate the angles between adajcent fingers.
     * 
     * featureVectorList[9] = ThumbToHandNormal Angle
     * featureVectorList[10] = IndexToHandNormal Angle
     * featureVectorList[11] = MiddleToHandNormal Angle
     * featureVectorList[12] = RingToHandNormal Angle
     * featureVectorList[13] = PinkyToHandNormal Angle
     */
    private void calculateHandToFingerAngles(List<double> featureVectorList, Frame frame)
    {
        foreach (Hand hand in frame.Hands)
        {
            Vector handNormal = hand.PalmNormal;

            foreach (Finger finger in hand.Fingers)
            {
                Vector currentFinger = (finger.TipPosition - hand.PalmPosition).Normalized;
                featureVectorList.Add(currentFinger.Dot(handNormal));
            }

            featureVectorList.Add(hand.SphereRadius);
        }
    }

    private int getTotalNumberOfFingers(Frame frame)
    {
        int totalNumberOfFingers = 0;

        foreach (Hand hand in frame.Hands)
        {
            foreach (Finger finger in hand.Fingers)
            {
                totalNumberOfFingers++;
            }
        }

        return totalNumberOfFingers;
    }
}
