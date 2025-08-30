using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

class SecretSharingSolver
{
    // Pair class for (x, y)
    class Pair
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Pair(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // Lagrange interpolation at x=0
    static double LagrangeInterpolation(int x, List<Pair> shares)
    {
        double total = 0.0;

        for (int i = 0; i < shares.Count; i++)
        {
            double xi = shares[i].X;
            double yi = shares[i].Y;
            double li = 1.0;

            for (int j = 0; j < shares.Count; j++)
            {
                if (i != j)
                {
                    double xj = shares[j].X;
                    li *= (x - xj) / (xi - xj);
                }
            }

            total += yi * li;
        }

        return Math.Round(total);
    }

    static void Main()
    {
        try
        {
            // ✅ Load JSON from Desktop
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string filePath = Path.Combine(desktop, "shares.json");

            string jsonText = File.ReadAllText(filePath);
            JObject root = JObject.Parse(jsonText);

            int k = (int)root["keys"]["k"];

            // ✅ Extract shares
            var shares = new List<Pair>();
            foreach (var prop in root.Properties())
            {
                if (prop.Name != "keys")
                {
                    int x = int.Parse(prop.Name);
                    int b = (int)prop.Value["base"];
                    string valueStr = (string)prop.Value["value"];

                    // Convert from base
                    int y = Convert.ToInt32(valueStr, b);
                    shares.Add(new Pair(x, y));
                }
            }

            // ✅ Use first k shares
            var subShares = shares.GetRange(0, k);

            // ✅ Compute secret
            double secret = LagrangeInterpolation(0, subShares);
            Console.WriteLine($"✅ Secret (constant term) = {(int)secret}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("❌ Error: " + ex.Message);
        }
    }
}
