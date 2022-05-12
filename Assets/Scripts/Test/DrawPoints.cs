using UnityEngine;

namespace Test
{
    public class DrawPoints : MonoBehaviour
    {
        void Start()
        {
            int numPoints = 10000000; // 点の個数
            float r = 10.0f; // 半径

            Mesh meshSurface = CreateSimpleSurfacePointMesh(numPoints, r);
            GetComponent<MeshFilter>().mesh = meshSurface;
        }

        /// <summary>
        /// 球の表面にランダムに点を生成
        /// </summary>
        /// <param name="numPoints">点の数</param>
        /// <param name="radius">球の半径</param>
        /// <returns></returns>
        Mesh CreateSimpleSurfacePointMesh(int numPoints, float radius)
        {
            Vector3[] points = new Vector3[numPoints];
            int[] indecies = new int[numPoints];
            Color[] colors = new Color[numPoints];

            for (int i = 0; i < numPoints; i++)
            {
                float z = Random.Range(-1.0f, 1.0f);
                float th = Mathf.Deg2Rad * Random.Range(0.0f, 360.0f);
                float x = Mathf.Sqrt(1.0f - z * z) * Mathf.Cos(th);
                float y = Mathf.Sqrt(1.0f - z * z) * Mathf.Sin(th);

                points[i] = new Vector3(x, y, z) * radius; // 頂点座標
                indecies[i] = i; // 配列番号をそのままインデックス番号に流用
                colors[i] = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f),
                    Random.Range(0.0f, 1.0f)) * 5f; // 追加
            }

            Mesh mesh = new Mesh();
            mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // 追加
            mesh.vertices = points;
            mesh.SetIndices(indecies, MeshTopology.Points, 0);
            mesh.colors = colors;

            return mesh;
        }
    }
}