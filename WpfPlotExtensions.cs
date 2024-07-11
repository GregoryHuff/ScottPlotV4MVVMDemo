using ScottPlot;
using ScottPlot.Plottable;
using ScottPlot.Plottables;
using ScottPlot.WPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Threading;

namespace ScottPlotV5MVVMDemo
{
    public static class ScottPlotMVVMExtension
    { 
        //Title
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.RegisterAttached(
                "Title",
                typeof(string),
                typeof(ScottPlotMVVMExtension),
                new PropertyMetadata("MVVM Plot", TitleChanged));

        private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfPlot plot)
            {
                string title = (string)e.NewValue;
                plot.Plot.Title(title);
                plot.Refresh();
            }
        }

        public static void SetTitle(WpfPlot element, string title) => element.SetValue(TitleProperty, title);

        public static string GetTitle(WpfPlot element) => (string)element.GetValue(TitleProperty);

        //Frame rate

        private const int defaultFrameRate = 30;

        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.RegisterAttached(
                "FrameRate",
                typeof(int),
                typeof(ScottPlotMVVMExtension),
                new PropertyMetadata(defaultFrameRate, FrameRateChanged));

        public static void SetFrameRate(WpfPlot element, int value) => element.SetValue(FrameRateProperty, value);

        public static int GetFrameRate(WpfPlot element) => (int)element.GetValue(FrameRateProperty);

        private static void FrameRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfPlot plot)
            {
                int frameRate = (int)e.NewValue;
                TimeSpan interval = TimeSpan.FromSeconds(1.0 / frameRate);

                if (plot.Tag is DispatcherTimer timer)
                {
                    timer.Interval = interval;
                }
                else
                {
                    timer = new DispatcherTimer { Interval = interval };
                    timer.Tick += (s, args) => plot.Refresh();
                    timer.Start();
                    plot.Tag = timer;
                }
            }
        }

        //ItemsSource (data to plot)

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ScottPlotMVVMExtension),
                new PropertyMetadata(OnItemsSourceChanged));

        public static void SetItemsSource(WpfPlot element, IEnumerable value) => element.SetValue(ItemsSourceProperty, value);

        public static IEnumerable GetItemsSource(WpfPlot element) => (IEnumerable)element.GetValue(ItemsSourceProperty);

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is WpfPlot plot)
            {
                if (e.NewValue is double[] newValues)
                {
                    plot.Plot.Clear();
                    plot.Plot.Add.Signal(newValues);
                    plot.Refresh();
                }
            }
        }
    }

}