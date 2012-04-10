//******************************************************************************************************
//  ChartWindow.xaml.cs - Gbtc
//
//  Copyright © 2010, Grid Protection Alliance.  All Rights Reserved.
//
//  Licensed to the Grid Protection Alliance (GPA) under one or more contributor license agreements. See
//  the NOTICE file distributed with this work for additional information regarding copyright ownership.
//  The GPA licenses this file to you under the Eclipse Public License -v 1.0 (the "License"); you may
//  not use this file except in compliance with the License. You may obtain a copy of the License at:
//
//      http://www.opensource.org/licenses/eclipse-1.0.php
//
//  Unless agreed to in writing, the subject software distributed under the License is distributed on an
//  "AS-IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. Refer to the
//  License for the specific language governing permissions and limitations.
//
//  Code Modification History:
//  ----------------------------------------------------------------------------------------------------
//  08/10/2010 - Stephen C. Wills
//       Generated original version of source code.
//
//******************************************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using openHistorian;
using openHistorian.V1;
using openHistorian.V1.Files;

namespace HistorianView
{
    /// <summary>
    /// Interaction logic for ChartWindow.xaml
    /// </summary>
    public partial class ChartWindow : Window
    {
        #region [ Members ]

        // Nested Types
        private struct ChartBoundary
        {
            public DateTime? Left { get; set; }
            public DateTime? Right { get; set; }
            public double? Top { get; set; }
            public double? Bottom { get; set; }
        }

        private class DataPointWrapper
        {
            private IDataPoint m_point;

            public DataPointWrapper(IDataPoint point)
            {
                m_point = point;
            }

            public IDataPoint ArchiveDataPoint
            {
                get
                {
                    return m_point;
                }
            }

            public DateTime Time
            {
                get
                {
                    return m_point.Time;
                }
            }

            public float Value
            {
                get
                {
                    return m_point.Value;
                }
            }
        }

        // Events
        public event EventHandler ChartUpdated;

        // Fields
        private ICollection<ArchiveFile> m_archiveFiles;
        private ICollection<MetadataRecord> m_visiblePoints;
        private int m_chartResolution;
        private LinkedList<ChartBoundary> m_backwardHistory;
        private LinkedList<ChartBoundary> m_forwardHistory;
        private List<Color> m_lineColors;
        private Rectangle m_selectionArea;
        private Point m_mousePressedPosition;
        private Point m_mouseDragPosition;
        private bool m_selecting;
        private bool m_ctrlPressed;
        private bool m_shiftPressed;

        #endregion

        #region [ Constructors ]

        public ChartWindow()
        {
            m_archiveFiles = new List<ArchiveFile>();
            m_visiblePoints = new List<MetadataRecord>();
            m_chartResolution = 100;

            m_backwardHistory = new LinkedList<ChartBoundary>();
            m_forwardHistory = new LinkedList<ChartBoundary>();

            InitializeComponent();
            InitializeColors();
            InitializeSelectionArea();
        }

        #endregion

        #region [ Properties ]

        public ICollection<ArchiveFile> ArchiveFiles
        {
            get
            {
                return m_archiveFiles;
            }
            set
            {
                m_archiveFiles = value;
                m_visiblePoints.Clear();
            }
        }

        public ICollection<MetadataRecord> VisiblePoints
        {
            get
            {
                return m_visiblePoints;
            }
            set
            {
                m_visiblePoints = value;
            }
        }

        public int ChartResolution
        {
            get
            {
                return m_chartResolution;
            }
            set
            {
                m_chartResolution = value;
            }
        }

        public DateTime StartTime
        {
            get
            {
                return m_xAxis.ActualMinimum.Value;
            }
        }

        public DateTime EndTime
        {
            get
            {
                return m_xAxis.ActualMaximum.Value;
            }
        }

        private Grid PlotArea
        {
            get
            {
                FrameworkElement chartChild = (VisualTreeHelper.GetChildrenCount(m_chart) == 0) ? null : VisualTreeHelper.GetChild(m_chart, 0) as FrameworkElement;
                return (chartChild == null) ? null : chartChild.FindName("PlotArea") as Grid;
            }
        }

        #endregion

        #region [ Methods ]

