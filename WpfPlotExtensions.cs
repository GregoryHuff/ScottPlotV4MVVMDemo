using ScottPlot;
using ScottPlot.Plottable;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows;

namespace ScottPlotV4MVVMDemo
{
    public static class WpfPlotExtensions
    {
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.RegisterAttached(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(WpfPlotExtensions),
                new PropertyMetadata(OnItemsSourceChanged)
            );

        public static readonly DependencyProperty ExtentsProperty =
            DependencyProperty.RegisterAttached(
                "Extents",
                typeof(Point),
                typeof(WpfPlotExtensions),
                new PropertyMetadata(OnExtentsChanged)
            );

        private static WpfPlot? _staticPlot;
        private static readonly List<Point> _points = new();
        private static int _numPoints;
        private static ScatterPlot? _thePlot;

        public static void SetItemsSource(WpfPlot element, ObservableCollection<Point> value)
        {
            Trace.WriteLine("SetItemsSource");
            element.SetValue(ItemsSourceProperty, value);
        }

        public static IEnumerable GetItemsSource(WpfPlot element)
        {
            Trace.WriteLine("GetItemsSource");
            return (ObservableCollection<Point>)element.GetValue(ItemsSourceProperty);
        }

        private static void Value_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null) return;

            foreach (Point point in e.NewItems)
            {
                if (point.X == 0)
                {
                    _points.Clear();
                }
                _points.Add(point);
                if (_points.Count == 640)
                {
                    UpdatePlot();
                }
            }
        }

        private static void OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not WpfPlot plot) return;

            _staticPlot = plot;

            if (e.OldValue is ObservableCollection<Point> oldCollection)
            {
                oldCollection.CollectionChanged -= Value_CollectionChanged;
                if (oldCollection.Count == 0)
                    PreparePlot();
                
            }
            else
            {
                PreparePlot();
            }
            if (e.NewValue is ObservableCollection<Point> newCollection)
            {
                newCollection.CollectionChanged += Value_CollectionChanged;
                if (newCollection.Count > 0)
                {
                    PlotPoints(newCollection.ToList());
                }
            }
        }

        private static void PreparePlot()
        {
            _staticPlot!.Reset();
            _staticPlot.Plot.XLabel("Pixel");
            _staticPlot.Plot.YLabel("Intensity");
        }

        private static void PlotPoints(List<Point> collection)
        {
            double[] xArray = collection.Select(p => p.X).ToArray();
            double[] yArray = collection.Select(p => p.Y).ToArray();

            if (_staticPlot != null)
            {
                _staticPlot.Plot.Legend();
                _thePlot = _staticPlot.Plot.AddScatter(xArray, yArray);
                _staticPlot.Refresh();
                _staticPlot.Render();
            }
        }

        private static void OnExtentsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not WpfPlot plot) return;

            _staticPlot = plot;
            if (e.NewValue is Point pt)
            {
                SetPlotSize(pt.X, pt.Y);
                _numPoints = (int)pt.X;
            }
        }

        private static void SetPlotSize(double x, double y)
        {
            _staticPlot?.Plot.SetAxisLimitsX(0, x);
            _staticPlot?.Plot.SetAxisLimitsY(0, y);
        }

        public static void SetExtents(WpfPlot element, Point value)
        {
            element.SetValue(ExtentsProperty, value);
        }

        public static Point GetExtents(WpfPlot element)
        {
            return (Point)element.GetValue(ExtentsProperty);
        }

        private static void UpdatePlot()
        {
            if (_staticPlot == null) return;

            if (_thePlot != null)
            {
                //convert points collection to array and update plot.
                double[] xArray = _points.Select(p => p.X).ToArray();
                double[] yArray = _points.Select(p => p.Y).ToArray();
                _thePlot.Update(xArray, yArray);
            }
            else
                PlotPoints(_points);

            _staticPlot.Refresh();
        }
    }
}