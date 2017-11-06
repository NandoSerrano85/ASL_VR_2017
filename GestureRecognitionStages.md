# Training Stage
---
1. Get a frame from the leap motion device to unity.
2. Gather data points from the leap frame.
3. Calculate the distance from the palm of the hand to each finger 
4. Store the data in the database.

# Testing Stage
---
1. Get a frame from the leap motion device to unity.
2. Gather data points from the leap frame.
3. Calculate the distance from the palm of the hand to each finger.
4. With this gesture, compare it with all other gestures in the database using either the Euclidean distance formula or the cosine similarity forumula.
5. The one with the highest score will be classified as our desired gesture.
