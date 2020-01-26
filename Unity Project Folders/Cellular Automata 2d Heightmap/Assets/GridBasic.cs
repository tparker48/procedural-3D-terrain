using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBasic : MonoBehaviour
{
    public GameObject cubeFab;

    public float P;
    public int n;
    public int heightEccentricity;

    public int dimension;
    public float cubeSize;
    float planeSize, planeScale;
    int range;

    float[,] currentState;
    float[,] nextState;
    GameObject[,] cubes;
    // Start is called before the first frame update
    void Start()
    {
        cubeSize = 1f;
        if(P == 0){P = 30;}
        range = (int)(100f / P);
        // initialize the grid's matrix (all 0's)
        currentState = new float[dimension,dimension];
        nextState = new float[dimension,dimension];

        // init cube matrix for spawning/erasing cubes
        cubes = new GameObject[dimension,dimension];


        for(int factor = 1; factor < System.Math.Log(dimension,2) + 1; factor++){ // 1 to 6
            neighborHoodUpdate(factor);
        }

        drawState();

        float maxHeight = currentState[0,0];
        for(int i = 0; i < dimension; i++){
            for(int j = 0; j < dimension; j++){
                if(currentState[i,j] > maxHeight){
                    maxHeight = currentState[i,j];
                }
            }
        }
        for(int i = 0; i < dimension; i++){
            for(int j = 0; j < dimension; j++){
                currentState[i,j] /= maxHeight;
            }
        }


        TerrainData td = new TerrainData();
        td.heightmapResolution = dimension+1;
        td.size = new Vector3 ( dimension, heightEccentricity, dimension);
        GameObject terrain = Terrain.CreateTerrainGameObject(td);
        td.SetHeights(0,0,currentState);
        
    }



    Vector3 getCubePosition(Vector3 local)
    {
        Vector3 global = new Vector3(local.x, local.y, local.z);

        global.x += cubeSize/2f;
        global.y += cubeSize/2f;
        global.z += cubeSize/2f;

        return global;
    }



    int mod(int a,int b)
    {
        return a - b * (int) System.Math.Floor((float)a / b);
    }

    void neighborHoodUpdate(int factor){
        //factor: 1, 2, 3, 4
        // 2x2,4x4,8x8,16x16,...
        int divisions = (int)System.Math.Pow(2.0,(double)factor);
        int divDimension = dimension / divisions;
        float neighborhoodSum;
        float randOffset;
        float[,] neighborhoods = new float[divisions,divisions];

        for(int x = 0; x < divisions; x++){
            for(int y = 0; y <divisions; y++){
                if(Random.Range(0,range) == 0){
                    randOffset = .5f + Random.Range(-0.5f, 0.5f);
                }
                else{
                    randOffset = 0f;
                }
                neighborhoodSum = 0.0f;

                for(int i = 0; i < divDimension; i++){
                    for(int j =0; j < divDimension; j++){
                        //yikes
                        neighborhoodSum += currentState[(x*divDimension)+i, (y*divDimension)+j] + randOffset;
                    }
                }
                neighborhoods[x,y] = neighborhoodSum / (divDimension*divDimension);
            }
        }

        for(int neighborHoodUpdates = 0; neighborHoodUpdates < n; neighborHoodUpdates++){
            neighborhoods = UpdateNeighborhoods(neighborhoods, divisions);
        }

        for(int x = 0; x < divisions; x++){
            for(int y = 0; y <divisions; y++){
                for(int i = 0; i < divDimension; i++){
                    for(int j =0; j < divDimension; j++){
                        //yikes
                        currentState[(x*divDimension)+i, (y*divDimension)+j] += neighborhoods[x,y];
                    }
                }
            }
        }

    }

    float[,] UpdateNeighborhoods(float[,] neighborhoods, int divisions){
        for(int i = 0; i < divisions; i++){
            for(int j = 0; j < divisions; j++){
                neighborhoods[i,j] = UpdatedCell(neighborhoods, i, j, divisions, 0);
            }
        }
        return neighborhoods;

    }
   float UpdatedCell(float[,] grid ,int i, int j, int dim, int clean)
    {
        float updatedVal = grid[i,j];
        float avgHeight= 0.0f;
        float[] adjacentHeight = new float[8];

        adjacentHeight[0] = grid[i, mod(j+1,dim)];
        adjacentHeight[1] = grid[i, mod(j-1,dim)];
        adjacentHeight[2] = grid[mod(i+1, dim), j];
        adjacentHeight[3] = grid[mod(i-1, dim), j];
        adjacentHeight[4] = grid[mod(i+1, dim), mod(j+1,dim)];
        adjacentHeight[5] = grid[mod(i-1, dim), mod(j+1,dim)];
        adjacentHeight[6] = grid[mod(i+1, dim), mod(j-1,dim)];
        adjacentHeight[7] = grid[mod(i-1, dim), mod(j-1,dim)];

        //represents slope from neighbors to current cell
        float relPeak = adjacentHeight[0];
        float relValley = adjacentHeight[0];

        for(int x = 1; x < 8; x++){
            if (adjacentHeight[x] > relPeak){
                relPeak = adjacentHeight[x];
            }
            if (adjacentHeight[x] < relValley){
                relValley = adjacentHeight[x];
            }
        }
        if(currentState[i,j] > relPeak){
            if(clean == 1){
                for(int x = 0; x < 8; x++){
                    avgHeight += adjacentHeight[x];
                }
                avgHeight = avgHeight / 8f;
                updatedVal = avgHeight + .1f;
            }
        }
        else if(currentState[i,j] < relValley){

        }
        else{
            // smooth / interpolate everything else
            updatedVal = (updatedVal + relPeak)/2;
        }
        
        return updatedVal;  
    }

    void CleanHeightMap(){
        for(int i = 0; i < dimension; i++){
            for(int j = 0; j < dimension; j++){
                currentState[i,j] = UpdatedCell(currentState, i, j, dimension, 1);
            }
        }
    }

    void drawState()
    {
        //GameObject c;
        float minHeight = currentState[0,0];
        // normalize heights: min height = 0
        for(int i = 0; i < dimension; i++){
            for(int j = 0; j < dimension; j++){
                if(currentState[i,j] < minHeight){
                    minHeight = currentState[i,j];
                }
            }
        }

        //UpdateState();
        for(int i = 0; i < dimension; i++){
            for(int j = 0; j < dimension; j++){
                currentState[i,j] = (currentState[i,j] - minHeight);
            }
        }
    }
}
