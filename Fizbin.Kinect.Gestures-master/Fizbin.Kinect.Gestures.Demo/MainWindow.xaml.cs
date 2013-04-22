using System.Windows;
using System.Windows.Data;
using Microsoft.Kinect;
using Microsoft.Kinect.Toolkit;
using Microsoft.Samples.Kinect.WpfViewers;
using System.ComponentModel;
using System;
using System.Timers;
using Fizbin.Kinect.Gestures.Segments;
using System.Runtime.InteropServices;
using System.Diagnostics;


namespace Fizbin.Kinect.Gestures.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged

    {
        private readonly KinectSensorChooser sensorChooser = new KinectSensorChooser();

        private Skeleton[] skeletons = new Skeleton[0];

        Timer _clearTimer;

        // skeleton gesture recognizer
        private GestureController gestureController;

        // SendInput import:
        [DllImport("user32.dll")]
        internal static extern UINT SendInput(
            UINT nInputs, 
            [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, 
            int cbSize);

        // Window control DLLs
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        internal static extern void MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, ExactSpelling = true, SetLastError = true)]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        //Window Control Constants
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        public MainWindow()
        {

            DataContext = this;

            InitializeComponent();

            // initialize the Kinect sensor manager
            KinectSensorManager = new KinectSensorManager();
            KinectSensorManager.KinectSensorChanged += this.KinectSensorChanged;

            // locate an available sensor
            sensorChooser.Start();

            // bind chooser's sensor value to the local sensor manager
            var kinectSensorBinding = new Binding("Kinect") { Source = this.sensorChooser };
            BindingOperations.SetBinding(this.KinectSensorManager, KinectSensorManager.KinectSensorProperty, kinectSensorBinding);

            // add timer for clearing last detected gesture
            _clearTimer = new Timer(2000);
            _clearTimer.Elapsed += new ElapsedEventHandler(clearTimer_Elapsed);
        }

        #region Kinect Discovery & Setup

        private void KinectSensorChanged(object sender, KinectSensorManagerEventArgs<KinectSensor> args)
        {
            if (null != args.OldValue)
                UninitializeKinectServices(args.OldValue);

            if (null != args.NewValue)
                InitializeKinectServices(KinectSensorManager, args.NewValue);
        }

        /// <summary>
        /// Kinect enabled apps should customize which Kinect services it initializes here.
        /// </summary>
        /// <param name="kinectSensorManager"></param>
        /// <param name="sensor"></param>
        private void InitializeKinectServices(KinectSensorManager kinectSensorManager, KinectSensor sensor)
        {
            // Application should enable all streams first.

            // configure the color stream
            kinectSensorManager.ColorFormat = ColorImageFormat.RgbResolution640x480Fps30;
            kinectSensorManager.ColorStreamEnabled = true;

            // configure the depth stream
            kinectSensorManager.DepthStreamEnabled = true;

            kinectSensorManager.TransformSmoothParameters =
                new TransformSmoothParameters
                {
                    Smoothing = 0.5f,
                    Correction = 0.5f,
                    Prediction = 0.5f,
                    JitterRadius = 0.05f,
                    MaxDeviationRadius = 0.04f
                };

            // configure the skeleton stream
            sensor.SkeletonFrameReady += OnSkeletonFrameReady;
            kinectSensorManager.SkeletonStreamEnabled = true;

            kinectSensorManager.KinectSensorEnabled = true;

            if (!kinectSensorManager.KinectSensorAppConflict)
            {
                // initialize the gesture recognizer
                gestureController = new GestureController();
                gestureController.GestureRecognized += OnGestureRecognized;

                // register the gestures for this demo
                RegisterGestures();
            }
        }

        /// <summary>
        /// Kinect enabled apps should uninitialize all Kinect services that were initialized in InitializeKinectServices() here.
        /// </summary>
        /// <param name="sensor"></param>
        private void UninitializeKinectServices(KinectSensor sensor)
        {
            // unregister the event handlers
            sensor.SkeletonFrameReady -= OnSkeletonFrameReady;
            gestureController.GestureRecognized -= OnGestureRecognized;
        }

        #endregion Kinect Discovery & Setup

        /// <summary>
        /// Helper function to register all available 
        /// </summary>
        private void RegisterGestures()
        {
            // define the gestures for the demo

            IRelativeGestureSegment[] joinedhandsSegments = new IRelativeGestureSegment[20];
            JoinedHandsSegment1 joinedhandsSegment = new JoinedHandsSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 10 times 
                joinedhandsSegments[i] = joinedhandsSegment;
            }
            gestureController.AddGesture("JoinedHands", joinedhandsSegments);

            IRelativeGestureSegment[] menuSegments = new IRelativeGestureSegment[20];
            MenuSegment1 menuSegment = new MenuSegment1();
            for (int i = 0; i < 20; i++)
            {
                // gesture consists of the same thing 20 times 
                menuSegments[i] = menuSegment;
            }
            gestureController.AddGesture("Menu", menuSegments);

            IRelativeGestureSegment[] swipeleftSegments = new IRelativeGestureSegment[3];
            swipeleftSegments[0] = new SwipeLeftSegment1();
            swipeleftSegments[1] = new SwipeLeftSegment2();
            swipeleftSegments[2] = new SwipeLeftSegment3();
            gestureController.AddGesture("SwipeLeft", swipeleftSegments);

            IRelativeGestureSegment[] swiperightSegments = new IRelativeGestureSegment[3];
            swiperightSegments[0] = new SwipeRightSegment1();
            swiperightSegments[1] = new SwipeRightSegment2();
            swiperightSegments[2] = new SwipeRightSegment3();
            gestureController.AddGesture("SwipeRight", swiperightSegments);

            IRelativeGestureSegment[] waveRightSegments = new IRelativeGestureSegment[6];
            WaveRightSegment1 waveRightSegment1 = new WaveRightSegment1();
            WaveRightSegment2 waveRightSegment2 = new WaveRightSegment2();
            waveRightSegments[0] = waveRightSegment1;
            waveRightSegments[1] = waveRightSegment2;
            waveRightSegments[2] = waveRightSegment1;
            waveRightSegments[3] = waveRightSegment2;
            waveRightSegments[4] = waveRightSegment1;
            waveRightSegments[5] = waveRightSegment2;
            gestureController.AddGesture("WaveRight", waveRightSegments);

            IRelativeGestureSegment[] waveLeftSegments = new IRelativeGestureSegment[6];
            WaveLeftSegment1 waveLeftSegment1 = new WaveLeftSegment1();
            WaveLeftSegment2 waveLeftSegment2 = new WaveLeftSegment2();
            waveLeftSegments[0] = waveLeftSegment1;
            waveLeftSegments[1] = waveLeftSegment2;
            waveLeftSegments[2] = waveLeftSegment1;
            waveLeftSegments[3] = waveLeftSegment2;
            waveLeftSegments[4] = waveLeftSegment1;
            waveLeftSegments[5] = waveLeftSegment2;
            gestureController.AddGesture("WaveLeft", waveLeftSegments);

            IRelativeGestureSegment[] zoomInSegments = new IRelativeGestureSegment[3];
            zoomInSegments[0] = new ZoomSegment1();
            zoomInSegments[1] = new ZoomSegment2();
            zoomInSegments[2] = new ZoomSegment3();
            gestureController.AddGesture("ZoomIn", zoomInSegments);

            IRelativeGestureSegment[] zoomOutSegments = new IRelativeGestureSegment[3];
            zoomOutSegments[0] = new ZoomSegment3();
            zoomOutSegments[1] = new ZoomSegment2();
            zoomOutSegments[2] = new ZoomSegment1();
            gestureController.AddGesture("ZoomOut", zoomOutSegments);

            IRelativeGestureSegment[] swipeUpSegments = new IRelativeGestureSegment[3];
            swipeUpSegments[0] = new SwipeUpSegment1();
            swipeUpSegments[1] = new SwipeUpSegment2();
            swipeUpSegments[2] = new SwipeUpSegment3();
            gestureController.AddGesture("SwipeUp", swipeUpSegments);

            IRelativeGestureSegment[] swipeDownSegments = new IRelativeGestureSegment[3];
            swipeDownSegments[0] = new SwipeDownSegment1();
            swipeDownSegments[1] = new SwipeDownSegment2();
            swipeDownSegments[2] = new SwipeDownSegment3();
            gestureController.AddGesture("SwipeDown", swipeDownSegments);

            IRelativeGestureSegment[] leanleftSegments = new IRelativeGestureSegment[2];
            leanleftSegments[0] = new LeanLeftSegment1();
            leanleftSegments[1] = new LeanLeftSegment2();
            gestureController.AddGesture("LeanLeft", leanleftSegments);

            IRelativeGestureSegment[] leanrightSegments = new IRelativeGestureSegment[2];
            leanrightSegments[0] = new LeanRightSegment1();
            leanrightSegments[1] = new LeanRightSegment2();
            gestureController.AddGesture("LeanRight", leanrightSegments);

            IRelativeGestureSegment[] MRStepRightSegments = new IRelativeGestureSegment[3];
            MRStepRightSegments[0] = new MRStepRightSegment1();
            MRStepRightSegments[1] = new MRStepRightSegment2();
            MRStepRightSegments[2] = new MRStepRightSegment3();
            gestureController.AddGesture("MRStepRight", MRStepRightSegments);
        }

        #region Properties

        public static readonly DependencyProperty KinectSensorManagerProperty =
            DependencyProperty.Register(
                "KinectSensorManager",
                typeof(KinectSensorManager),
                typeof(MainWindow),
                new PropertyMetadata(null));

        public KinectSensorManager KinectSensorManager
        {
            get { return (KinectSensorManager)GetValue(KinectSensorManagerProperty); }
            set { SetValue(KinectSensorManagerProperty, value); }
        }

        /// <summary>
        /// Gets or sets the last recognized gesture.
        /// </summary>
        private string _gesture;
        public String Gesture
        {
            get { return _gesture; }

            private set
            {
                if (_gesture == value)
                    return;

                _gesture = value;

                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("Gesture"));
            }
        }

        #endregion Properties

        #region Events

        /// <summary>
        /// Event implementing INotifyPropertyChanged interface.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Events

        #region Event Handlers

        /// <summary>
        ///
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Gesture event arguments.</param>
        private void OnGestureRecognized(object sender, GestureEventArgs e)
        {
            IntPtr winId;
            winId = GetForegroundWindow();

            switch (e.GestureName)
            {
                case "Menu":
                    System.Diagnostics.Process.Start("C:/Program Files (x86)/Notepad++/notepad++.exe");
                    Gesture = "Menu";
                    break;
                case "WaveRight":
                    Gesture = "Wave Right";
                    break;
                case "WaveLeft":
                    Gesture = "Wave Left";
                    break;
                case "JoinedHands":
                    System.Windows.Forms.SendKeys.SendWait("A");
                    Gesture = "Joined Hands";
                    break;
                case "SwipeLeft":
                    MoveWindow(winId, 0, 0, 720, 800, true);
                    Gesture = "Swipe Left";
                    break;
                case "SwipeRight":
                    MoveWindow(winId, 720, 0, 720, 800, true);
                    Gesture = "Swipe Right";
 					break;
                case "SwipeUp":
                    Gesture = "Swipe Up";
                    break;
                case "SwipeDown":
                    Gesture = "Swipe Down";
                    break;
                case "ZoomIn":
                    ShowWindowAsync(winId, SW_SHOWMAXIMIZED);
                    Gesture = "Zoom In";
                    break;
                case "ZoomOut":
                    ShowWindowAsync(winId, SW_SHOWMINIMIZED);
                    Gesture = "Zoom Out";
                    break;
                case "LeanLeft":
                    Gesture = "Lean Left";
                    // Scroll left:
                    ScrollMouse(4);
                    break;
                case "LeanRight":
                    Gesture = "Lean Right";
                    // Scroll right:
                    ScrollMouse(3);
                    break;
                case "MRStepRight":
                    Gesture = "Minority Report Right";
                    break;

                default:
                    break;
            }

            _clearTimer.Start();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {
            using (SkeletonFrame frame = e.OpenSkeletonFrame())
            {
                if (frame == null)
                    return;

                // resize the skeletons array if needed
                if (skeletons.Length != frame.SkeletonArrayLength)
                    skeletons = new Skeleton[frame.SkeletonArrayLength];

                // get the skeleton data
                frame.CopySkeletonDataTo(skeletons);

                foreach (var skeleton in skeletons)
                {
                    // skip the skeleton if it is not being tracked
                    if (skeleton.TrackingState != SkeletonTrackingState.Tracked)
                        continue;

                    // update the gesture controller
                    gestureController.UpdateAllGestures(skeleton);
                }
            }
        }

        public static void ScrollMouse(int scrollType)
            {
                INPUT mouseInput = new INPUT();
                mouseInput.type = (int)InputType.INPUT_MOUSE;
                mouseInput.mi.dx = 0;
                mouseInput.mi.dy = 0;
                switch (scrollType) {
                    case 1:
                        // Scroll up:
                        mouseInput.mi.dwFlags = (int)MOUSEEVENTF.HWHEEL;
                        mouseInput.mi.mouseData = 50;
                        break;
                    case 2:
                        // Scroll down:
                        mouseInput.mi.dwFlags = (int)MOUSEEVENTF.HWHEEL;
                        mouseInput.mi.mouseData = -50;
                        break;
                    case 3:
                        // Scroll right:
                        mouseInput.mi.dwFlags = (int)MOUSEEVENTF.WHEEL;
                        mouseInput.mi.mouseData = 50;
                        break;
                    case 4:
                        // Scroll left:
                        mouseInput.mi.dwFlags = (int)MOUSEEVENTF.WHEEL;
                        mouseInput.mi.mouseData = -50;
                        break;
                }

                SendInput(1, new INPUT[] { mouseInput }, Marshal.SizeOf(mouseInput));
            }

        /// <summary>
        /// Clear text after some time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void clearTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Gesture = "";
            _clearTimer.Stop();
        }

        /// <summary>
        /// Maybe continue later
        /// </summary>
        private void SwitchAppForward()
        {
            Process[] procs = Process.GetProcesses();
            if (procs.Length != 0)
            {
                int i = 0;
            }
        }

        #endregion Event Handlers

    }
}