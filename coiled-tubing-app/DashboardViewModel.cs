using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using LiveChartsCore.SkiaSharpView.VisualElements;
using SkiaSharp;
using System;
using System.Diagnostics;

namespace coiled_tubing_app
{
    public class DashboardViewModel
    {
        // Chart 1: Temperature Line Chart
        public ISeries[] TemperatureLineSeries { get; set; } = Array.Empty<ISeries>();

        // Chart 2: Pressure Column Chart  
        public ISeries[] PressureColumnSeries { get; set; } = Array.Empty<ISeries>();

        // Chart 3: Well Status Pie Chart
        public ISeries[] WellStatusPieSeries { get; set; } = Array.Empty<ISeries>();

        // Chart 4: Flow Rate Area Chart
        public ISeries[] FlowRateAreaSeries { get; set; } = Array.Empty<ISeries>();

        // Chart 5: Equipment Status Scatter Chart
        public ISeries[] EquipmentScatterSeries { get; set; } = Array.Empty<ISeries>();

        // Chart 6: Depth vs Time Line Chart
        public ISeries[] DepthTimeSeries { get; set; } = Array.Empty<ISeries>();

        public LabelVisual? Title { get; set; }

        public DashboardViewModel()
        {
            try
            {
                InitializeCharts();
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Debug.WriteLine($"COM Exception in DashboardViewModel: {ex}");
                InitializeFallbackCharts();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"General Exception in DashboardViewModel: {ex}");
                InitializeFallbackCharts();
            }
        }

        private void InitializeFallbackCharts()
        {
            // Initialize with empty arrays to prevent null reference exceptions
            TemperatureLineSeries = Array.Empty<ISeries>();
            PressureColumnSeries = Array.Empty<ISeries>();
            WellStatusPieSeries = Array.Empty<ISeries>();
            FlowRateAreaSeries = Array.Empty<ISeries>();
            EquipmentScatterSeries = Array.Empty<ISeries>();
            DepthTimeSeries = Array.Empty<ISeries>();
            Title = null;
        }

        private void InitializeCharts()
        {
            // Chart 1: Temperature Line Chart
            TemperatureLineSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 85.2, 87.5, 89.1, 91.3, 88.7, 92.4, 90.8, 94.1 },
                    Name = "Downhole Temperature",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 },
                    GeometryFill = new SolidColorPaint(SKColors.Red),
                    GeometryStroke = new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 2 }
                }
            };

            // Chart 2: Pressure Column Chart
            PressureColumnSeries = new ISeries[]
            {
                new ColumnSeries<double>
                {
                    Values = new double[] { 2500, 2750, 3200, 3850, 4100, 3900, 4200 },
                    Name = "Surface Pressure",
                    Fill = new SolidColorPaint(SKColors.Blue)
                },
                new ColumnSeries<double>
                {
                    Values = new double[] { 4500, 4800, 5200, 5600, 5900, 5700, 6100 },
                    Name = "Downhole Pressure",
                    Fill = new SolidColorPaint(SKColors.DarkBlue)
                }
            };

            // Chart 3: Well Status Pie Chart
            WellStatusPieSeries = new ISeries[]
            {
                new PieSeries<double>
                {
                    Values = new double[] { 45 },
                    Name = "Active Wells",
                    Fill = new SolidColorPaint(SKColors.Green)
                },
                new PieSeries<double>
                {
                    Values = new double[] { 25 },
                    Name = "Maintenance",
                    Fill = new SolidColorPaint(SKColors.Orange)
                },
                new PieSeries<double>
                {
                    Values = new double[] { 20 },
                    Name = "Standby",
                    Fill = new SolidColorPaint(SKColors.Gray)
                },
                new PieSeries<double>
                {
                    Values = new double[] { 10 },
                    Name = "Offline",
                    Fill = new SolidColorPaint(SKColors.Red)
                }
            };

            // Chart 4: Flow Rate Area Chart
            FlowRateAreaSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 150, 180, 220, 195, 245, 280, 260, 310, 295, 340 },
                    Name = "Oil Flow Rate",
                    Fill = new SolidColorPaint(SKColors.Green.WithAlpha(100)),
                    Stroke = new SolidColorPaint(SKColors.DarkGreen) { StrokeThickness = 2 },
                    GeometryFill = new SolidColorPaint(SKColors.Green),
                    GeometryStroke = new SolidColorPaint(SKColors.DarkGreen)
                },
                new LineSeries<double>
                {
                    Values = new double[] { 80, 95, 110, 105, 125, 140, 135, 160, 155, 175 },
                    Name = "Gas Flow Rate",
                    Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(100)),
                    Stroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 2 },
                    GeometryFill = new SolidColorPaint(SKColors.Blue),
                    GeometryStroke = new SolidColorPaint(SKColors.DarkBlue)
                }
            };

            // Chart 5: Equipment Status Scatter Chart
            EquipmentScatterSeries = new ISeries[]
            {
                new ScatterSeries<double>
                {
                    Values = new double[] { 95, 87, 92, 88, 94, 89, 96, 91 },
                    Name = "Equipment Efficiency",
                    Fill = new SolidColorPaint(SKColors.Purple),
                    Stroke = new SolidColorPaint(SKColors.DarkMagenta) { StrokeThickness = 2 },
                    GeometrySize = 12
                }
            };

            // Chart 6: Depth vs Time Line Chart
            DepthTimeSeries = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = new double[] { 0, 500, 1200, 1800, 2500, 3200, 3800, 4500, 5200, 5800 },
                    Name = "Current Depth",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 4 },
                    GeometryFill = new SolidColorPaint(SKColors.Orange),
                    GeometryStroke = new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 2 },
                    GeometrySize = 8
                },
                new LineSeries<double>
                {
                    Values = new double[] { 0, 450, 1100, 1650, 2300, 2900, 3500, 4200, 4900, 5500 },
                    Name = "Target Depth",
                    Fill = null,
                    Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 2, PathEffect = new DashEffect(new float[] { 5, 5 }) },
                    GeometryFill = new SolidColorPaint(SKColors.LightGray),
                    GeometryStroke = new SolidColorPaint(SKColors.Gray),
                    GeometrySize = 6
                }
            };

            // Initialize Title
            Title = new LabelVisual
            {
                Text = "Coiled Tubing Operations Dashboard",
                TextSize = 28,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            };
        }
    }
}