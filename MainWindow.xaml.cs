using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using Microsoft.Win32;

namespace CarWPFApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Timers
        // To handling Accelerate
        private static Timer accTimer;
        // To handling Decelerate
        private static Timer decTimer;
        // To handling Direction when turning left
        private static Timer turnLTimer;
        // To handling Direction when turning right
        private static Timer turnRTimer;

        // Counter for multiplication
        private int aCounter = 0;
        
        // Constant Acceleration
        // When car accelerate to 100 km/h in 5 seconds, 
        // calculation formula is: (100 * 1000 m/3600 s) / 5 s => 5.56 m/s
        private double constantAcceleration = 5.56;

        // Variant to convert velocity from m/s to km/h
        // e.g. 5.56 m/s * 3.6 = 20.016 km/h
        private double velocityVariantFromMeterPerSecondToKmPerHour = 3.6;

        // Min velocity as default
        private double minVelocity = 0;

        // Max velocity
        private double maxVelocity = 500;

        // Velocity
        public double velocity;

        // Min direction as default
        // Direction in degrees cannot be less than 0
        private int minDirection = 0;

        // Max direction
        private int maxDirection = 359;

        // Direction
        public int direction;

        // Celsius
        private string strCelsius = "°";

        // Engine
        // Handling is it running or not
        public bool engineOn = false;

        // Input texts variables
        private string engineOnText = "Engine On";
        private string engineOffText = "Engine Off";

        // Message types
        private string msgTypeEngineOff = "EngineOff";
        //private const int msgTypeEngineOff = 1;

        // Message text
        private string msgEngineOff = "You cannot stop the engine, when valicity is not 0!";

        

        public MainWindow()
        {
            InitializeComponent();

            // Initialize engine on/off
            txtBoxEngineOnOff.Text = GetInputText();

            // Initialize velocity
            velocity = minVelocity;
            txtBoxVelocity.Text = velocity.ToString();

            // Initialize direction
            direction = minDirection;
            txtBoxDirection.Text = direction.ToString() + strCelsius;

            // Initialize buttons
            SetActivityOfButtons();
        }

        // Method to get right input text to the txtBoxEngineOnOff field
        // Case on: Engine On text is shown.
        // Case off: Engine Off text is shown.
        private string GetInputText()
        {
            return (engineOn) ? engineOnText : engineOffText;
        }

        // Method to enabled or disabled the buttons
        // depending on is engine on or off.
        private void SetActivityOfButtons()
        {
            // User hasn't start the engine or has stopped the engine, buttons are disabled
            if (!engineOn)
            {
                btnEngineOn.IsEnabled = true;
                btnEngineOff.IsEnabled = false;
                btnAccelerate.IsEnabled = false;
                btnDecelerate.IsEnabled = false;
                btnTurnLeft.IsEnabled = false;
                btnTurnRight.IsEnabled = false;
            }
            // User has started the engine, buttons are enabled
            else
            {
                btnEngineOn.IsEnabled = false;
                btnEngineOff.IsEnabled = true;
                btnAccelerate.IsEnabled = true;
                btnDecelerate.IsEnabled = true;
                btnTurnLeft.IsEnabled = true;
                btnTurnRight.IsEnabled = true;
            }
        }

        // Method to show MessageBox with content of different message type
        private void ShowMessageBox(string messageType)
        {
            /*
            switch (messageType)
            {
                case 1:

            }*/

            if (messageType == msgTypeEngineOff)
            {
                // Initialize the MessageBoxButton variables
                MessageBoxButton button = MessageBoxButton.OK;
                string caption = "Error Detected in Stop Engine";
                // Display the MessageBox
                MessageBox.Show(msgEngineOff, caption, button);
            }
        }

        // Method to set timer
        private void SetTimer(string timerType, Timer timer)
        {
            // Set a counter
            aCounter = 0;

            // Create a timer with a second interval
            timer = new Timer(1000);

            if (timerType.Equals("accTimer"))
            {
                // Hook up the Elapsed event for the timer
                timer.Elapsed += new ElapsedEventHandler(EngineOnTimer_ElapsedEventHandler);
                //timer.Elapsed += AccTimer_Elapsed; // jäi päälle + exception
            }
            // Set timer's AutoReset and Enabled
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        // Method to stop timer
        private void StopTimer(Timer timer)
        {
            timer.Stop();
            timer.Dispose();
        }

        // Method to count and show velocity for Acceleration
        private void EngineOnTimer_ElapsedEventHandler(object sender, ElapsedEventArgs e)
        {
            // Add counter for counting seconds
            aCounter++;
            // Count multiplication for velocity
            velocity = constantAcceleration * aCounter * velocityVariantFromMeterPerSecondToKmPerHour;
            // Show velocity
            txtBoxVelocity.Text = velocity.ToString();

        }

        // Method to count and show velocity for Acceleration
        private void AccTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Add counter for counting seconds
            aCounter++;
            // Count multiplication for velocity
            velocity = constantAcceleration * aCounter * velocityVariantFromMeterPerSecondToKmPerHour;
            // Show velocity
            txtBoxVelocity.Text = velocity.ToString();
        }

        private void btnEngineOn_Click(object sender, RoutedEventArgs e)
        {
            // Set engine started
            engineOn = true;
            // Show the text
            txtBoxEngineOnOff.Text = GetInputText();
            // Set background of the text box field
            txtBoxEngineOnOff.Background = Brushes.LightGreen;
            // Activate buttons
            SetActivityOfButtons();
        }

        private void btnEngineOff_Click(object sender, RoutedEventArgs e)
        {
            // Check the velocity
            // Engine can be stopped, if velocity is minVelocity
            if (velocity == minVelocity)
            {
                engineOn = false;
                txtBoxEngineOnOff.Text = GetInputText();
                txtBoxEngineOnOff.Background = Brushes.White;
                SetActivityOfButtons();
            }
            // Otherwise show MessageBox to user
            // Engine cannot be stopped
            else
            {
                ShowMessageBox(msgTypeEngineOff);
            }
        }

        private void btnAccelerate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Set the timer for counting velocity and showing it
            if (engineOn)
            {
                SetTimer("accTimer", accTimer);
            }
        }

        private void btnAccelerate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            

            // Set the timer for counting velocity and showing it
            if (engineOn)
            {
                SetTimer("accTimer", accTimer);
            }
            
        }

        private void btnAccelerate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Stop timer
            StopTimer(accTimer);
        }

        private void btnAccelerate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop timer
            StopTimer(accTimer);
        }

        private void btnDecelerate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnDecelerate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnTurnLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnTurnLeft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnTurnRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnTurnRight_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
