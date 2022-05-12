using System.Runtime.InteropServices;
using UnityEngine;

namespace Test
{
    public class DrawCircles : MonoBehaviour
    {
        // [SerializeField] private Mesh mesh;
        //     [SerializeField] private Material mat;

        [SerializeField] private MeshRenderer meshRenderer;

        private GraphicsBuffer _positionBuffer;
        private GraphicsBuffer _colorBuffer;

        private const int Length = 100000;

        private readonly Vector3[] _positions = new Vector3[Length];
        private static readonly int PositionID = Shader.PropertyToID("_Positions");

        private readonly Color[] _colors = new Color[Length];
        private static readonly int ColorID = Shader.PropertyToID("_Colors");

        private void Start()
        {
            /*
                        mesh = new Mesh();
                        mesh.SetVertices(new[]
                        {
                            Vector3.up,
                            Vector3.right,
                            Vector3.left
                        });
                        mesh.SetTriangles(new[] { 0, 1, 2 }, 0);
                        mesh.uv = new Vector2[]
                        {
                            Vector3.up,
                            Vector3.right,
                            Vector3.left
                        };*/
            for (var i = 0; i < Length; i++)
            {
                _positions[i] = Random.insideUnitCircle * 10;
                _colors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }


            InitPositions();
            InitColors();
        }


        private void InitPositions()
        {
            _positionBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, Length, Marshal.SizeOf<Vector3>());
            _positionBuffer.SetData(_positions);

            meshRenderer.material.SetBuffer(PositionID, _positionBuffer);
        }

        private void InitColors()
        {
            _colorBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, Length, Marshal.SizeOf<Color>());
            _colorBuffer.SetData(_colors);

            meshRenderer.material.SetBuffer(ColorID, _colorBuffer);
        }
    }
}