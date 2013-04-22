using Microsoft.Kinect;
//using System;

namespace Fizbin.Kinect.Gestures.Segments
{
    /// <summary>
    /// The first part of the leftpushforward gesture
    /// </summary>
    public class LeftPushForwardSegment1 : IRelativeGestureSegment
    {

        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            //        double ewlen = Sqrt(System.Convert.ToDouble(((skeleton.Joints[JointType.ElbowLeft].Position.X) - (skeleton.Joints[JointType.WristLeft].Position.X))^2 + ((skeleton.Joints[JointType.ElbowLeft].Position.Y) - (skeleton.Joints[JointType.WristLeft].Position.Y))^2 + ((skeleton.Joints[JointType.ElbowLeft].Position.Z) - (skeleton.Joints[JointType.WristLeft].Position.Z))^2));
            //        double selen = Sqrt(System.Convert.ToDouble(((Skeleton.Joints[JointType.ElbowLeft].Position.X) - (Skeleton.Joints[JointType.ShoulderLeft].Position.X)) ^ 2 + ((Skeleton.Joints[JointType.ElbowLeft].Position.Y) - (Skeleton.Joints[JointType.ShoulderLeft].Position.Y)) ^ 2 + ((Skeleton.Joints[JointType.ElbowLeft].Position.Z) - (Skeleton.Joints[JointType.ShoulderLeft].Position.Z)) ^ 2));

            // left and right elbows and wrists under left and right shoulders
            if (skeleton.Joints[JointType.ElbowLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y && skeleton.Joints[JointType.WristLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y && skeleton.Joints[JointType.ElbowRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y && skeleton.Joints[JointType.ElbowRight].Position.Y > skeleton.Joints[JointType.WristRight].Position.Y)
            {
                // left hand above left wrist
                if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.ElbowLeft].Position.Y)
                {
                    // right hand under right wrist
                    if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.WristRight].Position.Y)
                    {
                        return GesturePartResult.Succeed;
                    }
                    return GesturePartResult.Pausing;
                }
                return GesturePartResult.Fail;
            }
            return GesturePartResult.Fail;
        }
    }

    /// <summary>
    /// The second part of the swipe left gesture
    /// </summary>
    public class LeftPushForwardSegment2 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // right hand under right elbow under right shoulder
            if (skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.ElbowRight].Position.Y && skeleton.Joints[JointType.ElbowRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y)
            {
                // left hand above left wrist
                if (skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.WristLeft].Position.Y)
                {
                    // left hand in front of left elbow in front of left shoulder
                    if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.ElbowLeft].Position.Z < skeleton.Joints[JointType.ShoulderLeft].Position.Z)
                    {
                        double len = (skeleton.Joints[JointType.HandLeft].Position.Y - skeleton.Joints[JointType.WristLeft].Position.Y) * 3 / 2;
                        // left arm is at constant height within limit set by len
                        if ((((skeleton.Joints[JointType.WristLeft].Position.Y - skeleton.Joints[JointType.ElbowLeft].Position.Y) - len) < 0 || ((skeleton.Joints[JointType.ElbowLeft].Position.Y - skeleton.Joints[JointType.WristLeft].Position.Y) - len) < 0) && (((skeleton.Joints[JointType.ElbowLeft].Position.Y - skeleton.Joints[JointType.ShoulderLeft].Position.Y) - len) < 0 || ((skeleton.Joints[JointType.ShoulderLeft].Position.Y - skeleton.Joints[JointType.ElbowLeft].Position.Y) - len) < 0))
                        {
                            return GesturePartResult.Succeed;
                        }
                        return GesturePartResult.Pausing;
                    }
                    return GesturePartResult.Fail;
                }
                return GesturePartResult.Fail;
            }
            return GesturePartResult.Fail;
        }
    }

}