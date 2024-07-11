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
    //public static class ScottPlotMVVMExtension
    //{
    //    private const int DefaultFrameRate = 10;
    //    public static readonly DependencyProperty TitleProperty =
    //        DependencyProperty.RegisterAttached(
    //            "Title",
    //            typeof(string),
    //            typeof(ScottPlotMVVMExtension),
    //            new PropertyMetadata("MVVM Plot", TitleChanged));

    //    private static void TitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is WpfPlot plot)
    //        {
    //            _staticPlot = plot; //I guess?
    //            string title = (string)e.NewValue;
    //            //_staticPlot ??= new WpfPlot();
    //            _staticPlot.Plot.Title(title);
    //        }
    //    }

    //    public static void SetTitle(WpfPlot element, string title)
    //    {
    //        element.SetValue(TitleProperty, title);
    //    }
    //    public static string GetTitle(WpfPlot element, string title)
    //    {
    //        string? _title = element.GetValue(TitleProperty) as string;
    //        _title ??= "MVVM Plot";
    //        return _title;
    //    }

    //    public static readonly DependencyProperty FrameRateProperty =
    //        DependencyProperty.RegisterAttached(
    //            "FrameRate",
    //            typeof(int),
    //            typeof(ScottPlotMVVMExtension),
    //            new PropertyMetadata(DefaultFrameRate, FrameRateChanged)); // Default to 30 FPS

    //    public static void SetFrameRate(WpfPlot element, int value)
    //    {
    //        element.SetValue(FrameRateProperty, value);
    //    }

    //    public static int GetFrameRate(WpfPlot element)
    //    {
    //        return (int)element.GetValue(FrameRateProperty);
    //    }

    //    private static DispatcherTimer? _refreshTimer;
    //    private static TimeSpan _maxUpdateInterval = TimeSpan.FromSeconds(1.0 / DefaultFrameRate); 
    //    private static WpfPlot? _staticPlot = new WpfPlot();
    //    private static Signal? signalPlot;

    //    static ScottPlotMVVMExtension()
    //    {
    //        _refreshTimer = new DispatcherTimer
    //        {
    //            Interval = _maxUpdateInterval
    //        };
    //        _refreshTimer.Tick += (s, e) =>
    //        {
    //            _staticPlot?.Refresh();
    //        };
    //        _refreshTimer.Start();
    //    }

    //    private static void FrameRateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is WpfPlot plot)
    //        {
    //            _maxUpdateInterval = TimeSpan.FromSeconds(1.0 / (int)e.NewValue);
    //            if (_refreshTimer != null)
    //            {
    //                _refreshTimer.Interval = _maxUpdateInterval;
    //            }
    //        }
    //    }

    //    public static readonly DependencyProperty ItemsSourceProperty =
    //        DependencyProperty.RegisterAttached(
    //            "ItemsSource",
    //            typeof(IEnumerable),
    //            typeof(ScottPlotMVVMExtension),
    //            new PropertyMetadata(OnItemsSourceChanged));

    //    public static void SetItemsSource(WpfPlot element, IEnumerable value)
    //    {
    //        element.SetValue(ItemsSourceProperty, value);
    //    }

    //    public static IEnumerable GetItemsSource(WpfPlot element)
    //    {
    //        return (IEnumerable)element.GetValue(ItemsSourceProperty);
    //    }

    //    private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    //    {
    //        if (d is WpfPlot plot)
    //        {
    //            _staticPlot = plot;
    //            if (e.NewValue is double[] newValues)
    //            {
    //                //_staticPlot ??= new WpfPlot();
    //                _staticPlot.Plot.Clear();
    //                signalPlot = plot.Plot.Add.Signal(newValues);
    //                plot.Refresh();
    //            }
    //        }
    //    }
    //}


    ///GPT:
    public static class ScottPlotMVVMExtension
    {
        private const int defaultFrameRate = 30;

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

        public static void SetTitle(WpfPlot element, string title)
        {
            element.SetValue(TitleProperty, title);
        }

        public static string GetTitle(WpfPlot element)
        {
            return (string)element.GetValue(TitleProperty);
        }

        public static readonly DependencyProperty FrameRateProperty =
            DependencyProperty.RegisterAttached(
                "FrameRate",
                typeof(int),
                typeof(ScottPlotMVVMExtension),
                new PropertyMetadata(defaultFrameRate, FrameRateChanged));

        public static void SetFrameRate(WpfPlot element, int value)
        {
            element.SetValue(FrameRateProperty, value);
        }

        public static int GetFrameRate(WpfPlot element)
        {
            return (int)element.GetValue(FrameRateProperty);
        }

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

        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(ScottPlotMVVMExtension),
                new PropertyMetadata(OnItemsSourceChanged));

        public static void SetItemsSource(WpfPlot element, IEnumerable value)
        {
            element.SetValue(ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(WpfPlot element)
        {
            return (IEnumerable)element.GetValue(ItemsSourceProperty);
        }

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