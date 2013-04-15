using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Kinect.Toolbox;
using Microsoft.Kinect;

namespace GesturesViewer
{
    partial class MainWindow
    {
        void LoadCircleGestureDetector()
        {
            using (Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate))
            {
                circleGestureRecognizer = new TemplatedGestureDetector("Look at me!", recordStream);
                circleGestureRecognizer.DisplayCanvas = gesturesCanvas;
                circleGestureRecognizer.OnGestureDetected += OnGestureDetected;

                MouseController.Current.ClickGestureDetector = circleGestureRecognizer;
            }
        }


        void LoadWGestureDetector()
        {
            using (Stream recordStream = File.Open(circleKBPath, FileMode.OpenOrCreate))
            {
                wGestureRecognizer = new TemplatedGestureDetector("Dubya", recordStream);
                wGestureRecognizer.DisplayCanvas = gesturesCanvas;
                wGestureRecognizer.OnGestureDetected += OnGestureDetected;

                MouseController.Current.ClickGestureDetector = wGestureRecognizer;
            }
        }


        private void recordGesture_Click(object sender, RoutedEventArgs e)
        {
            if (circleGestureRecognizer.IsRecordingPath)
            {
                circleGestureRecognizer.EndRecordTemplate();
                recordGesture.Content = "Record Gesture";
                return;
            }

            circleGestureRecognizer.StartRecordTemplate();
            recordGesture.Content = "Stop Recording";
        }

        void OnGestureDetected(string gesture)
        {
            int pos = detectedGestures.Items.Add(string.Format("{0} : {1}", gesture, DateTime.Now));

            if(String.Equals(gesture, "Look at Me!", StringComparison.OrdinalIgnoreCase)) System.Diagnostics.Process.Start("C:/Program Files (x86)/Notepad++/notepad++.exe");
            detectedGestures.SelectedIndex = pos;
        }

        void CloseGestureDetector()
        {
            if (circleGestureRecognizer == null)
                return;

            using (Stream recordStream = File.Create(circleKBPath))
            {
                circleGestureRecognizer.SaveState(recordStream);
            }
            circleGestureRecognizer.OnGestureDetected -= OnGestureDetected;
        }
    }
}
