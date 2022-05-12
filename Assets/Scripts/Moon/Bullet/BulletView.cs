using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Moon.Bullet
{
    public class BulletView : IDisposable
    {
        private readonly Mesh _mesh;
        private readonly Material _mat;

        private GraphicsBuffer _argsBuffer;
        private GraphicsBuffer _bulletBuffer;

        private const int MaxLength = 1000000;

        private static readonly int BulletID = Shader.PropertyToID("_Bullets");
        private readonly List<BulletViewData> _bullets = new(MaxLength);


        private readonly uint[] _args = new uint[5];

        public BulletView(Material material)
        {
            _mat = material;
            _mesh = new Mesh
            {
                vertices = new[]
                {
                    Vector3.up,
                    Vector3.right,
                    Vector3.zero
                },
                triangles = new[] { 0, 1, 2 },
                uv = new[]
                {
                    Vector2.up,
                    Vector2.right,
                    Vector2.zero
                }
            };

            InitBuffer();
            InitArgs();
        }

        private void UpdateArgs(int length)
        {
            _args[1] = (uint)length;
            _argsBuffer.SetData(_args);
        }

        private void InitArgs()
        {
            _args[0] = _mesh.GetIndexCount(0);
            _argsBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.IndirectArguments,
                1,
                _args.Length * sizeof(uint));
        }

        private void InitBuffer()
        {
            _bulletBuffer =
                new GraphicsBuffer(GraphicsBuffer.Target.Structured, MaxLength, Marshal.SizeOf<BulletViewData>());
        }


        public void UpdateView(IEnumerable<BulletData> bulletDatas)
        {
            _bullets.Clear();

            _bullets.AddRange(bulletDatas.Where(value => value.IsActive).Select(value =>
            {
                var color = value.Color;

                var v = new BulletViewData(value.Position,
                    new Vector3(color.r, color.g, color.b) * 10,
                    0.1f);

                return v;
            }));

            if (!_bullets.Any())
                return;

            _bulletBuffer.SetData(_bullets);

            _mat.SetBuffer(BulletID, _bulletBuffer);


            UpdateArgs(_bullets.Count);

            Graphics.DrawMeshInstancedIndirect(
                _mesh,
                0,
                _mat,
                new Bounds(Vector3.zero, Vector3.zero),
                _argsBuffer);
        }


        public void Dispose()
        {
            _argsBuffer.Release();
            _bulletBuffer.Release();
        }

        private struct BulletViewData
        {
            private readonly Vector2 _position;
            private readonly Vector3 _color;
            private readonly float _scale;

            public BulletViewData(Vector2 position, Vector3 color, float scale)
            {
                _position = position;
                _color = color;
                _scale = scale;
            }
        }
    }
}