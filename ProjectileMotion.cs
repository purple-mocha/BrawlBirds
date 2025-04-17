using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace birds
{
    internal class ProjectileMotion
    {
        private double g = 9.81;

        private List<double> X = new List<double>();
        private List<double> Y = new List<double>();
        List<double> Velocity_X = new List<double>();
        List<double> Velocity_Y = new List<double>();

        public double x, y, scorost, ugol, k, m; 
        static public double timeStep = 0.01;

        public List<double> getXes()
        {
            return X;
        }
        public List<double> getYes()
        {
            return Y;
        }

        public ProjectileMotion(double x = 0, double y = 0, double scorost = 0, double ugol = 0, double k = 0, double m = 0)
        {
            this.x = x;
            this.y = y;
            this.scorost = scorost;
            this.ugol = ugol * (Math.PI / 180);
            this.k = k;
        }
        public void ReadInputData(string inputFile)
        {
            string[] lines = File.ReadAllLines(inputFile);
            scorost = double.Parse(lines[0]);
            ugol = double.Parse(lines[1]) * (Math.PI / 180);
            m = double.Parse(lines[2]);
            k = double.Parse(lines[3]);
        }
        public delegate void Message(string massage, StreamWriter file);
        public event Message sms;





        public void CalculateTrajectory(string outputfile)
        {

            using (StreamWriter file = new StreamWriter(outputfile))
            {
                int i = 0;
                double time = 0;
                Velocity_X.Add((k * scorost * Math.Cos(ugol)) / m);
                Velocity_Y.Add(scorost * Math.Sin(ugol));
                X.Add(i);
                Y.Add(i);
                file.WriteLine($"t: {time:F2} s, x: {X[i]:F2} m, y: {Y[i]:F2} m");


                while (true)
                {
                    time += timeStep;
                    
                    Velocity_X.Add(Velocity_X[i] - timeStep * (k * Velocity_X[i]) / m);
                    Velocity_Y.Add(Velocity_Y[i] - timeStep * (g + k * Velocity_X[i] / m));

                    X.Add(X[i] + timeStep * Velocity_X[i]);
                    Y.Add(Y[i] + timeStep * Velocity_Y[i]);
                    i++;

                    file.WriteLine($"t: {time:F2} s, x: {X[i]:F2} m, y: {Y[i]:F2} m");
                    if (Y[i] <= 0 && time != 0)
                    {
                        sms += upalo;
                        Hmax(file);
                        break;
                    }
                }
            }
        }
        public void Hmax(StreamWriter file)
        {
            double hMax = Y.Max();
            sms.Invoke($"Максимальная высота полёта: {hMax:F2} м", file);
        }

        public void upalo(string message, StreamWriter file)
        {        
                file.WriteLine(message);
        }

    }
}
