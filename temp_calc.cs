using System;

class Program {
    static void Main() {
        double shieldRadius = 80.0;
        double realRadius = 510.0;
        int totalGenerators = 1; // First generator at pole
        
        int rCount = (int)Math.Ceiling((Math.PI * realRadius) / (2.0 * shieldRadius));
        Console.WriteLine($"Latitude rings (rCount): {rCount}");
        
        // Add one generator for each latitude ring
        totalGenerators += rCount;
        
        // Calculate generators along each latitude ring
        for (int i = 1; i <= rCount; i++) {
            double sita = (double)i * Math.PI / (double)rCount;
            double r2 = realRadius * Math.Sin(sita);
            int r2Count = (int)Math.Ceiling((Math.PI * r2 * 2.0) / (2.0 * shieldRadius));
            totalGenerators += r2Count - 1; // -1 because we already counted the main ring generator
            Console.WriteLine($"Ring {i}: Adding {r2Count-1} generators (radius={r2:F2})");
        }
        
        Console.WriteLine($"Total generators needed: {totalGenerators}");
    }
} 