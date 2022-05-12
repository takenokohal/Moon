using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Test
{
    public class Drawer : MonoBehaviour
    {
        [SerializeField] private Mesh mesh;
        [SerializeField] private Material mat;

        private GraphicsBuffer _argsBuffer;
        private GraphicsBuffer _positionBuffer;
        private GraphicsBuffer _colorBuffer;

        private const int Length = 100;

        private readonly Vector2[] _positions = new Vector2[Length];
        private static readonly int PositionID = Shader.PropertyToID("_Positions");

        private readonly Vector3[] _colors = new Vector3[Length];
        private static readonly int ColorID = Shader.PropertyToID("_Colors");

        private void Start()
        {
            mesh = new Mesh();
            mesh.SetVertices(new[]
            {
                Vector3.up,
                Vector3.right,
                Vector3.zero
            });
            mesh.SetTriangles(new[] { 0, 1, 2 }, 0);
            mesh.uv = new Vector2[]
            {
                Vector3.up,
                Vector3.right,
                Vector3.zero
            };
            for (var i = 0; i < Length; i++)
            {
                //   _positions[i] = Random.insideUnitCircle * 10;
                _positions[i] = new Vector2();
                _colors[i] = new Vector3(
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f),
                    Random.Range(0f, 1f));
            }


            InitArgs();
            InitPositions();
            InitColors();
        }

        // Update is called once per frame
        void Update()
        {
            Graphics.DrawMeshInstancedIndirect(
                mesh,
                0,
                mat,
                new Bounds(Vector3.zero, Vector3.zero),
                _argsBuffer);
        }

        private void InitArgs()
        {
            var args = new uint[5];
            args[0] = mesh.GetIndexCount(0);
            args[1] = (uint)_positions.Length;
            args[2] = 0;
            args[3] = 0;
            args[4] = 0;

            _argsBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.IndirectArguments,
                1,
                args.Length * sizeof(uint));

            _argsBuffer.SetData(args);
        }

        private void InitPositions()
        {
            _positionBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, Length, Marshal.SizeOf<Vector2>());
            _positionBuffer.SetData(_positions);

            mat.SetBuffer(PositionID, _positionBuffer);
        }

        private void InitColors()
        {
            _colorBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, Length, Marshal.SizeOf<Vector3>());
            _colorBuffer.SetData(_colors);

            mat.SetBuffer(ColorID, _colorBuffer);
        }
    }
}