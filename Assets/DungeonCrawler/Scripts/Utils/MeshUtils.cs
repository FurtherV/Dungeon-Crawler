using System;
using UnityEngine;

namespace DungeonCrawler.Scripts.Utils
{
    public static class MeshUtils
    {
        private static Quaternion GetQuaternionEuler(float rotFloat)
        {
            var rot = Mathf.RoundToInt(rotFloat);
            rot = rot % 360;
            if (rot < 0) rot += 360;
            return Quaternion.Euler(0, 0, rot);
        }

        public static Mesh CreateEmptyMesh()
        {
            var mesh = new Mesh
            {
                vertices = Array.Empty<Vector3>(),
                uv = Array.Empty<Vector2>(),
                triangles = Array.Empty<int>()
            };
            return mesh;
        }

        public static void CreateEmptyMeshArrays(int quadCount, out Vector3[] vertices, out Vector2[] uv,
            out int[] triangles)
        {
            vertices = new Vector3[4 * quadCount];
            uv = new Vector2[4 * quadCount];
            triangles = new int[6 * quadCount];
        }

        public static void AddToMeshArrays(Vector3[] vertices, Vector2[] uv, int[] triangles, int index, Vector3 pos,
            float rot, Vector3 baseSize, Vector2 uv00, Vector2 uv11)
        {
            //Relocate vertices
            var vIndex = index * 4;
            var vIndex0 = vIndex;
            var vIndex1 = vIndex + 1;
            var vIndex2 = vIndex + 2;
            var vIndex3 = vIndex + 3;

            baseSize *= .5f;

            var isSkewed = baseSize.x != baseSize.y;
            if (isSkewed)
            {
                vertices[vIndex0] = pos + (GetQuaternionEuler(rot) * new Vector3(-baseSize.x, baseSize.y));
                vertices[vIndex1] = pos + (GetQuaternionEuler(rot) * new Vector3(-baseSize.x, -baseSize.y));
                vertices[vIndex2] = pos + (GetQuaternionEuler(rot) * new Vector3(baseSize.x, -baseSize.y));
                vertices[vIndex3] = pos + (GetQuaternionEuler(rot) * baseSize);
            }
            else
            {
                vertices[vIndex0] = pos + (GetQuaternionEuler(rot - 270) * baseSize);
                vertices[vIndex1] = pos + (GetQuaternionEuler(rot - 180) * baseSize);
                vertices[vIndex2] = pos + (GetQuaternionEuler(rot - 90) * baseSize);
                vertices[vIndex3] = pos + (GetQuaternionEuler(rot - 0) * baseSize);
            }

            //Relocate UVs
            uv[vIndex0] = new Vector2(uv00.x, uv11.y);
            uv[vIndex1] = new Vector2(uv00.x, uv00.y);
            uv[vIndex2] = new Vector2(uv11.x, uv00.y);
            uv[vIndex3] = new Vector2(uv11.x, uv11.y);

            //Create triangles
            var tIndex = index * 6;

            triangles[tIndex + 0] = vIndex0;
            triangles[tIndex + 1] = vIndex3;
            triangles[tIndex + 2] = vIndex1;

            triangles[tIndex + 3] = vIndex1;
            triangles[tIndex + 4] = vIndex3;
            triangles[tIndex + 5] = vIndex2;
        }
    }
}