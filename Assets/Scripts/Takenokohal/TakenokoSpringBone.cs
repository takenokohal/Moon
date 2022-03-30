using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Jobs;
using VRM;

namespace Takenokohal
{
    public sealed class TakenokoSpringBone : MonoBehaviour
    {
        [SerializeField] [Range(0, 4)] [Header("Settings")]
        public float m_stiffnessForce = 1.0f;

        [SerializeField] [Range(0, 2)] public float m_gravityPower;

        [SerializeField] public Vector3 m_gravityDir = new Vector3(0, -1.0f, 0);

        [SerializeField] [Range(0, 1)] public float m_dragForce = 0.4f;

        [SerializeField] public List<Transform> RootBones = new();
        Dictionary<Transform, Quaternion> m_initialLocalRotationMap;

        private readonly List<Transform> _originTransforms = new();
        private TransformAccessArray _transformAccessArray;

        private List<Parameter> _tempParameters = new();
        private NativeArray<Parameter> _parameters;

        private struct Parameter
        {
            public readonly Vector3 BoneAxis;
            public Vector3 CurrentTail;
            public Vector3 PrevTail;
            public readonly float Length;
            public readonly Quaternion LocalRotation;
            public Quaternion ParentRotation;

            public Parameter(Transform transform, Vector3 localChildPosition, Quaternion parentRotation)
            {
                var worldChildPosition = transform.TransformPoint(localChildPosition);
                CurrentTail = worldChildPosition;
                PrevTail = CurrentTail;
                LocalRotation = transform.localRotation;
                BoneAxis = localChildPosition.normalized;
                Length = localChildPosition.magnitude;
                ParentRotation = parentRotation;
            }
        }


        private void Start()
        {
            Setup();
        }

        private void OnDisable()
        {
            _parameters.Dispose();
            _transformAccessArray.Dispose();
        }

        private void Setup(bool force = false)
        {
            if (RootBones != null)
            {
                if (force || m_initialLocalRotationMap == null)
                {
                    m_initialLocalRotationMap = new Dictionary<Transform, Quaternion>();
                }
                else
                {
                    foreach (var kv in m_initialLocalRotationMap)
                        kv.Key.localRotation = kv.Value;

                    m_initialLocalRotationMap.Clear();
                }


                foreach (var go in RootBones)
                {
                    if (go != null)
                    {
                        foreach (var x in go.transform.GetComponentsInChildren<Transform>(true))
                            m_initialLocalRotationMap[x] = x.localRotation;

                        SetupRecursive(go);
                    }
                }

                _parameters = new NativeArray<Parameter>(_tempParameters.ToArray(), Allocator.Persistent);
                _tempParameters = null;

                _transformAccessArray = new TransformAccessArray(_originTransforms.ToArray());
            }
        }


        private static IEnumerable<Transform> GetChildren(Transform parent)
        {
            for (var i = 0; i < parent.childCount; ++i) yield return parent.GetChild(i);
        }

        private void SetupRecursive(Transform parent)
        {
            Vector3 localPosition = default;
            Vector3 scale = default;
            if (parent.childCount == 0)
            {
                // 子ノードが無い。7cm 固定
                var delta = parent.position - parent.parent.position;
                var childPosition = parent.position + delta.normalized * 0.07f * parent.UniformedLossyScale();
                localPosition = parent.worldToLocalMatrix.MultiplyPoint(childPosition); // cancel scale
                scale = parent.lossyScale;
            }
            else
            {
                var firstChild = GetChildren(parent).First();
                localPosition = firstChild.localPosition;
                scale = firstChild.lossyScale;
            }

            _tempParameters.Add(new Parameter(parent, new Vector3(
                localPosition.x * scale.x,
                localPosition.y * scale.y,
                localPosition.z * scale.z), parent.parent.rotation));

            _originTransforms.Add(parent);


            foreach (Transform child in parent)
                SetupRecursive(child);
        }

        private void Update()
        {
            UpdateProcess(Time.deltaTime);
        }


        private void UpdateProcess(float deltaTime)
        {
            var stiffness = m_stiffnessForce * deltaTime;
            var external = m_gravityDir * (m_gravityPower * deltaTime);

            for (var i = 0; i < _originTransforms.Count; i++)
            {
                var v = _parameters[i];
                var parent = _originTransforms[i].parent;
                v.ParentRotation = parent != null ? parent.rotation : Quaternion.identity;
                _parameters[i] = v;
            }

            var job = new UpdateJob
            {
                CurrentParameters = _parameters,
                External = external,
                DragForce = m_dragForce,
                StiffnessForce = stiffness
            };
            var handle = job.Schedule(_transformAccessArray);
            handle.Complete();
        }

        private struct UpdateJob : IJobParallelForTransform
        {
            [ReadOnly] public float DragForce;
            [ReadOnly] public float StiffnessForce;
            [ReadOnly] public Vector3 External;

            public NativeArray<Parameter> CurrentParameters;

            public void Execute(int index, TransformAccess transform)
            {
                var currentParameter = CurrentParameters[index];

                Debug.Log(currentParameter.BoneAxis);

                // verlet積分で次の位置を計算
                var nextTail = currentParameter.CurrentTail
                               + (currentParameter.CurrentTail - currentParameter.PrevTail) *
                               (1.0f - DragForce) // 前フレームの移動を継続する(減衰もあるよ)
                               + currentParameter.ParentRotation * transform.localRotation * currentParameter.BoneAxis *
                               StiffnessForce // 親の回転による子ボーンの移動目標
                               + External; // 外力による移動量

                // 長さをboneLengthに強制
                var position = transform.position;
                nextTail = position + (nextTail - position).normalized * currentParameter.Length;


                currentParameter.PrevTail = currentParameter.CurrentTail;
                currentParameter.CurrentTail = nextTail;

                CurrentParameters[index] = currentParameter;


                //回転を適用

                var rotation = currentParameter.ParentRotation * currentParameter.LocalRotation;
                var to = Quaternion.FromToRotation(rotation * currentParameter.BoneAxis,
                    nextTail - transform.position) * rotation;

                transform.rotation = to;
            }
        }
    }
}