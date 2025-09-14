using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using SkiaSharp;
using System;

namespace coiled_tubing_app.ViewModels
{
    public class ChartViewModel
    {
        public string ChartId { get; set; } = string.Empty;
        public string ChartName { get; set; } = string.Empty;
        public ISeries[] Series { get; set; } = Array.Empty<ISeries>();

        public ChartViewModel(string chartId, string chartName)
        {
            ChartId = chartId;
            ChartName = chartName;
            InitializeChart();
        }

        private void InitializeChart()
        {
            // Initialize chart data based on chart ID
            switch (ChartId)
            {
                case "temp_chart":
                    Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = new double[] { 85.2, 87.5, 89.1, 91.3, 88.7, 92.4, 90.8, 94.1, 96.2, 93.5 },
                            Name = "Downhole Temperature (°C)",
                            Fill = null,
                            Stroke = new SolidColorPaint(SKColors.Red) { StrokeThickness = 3 },
                            GeometryFill = new SolidColorPaint(SKColors.Red),
                            GeometryStroke = new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 2 },
                            GeometrySize = 8
                        }
                    };
                    break;

                case "pressure_chart":
                    Series = new ISeries[]
                    {
                        new ColumnSeries<double>
                        {
                            Values = new double[] { 2500, 2750, 3200, 3850, 4100, 3900, 4200, 4450, 4600, 4350 },
                            Name = "Surface Pressure (PSI)",
                            Fill = new SolidColorPaint(SKColors.Blue)
                        },
                        new ColumnSeries<double>
                        {
                            Values = new double[] { 4500, 4800, 5200, 5600, 5900, 5700, 6100, 6300, 6500, 6200 },
                            Name = "Downhole Pressure (PSI)",
                            Fill = new SolidColorPaint(SKColors.DarkBlue)
                        }
                    };
                    break;

                case "flow_chart":
                    Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = new double[] { 150, 180, 220, 195, 245, 280, 260, 310, 295, 340, 325, 360 },
                            Name = "Oil Flow Rate (bbl/day)",
                            Fill = new SolidColorPaint(SKColors.Green.WithAlpha(100)),
                            Stroke = new SolidColorPaint(SKColors.DarkGreen) { StrokeThickness = 2 },
                            GeometryFill = new SolidColorPaint(SKColors.Green),
                            GeometryStroke = new SolidColorPaint(SKColors.DarkGreen)
                        },
                        new LineSeries<double>
                        {
                            Values = new double[] { 80, 95, 110, 105, 125, 140, 135, 160, 155, 175, 165, 190 },
                            Name = "Gas Flow Rate (Mcf/day)",
                            Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(100)),
                            Stroke = new SolidColorPaint(SKColors.DarkBlue) { StrokeThickness = 2 },
                            GeometryFill = new SolidColorPaint(SKColors.Blue),
                            GeometryStroke = new SolidColorPaint(SKColors.DarkBlue)
                        }
                    };
                    break;

                case "depth_chart":
                    Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = new double[] { 0, 500, 1200, 1800, 2500, 3200, 3800, 4500, 5200, 5800, 6400, 7000 },
                            Name = "Current Depth (ft)",
                            Fill = null,
                            Stroke = new SolidColorPaint(SKColors.OrangeRed) { StrokeThickness = 4 },
                            GeometryFill = new SolidColorPaint(SKColors.Orange),
                            GeometryStroke = new SolidColorPaint(SKColors.DarkRed) { StrokeThickness = 2 },
                            GeometrySize = 10
                        },
                        new LineSeries<double>
                        {
                            Values = new double[] { 0, 450, 1100, 1650, 2300, 2900, 3500, 4200, 4900, 5500, 6100, 6700 },
                            Name = "Target Depth (ft)",
                            Fill = null,
                            Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 2, PathEffect = new DashEffect(new float[] { 5, 5 }) },
                            GeometryFill = new SolidColorPaint(SKColors.LightGray),
                            GeometryStroke = new SolidColorPaint(SKColors.Gray),
                            GeometrySize = 6
                        }
                    };
                    break;

                case "sensor_status":
                    Series = new ISeries[]
                    {
                        new PieSeries<double>
                        {
                            Values = new double[] { 75 },
                            Name = "Active Sensors",
                            Fill = new SolidColorPaint(SKColors.Green)
                        },
                        new PieSeries<double>
                        {
                            Values = new double[] { 15 },
                            Name = "Warning Status",
                            Fill = new SolidColorPaint(SKColors.Orange)
                        },
                        new PieSeries<double>
                        {
                            Values = new double[] { 8 },
                            Name = "Maintenance Required",
                            Fill = new SolidColorPaint(SKColors.Yellow)
                        },
                        new PieSeries<double>
                        {
                            Values = new double[] { 2 },
                            Name = "Offline",
                            Fill = new SolidColorPaint(SKColors.Red)
                        }
                    };
                    break;

                case "system_load":
                    Series = new ISeries[]
                    {
                        new ScatterSeries<double>
                        {
                            Values = new double[] { 45, 52, 38, 67, 58, 43, 71, 62, 55, 48, 64, 59 },
                            Name = "CPU Load (%)",
                            Fill = new SolidColorPaint(SKColors.Purple),
                            Stroke = new SolidColorPaint(SKColors.DarkMagenta) { StrokeThickness = 2 },
                            GeometrySize = 15
                        },
                        new ScatterSeries<double>
                        {
                            Values = new double[] { 32, 28, 45, 39, 52, 35, 48, 41, 37, 44, 39, 46 },
                            Name = "Memory Usage (%)",
                            Fill = new SolidColorPaint(SKColors.Orange),
                            Stroke = new SolidColorPaint(SKColors.DarkOrange) { StrokeThickness = 2 },
                            GeometrySize = 12
                        }
                    };
                    break;

                default:
                    // Default empty chart
                    Series = new ISeries[]
                    {
                        new LineSeries<double>
                        {
                            Values = new double[] { 0 },
                            Name = "No Data Available",
                            Fill = null,
                            Stroke = new SolidColorPaint(SKColors.Gray) { StrokeThickness = 2 }
                        }
                    };
                    break;
            }
        }
    }
}