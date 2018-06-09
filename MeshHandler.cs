using System.Collections.Generic;
using UnityEngine;

namespace LabelsOnFloor
{
    public class MeshHandler
    {
        private readonly Dictionary<string, Mesh> _cachedMeshes = new Dictionary<string, Mesh>();

        private readonly FontHandler _fontHandler;


        public MeshHandler(FontHandler fontHandler)
        {
            _fontHandler = fontHandler;
        }

        public Mesh GetMeshFor(string label)
        {
            if (string.IsNullOrEmpty(label))
                return null;

            if (!_fontHandler.IsFontLoaded())
                return null;

            if (!_cachedMeshes.ContainsKey(label))
            {
                _cachedMeshes[label] = CreateMeshFor(label);
            }

            return _cachedMeshes[label];
        }

        private Mesh CreateMeshFor(string label)
        {
            var vertices = new List<Vector3>();
            var uvMap = new List<Vector2>();
            var triangles = new List<int>();
            var size = new Vector2
            {
                x = 1f,
                y = 2f
            };

            var boundsInTexture = _fontHandler.GetBoundsInTextureFor(label);
            var startingTriangleVertex = 0;
            var startingVertexXOffset = 0f;
            var yTop = size.y - 0.4f;
            foreach (var charBoundsInTexture in boundsInTexture)
            {
                vertices.Add(new Vector3(startingVertexXOffset, 0f, -0.4f));
                vertices.Add(new Vector3(startingVertexXOffset, 0f, yTop));
                vertices.Add(new Vector3(startingVertexXOffset + size.x, 0f, yTop));
                vertices.Add(new Vector3(startingVertexXOffset + size.x, 0f, -0.4f));
                startingVertexXOffset += size.x;

                uvMap.Add(new Vector2(charBoundsInTexture.Left, 0f));
                uvMap.Add(new Vector2(charBoundsInTexture.Left, 1f));
                uvMap.Add(new Vector2(charBoundsInTexture.Right, 1f));
                uvMap.Add(new Vector2(charBoundsInTexture.Right, 0f));

                triangles.Add(startingTriangleVertex + 0);
                triangles.Add(startingTriangleVertex + 1);
                triangles.Add(startingTriangleVertex + 2);
                triangles.Add(startingTriangleVertex + 0);
                triangles.Add(startingTriangleVertex + 2);
                triangles.Add(startingTriangleVertex + 3);
                startingTriangleVertex += 4;
            }

            var mesh = new Mesh
            {
                name = "NewPlaneMesh()",
                vertices = vertices.ToArray(),
                uv = uvMap.ToArray()
            };
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
    }
}