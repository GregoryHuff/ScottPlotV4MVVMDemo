using ScottPlot.Plottable;
using ScottPlot;
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
using System.Diagnostics;
using System.Windows.Threading;

namespace ScottPlotV5MVVMDemo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly double[] Values = new double[640];
        private readonly Stopwatch stopwatch = new Stopwatch();
        private DispatcherTimer? Timer1 = new();
        public MainWindow()
        {
            InitializeComponent();
            ScottPlot.Plottables.Signal signalPlot = NonMVVMPlot.Plot.Add.Signal(Values);
            NonMVVMPlot.Plot.Title("Non-MVVM plot");
            NonMVVMPlot.Refresh();
            //ScottPlot.Plot testPlot = new Plot();
            //testPlot.Axes.Title.Label.Text = "what?";
            Timer1.Tick += Timer1_Tick;
            Timer1.Start();
            stopwatch.Start();
        }

        private void Timer1_Tick(object? sender, EventArgs e)
        {
            // change the values inside the array any time
            double phase = stopwatch.Elapsed.TotalSeconds;
            double multiplier = 2 * Math.PI / Values.Length;
            for (int i = 0; i < Values.Length; i++)
                Values[i] = Math.Sin(i * multiplier + phase);

            // force a redraw after changing the data values
            NonMVVMPlot.Refresh();
        }

    }

}