        /// <summary>
        /// Sets the boundaries for the x-axis.
        /// </summary>
        /// <param name="startTimeString">A string representation of the lower x-axis boundary.</param>
        /// <param name="endTimeString">A string representation of the upper x-axis boundary.</param>
        public void SetInterval(string startTimeString, string endTimeString)
        {
            DateTime startTime = TimeTag.Parse(startTimeString).ToDateTime();
            DateTime endTime = TimeTag.Parse(endTimeString).ToDateTime();

            if (startTime > endTime)
                throw new ArgumentException("startTime > endTime");

            ClearHistory();
            m_xAxis.Minimum = null;
            m_xAxis.Maximum = null;
            m_yAxis.Minimum = null;
            m_yAxis.Maximum = null;

            m_xAxis.Minimum = startTime;
            m_xAxis.Maximum = endTime;
        }

        /// <summary>
        /// Updates the chart after changes have been made to the chart boundaries or the data displayed in the chart.
        /// </summary>
        public void UpdateChart()
        {
            Cursor windowCursor = Cursor;
            int colorIndex = 0;
            DateTime startTime = m_xAxis.Minimum ?? TimeTag.MinValue.ToDateTime();
            DateTime endTime = m_xAxis.Maximum ?? TimeTag.MaxValue.ToDateTime();

            Cursor = Cursors.Wait;

            m_chart.Series.Clear();
            foreach (ArchiveFile file in m_archiveFiles)
            {
                foreach (MetadataRecord record in file.MetadataFile.Read())
                {
                    if (m_chartResolution > 0 && colorIndex < m_lineColors.Count && m_visiblePoints.Any(point => record == point))
                    {
                        LineSeries series = new LineSeries();
                        IEnumerable<IDataPoint> data = file.ReadData(record.HistorianID, startTime, endTime);
                        int interval = (data.Count() / m_chartResolution) + 1;
                        int pointCount = 0;

                        // Change how data points are displayed.
                        series.DataPointStyle = new Style(typeof(LineDataPoint));
                        series.DataPointStyle.Setters.Add(new Setter(LineDataPoint.BackgroundProperty, new SolidColorBrush(m_lineColors[colorIndex])));
                        series.DataPointStyle.Setters.Add(new Setter(LineDataPoint.TemplateProperty, new ControlTemplate()));
                        colorIndex++;

                        // Set the title of the series as it will appear in the legend.
                        series.Title = record.Name;

                        // Filter the data to 100 data points.
                        series.ItemsSource = data.Where(point => (pointCount++ % interval) == 0).Select(point => new DataPointWrapper(point));
                        series.IndependentValuePath = "Time";
                        series.DependentValuePath = "Value";

                        // Add the series to the chart.
                        m_chart.Series.Add(series);
                    }
                }
            }

            UpdateLayout();
            Cursor = windowCursor;
            OnChartUpdated();
        }

        // Initializes the list of colors used for lines on the graph.
        private void InitializeColors()
        {
            m_lineColors = new List<Color>();
            m_lineColors.Add(Colors.Blue);
            m_lineColors.Add(Colors.Red);
            m_lineColors.Add(Colors.Yellow);
            m_lineColors.Add(Colors.Green);
            m_lineColors.Add(Colors.Orange);
            m_lineColors.Add(Colors.Purple);
            m_lineColors.Add(Colors.Black);
            m_lineColors.Add(Colors.White);
            m_lineColors.Add(Colors.Brown);
            m_lineColors.Add(Colors.Cyan);
            m_lineColors.Add(Colors.Lime);
            m_lineColors.Add(Colors.Salmon);
        }

        // Initializes the selection area used to select regions of the graph for zooming in.
        private void InitializeSelectionArea()
        {
            m_selectionArea = new Rectangle();
            m_selectionArea.Stroke = new SolidColorBrush(Colors.Black);
            m_selectionArea.HorizontalAlignment = HorizontalAlignment.Left;
            m_selectionArea.VerticalAlignment = VerticalAlignment.Top;
        }

