using ScottPlot;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Lab2DPF
{
    internal class Plotter
    {
        public WpfPlot PlotWindow { get; init; }
        public double Step { get; set; }

        public List<double> XValues { get; set; } = new List<double>();

        public Plotter(in WpfPlot PlotWindow)
        {
            this.PlotWindow = PlotWindow;
            PlotWindow.Plot.Add.HorizontalLine(0, color: Color.FromSDColor(System.Drawing.Color.Black));
            PlotWindow.Plot.Add.VerticalLine(0, color: Color.FromSDColor(System.Drawing.Color.Black));
        }

        public void PlotGivenDiscrete(double I1, double I2)
        {
            Step = Math.Abs(I2 - I1) / (CalcUnit.GivenValues.Count()-1);

            List<double> dataX = new List<double>();
            List<double> dataY = new List<double>();

            int n = 0;

            for (double i = I1; i <= I2 && n < CalcUnit.GivenValues.Count(); i += Step)
            {
                XValues.Add(i);
                dataX.Add(i);
                dataY.Add(CalcUnit.GivenValues[n]);
                n++;
            }

            var scatter = PlotWindow.Plot.Add.ScatterPoints(dataX.ToArray(), dataY.ToArray());
            PlotWindow.Refresh();
        }

        public void PlotSpline(double i1, double i2, string selectedMethod)
        {
            if(string.IsNullOrEmpty(selectedMethod)) return;
            double splineDrawStep = 0.01;

            List<double> dataX = new List<double>();
            List<double> dataY = new List<double>();

            if (selectedMethod == "2 Deriv.")
            {
                var spline = new CubicSplineSecDerivative();
                spline.Build(i1, i2);

                for (double x = i1 - 5 * Math.PI; x <= i2 + 5 * Math.PI; x += splineDrawStep)
                {
                    double interpolatedY = spline.Interpolate(x);
                    dataX.Add(x);
                    dataY.Add(interpolatedY);
                }
            }
            else
            {
                var spline = new CubicSplineMatrix(CalcUnit.GivenValues.ToArray(), i1, i2);
                for (double x = i1 - 5 * Math.PI; x <= i2 + 5 * Math.PI; x += splineDrawStep)
                {
                    double interpolatedY = spline.GetY(x);
                    dataX.Add(x);
                    dataY.Add(interpolatedY);
                }
            }

            var scatter = PlotWindow.Plot.Add.ScatterPoints(dataX.ToArray(), dataY.ToArray());
            PlotWindow.Refresh();
        }


        public void ClearPlot()
        {
            PlotWindow.Plot.Clear();

            PlotWindow.Plot.Add.HorizontalLine(0, color: Color.FromSDColor(System.Drawing.Color.Black));
            PlotWindow.Plot.Add.VerticalLine(0, color: Color.FromSDColor(System.Drawing.Color.Black));

            PlotWindow.Refresh();
        }
    }
}