using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBasic : MonoBehaviour
{
    public GameObject planeFab;
    public GameObject cubeFab;

    public float updateDelay;

    public int dimensionH;
    public int dimensionV;

    public float cubeSize;
    float planeSize, planeScale;

    public int dieHigh, dieLow, birthHigh, birthLow;

    public int n;
    int count;
    
    int[,,] currentState;
    int[,,] nextState;
    GameObject[,,] cubes;

    Stack activeCells;
    Stack activeCubes;
    
    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        // initialize the grid's matrix (all 0's)
        currentState = new int[dimensionH,dimensionH, dimensionV];
        nextState = new int[dimensionH,dimensionH, dimensionV];

        // init cube matrix for spawning/erasing cubes
        cubes = new GameObject[dimensionH,dimensionH, dimensionV];

        for(int i = 0; i < dimensionH; i++)
        {
            for(int j = 0; j < dimensionH; j++)
            {
                for(int r = 0; r < dimensionV; r++){
                    if(Random.Range(0, 2) == 0)
                    {
                        currentState[i,j,r] = 1;
                    }
                    cubes[i,j,r] = (GameObject) Instantiate(cubeFab, getCubePosition(new Vector3(i,r,j)), Quaternion.identity);
                    cubes[i,j,r].transform.localScale = new Vector3(cubeSize,cubeSize,cubeSize);
                    cubes[i,j,r].GetComponent<Renderer>().material.SetColor("_Color", new Color(((float)i/dimensionH)+.5f,((float)j/dimensionH)+.5f,((float)r/dimensionV)+.5f));
                }
                
            }
        }


        InvokeRepeating("UpdateState", 0.0f, updateDelay);
        /*for(int i = 0; i < n; i++)
        {
            UpdateState();
        }
        */
        
    }


    Vector3 getCubePosition(Vector3 local)
    {
        Vector3 global = new Vector3(local.x, local.y, local.z);
        global.x *= cubeSize;
        global.y *= cubeSize;
        global.z *= cubeSize;

        global.x += cubeSize/2f;
        global.y += cubeSize/2f;
        global.z += cubeSize/2f;

        return global;
    }

    // Update is called once per frame
    void Update()
    {
        drawCurrentState();
    }

    int mod(int a,int b)
    {
        return a - b * (int) System.Math.Floor((float)a / b);
    }

    void UpdateState()
    {
        if(count < n)
        {
            for(int i = 0; i < dimensionH; i++){
                for(int j = 0; j < dimensionH; j++){
                    for(int r = 0; r < dimensionV; r++){
                        nextState[i,j,r] = UpdatedCell(i,j,r);
                    }
                }
            }

            currentState = nextState;
            nextState = new int[dimensionH,dimensionH,dimensionV];
            count++;
        }                
    }

    int UpdatedCell(int i, int j, int r)
    {
        int nCount;

        // cell's layer
        nCount  = currentState[i, mod(j+1,dimensionH), r];
        nCount += currentState[i, mod(j-1,dimensionH), r];
        nCount += currentState[mod(i+1, dimensionH), j, r];
        nCount += currentState[mod(i-1, dimensionH), j, r];
        nCount += currentState[mod(i+1, dimensionH), mod(j+1,dimensionH), r];
        nCount += currentState[mod(i-1, dimensionH), mod(j+1,dimensionH), r];
        nCount += currentState[mod(i+1, dimensionH), mod(j-1,dimensionH), r];
        nCount += currentState[mod(i-1, dimensionH), mod(j-1,dimensionH), r];
        //above layer
        nCount += currentState[i, j, mod(r+1,dimensionV)];
        nCount  = currentState[i, mod(j+1,dimensionH), mod(r+1,dimensionV)];
        nCount += currentState[i, mod(j-1,dimensionH), mod(r+1,dimensionV)];
        nCount += currentState[mod(i+1, dimensionH), j, mod(r+1,dimensionV)];
        nCount += currentState[mod(i-1, dimensionH), j, mod(r+1,dimensionV)];
        nCount += currentState[mod(i+1, dimensionH), mod(j+1,dimensionH), mod(r+1,dimensionV)];
        nCount += currentState[mod(i-1, dimensionH), mod(j+1,dimensionH), mod(r+1,dimensionV)];
        nCount += currentState[mod(i+1, dimensionH), mod(j-1,dimensionH), mod(r+1,dimensionV)];
        nCount += currentState[mod(i-1, dimensionH), mod(j-1,dimensionH), mod(r+1,dimensionV)];
        // below layer
        nCount += currentState[i, j, mod(r-1,dimensionV)];
        nCount  = currentState[i, mod(j+1,dimensionH), mod(r-1,dimensionV)];
        nCount += currentState[i, mod(j-1,dimensionH), mod(r-1,dimensionV)];
        nCount += currentState[mod(i+1, dimensionH), j, mod(r-1,dimensionV)];
        nCount += currentState[mod(i-1, dimensionH), j, mod(r-1,dimensionV)];
        nCount += currentState[mod(i+1, dimensionH), mod(j+1,dimensionH), mod(r-1,dimensionV)];
        nCount += currentState[mod(i-1, dimensionH), mod(j+1,dimensionH), mod(r-1,dimensionV)];
        nCount += currentState[mod(i+1, dimensionH), mod(j-1,dimensionH), mod(r-1,dimensionV)];
        nCount += currentState[mod(i-1, dimensionH), mod(j-1,dimensionH), mod(r-1,dimensionV)];

        if(r < 2) return 1;
        else if(r == dimensionV-1) return 0;
        if(r > 10 && Random.Range(0,r+15) > dimensionV){
            return 0;
        }


        if(currentState[i,j,r] == 1){
            if(nCount < dieLow || nCount > dieHigh) return 0;
            else return 1;
        }
        else{
            if(nCount > birthLow && nCount < birthHigh) return 1;
            else return 0;
        }
    }

    void drawCurrentState()
    {
        for(int i = 0; i < dimensionH; i++){
            for(int j = 0; j < dimensionH; j++){
                for(int r= 0; r < dimensionV; r++){
                    cubes[i,j,r].SetActive(currentState[i,j,r] == 1);
                }
                
            }
        }
    }
}
