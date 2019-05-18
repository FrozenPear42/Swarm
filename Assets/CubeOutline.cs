using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeOutline : MonoBehaviour {
    public float width;
    public float height;
    public float depth;
    public Color lineColor;

    private Material lineMaterial;

    void Awake() {
        CreateMaterial();
    }

    void OnRenderObject() {
        RenderLines();
    }

    void OnDrawGizmos() {
        if (lineMaterial == null)
            CreateMaterial();
        RenderLines();
    }

    void CreateMaterial() {
        var shader = Shader.Find("Hidden/Internal-Colored");
        lineMaterial = new Material(shader);
        lineMaterial.hideFlags = HideFlags.HideAndDontSave;
        lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
        lineMaterial.SetInt("_ZWrite", 0);
    }

    void RenderLines() {
        lineMaterial.SetPass(0);

        GL.PushMatrix();
        GL.MultMatrix(transform.localToWorldMatrix);
        GL.Begin(GL.LINES);
        GL.Color(lineColor);
//    GL.Color(Color.red);
        var x = width / 2;
        var y = height / 2;
        var z = depth / 2;

        //bottom floor
        GL.Vertex(new Vector3(-x, -y, -z));
        GL.Vertex(new Vector3(+x, -y, -z));

        GL.Vertex(new Vector3(+x, -y, -z));
        GL.Vertex(new Vector3(+x, -y, +z));

        GL.Vertex(new Vector3(+x, -y, +z));
        GL.Vertex(new Vector3(-x, -y, +z));

        GL.Vertex(new Vector3(-x, -y, +z));
        GL.Vertex(new Vector3(-x, -y, -z));

        //top floor
        GL.Vertex(new Vector3(-x, +y, -z));
        GL.Vertex(new Vector3(+x, +y, -z));

        GL.Vertex(new Vector3(+x, +y, -z));
        GL.Vertex(new Vector3(+x, +y, +z));

        GL.Vertex(new Vector3(+x, +y, +z));
        GL.Vertex(new Vector3(-x, +y, +z));

        GL.Vertex(new Vector3(-x, +y, +z));
        GL.Vertex(new Vector3(-x, +y, -z));

        //vertical lines
        GL.Vertex(new Vector3(-x, -y, -z));
        GL.Vertex(new Vector3(-x, +y, -z));

        GL.Vertex(new Vector3(+x, -y, -z));
        GL.Vertex(new Vector3(+x, +y, -z));

        GL.Vertex(new Vector3(+x, -y, +z));
        GL.Vertex(new Vector3(+x, +y, +z));

        GL.Vertex(new Vector3(-x, -y, +z));
        GL.Vertex(new Vector3(-x, +y, +z));

        GL.End();
        GL.PopMatrix();
    }
}