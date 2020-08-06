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
        private const int msgTypeEngineOff = 1;
        private int msgTypeDecelerateStop = 2;

        // Message texts
        private string msgEngineOff = "You cannot stop the engine, when valicity is not 0!";
        private string msgDecelerateStop = "Breaking is stopped, velicity is 0!";

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
        //private void ShowMessageBox(string messageType)
        private void ShowMessageBox(int messageType)
        {
            switch (messageType)
            {
                case 1:
                    // Initialize the MessageBoxButton variables
                    MessageBoxButton button = MessageBoxButton.OK;
                    string caption = "Error Detected in Stop Engine";
                    // Display the MessageBox
                    MessageBox.Show(msgEngineOff, caption, button);
                    break;
                case 2:
                    // Initialize the MessageBoxButton variables
                    button = MessageBoxButton.OK;
                    caption = "Error Detected in Breaking";
                    // Display the MessageBox
                    MessageBox.Show(msgDecelerateStop, caption, button);
                    break;
                default:
                    break;
            }
        }

        // Method to set timer
        private void SetTimer(string timerType, ref Timer timer)
        {
            // Set a counter
            aCounter = 0;

            // Create a timer with a second interval
            timer = new Timer(1000);

            if (timerType.Equals("accTimer"))
            {
                // Hook up the Elapsed event for the timer
                timer.Elapsed += AccTimer_Elapsed; 
            }
            else if (timerType.Equals("decTimer"))
            {
                timer.Elapsed += DecTimer_Elapsed;
            }
            // Set timer's AutoReset and Enabled
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        // Method to stop timer
        private void StopTimer(ref Timer timer)
        {
            timer.Stop();
            timer.Dispose();
        }

        // Method to count and show velocity for Acceleration
        private void AccTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Add counter for counting seconds
            aCounter++;
            // Count multiplication for velocity
            double result = constantAcceleration * aCounter * velocityVariantFromMeterPerSecondToKmPerHour;
            velocity += result;
            // Show velocity
            txtBoxVelocity.Dispatcher.Invoke(() =>
            {
                txtBoxVelocity.Text = velocity.ToString();
            });
            
        }

        // Method to count and show velocity for Deceleration
        private void DecTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (velocity >= minVelocity)
            {
                // Add counter for counting seconds
                aCounter++;
                // Count multiplication for velocity
                double result = constantAcceleration * aCounter * velocityVariantFromMeterPerSecondToKmPerHour;
                velocity -= result;

                // Show velocity
                txtBoxVelocity.Dispatcher.Invoke(() =>
                {
                    txtBoxVelocity.Text = velocity.ToString();
                });
            }
            else
            {
                // Stop timer
                decTimer.Enabled = false;
                StopTimer(ref decTimer);
                // Show velocity
                velocity = 0;
                txtBoxVelocity.Dispatcher.Invoke(() =>
                {
                    txtBoxVelocity.Text = velocity.ToString();
                });
                // Inform user that velocity has reached 0.
                ShowMessageBox(msgTypeDecelerateStop);
            }
            
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
                SetTimer("accTimer", ref accTimer);
            }
        }

        private void btnAccelerate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Stop timer
            StopTimer(ref accTimer);           
        }

        private void btnDecelerate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Set the timer for counting velocity and showing it
            if (engineOn)
            {
                SetTimer("decTimer", ref decTimer);
            }
        }

        private void btnDecelerate_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Stop timer
            StopTimer(ref decTimer);
        }

        // TODO
        private void btnTurnLeft_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (engineOn)
            {
                if (velocity > 0 && direction <= maxDirection)
                {
                    // Set timer
                }

            }
        }
        
        // TODO
        private void btnTurnLeft_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Stop timer
        }
        
        // TODO
        private void btnTurnRight_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (engineOn)
            {
                if (velocity > 0 && direction <= maxDirection)
                {
                    // Set timer
                }

            }
        }

        // TODO
        private void btnTurnRight_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Stop timer
        }

        // TODO: not working???
        private void btnAccelerate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the timer for counting velocity and showing it
            if (engineOn)
            {
                SetTimer("accTimer", ref accTimer);
            }
            
        }

        // TODO: not working???
        private void btnAccelerate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // Stop timer
            StopTimer(ref accTimer);
        }

        // TODO: not working???
        private void btnDecelerate_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Set the timer for counting velocity and showing it
            if (engineOn)
            {
                SetTimer("decTimer", ref decTimer);
            }
        }
        // TODO: not working???
        private void btnDecelerate_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        // TODO: not working???
        private void btnTurnLeft_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        // TODO: not working???
        private void btnTurnLeft_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
        // TODO: not working???
        private void btnTurnRight_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

        }
        // TODO: not working???
        private void btnTurnRight_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

    }
}
