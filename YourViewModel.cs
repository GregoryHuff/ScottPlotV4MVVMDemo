using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Threading;

namespace ScottPlotV5MVVMDemo
{
    public partial class YourViewModel : INotifyPropertyChanged
    {
        private readonly DispatcherTimer timer;
        private readonly Stopwatch stopwatch = new();

        private double[] values;
        public double[] Values
        {
            get => values;
            set
            {
                values = value;
                OnPropertyChanged();
            }
        }

        private double[] moreValues;
        public double[] MoreValues
        {
            get => moreValues;
            set
            {
                moreValues = value;
                OnPropertyChanged();
            }
        }

        public YourViewModel()
        {
            values = new double[640];
            moreValues = new double[640];
            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            timer.Tick += Timer_Tick;
            timer.Start();
            stopwatch.Start();
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            double phase = stopwatch.Elapsed.TotalSeconds;
            double multiplier = 2 * Math.PI / Values.Length;
            //update values
            for (int i = 0; i < Values.Length; i++)
                Values[i] = Math.Sin(i * multiplier + phase);
            //and more values
            for (int i = 0; i < MoreValues.Length; i++)
                MoreValues[i] = Math.Cos(i * multiplier + phase);

            OnPropertyChanged(nameof(Values));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



}