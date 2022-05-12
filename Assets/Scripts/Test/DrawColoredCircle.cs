using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Test
{
    public class DrawColoredCircle : MonoBehaviour
    {
        [SerializeField] private Material mat;

        private Mesh _mesh;

        private const int VerticesCount = 12;


        private GraphicsBuffer _argsBuffer;
        private GraphicsBuffer _positionBuffer;
        private GraphicsBuffer _colorBuffer;

        private const int Length = 1000000;

        private readonly Vector3[] _positions = new Vector3[Length];
        private static readonly int PositionID = Shader.PropertyToID("_Positions");

        private readonly Color[] _colors = new Color[Length];
        private static readonly int ColorID = Shader.PropertyToID("_Colors");

        private void Start()
        {
            _mesh = new Mesh();
            var v = new Vector3[VerticesCount];
            var t = new List<int>(VerticesCount * 3);
            for (int i = 0; i < VerticesCount; i++)
            {
                var theta = Mathf.PI * 2f / VerticesCount * i;
                v[i] = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta));
                for (int j = 0; j < 3; j++)
                {
                    t.Add(i);
                    t.Add(i != 0 ? i - 1 : VerticesCount - 1);
                    t.Add(0);
                }
            }

            _mesh.SetVertices(v);
            _mesh.SetTriangles(t, 0);


            for (var i = 0; i < Length; i++)
            {
                _positions[i] = Random.insideUnitCircle * 10;
                _colors[i] = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }

            InitArgs();
            InitColors();
            InitPositions();
        }

        private void Update()
        {
            Graphics.DrawMeshInstancedIndirect(
                _mesh,
                0,
                mat,
                new Bounds(Vector3.zero, Vector3.zero),
                _argsBuffer);
        }

        private void InitArgs()
        {
            var args = new uint[5];
            args[0] = _mesh.GetIndexCount(0);
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
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, Length, Marshal.SizeOf<Vector3>());
            _positionBuffer.SetData(_positions);

            mat.SetBuffer(PositionID, _positionBuffer);
        }

        private void InitColors()
        {
            _colorBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, Length, Marshal.SizeOf<Color>());
            _colorBuffer.SetData(_colors);

            mat.SetBuffer(ColorID, _colorBuffer);
        }
    }
}