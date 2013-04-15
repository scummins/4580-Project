using Microsoft.Kinect;
using System.Runtime.InteropServices;
using System;
using System.Windows;

namespace Fizbin.Kinect.Gestures.Segments
{

    /// <summary>
    /// The first part of the swipe right gesture
    /// </summary>
    public class SwipeRightSegment1 : IRelativeGestureSegment
    {

        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // //left hand in front of left Shoulder
            if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
            {
                // Debug.WriteLine("GesturePart 0 - left hand in front of left Shoulder - PASS");
                // //left hand below shoulder height but above hip height
                if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    // Debug.WriteLine("GesturePart 0 - left hand below shoulder height but above hip height - PASS");
                    // //left hand left of left Shoulder
                    if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderLeft].Position.X)
                    {
                        // Debug.WriteLine("GesturePart 0 - left hand left of left Shoulder - PASS");
                        return GesturePartResult.Succeed;
                    }

                    // Debug.WriteLine("GesturePart 0 - left hand left of left Shoulder - UNDETERMINED");
                    return GesturePartResult.Pausing;
                }

                // Debug.WriteLine("GesturePart 0 - left hand below shoulder height but above hip height - FAIL");
                return GesturePartResult.Fail;
            }

            // Debug.WriteLine("GesturePart 0 - left hand in front of left Shoulder - FAIL");
            return GesturePartResult.Fail;
        }
    }

    /// <summary>
    /// The second part of the swipe right gesture
    /// </summary>
    public class SwipeRightSegment2 : IRelativeGestureSegment
    {
        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // //left hand in front of left Shoulder
            if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
            {
                // Debug.WriteLine("GesturePart 1 - left hand in front of left Shoulder - PASS");
                // /left hand below shoulder height but above hip height
                if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    // Debug.WriteLine("GesturePart 1 - left hand below shoulder height but above hip height - PASS");
                    // //left hand left of left Shoulder
                    if (skeleton.Joints[JointType.HandLeft].Position.X < skeleton.Joints[JointType.ShoulderRight].Position.X && skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderLeft].Position.X)
                    {
                        // Debug.WriteLine("GesturePart 1 - left hand left of left Shoulder - PASS");
                        return GesturePartResult.Succeed;
                    }

                    // Debug.WriteLine("GesturePart 1 - left hand left of left Shoulder - UNDETERMINED");
                    return GesturePartResult.Pausing;
                }

                // Debug.WriteLine("GesturePart 1 - left hand below shoulder height but above hip height - FAIL");
                return GesturePartResult.Fail;
            }

            // Debug.WriteLine("GesturePart 1 - left hand in front of left Shoulder - FAIL");
            return GesturePartResult.Fail;
        }
    }

    /// <summary>
    /// The third part of the swipe right gesture
    /// </summary>
    public class SwipeRightSegment3 : IRelativeGestureSegment
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern void MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);


        /// <summary>
        /// Checks the gesture.
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <returns>GesturePartResult based on if the gesture part has been completed</returns>
        public GesturePartResult CheckGesture(Skeleton skeleton)
        {
            // //left hand in front of left Shoulder
            if (skeleton.Joints[JointType.HandLeft].Position.Z < skeleton.Joints[JointType.ElbowLeft].Position.Z && skeleton.Joints[JointType.HandRight].Position.Y < skeleton.Joints[JointType.HipCenter].Position.Y)
            {
                // //left hand below shoulder height but above hip height
                if (skeleton.Joints[JointType.HandLeft].Position.Y < skeleton.Joints[JointType.Head].Position.Y && skeleton.Joints[JointType.HandLeft].Position.Y > skeleton.Joints[JointType.HipCenter].Position.Y)
                {
                    // //left hand left of left Shoulder
                    if (skeleton.Joints[JointType.HandLeft].Position.X > skeleton.Joints[JointType.ShoulderRight].Position.X)
                    {
                        IntPtr winId;
                        winId = GetForegroundWindow();
                        MoveWindow(winId, 720, 0, 720, 800, true);
                        return GesturePartResult.Succeed;
                    }

                    return GesturePartResult.Pausing;
                }

                return GesturePartResult.Fail;
            }

            return GesturePartResult.Fail;
        }
    }
}
