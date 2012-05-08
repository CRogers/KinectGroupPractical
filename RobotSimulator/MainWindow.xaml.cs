using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RobotSimulator.View;
using RobotSimulator.Utility;
using RobotSimulator.Model;
using RobotSimulator.Controller;

namespace RobotSimulator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window,I_Observable<I_KeyListener>
    {
        private ICollection<I_KeyListener> key_listeners = new LinkedList<I_KeyListener>();
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Initialise the display:
            //new Linker(this,viewport);
            //An example of how you might use the Portable class
            Portable p = new Portable(viewport);
            p.setCameraToListenToKeys(this);
            p.setMotor(MotorManager.LEFT_SHOULDER_MOTOR1, 90);
        }

        private void viewport_KeyDown(object sender, KeyEventArgs e)
        {
            //Forward the key down event to all the listeners:
            foreach (var listener in key_listeners)
            {
                listener.KeyPressed(e);
            }
        }
        private void viewport_KeyUp(object sender, KeyEventArgs e)
        {
            //Forward the key up event to all the listeners:
            foreach (var listener in key_listeners)
            {
                listener.KeyReleased(e);
            }
        }

        public void addListener(I_KeyListener listener)
        {
            key_listeners.Add(listener);
        }

        public void removeListener(I_KeyListener listener)
        {
            key_listeners.Remove(listener);
        }
    }
}
