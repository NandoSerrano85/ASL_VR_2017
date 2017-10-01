using Leap;
using Leap.Unity;
using UnityEngine;

public class SendHandData: MonoBehaviour
{
   private LeapServiceProvider leapProvider;
   private NetworkSocket networkSocket;

   void Start()
   {
     leapProvider = FindObjectOfType<LeapServiceProvider>();
     networkSocket = new NetworkSocket();
    }

   void Update()
   {
     Frame currentFrame = leapProvider.CurrentFrame;

     foreach(Hand hand in currentFrame.Hands)
     {
       if(hand.IsLeft)
       {
          networkSocket.writeSocket("Left hand is @ " + hand.PalmPosition);
       }
       else
       {
          networkSocket.writeSocket("Right hand is @ " + hand.PalmPosition);
       }

       networkSocket.writeSocket("---Next Frame---");
     }
   }
}
