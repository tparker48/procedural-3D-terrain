using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public class Cube{
        int[] verts;
        int index;

        public Cube(int[] points){
            int val = 1;
            index = 0;

            for(int i = 0; i < 8; i++){
                if(points[i] == 1){
                    index += val;
                }
                val *=2;
            }
        }

        public int[] getTriangles(){
            return TriTable.getTriangles(index);
        }
    }

    public int dim;
    public int height;
    public int N;
    public int P;
    public int overpopLimit, starveLimit, birthRangeLower, birthRangeUpper;
    
    Mesh mesh;
    Vector3[] vertices;
    Vector3[] defaultVertices;
    int[] triangles;
    int nextVertexIndex, nextTriIndex;

    int[,,] data;

    void Start(){
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;

        InitMeshVertices();
    }

    void Update(){
        if (Input.GetKeyUp("space"))
        {
            GenerateMesh();
        }
    }

    void GenerateMesh()
    {
        int[] CArules = {overpopLimit, starveLimit, birthRangeLower, birthRangeUpper};

        triangles = new int[dim * dim * height * 15];
        vertices = new Vector3[12*dim*dim*height];

        data = GridBasic.Generate(dim,height, CArules, N, P);
        CreateMeshFromData(data, dim);
        UpdateMesh();
    }


    void CreateMeshFromData(int[,,] verts, int dimension){
        int[] cubeVerts;
        int[] tempTriangles;

        nextVertexIndex = 0;
        nextTriIndex = 0;

        for(int x = 0; x < dimension-1; x++){
            for(int y = 0; y < dimension-1; y++){
                for(int z = 0; z < height-1; z++){
                    cubeVerts = createVerts(verts, x, y, z, dimension);
                    Cube cube = new Cube(cubeVerts);
                    tempTriangles =  cube.getTriangles();
                    UpdateMeshData(tempTriangles, x, y, z);
                }
            }
        }
    }

    int[] createVerts(int[,,] verts, int x, int y, int z, int dimension){
        int[] cubeVerts = new int[8];

        int x1, y1, z1;
        x1 = Utilities.IndexBound(x+1, dimension);
        y1 = Utilities.IndexBound(y+1, dimension);
        z1 = Utilities.IndexBound(z+1, height);

        cubeVerts[0] = verts[ x , y , z  ];
        cubeVerts[1] = verts[ x1, y , z  ];
        cubeVerts[2] = verts[ x1, y, z1  ];
        cubeVerts[3] = verts[ x , y,  z1  ];
        cubeVerts[4] = verts[ x , y1 , z ];
        cubeVerts[5] = verts[ x1, y1 , z ];
        cubeVerts[6] = verts[ x1, y1, z1 ];
        cubeVerts[7] = verts[ x , y1, z1 ];

        return cubeVerts;
    }

    void UpdateMeshData(int[] addTriangles, int x, int y, int z){
        // append triangles
        Vector3 v1, v2,v3;
        Vector3 coords = new Vector3(x,y,z);
        for(int i = 0; i < 15; i+=3){
            if(addTriangles[i] != -1){
                v1 = defaultVertices[ addTriangles[i]   ];
                v2 = defaultVertices[ addTriangles[i+1] ];
                v3 = defaultVertices[ addTriangles[i+2] ];

                v1 += coords;
                v2 += coords;
                v3 += coords;

                vertices[nextVertexIndex++] = v1;
                triangles[nextTriIndex++] = nextVertexIndex-1;

                vertices[nextVertexIndex++] = v2;
                triangles[nextTriIndex++] = nextVertexIndex-1;

                vertices[nextVertexIndex++] = v3;
                triangles[nextTriIndex++] = nextVertexIndex-1;
            }
        }
    }

    void InitMeshVertices(){
        defaultVertices = new Vector3[12]{
            new Vector3(.5f, 0,  0),     //0
            new Vector3(1  , 0, .5f),   //1
            new Vector3(.5f, 0,  1),     //2
            new Vector3(0  , 0, .5f),    //3
            new Vector3(.5f, 1,  0),     //4
            new Vector3(1  , 1, .5f),   //5
            new Vector3(.5f, 1,  1),     //6
            new Vector3(0  , 1, .5f),    //7
            new Vector3(0, .5f,  0),     //8
            new Vector3(1  , .5f, 0),   //9
            new Vector3(1, .5f,  1),     //10
            new Vector3(0  , .5f, 1)     //11
        };
    }

    void UpdateMesh(){
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        Color[] colors = new Color[vertices.Length];

        for (int i = 0; i < vertices.Length; i++){
            colors[i] = Color.Lerp(Color.red, Color.blue, (vertices[i].z/height+((vertices[i].x+vertices[i].y)/(2f*dim)))/2f);
        }

        // assign the array of colors to the Mesh.
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }
}
