using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTkIntegrationTest
{
    public static class ObjLoader
    {
        public static void Load(string path, out float[] vertices, out uint[] indices)
        {
            var tempPositions = new List<Vector3>();
            var tempUVs = new List<Vector2>();

            var combinedVertices = new List<float>();

            var vertexMap = new Dictionary<(int posIdx, int uvIdx), uint>();

            var tempIndices = new List<uint>();

            foreach (string line in File.ReadLines(path))
            {
                if (line.StartsWith("v "))
                {
                    var parts = line.Split(' ');
                    float.TryParse(parts[1], CultureInfo.InvariantCulture, out float x);
                    float.TryParse(parts[2], CultureInfo.InvariantCulture, out float y);
                    float.TryParse(parts[3], CultureInfo.InvariantCulture, out float z);

                    tempPositions.Add(new Vector3(x, y, z));

                }
                else if (line.StartsWith("vt "))
                {
                    var parts = line.Split(' ');
                    float.TryParse(parts[1], CultureInfo.InvariantCulture, out float u);
                    float.TryParse(parts[2], CultureInfo.InvariantCulture, out float v);
                    tempUVs.Add(new Vector2(u, v));
                }
                else if (line.StartsWith("f "))
                {
                    var parts = line.Split(' ');
                    for (int i = 1; i <= 3; i++)
                    {
                        var vertexData = parts[i].Split('/');
                        int.TryParse(vertexData[0], CultureInfo.InvariantCulture, out int posIdx);
                        if (posIdx != 0) posIdx--;
                        int.TryParse(vertexData[1], CultureInfo.InvariantCulture, out int uvIdx);
                        if (uvIdx != 0) uvIdx--;

                        if (!vertexMap.TryGetValue((posIdx, uvIdx), out uint combinedIndex))
                        {

                            combinedVertices.Add(tempPositions[posIdx].X);
                            combinedVertices.Add(tempPositions[posIdx].Y);
                            combinedVertices.Add(tempPositions[posIdx].Z);
                            combinedVertices.Add(tempUVs[uvIdx].X);
                            combinedVertices.Add(tempUVs[uvIdx].Y);

                            combinedIndex = (uint)(combinedVertices.Count / 5 - 1); // 5 floats for 1 vertex
                            vertexMap[(posIdx, uvIdx)] = combinedIndex;
                        }

                        tempIndices.Add(combinedIndex);
                    }
                }
            }

            vertices = combinedVertices.ToArray();
            indices = tempIndices.ToArray();

        }
    }
}
