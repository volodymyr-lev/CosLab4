using System.Numerics;
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

namespace Lab2DPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Plotter plotter;
        private double I1, I2;

        private string selectedMethod;

        public MainWindow()
        {
            InitializeComponent();

            plotter = new Plotter(WpfPlot1);
        }

        private void BtnDrawGiven_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            plotter.PlotGivenDiscrete(I1, I2);
        }

        private void BtnDrawSpline_Click(object sender, RoutedEventArgs e)
        {
            GetData();
            plotter.PlotSpline(I1, I2, selectedMethod);
        }

        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            plotter.ClearPlot();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            RadioButton radioButton= sender as RadioButton;

            selectedMethod = radioButton.Content.ToString();
        }

        private bool GetData()
        {
            if (string.IsNullOrEmpty(TextBoxI1.Text) || string.IsNullOrEmpty(TextBoxI2.Text)) return false;

            (I1, I2) = InputConverter.GetIntervals(TextBoxI1.Text, TextBoxI2.Text);
            return true;
        }
    }
}