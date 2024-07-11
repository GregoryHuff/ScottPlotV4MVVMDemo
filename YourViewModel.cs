using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ScottPlotV4MVVMDemo
{
    public partial class YourViewModel : ObservableValidator
    {
        [ObservableProperty]
        private Point _extents = new Point(640, 65535);
        private const int maxIterations = 5000;
        private int _iterationCount = 0;

        [ObservableProperty]
        private ObservableCollection<Point> _points = new ObservableCollection<Point>();

        private DispatcherTimer _timer;
        private readonly Random _random = new Random();
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public YourViewModel()
        {
            InitializeTimer();
        }

        private void InitializeTimer()
        {
            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000 / 100) // 100 Hz
            };
            _timer.Tick += TimerTick;
            _timer.Start();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            _stopwatch.Restart();
            RandomPoints();
            _stopwatch.Stop();
            double renderTime = _stopwatch.Elapsed.TotalMilliseconds;
            double fps = 1000 / renderTime;
            Trace.WriteLine($"Frame rendered in {renderTime} ms ({fps:F2} FPS)");
            _iterationCount++;
            if (_iterationCount >= maxIterations)
            {
                _timer.Stop();
                Trace.WriteLine("DONE");
            }
        }

        private void RandomPoints()
        {
            Points.Clear();
            for (int i = 0; i < 640; i++)
            {
                ushort val = (ushort)_random.Next(0, 65535);
                Points.Add(new Point(i, val));
            }
        }
    }
}