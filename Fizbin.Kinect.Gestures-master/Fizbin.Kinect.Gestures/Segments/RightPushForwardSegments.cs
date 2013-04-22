using Microsoft.Kinect;
//using System;

namespace Fizbin.Kinect.Gestures.Segments
{
    /// <summary>
    /// The first part of the Rightpushforward gesture
    /// </summary>
    public class RightPushForwardSegment1 : IRelativeGestureSegment
    {

        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            //        double ewlen = Sqrt(System.Convert.ToDouble(((skeleton.Joints[JointType.ElbowRight].Position.X) - (skeleton.Joints[JointType.WristRight].Position.X))^2 + ((skeleton.Joints[JointType.ElbowRight].Position.Y) - (skeleton.Joints[JointType.WristRight].Position.Y))^2 + ((skeleton.Joints[JointType.ElbowRight].Position.Z) - (skeleton.Joints[JointType.WristRight].Position.Z))^2));
            //        double selen = Sqrt(System.Convert.ToDouble(((Skeleton.Joints[JointType.ElbowRight].Position.X) - (Skeleton.Joints[JointType.ShoulderRight].Position.X)) ^ 2 + ((Skeleton.Joints[JointType.ElbowRight].Position.Y) - (Skeleton.Joints[JointType.ShoulderRight].Position.Y)) ^ 2 + ((Skeleton.Joints[JointType.ElbowRight].Position.Z) - (Skeleton.Joints[JointType.ShoulderRight].Position.Z)) ^ 2));

            // Right and Left elbows and wrists under Right and Left shoulders
            if (skeleton.Joints[JointType.ElbowRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y && skeleton.Joints[JointType.WristRight].Position.Y < skeleton.Joints[JointType.ShoulderRight].Position.Y && skeleton.Joints[JointType.ElbowLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y && skeleton.Joints[JointType.ElbowLeft].Position.Y > skeleton.Joints[JointType.WristLeft].Position.Y)
            {
                // Right hand above Right wrist
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.ElbowRight].Position.Y)
                {
                    // Left hand under Left wrist
                    if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.WristLeft].Position.Y)
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
    /// The second part of the swipe Right gesture
    /// </summary>
    public class RightPushForwardSegment2 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // Left hand under Left elbow under Left shoulder
            if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.ElbowLeft].Position.Y && skeleton.Joints[JointType.ElbowLeft].Position.Y < skeleton.Joints[JointType.ShoulderLeft].Position.Y)
            {
                // Right hand above Right wrist
                if (skeleton.Joints[JointType.HandRight].Position.Y > skeleton.Joints[JointType.WristRight].Position.Y)
                {
                    // Right hand in front of Right elbow in front of Right shoulder
                    if (skeleton.Joints[JointType.HandRight].Position.Z < skeleton.Joints[JointType.ElbowRight].Position.Z && skeleton.Joints[JointType.ElbowRight].Position.Z < skeleton.Joints[JointType.ShoulderRight].Position.Z)
                    {
                        float len = (skeleton.Joints[JointType.HandRight].Position.Y - skeleton.Joints[JointType.WristRight].Position.Y) * 4.0f / 2.0f;
                        // Right arm is at constant height within limit set by len
                        if ((((skeleton.Joints[JointType.WristRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y) > 0.0f && ((skeleton.Joints[JointType.WristRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y) - len) < 0.0f) || ((skeleton.Joints[JointType.ElbowRight].Position.Y - skeleton.Joints[JointType.WristRight].Position.Y) > 0.0f && ((skeleton.Joints[JointType.ElbowRight].Position.Y - skeleton.Joints[JointType.WristRight].Position.Y) - len) < 0.0f)) && (((skeleton.Joints[JointType.ElbowRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y) > 0.0f && ((skeleton.Joints[JointType.ElbowRight].Position.Y - skeleton.Joints[JointType.ShoulderRight].Position.Y) - len) < 0.0f) || ((skeleton.Joints[JointType.ShoulderRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y) > 0.0f && ((skeleton.Joints[JointType.ShoulderRight].Position.Y - skeleton.Joints[JointType.ElbowRight].Position.Y) - len) < 0.0f)))
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