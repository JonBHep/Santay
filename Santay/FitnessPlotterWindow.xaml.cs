using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Santay;

public partial class FitnessPlotterWindow
{
    public FitnessPlotterWindow()
    {
        InitializeComponent();
        _coreData = new PersonProfile();
    }

    private readonly PersonProfile _coreData;
    private double _horizontalDateIncrement = 1.5; // sets the horizontal pixels per day

    private struct GraphPoint
    {
        public double X { get; }
        public double Y { get; }

        public GraphPoint(double xval, double yval)
        {
            X = xval;
            Y = yval;
        }
    }

    private void ButtonClose_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void ButtonPlotWeight_Click(object sender, RoutedEventArgs e)
    {
        PlotWeight();
    }

    private void PlotWeight()
    {
        StatisticsBorder.Visibility = Visibility.Visible;

        DateOnly earliestDate = DateOnly.MaxValue;

        double newmeasuredweight = 0;
        double minmeasuredweight = double.MaxValue;
        double maxmeasuredweight = double.MinValue;

        DateOnly newdate =DateOnly.FromDateTime(DateTime.Today);
        DateOnly mindate =DateOnly.FromDateTime(DateTime.Today);
        DateOnly maxdate =DateOnly.FromDateTime(DateTime.Today);

        foreach (PersonProfile.Weight wt in _coreData.WeightReadings)
        {
            if (wt.WgtWhen.CompareTo(earliestDate) < 0)
            {
                earliestDate = wt.WgtWhen;
            }

            newmeasuredweight = wt.WgtKilograms;
            newdate = wt.WgtWhen;
            if (wt.WgtKilograms < minmeasuredweight)
            {
                minmeasuredweight = wt.WgtKilograms;
                mindate = wt.WgtWhen;
            }

            if (wt.WgtKilograms > maxmeasuredweight)
            {
                maxmeasuredweight = wt.WgtKilograms;
                maxdate = wt.WgtWhen;
            }
        }

        NewDateTb.Text = newdate.ToShortDateString();
        MinDateTb.Text = mindate.ToShortDateString();
        MaxDateTb.Text = maxdate.ToShortDateString();
        NewValueTb.Text
            = $"{newmeasuredweight.ToString("0.0")} kg = {BodyStatics.WeightAsStonesAndPoundsString(newmeasuredweight)} = BMI {BodyStatics.BmiOf(newmeasuredweight, _coreData.HeightInMetres).ToString("0.0")}";
        MinValueTb.Text
            = $"{minmeasuredweight.ToString("0.0")} kg = {BodyStatics.WeightAsStonesAndPoundsString(minmeasuredweight)} = BMI {BodyStatics.BmiOf(minmeasuredweight, _coreData.HeightInMetres).ToString("0.0")}";
        MaxValueTb.Text
            = $"{maxmeasuredweight.ToString("0.0")} kg = {BodyStatics.WeightAsStonesAndPoundsString(maxmeasuredweight)} = BMI {BodyStatics.BmiOf(maxmeasuredweight, _coreData.HeightInMetres).ToString("0.0")}";

        // Create a list of points
        List<GraphPoint> pointList = new List<GraphPoint>();
        foreach (PersonProfile.Weight wt in _coreData.WeightReadings)
        {
            int ts = wt.WgtWhen.DayNumber - earliestDate.DayNumber; // elapsed days from base date
            GraphPoint gp = new GraphPoint(ts, wt.WgtKilograms);
            pointList.Add(gp);
        }

        // find min and max values on each axis
        double minX = double.MaxValue;
        double maxX = double.MinValue;
        double minY = double.MaxValue;
        double maxY = double.MinValue;
        foreach (GraphPoint gp in pointList)
        {
            if (gp.X < minX)
            {
                minX = gp.X;
            }

            if (gp.X > maxX)
            {
                maxX = gp.X;
            }

            if (gp.Y < minY)
            {
                minY = gp.Y;
            }

            if (gp.Y > maxY)
            {
                maxY = gp.Y;
            }
        }

        // ensure my ideal weight's upper and lower limits are within these limits
        double tgtLo = _coreData.IdealWeightLowerLimit;
        if (tgtLo < minY)
        {
            minY = tgtLo;
        }

        if (tgtLo > maxY)
        {
            maxY = tgtLo;
        }

        double tgtHi = _coreData.IdealWeightHigherLimit;
        if (tgtHi < minY)
        {
            minY = tgtHi;
        }

        if (tgtHi > maxY)
        {
            maxY = tgtHi;
        }

        // add some leeway below minimum and above maximum
        minY -= 0.8;
        maxY += 0.8;

        double valueSpanHorizontal = maxX - minX;
        double valueSpanVertical = maxY - minY;

        CanvasGraph.Children.Clear();
        double marginHorizontal = 12;
        double marginVertical = 12;

        if (CanvasGraph.ActualHeight == 0)
        {
            return;
        }

        double physicalSpanFitToScreen = CanvasGraph.ActualWidth - (2 * marginHorizontal);
        double physicalSpanSpaceOutValues = _horizontalDateIncrement * valueSpanHorizontal;
        double physicalSpanHorizontal;
        if (physicalSpanSpaceOutValues > physicalSpanFitToScreen)
        {
            physicalSpanHorizontal = physicalSpanSpaceOutValues;
            CanvasGraph.Width = physicalSpanHorizontal + (2 * marginHorizontal);
        }
        else
        {
            physicalSpanHorizontal = physicalSpanFitToScreen;
        }

        double physicalSpanVertical = CanvasGraph.ActualHeight - (2 * marginVertical);

        // test graph area
        Rectangle outliner = new Rectangle
        {
            Width = physicalSpanHorizontal, Height = physicalSpanVertical, Stroke = new SolidColorBrush(Colors.Gray)
            , Fill = new SolidColorBrush(Colors.WhiteSmoke)
        };
        Canvas.SetLeft(outliner, marginHorizontal);
        Canvas.SetTop(outliner, marginVertical);
        CanvasGraph.Children.Add(outliner);

        // Draw vertical lines and labels for each January 1st
        int year = earliestDate.Year;
        int thisYear = DateTime.Today.Year;
        while (year < thisYear)
        {
            year++;
            DateOnly newyearsday = new DateOnly(year, 1, 1);
            int ts = newyearsday.DayNumber - earliestDate.DayNumber; // elapsed days from base date
            Line ln = new Line
            {
                Stroke = new SolidColorBrush(Colors.Gray), StrokeThickness = 1
                , X1 = marginHorizontal + (ts - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
                , Y1 = marginVertical + physicalSpanVertical
            };
            ln.X2 = ln.X1;
            ln.Y2 = marginVertical;
            CanvasGraph.Children.Add(ln);

            Border br = new Border
            {
                BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.SaddleBrown)
                , CornerRadius = new CornerRadius(4), Background = new SolidColorBrush(Colors.SeaShell)
            };
            TextBlock tb = new TextBlock();
            Canvas.SetLeft(br, ln.X1 + 2);
            Canvas.SetTop(br, marginVertical + 2);
            tb.Background = new SolidColorBrush(Colors.SeaShell);
            tb.Foreground = new SolidColorBrush(Colors.SaddleBrown);
            tb.Margin = new Thickness(4);
            tb.Padding = new Thickness(4);
            tb.Text = year.ToString();
            br.Child = tb;
            CanvasGraph.Children.Add(br);
        }

        // add horizontal line and label to indicate my minimum recorded weight
        Line lineMin = new Line()
        {
            Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 1
            , X1 = marginHorizontal + physicalSpanHorizontal
            , Y1 = marginVertical + physicalSpanVertical -
                   ((minmeasuredweight - minY) * (physicalSpanVertical / valueSpanVertical))
            , X2 = marginHorizontal
        };
        lineMin.Y2 = lineMin.Y1;
        lineMin.StrokeDashArray = new DoubleCollection {4, 8};
        CanvasGraph.Children.Add(lineMin);
        // add label to indicate my minimum recorded weight
        Border borderMin = new Border()
        {
            BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.DarkGreen)
            , CornerRadius = new CornerRadius(3), Background = new SolidColorBrush(Colors.LightCyan)
        };
        TextBlock textblockMin = new TextBlock()
        {
            Background = new SolidColorBrush(Colors.LightCyan), Foreground = new SolidColorBrush(Colors.DarkGreen)
            , Margin = new Thickness(2), Padding = new Thickness(4, 2, 4, 2)
            , Text = $"Minumum recorded weight {minmeasuredweight:#0.0} kg"
        };
        borderMin.Child = textblockMin;
        Canvas.SetRight(borderMin, 200);
        Canvas.SetTop(borderMin, lineMin.Y1 - 14);
        CanvasGraph.Children.Add(borderMin);