        // Updates the rectangle indicating the area selected by the user for zooming.
        private void UpdateSelectionArea()
        {
            Grid plotArea = PlotArea;

            if (plotArea != null)
            {
                double left = Math.Min(m_mousePressedPosition.X, m_mouseDragPosition.X);
                double top = Math.Min(m_mousePressedPosition.Y, m_mouseDragPosition.Y);
                double right = Math.Max(m_mousePressedPosition.X, m_mouseDragPosition.X);
                double bottom = Math.Max(m_mousePressedPosition.Y, m_mouseDragPosition.Y);

                left = Math.Max(left, 0.0);
                top = Math.Max(top, 0.0);
                right = Math.Min(right, plotArea.ActualWidth);
                bottom = Math.Min(bottom, plotArea.ActualHeight);

                if (m_ctrlPressed)
                {
                    top = 0.0;
                    bottom = plotArea.ActualHeight;
                }

                if (m_shiftPressed)
                {
                    left = 0.0;
                    right = plotArea.ActualWidth;
                }

                m_selectionArea.Margin = new Thickness(left, top, 0.0, 0.0);
                m_selectionArea.Width = right - left;
                m_selectionArea.Height = bottom - top;
            }
        }

        // Triggers the ChartUpdated event.
        private void OnChartUpdated()
        {
            if (ChartUpdated != null)
                ChartUpdated(this, new EventArgs());
        }

        // Moves backward in the chart history.
        private void GoBackInHistory()
        {
            if (m_backwardHistory.Count > 0)
            {
                ChartBoundary boundary = m_backwardHistory.Last.Value;

                AddCurrentToForwardHistory();
                m_backwardHistory.RemoveLast();
                m_xAxis.Minimum = boundary.Left;
                m_xAxis.Maximum = boundary.Right;
                m_yAxis.Maximum = boundary.Top;
                m_yAxis.Minimum = boundary.Bottom;

                if (m_backwardHistory.Count == 0)
                    m_backButton.IsEnabled = false;

                UpdateChart();
            }
        }

        // Moves forward in the chart history.
        private void GoForwardInHistory()
        {
            if (m_forwardHistory.Count > 0)
            {
                ChartBoundary boundary = m_forwardHistory.First.Value;

                AddCurrentToBackwardHistory();
                m_forwardHistory.RemoveFirst();
                m_xAxis.Minimum = boundary.Left;
                m_xAxis.Maximum = boundary.Right;
                m_yAxis.Maximum = boundary.Top;
                m_yAxis.Minimum = boundary.Bottom;

                if (m_forwardHistory.Count == 0)
                    m_forwardButton.IsEnabled = false;

                UpdateChart();
            }
        }

        // Changes the chart boundaries based on the given percentages.
        private void Zoom(double leftPercent, double topPercent, double rightPercent, double bottomPercent)
        {
            DateTime xAxisMinimum = m_xAxis.ActualMinimum.Value;
            DateTime xAxisMaximum = m_xAxis.ActualMaximum.Value;
            double yAxisMinimum = m_yAxis.Minimum ?? m_yAxis.ActualMinimum.Value;
            double yAxisMaximum = m_yAxis.Maximum ?? m_yAxis.ActualMaximum.Value;

            // Save current boundary to backward chart history and clear out forward history.
            AddCurrentToBackwardHistory();
            ClearForwardHistory();

            // Clear chart boundaries.
            //m_xAxis.Minimum = null;
            //m_xAxis.Maximum = null;
            //m_yAxis.Minimum = null;
            //m_yAxis.Maximum = null;

            // Update chart boundaries.
            m_xAxis.Minimum = new DateTime((long)(xAxisMaximum.Ticks * leftPercent + xAxisMinimum.Ticks * (1.0 - leftPercent)));
            m_xAxis.Maximum = new DateTime((long)(xAxisMaximum.Ticks * rightPercent + xAxisMinimum.Ticks * (1.0 - rightPercent)));
            m_yAxis.Minimum = yAxisMinimum * bottomPercent + yAxisMaximum * (1.0 - bottomPercent);
            m_yAxis.Maximum = yAxisMinimum * topPercent + yAxisMaximum * (1.0 - topPercent);
            UpdateChart();
        }

        // Adds the current boundary to the backward history.
        private void AddCurrentToBackwardHistory()
        {
            m_backwardHistory.AddLast(GetCurrentChartBoundary());
            m_backButton.IsEnabled = true;

            if (m_backwardHistory.Count > 20)
                m_backwardHistory.RemoveFirst();
        }

        // Adds the current boundary to the forward history.
        private void AddCurrentToForwardHistory()
        {
            m_forwardHistory.AddFirst(GetCurrentChartBoundary());
            m_forwardButton.IsEnabled = true;
        }

