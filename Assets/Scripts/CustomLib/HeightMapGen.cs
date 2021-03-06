using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Custom
{

    /// <summary>
    /// A height map generator that simulates a designer following the planning policies.
    /// This is for testing the capabilities of the system when using ML Agents. 
    /// In future, this would be generated by another agent.
    /// </summary>
    public class HeightMapGen
    {
        private Plot3D plot;

        private int width;
        private int depth;
        private float scale;

        private int minHeight;
        private int maxHeight;
        private int attractorsCount = 2;

        public int[,] heightMap;
        private List<Tuple<int, int, int>> attractors;

        /// <summary>
        /// Takes inputs to generate a heighmap that will dictate the skyline of the aggregation.
        /// </summary>
        /// <param name="plot">The plot where the heightmap is operating.</param>
        /// <param name="minHeight">The minimim height expected.</param>
        /// <param name="maxHeight">The minimim height expected.</param>
        /// <param name="attractorsCount">The number of attractors, these are created randomly. </param>
        public HeightMapGen(Plot3D plot, int minHeight, int maxHeight, int attractorsCount)
        {
            this.plot = plot;
            width = plot.width;
            depth = plot.depth;
            this.maxHeight = maxHeight < plot.height - 2 ? maxHeight : plot.height - 2;

            this.minHeight = minHeight;
            scale = plot.scale;
            heightMap = new int[width, depth];
            this.attractorsCount = attractorsCount;
            attractors = new List<Tuple<int, int, int>>();

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    heightMap[x, z] = minHeight;
                }
            }

            for (int i = 0; i < attractorsCount; i++)
            {
                AddAttractor(UnityEngine.Random.Range(0, width), UnityEngine.Random.Range(0, depth), UnityEngine.Random.Range(minHeight, maxHeight));
            }

            GenerateMap();
        }

        /// <summary>
        /// Adds one attractor to the list of attractors.
        /// </summary>
        /// <param name="attractorX">The x value to position the attractor.</param>
        /// <param name="attractorZ">The z value to position the attractor.</param>
        /// <param name="heightValue">Relative strength of the attractor.</param>
        public void AddAttractor(int attractorX, int attractorZ, int heightValue)
        {
            attractors.Add(new Tuple<int, int, int>(attractorX, attractorZ, heightValue));
        }

        /// <summary>
        /// Adds one attractor to the list of attractors.
        /// </summary>
        /// <param name="attractor">A coordinate 2D to position the attractor.</param>
        /// <param name="heightValue">Relative strength of the attractor.</param>
        public void AddAttractor(Coordinate2D attractor, int heightValue)
        {
            attractors.Add(new Tuple<int, int, int>(attractor.X, attractor.Z, heightValue));
        }

        /// <summary>
        /// Takes the attractors and generates a heightmap based on their relative strength.
        /// It provides a remapping to min and max values input by the user.
        /// </summary>

        public void GenerateMap()
        {

            float[,] distances = new float[width, depth];
            float minDist = float.MaxValue;
            float maxDist = float.MinValue;


            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    float value = 0;
                    for (int i = 0; i < attractors.Count; i++)
                    {
                        float valueM = (float)(1 / (0.5 + (depth + width) + Mathf.Pow(CalculateManhattanDistance(x, attractors[i].Item1, z, attractors[i].Item2), 2)));
                        float valueE = (float)(1 / (0.5 + (depth + width) + Mathf.Pow(CalculateEuclideanDistance(x, attractors[i].Item1, z, attractors[i].Item2), 2)));
                        value += (attractors[i].Item3 * (valueM + valueE));
                    }
                    if (value < minDist) minDist = value;
                    if (value > maxDist) maxDist = value;
                    distances[x, z] = value;
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int z = 0; z < depth; z++)
                {
                    heightMap[x, z] = Mathf.RoundToInt(RemapFloats(distances[x, z], minDist, maxDist, minHeight, maxHeight));
                }
            }
        }


        /// <summary>
        /// Takes inputs to generate a heighmap with 2 attractors that will dictate the skyline of the aggregation.
        /// </summary>
        /// <param name="plot">The plot where the heightmap is operating.</param>
        /// <param name="minHeight">The minimim height expected.</param>
        /// <param name="maxHeight">The minimim height expected.</param>
        /// <param name="attractorsCount">The number of attractors, these are created randomly. </param>
        /// <returns>A new heightmap</returns>

        public static int[,] GenerateFixedMap(Plot3D plot, int minHeight, int maxHeight)
        {
            int[,] heightMap = new int[plot.width, plot.depth];
            float scale = plot.scale;
            List<Tuple<int, int, int>> attractors = new List<Tuple<int, int, int>>();

            for (int x = 0; x < plot.width; x++)
            {
                for (int z = 0; z < plot.depth; z++)
                {
                    heightMap[x, z] = minHeight;
                }
            }

            //hard coded attractors
            attractors.Add(new Tuple<int, int, int>(1, plot.depth - 4, 5));
            attractors.Add(new Tuple<int, int, int>(plot.width - 2, 3, 7));

            //generate map function
            float[,] distances = new float[plot.width, plot.depth];

            float minDist = float.MaxValue;
            float maxDist = float.MinValue;


            for (int x = 0; x < plot.width; x++)
            {
                for (int z = 0; z < plot.depth; z++)
                {
                    float value = 0;
                    for (int i = 0; i < attractors.Count; i++)
                    {
                        float valueM = (float)(1 / (0.5 + (plot.depth + plot.width) + Mathf.Pow(CalculateManhattanDistance(x, attractors[i].Item1, z, attractors[i].Item2), 2)));
                        float valueE = (float)(1 / (0.5 + (plot.depth + plot.width) + Mathf.Pow(CalculateEuclideanDistance(x, attractors[i].Item1, z, attractors[i].Item2), 2)));
                        value += (attractors[i].Item3 * (valueM + valueE));
                    }
                    if (value < minDist) minDist = value;
                    if (value > maxDist) maxDist = value;
                    distances[x, z] = value;
                }
            }

            for (int x = 0; x < plot.width; x++)
            {
                for (int z = 0; z < plot.depth; z++)
                {
                    heightMap[x, z] = Mathf.RoundToInt(RemapFloats(distances[x, z], minDist, maxDist, minHeight, maxHeight));
                }
            }
            return heightMap;
        }

        /// <summary>
        /// Calculates the Manhattan distance between the two points.
        /// From https://codereview.stackexchange.com/questions/120933/calculating-distance-with-euclidean-manhattan-and-chebyshev-in-c
        /// </summary>
        /// <param name="x1">The first x coordinate.</param>
        /// <param name="x2">The second x coordinate.</param>
        /// <param name="y1">The first y coordinate.</param>
        /// <param name="y2">The second y coordinate.</param>
        /// <returns>The Manhattan distance between (x1, y1) and (x2, y2)</returns>
        private static int CalculateManhattanDistance(int x1, int x2, int y1, int y2)
        {
            return Mathf.Abs(x1 - x2) + Mathf.Abs(y1 - y2);
        }

        /// <summary>
        /// Calculates linear distance between two points.
        /// </summary>
        /// <param name="x1">The first x coordinate.</param>
        /// <param name="x2">The second x coordinate.</param>
        /// <param name="y1">The first y coordinate.</param>
        /// <param name="y2">The second y coordinate.</param>
        /// <returns>The Euclidean distance between (x1, y1) and (x2, y2)</returns>
        private static int CalculateEuclideanDistance(int x1, int x2, int y1, int y2)
        {
            return (int)Mathf.Sqrt(Mathf.Pow(x1 - x2, 2) + Mathf.Pow(y1 - y2, 2));
        }

        /// <summary>
        /// The methods provides remapping for a value.
        /// </summary>
        /// <param name="value">The value to be remapped.</param>
        /// <param name="oldMin">The origin lower bound.</param>
        /// <param name="oldMax">The origin upper bound.</param>
        /// <param name="newLow">The target lower bound.</param>
        /// <param name="newMax">The target upper bound.</param>
        /// <returns>The remapped float value given the origin and target bounds.</returns>
        private static float RemapFloats(float value, float oldMin, float oldMax, float newLow, float newMax)
        {
            return newLow + (value - oldMin) * (newMax - newLow) / (oldMax - oldMin);
        }

        /// <summary>
        /// The methods provides remapping for a value.
        /// </summary>
        /// <param name="value">The value to be remapped.</param>
        /// <param name="oldMin">The origin lower bound.</param>
        /// <param name="oldMax">The origin upper bound.</param>
        /// <param name="newLow">The target lower bound.</param>
        /// <param name="newMax">The target upper bound.</param>
        /// <returns>The remapped integer value given the origin and target bounds.</returns>
        private static float RemapInt(int value, int oldMin, int oldMax, int newLow, int newMax)
        {
            return Mathf.FloorToInt(newLow + (value - oldMin) * (newMax - newLow) / (float)(oldMax - oldMin));
        }
    }

}
