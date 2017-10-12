# Gather Data Set
---
1. Leap gives a frame to the unity client.
2. Transform the data in the frame into a json object
3. Store the json in the database.

# Training the Model
---
1. Load up the training data from the database.
2. Preprocess the data.
3. Train the model.
4. Validate the model.
5. Test the model
6. Repeat steps 3-5 until satisfied with the accuracy of the model.
7. Save the model.

# Production
1. Leap gives a frame to the unity client.
2. Transform the data in the frame into a json object
3. Send json to the python server through the socket.
4. preprocess the data.
5. Run the data through the model.
6. Return its prediction(Gesture).