        // Clears all history.
        private void ClearHistory()
        {
            ClearBackwardHistory();
            ClearForwardHistory();
        }

        // Clears the backward history.
        private void ClearBackwardHistory()
        {
            m_backwardHistory.Clear();
            m_backButton.IsEnabled = false;
        }

        // Clears the forward history.
        private void ClearForwardHistory()
        {
            m_forwardHistory.Clear();
            m_forwardButton.IsEnabled = false;
        }

        // Gets the current bounds of the chart's axes.
        private ChartBoundary GetCurrentChartBoundary()
        {
            return new ChartBoundary()
            {
                Left = m_xAxis.Minimum ?? m_xAxis.ActualMinimum,
                Right = m_xAxis.Maximum ?? m_xAxis.ActualMaximum,
                Top = m_yAxis.Maximum ?? m_yAxis.ActualMaximum,
                Bottom = m_yAxis.Minimum ?? m_yAxis.ActualMinimum
            };
        }

        // Detects left ctrl and left shift key presses used for different types of zooming.
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            m_ctrlPressed = m_ctrlPressed || e.Key == Key.LeftCtrl;
            m_shiftPressed = m_shiftPressed || e.Key == Key.LeftShift;
            UpdateSelectionArea();
        }

        // Detects left ctrl and left shift key releases to return zooming to normal.
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            m_ctrlPressed = m_ctrlPressed && e.Key != Key.LeftCtrl;
            m_shiftPressed = m_shiftPressed && e.Key != Key.LeftShift;
            UpdateSelectionArea();
        }

        // Detects mouse clicks on the plot area used for selecting regions for zooming.
        private void PlotArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid plotArea = sender as Grid;

            if (plotArea != null)
                m_mousePressedPosition = e.GetPosition(plotArea);

            m_selecting = true;
        }

        // Detects mouse movements inside the window used for selecting regions for zooming.
        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            Grid plotArea = PlotArea;

            if (plotArea != null && m_selecting)
            {
                m_mouseDragPosition = e.GetPosition(plotArea);
                UpdateSelectionArea();

                if (!plotArea.Children.Contains(m_selectionArea))
                    plotArea.Children.Add(m_selectionArea);
            }
        }

        // Detects mouse releases inside the window used for selecting regions for zooming.
        private void Window_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Grid plotArea = PlotArea;

            if (plotArea != null && plotArea.Children.Contains(m_selectionArea))
            {
                double leftPercent = m_selectionArea.Margin.Left / plotArea.ActualWidth;
                double topPercent = m_selectionArea.Margin.Top / plotArea.ActualHeight;
                double rightPercent = leftPercent + (m_selectionArea.Width / plotArea.ActualWidth);
                double bottomPercent = topPercent + (m_selectionArea.Height / plotArea.ActualHeight);

                Zoom(leftPercent, topPercent, rightPercent, bottomPercent);
                plotArea.Children.Remove(m_selectionArea);
            }

            m_selecting = false;
        }

        // Sets chart boundaries to the most recent value in backward history.
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            GoBackInHistory();
        }

        // Sets chart boundaries to the least recent value in forward history.
        private void ForwardButton_Click(object sender, RoutedEventArgs e)
        {
            GoForwardInHistory();
        }

        // Zooms in on the chart by 10%.
        private void ZoomInButton_Click(object sender, RoutedEventArgs e)
        {
            Zoom(.1, .1, .9, .9);
        }

        // Zooms out on the chart by 10%.
        private void ZoomOutButton_Click(object sender, RoutedEventArgs e)
        {
            Zoom(-.1, -.1, 1.1, 1.1);
        }

        // Zooms to fit the data in the current time window.
        private void ZoomFitButton_Click(object sender, RoutedEventArgs e)
        {
            AddCurrentToBackwardHistory();
            ClearForwardHistory();

            // Update the minimum value of the x-axis.
            m_xAxis.Minimum = null;
            m_xAxis.Minimum = m_xAxis.ActualMinimum;

            // Update the maximum value of the x-axis.
            m_xAxis.Maximum = null;
            m_xAxis.Maximum = m_xAxis.ActualMaximum;

            // Update the y-axis boundaries.
            m_yAxis.Minimum = null;
            m_yAxis.Maximum = null;

            UpdateChart();
        }

        #endregion
    }
}