        // draw horizontal lines to indicate my ideal weight/BMI - lower and upper limits and target

        Line lineLoIdeal = new Line()
        {
            Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 1
            , X1 = marginHorizontal + physicalSpanHorizontal
            , Y1 = marginVertical + physicalSpanVertical - ((tgtLo - minY) * (physicalSpanVertical / valueSpanVertical))
            , X2 = marginHorizontal
        };
        lineLoIdeal.Y2 = lineLoIdeal.Y1;
        CanvasGraph.Children.Add(lineLoIdeal);
        // add label to indicate my target weight/BMI
        Border borderLoIdeal = new Border()
        {
            BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Green)
            , CornerRadius = new CornerRadius(3), Background = new SolidColorBrush(Colors.MintCream)
        };
        TextBlock textblockLoIdeal = new TextBlock()
        {
            Background = new SolidColorBrush(Colors.MintCream), Foreground = new SolidColorBrush(Colors.Green)
            , Margin = new Thickness(2), Padding = new Thickness(4, 2, 4, 2)
            , Text
                = $"Ideal weight minimum {tgtLo:#0.0} kg = BMI {BodyStatics.BmiOf(tgtLo, _coreData.HeightInMetres).ToString("0.0")}"
        };
        Canvas.SetRight(borderLoIdeal, 200);
        Canvas.SetTop(borderLoIdeal, lineLoIdeal.Y1 - 14);
        borderLoIdeal.Child = textblockLoIdeal;
        CanvasGraph.Children.Add(borderLoIdeal);

        Line lineHiIdeal = new Line()
        {
            Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 1
            , X1 = marginHorizontal + physicalSpanHorizontal
            , Y1 = marginVertical + physicalSpanVertical - ((tgtHi - minY) * (physicalSpanVertical / valueSpanVertical))
            , X2 = marginHorizontal
        };
        lineHiIdeal.Y2 = lineHiIdeal.Y1;
        CanvasGraph.Children.Add(lineHiIdeal);
        // add label to indicate my target weight/BMI
        Border borderHiIdeal = new Border()
        {
            BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Green)
            , CornerRadius = new CornerRadius(3), Background = new SolidColorBrush(Colors.MintCream)
        };
        TextBlock textblockHiIdeal = new TextBlock()
        {
            Background = new SolidColorBrush(Colors.MintCream), Foreground = new SolidColorBrush(Colors.Green)
            , Margin = new Thickness(2), Padding = new Thickness(4, 2, 4, 2)
            , Text
                = $"Ideal weight maximum {tgtHi:#0.0} kg = BMI {BodyStatics.BmiOf(tgtHi, _coreData.HeightInMetres).ToString("0.0")}"
        };
        Canvas.SetRight(borderHiIdeal, 200);
        Canvas.SetTop(borderHiIdeal, lineHiIdeal.Y1 - 14);
        borderHiIdeal.Child = textblockHiIdeal;
        CanvasGraph.Children.Add(borderHiIdeal);

        double tgtMd = tgtLo + ((tgtHi - tgtLo) / 2);
        Line lineMdIdeal = new Line()
        {
            Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 2
            , X1 = marginHorizontal + physicalSpanHorizontal
            , Y1 = marginVertical + physicalSpanVertical - ((tgtMd - minY) * (physicalSpanVertical / valueSpanVertical))
            , X2 = marginHorizontal
        };
        lineMdIdeal.Y2 = lineMdIdeal.Y1;
        CanvasGraph.Children.Add(lineMdIdeal);
        // add label to indicate my target weight/BMI
        Border borderMdIdeal = new Border()
        {
            BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Green)
            , CornerRadius = new CornerRadius(3), Background = new SolidColorBrush(Colors.MintCream)
        };
        TextBlock textblockMdIdeal = new TextBlock()
        {
            Background = new SolidColorBrush(Colors.MintCream), Foreground = new SolidColorBrush(Colors.Green)
            , Margin = new Thickness(2), Padding = new Thickness(4, 2, 4, 2)
            , Text
                = $"Ideal weight mean {tgtMd:#0.0} kg = BMI {BodyStatics.BmiOf(tgtMd, _coreData.HeightInMetres).ToString("0.0")}"
        };
        Canvas.SetRight(borderMdIdeal, 200);
        Canvas.SetTop(borderMdIdeal, lineMdIdeal.Y1 - 14);
        borderMdIdeal.Child = textblockMdIdeal;
        CanvasGraph.Children.Add(borderMdIdeal);

        // Plot line graph
        double lastx = 0;
        double lasty = 0;
        bool begun = false;
        foreach (GraphPoint gp in pointList)
        {
            if (begun)
            {
                Line ln = new Line
                {
                    Stroke = new SolidColorBrush(Colors.Blue), StrokeThickness = 2
                    , X1 = marginHorizontal + (lastx - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
                    , Y1 = marginVertical + physicalSpanVertical -
                           ((lasty - minY) * (physicalSpanVertical / valueSpanVertical))
                    , X2 = marginHorizontal + (gp.X - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
                    , Y2 = marginVertical + physicalSpanVertical -
                           ((gp.Y - minY) * (physicalSpanVertical / valueSpanVertical))
                };
                CanvasGraph.Children.Add(ln);
            }

            begun = true;
            lastx = gp.X;
            lasty = gp.Y;
        }

        GraphScrollViewer.ScrollToRightEnd();
    }

    // private void buttonPlotWaist_Click(object sender, RoutedEventArgs e)
    // {
    //     PlotWaist();
    // }

    // private void PlotWaist()
    // {
    //     StatisticsBorder.Visibility = Visibility.Visible;
    //
    //     DateOnly earliestDate = DateOnly.MaxValue;
    //
    //     double newmeasuredwaist = 0;
    //     double minmeasuredwaist = double.MaxValue;
    //     double maxmeasuredwaist = double.MinValue;
    //
    //     DateOnly newdate =DateOnly.FromDateTime( DateTime.Today);
    //     DateOnly mindate = DateOnly.FromDateTime( DateTime.Today);
    //     DateOnly maxdate = DateOnly.FromDateTime( DateTime.Today);
    //
    //     foreach (PersonProfile.Waist wt in _coreData.WaistReadings)
    //     {
    //         if (wt.WstWhen.CompareTo(earliestDate) < 0)
    //         {
    //             earliestDate = wt.WstWhen;
    //         }
    //
    //         newmeasuredwaist = wt.WstCentimetres;
    //         newdate = wt.WstWhen;
    //         if (wt.WstCentimetres < minmeasuredwaist)
    //         {
    //             minmeasuredwaist = wt.WstCentimetres;
    //             mindate = wt.WstWhen;
    //         }
    //
    //         if (wt.WstCentimetres > maxmeasuredwaist)
    //         {
    //             maxmeasuredwaist = wt.WstCentimetres;
    //             maxdate = wt.WstWhen;
    //         }
    //     }
    //
    //     NewDateTb.Text = newdate.ToShortDateString();
    //     MinDateTb.Text = mindate.ToShortDateString();
    //     MaxDateTb.Text = maxdate.ToShortDateString();
    //     NewValueTb.Text = $"{newmeasuredwaist:0.0} cm";
    //     MinValueTb.Text = $"{minmeasuredwaist:0.0} cm";
    //     MaxValueTb.Text = $"{maxmeasuredwaist:0.0} cm";
    //
    //     // Create a list of points
    //     List<GraphPoint> pointList = new List<GraphPoint>();
    //     foreach (PersonProfile.Waist wt in _coreData.WaistReadings)
    //     {
    //         int ts = wt.WstWhen.DayNumber - earliestDate.DayNumber; // elapsed days from base date
    //         GraphPoint gp = new GraphPoint(ts, wt.WstCentimetres);
    //         pointList.Add(gp);
    //     }
    //
    //     // find min and max values on each axis
    //     double minX = double.MaxValue;
    //     double maxX = double.MinValue;
    //     double minY = double.MaxValue;
    //     double maxY = double.MinValue;
    //     foreach (GraphPoint gp in pointList)
    //     {
    //         if (gp.X < minX)
    //         {
    //             minX = gp.X;
    //         }
    //
    //         if (gp.X > maxX)
    //         {
    //             maxX = gp.X;
    //         }
    //
    //         if (gp.Y < minY)
    //         {
    //             minY = gp.Y;
    //         }
    //
    //         if (gp.Y > maxY)
    //         {
    //             maxY = gp.Y;
    //         }
    //     }
    //
    //     double minmeasuredweight = minY;
    //     // ensure my target waist measurement is within these limits
    //     double tgt = BodyStatics.TargetWaist;
    //     if (tgt < minY)
    //     {
    //         minY = tgt;
    //     }
    //
    //     if (tgt > maxY)
    //     {
    //         maxY = tgt;
    //     }
    //
    //     // add some leeway below minimum and above maximum on Y axis
    //     minY -= 0.8;
    //     maxY += 0.8;
    //
    //     double valueSpanHorizontal = maxX - minX;
    //     double valueSpanVertical = maxY - minY;
    //
    //     CanvasGraph.Children.Clear();
    //     double marginHorizontal = 12;
    //     double marginVertical = 12;
    //
    //     if (CanvasGraph.ActualHeight == 0)
    //     {
    //         return;
    //     }
    //
    //     CanvasGraph.Width = _horizontalDateIncrement * valueSpanHorizontal + (2 * marginHorizontal);
    //     double physicalSpanHorizontal = _horizontalDateIncrement * valueSpanHorizontal;
    //     double physicalSpanVertical = CanvasGraph.ActualHeight - (2 * marginVertical);
    //
    //     // test graph area
    //     Rectangle outliner = new Rectangle
    //     {
    //         Width = physicalSpanHorizontal, Height = physicalSpanVertical, Stroke = new SolidColorBrush(Colors.Gray)
    //         , Fill = new SolidColorBrush(Colors.WhiteSmoke)
    //     };
    //     Canvas.SetLeft(outliner, marginHorizontal);
    //     Canvas.SetTop(outliner, marginVertical);
    //     CanvasGraph.Children.Add(outliner);
    //
    //     // Draw vertical lines and labels for each January 1st
    //     int year = earliestDate.Year;
    //     while (year < DateTime.Today.Year)
    //     {
    //         year++;
    //         DateOnly newyearsday = new DateOnly(year, 1, 1);
    //         int ts = newyearsday.DayNumber - earliestDate.DayNumber; // elapsed days from base date
    //         Line ln = new Line()
    //         {
    //             Stroke = new SolidColorBrush(Colors.Gray), StrokeThickness = 1
    //             , X1 = marginHorizontal + (ts - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
    //             , Y1 = marginVertical + physicalSpanVertical
    //         };
    //         ln.X2 = ln.X1;
    //         ln.Y2 = marginVertical;
    //         CanvasGraph.Children.Add(ln);
    //
    //         Border br = new Border()
    //         {
    //             BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.SaddleBrown)
    //             , CornerRadius = new CornerRadius(4), Background = new SolidColorBrush(Colors.SeaShell)
    //         };
    //         TextBlock tb = new TextBlock();
    //         Canvas.SetLeft(br, ln.X1 + 2);
    //         Canvas.SetTop(br, marginVertical + 2);
    //         tb.Background = new SolidColorBrush(Colors.SeaShell);
    //         tb.Foreground = new SolidColorBrush(Colors.SaddleBrown);
    //         tb.Margin = new Thickness(4);
    //         tb.Padding = new Thickness(4);
    //         tb.Text = year.ToString();
    //         br.Child = tb;
    //         CanvasGraph.Children.Add(br);
    //     }
    //
    //     // draw horizontal line to indicate my target waist measurement
    //     Line lntarget = new Line()
    //     {
    //         Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 2
    //         , X1 = marginHorizontal + physicalSpanHorizontal
    //         , Y1 = marginVertical + physicalSpanVertical - ((tgt - minY) * (physicalSpanVertical / valueSpanVertical))
    //         , X2 = marginHorizontal
    //     };
    //     lntarget.Y2 = lntarget.Y1;
    //     CanvasGraph.Children.Add(lntarget);
    //     // add label to indicate my target waist measurement
    //     Border brtarget = new Border
    //     {
    //         BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.Green)
    //         , CornerRadius = new CornerRadius(3), Background = new SolidColorBrush(Colors.MintCream)
    //     };
    //     TextBlock tbtarget = new TextBlock();
    //     Canvas.SetRight(brtarget, 200);
    //     Canvas.SetTop(brtarget, lntarget.Y1 - 14);
    //     tbtarget.Background = new SolidColorBrush(Colors.MintCream);
    //     tbtarget.Foreground = new SolidColorBrush(Colors.Green);
    //     tbtarget.Margin = new Thickness(2);
    //     tbtarget.Padding = new Thickness(4, 2, 4, 2);
    //     tbtarget.Text = $"Target waist measurement {tgt:0.0} cm";
    //     brtarget.Child = tbtarget;
    //     CanvasGraph.Children.Add(brtarget);
    //
    //     // draw horizontal line to indicate my minimum recorded waist
    //     Line lnmin = new Line()
    //     {
    //         Stroke = new SolidColorBrush(Colors.Green), StrokeThickness = 1
    //         , X1 = marginHorizontal + physicalSpanHorizontal
    //         , Y1 = marginVertical + physicalSpanVertical -
    //                ((minmeasuredweight - minY) * (physicalSpanVertical / valueSpanVertical))
    //         , X2 = marginHorizontal
    //     };
    //     lnmin.Y2 = lnmin.Y1;
    //     lnmin.StrokeDashArray = new DoubleCollection {4, 8};
    //     CanvasGraph.Children.Add(lnmin);
    //     // add label to indicate my minimum recorded waist
    //     Border brmin = new Border
    //     {
    //         BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.DarkGreen)
    //         , CornerRadius = new CornerRadius(3), Background = new SolidColorBrush(Colors.LightCyan)
    //     };
    //     TextBlock tbmin = new TextBlock()
    //     {
    //         Background = new SolidColorBrush(Colors.LightCyan), Foreground = new SolidColorBrush(Colors.DarkGreen)
    //         , Margin = new Thickness(2), Padding = new Thickness(4, 2, 4, 2)
    //         , Text = $"Minimum recorded waist measurement {minmeasuredweight:0.0}"
    //     };
    //     Canvas.SetRight(brmin, 200);
    //     Canvas.SetTop(brmin, lnmin.Y1 - 14);
    //     brmin.Child = tbmin;
    //     CanvasGraph.Children.Add(brmin);
    //
    //     // Plot line graph
    //     double lastx = 0;
    //     bool begun = false;
    //     double lasty = 0;
    //     foreach (GraphPoint gp in pointList)
    //     {
    //         if (begun)
    //         {
    //             Line ln = new Line
    //             {
    //                 Stroke = new SolidColorBrush(Colors.Blue), StrokeThickness = 2
    //                 , X1 = marginHorizontal + (lastx - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
    //                 , Y1 = marginVertical + physicalSpanVertical -
    //                        ((lasty - minY) * (physicalSpanVertical / valueSpanVertical))
    //                 , X2 = marginHorizontal + (gp.X - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
    //                 , Y2 = marginVertical + physicalSpanVertical -
    //                        ((gp.Y - minY) * (physicalSpanVertical / valueSpanVertical))
    //             };
    //             CanvasGraph.Children.Add(ln);
    //         }
    //
    //         begun = true;
    //         lastx = gp.X;
    //         lasty = gp.Y;
    //     }
    //
    //     GraphScrollViewer.ScrollToRightEnd();
    // }

    private void buttonPlotBloodPressure_Click(object sender, RoutedEventArgs e)
    {
        PlotBloodPressure();
    }

    private void PlotBloodPressure()
    {
        StatisticsBorder.Visibility = Visibility.Hidden;

        var earliestDate = DateTime.MaxValue;
        var latestDate = DateTime.MinValue;
        foreach (var wt in _coreData.TensionReadings)
        {
            if (wt.BprWhen.CompareTo(earliestDate) < 0)
            {
                earliestDate = wt.BprWhen;
            }

            if (wt.BprWhen.CompareTo(latestDate) > 0)
            {
                latestDate = wt.BprWhen;
            }
        }

        // Create lists of points
        var syspointList = new List<GraphPoint>();
        var diapointList = new List<GraphPoint>();
        var pulpointList = new List<GraphPoint>();
        foreach (var wt in _coreData.TensionReadings)
        {
            var elapsed = wt.BprWhen - earliestDate;
            var dd = elapsed.TotalDays;
            
            diapointList.Add(new GraphPoint(dd, wt.BpDiastolic));
            syspointList.Add(new GraphPoint(dd, wt.BpSystolic));
            pulpointList.Add(new GraphPoint(dd, wt.Pulse));
        }

        // find min and max values on each axis
        var minX = double.MaxValue;
        var maxX = double.MinValue;
        var minY = double.MaxValue;
        var maxY = double.MinValue;
        foreach (var gp in diapointList)
        {
            if (gp.X < minX)
            {
                minX = gp.X;
            }

            if (gp.X > maxX)
            {
                maxX = gp.X;
            }

            if (gp.Y < minY)
            {
                minY = gp.Y;
            }

            if (gp.Y > maxY)
            {
                maxY = gp.Y;
            }
        }

        foreach (var gp in syspointList)
        {
            if (gp.Y < minY)
            {
                minY = gp.Y;
            }

            if (gp.Y > maxY)
            {
                maxY = gp.Y;
            }
        }

        foreach (var gp in pulpointList)
        {
            if (gp.Y < minY)
            {
                minY = gp.Y;
            }

            if (gp.Y > maxY)
            {
                maxY = gp.Y;
            }
        }

        // ensure ideal ranges are within chart
        if (139 > maxY)
        {
            maxY = 139;
        }

        if (85 < minY)
        {
            minY = 85;
        }

        // add some leeway below minimum and above maximum
        minY -= 0.8;
        maxY += 0.8;

        var valueSpanHorizontal = maxX - minX;
        var valueSpanVertical = maxY - minY;

        CanvasGraph.Children.Clear();
        double marginHorizontal = 12;
        double marginVertical = 12;

        if (CanvasGraph.ActualHeight == 0)
        {
            return; // TODO Is this why they dont plot on content rendered?
        }

        CanvasGraph.Width = _horizontalDateIncrement * valueSpanHorizontal + (2 * marginHorizontal);
        var physicalSpanHorizontal = _horizontalDateIncrement * valueSpanHorizontal;
        if (physicalSpanHorizontal < 100)
        {
            physicalSpanHorizontal = 100;
        }

        var physicalSpanVertical = CanvasGraph.ActualHeight - (2 * marginVertical);

        // test graph area
        var outliner = new Rectangle
        {
            Width = physicalSpanHorizontal, Height = physicalSpanVertical, Stroke = new SolidColorBrush(Colors.Gray)
            , Fill = new SolidColorBrush(Colors.WhiteSmoke)
        };
        Canvas.SetLeft(outliner, marginHorizontal);
        Canvas.SetTop(outliner, marginVertical);
        CanvasGraph.Children.Add(outliner);

        // Indicate ideal ranges 85-89 and 130-139
        var rct = new Rectangle
        {
            Stroke = Brushes.PaleGreen, Fill = Brushes.PaleGreen
            , Height = 4 * (physicalSpanVertical / valueSpanVertical), Width = physicalSpanHorizontal - 2
        };
        Canvas.SetTop(rct
            , marginVertical + physicalSpanVertical - ((89 - minY) * (physicalSpanVertical / valueSpanVertical)));
        Canvas.SetLeft(rct, marginHorizontal + 1);
        CanvasGraph.Children.Add(rct);

        rct = new Rectangle
        {
            Stroke = Brushes.PaleGreen, Fill = Brushes.PaleGreen
            , Height = 9 * (physicalSpanVertical / valueSpanVertical), Width = physicalSpanHorizontal - 2
        };
        Canvas.SetTop(rct
            , marginVertical + physicalSpanVertical - ((139 - minY) * (physicalSpanVertical / valueSpanVertical)));
        Canvas.SetLeft(rct, marginHorizontal + 1);
        CanvasGraph.Children.Add(rct);

        // Draw vertical lines and labels for each January 1st
        var year = earliestDate.Year;
        while (year < latestDate.Year)
        {
            year++;
            var newyearsday = new DateTime(year, 1, 1);
            var elapsed = newyearsday - earliestDate;
            var dd = elapsed.TotalDays;
            var ln = new Line
            {
                Stroke = new SolidColorBrush(Colors.Gray), StrokeThickness = 1
                , X1 = marginHorizontal + (dd - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
                , Y1 = marginVertical + physicalSpanVertical
            };
            ln.X2 = ln.X1;
            ln.Y2 = marginVertical;
            CanvasGraph.Children.Add(ln);

            var br = new Border
            {
                BorderThickness = new Thickness(1), BorderBrush = new SolidColorBrush(Colors.SaddleBrown)
                , CornerRadius = new CornerRadius(4), Background = new SolidColorBrush(Colors.SeaShell)
            };
            var tb = new TextBlock();
            Canvas.SetLeft(br, ln.X1 + 2);
            Canvas.SetTop(br, marginVertical + 2);
            tb.Background = new SolidColorBrush(Colors.SeaShell);
            tb.Foreground = new SolidColorBrush(Colors.SaddleBrown);
            tb.Margin = new Thickness(4);
            tb.Padding = new Thickness(4);
            tb.Text = year.ToString();
            br.Child = tb;
            CanvasGraph.Children.Add(br);
        }

        // // Plot diastolic line graph
        // double lastx = -11267;
        // double lasty = 0;
        // var begun = false;
        // foreach (var gp in diapointList)
        // {
        //     if (begun)
        //     {
        //         var ln = new Line
        //         {
        //             Stroke = new SolidColorBrush(Colors.Blue), StrokeThickness = 2
        //             , X1 = marginHorizontal + (lastx - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
        //             , Y1 = marginVertical + physicalSpanVertical -
        //                    ((lasty - minY) * (physicalSpanVertical / valueSpanVertical))
        //             , X2 = marginHorizontal + (gp.X - minX) * (physicalSpanHorizontal / valueSpanHorizontal)
        //             , Y2 = marginVertical + physicalSpanVertical -
        //                    ((gp.Y - minY) * (physicalSpanVertical / valueSpanVertical))
        //         };
        //         CanvasGraph.Children.Add(ln);
        //     }
        //
        //     begun = true;
        //     lastx = gp.X;
        //     lasty = gp.Y;
        // }

        // Plot systolic blob graph
        foreach (var gp in syspointList)
        {
            var a = new Ellipse()
            {
                Width = 7, Height = 7, Fill = new SolidColorBrush(Colors.DarkViolet)
                , Stroke = new SolidColorBrush(Colors.DarkBlue), StrokeThickness = 1
            };
            Canvas.SetLeft(a,marginHorizontal -3 + (gp.X - minX) * (physicalSpanHorizontal / valueSpanHorizontal));
            Canvas.SetTop(a, marginVertical -3  + physicalSpanVertical -
                             (gp.Y - minY) * (physicalSpanVertical / valueSpanVertical));
            CanvasGraph.Children.Add(a);
            
        }
        
        // Plot diastolic blob graph
        foreach (var gp in diapointList)
        {
            var a = new Ellipse()
            {
                Width = 7, Height = 7, Fill = new SolidColorBrush(Colors.Blue)
                , Stroke = new SolidColorBrush(Colors.DarkBlue), StrokeThickness = 1
            };
            Canvas.SetLeft(a,marginHorizontal -3 + (gp.X - minX) * (physicalSpanHorizontal / valueSpanHorizontal));
            Canvas.SetTop(a, marginVertical -3 + physicalSpanVertical -
                                             (gp.Y - minY) * (physicalSpanVertical / valueSpanVertical));
            CanvasGraph.Children.Add(a);
            
        }

        // Plot pulse rate as points
        foreach (var gp in pulpointList)
        {
            var pt = new Ellipse()
            {
                Width = 9, Height = 9, Fill = new SolidColorBrush(Colors.Lime)
                , Stroke = new SolidColorBrush(Colors.DarkGreen), StrokeThickness = 1
            };
            Canvas.SetLeft(pt, marginHorizontal - 4 + (gp.X - minX) * (physicalSpanHorizontal / valueSpanHorizontal));
            Canvas.SetTop(pt
                , marginVertical - 4 +
                  (physicalSpanVertical - ((gp.Y - minY) * (physicalSpanVertical / valueSpanVertical))));
            CanvasGraph.Children.Add(pt);
        }

        GraphScrollViewer.ScrollToRightEnd();
    }

    private void Window_ContentRendered(object sender, EventArgs e)
    {
        StatisticsBorder.Visibility = Visibility.Hidden;
        GraphScrollViewer.Width = OuterCanvas.ActualWidth;
        GraphScrollViewer.Height = OuterCanvas.ActualHeight;
    }
}