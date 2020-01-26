using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBasic : MonoBehaviour
{

    static int overpopLimit, starveLimit, birthRangeLower, birthRangeUpper;
    static int N;
    
    static int[,,] currentState;
    static int[,,] nextState;

    static int dimension;
    static int height;


    static public int[,,] Generate(int w, int h, int[] rules, int N, float initProb){
        // initialize the grid's matrix (all 0's)
        dimension = w;
        height = h;

        overpopLimit = rules[0];
        starveLimit = rules[1];
        birthRangeLower = rules[2];
        birthRangeUpper = rules[3];

        currentState = new int[dimension,dimension, height];

        InitializeCurrentState(initProb);

        for(int i = 0; i < N; i++){
            UpdateState();
        }

        return currentState;
    }

    static void InitializeCurrentState(float initProb){
        float random;
        for(int x = 0; x < dimension; x++){
            for(int y = 0; y < dimension; y++){
                for(int z = 0; z < height; z++){
                    random = Random.Range(0.0f,100.0f);
                    if(random <= initProb){
                        currentState[x,y,z] = 1;
                    }
                }
            }
        }
    }

    static int mod(int a,int b)
    {
        return a - b * (int) System.Math.Floor((float)a / b);
    }

    public static void UpdateState()
    {
        nextState = new int[dimension,dimension, height];

        for(int i = 0; i < dimension; i++){
            for(int j = 0; j < dimension; j++){
                for(int r = 0; r < height; r++){
                    nextState[i,j,r] = UpdatedCell(i,j,r);
                }
            }
        }
        currentState = nextState;
    }

    static int UpdatedCell(int i, int j, int r)
    {
        int nCount = 0;

        // moore neighboorhood in 3D
        // cell's layer
        nCount += currentState[i, mod(j+1,dimension), r];
        nCount += currentState[i, mod(j-1,dimension), r];
        nCount += currentState[mod(i+1, dimension), j, r];
        nCount += currentState[mod(i-1, dimension), j, r];
        nCount += currentState[mod(i+1, dimension), mod(j+1,dimension), r];
        nCount += currentState[mod(i-1, dimension), mod(j+1,dimension), r];
        nCount += currentState[mod(i+1, dimension), mod(j-1,dimension), r];
        nCount += currentState[mod(i-1, dimension), mod(j-1,dimension), r];
        //above layer
        nCount += currentState[i, j, mod(r+1,height)];
        nCount  = currentState[i, mod(j+1,dimension), mod(r+1,height)];
        nCount += currentState[i, mod(j-1,dimension), mod(r+1,height)];
        nCount += currentState[mod(i+1, dimension), j, mod(r+1,height)];
        nCount += currentState[mod(i-1, dimension), j, mod(r+1,height)];
        nCount += currentState[mod(i+1, dimension), mod(j+1,dimension), mod(r+1,height)];
        nCount += currentState[mod(i-1, dimension), mod(j+1,dimension), mod(r+1,height)];
        nCount += currentState[mod(i+1, dimension), mod(j-1,dimension), mod(r+1,height)];
        nCount += currentState[mod(i-1, dimension), mod(j-1,dimension), mod(r+1,height)];
        // below layer
        nCount += currentState[i, j, mod(r-1,height)];
        nCount  = currentState[i, mod(j+1,dimension), mod(r-1,height)];
        nCount += currentState[i, mod(j-1,dimension), mod(r-1,height)];
        nCount += currentState[mod(i+1, dimension), j, mod(r-1,height)];
        nCount += currentState[mod(i-1, dimension), j, mod(r-1,height)];
        nCount += currentState[mod(i+1, dimension), mod(j+1,dimension), mod(r-1,height)];
        nCount += currentState[mod(i-1, dimension), mod(j+1,dimension), mod(r-1,height)];
        nCount += currentState[mod(i+1, dimension), mod(j-1,dimension), mod(r-1,height)];
        nCount += currentState[mod(i-1, dimension), mod(j-1,dimension), mod(r-1,height)];

        if(r < 2) return 1;
        else if(r == height-1) return 0;
        if(r > 10 && Random.Range(0,r+15) > height){
            return 0;
        }

        if(currentState[i,j,r] == 1){
            if(nCount < starveLimit || nCount > overpopLimit) return 0;
            else return 1;
        }
        else{
            if(nCount > birthRangeLower && nCount < birthRangeUpper) return 1;
            else return 0;
        }
    }
}
