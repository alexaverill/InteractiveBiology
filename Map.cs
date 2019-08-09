using Godot;
using System;
using System.Collections.Generic;
public enum TileMap{
      ground = 0,
      river = 1,
      waterEdge = 2,
      waterMiddle = 3,
      lake = 4,
      riverEnd = 5,
      waterCorner=6,
      food = 7
      
}
public class Map{
    public event Action<Vector2> foodCreated;
    private int _height;
    public int Height{
        get{ return _height;}
    }
    private int _width;
    public int Width{
        get{return _width;}
    }
    private float foodPercentage;
    private int[,] currentMap;
    private int[,] foodMap;
    public int[,] FoodRepresentation{
        get{ return foodMap;}
    }
    public int[,] MapRepresentation{
        get { return currentMap;}
    }
    System.Random rand = new System.Random();
    public Map(int width, int height, int _foodPercentage){
        _width = width;
        _height = height;
        foodPercentage = _foodPercentage/100; //convert to normalized value between 0 and 1;
        currentMap = new int[_height,_width];
        foodMap = new int[_height,_width];
        generateBaseLevel();
    }
    public List<Vector2> getNeighbors(Vector2 position){
        List<Vector2> neighbors = new List<Vector2>();
        if(position.x>0){
            neighbors.Add(new Vector2(position.x-1,position.y));
        }
        if(position.x < Height-1){
            neighbors.Add(new Vector2(position.x +1,position.y));
        }
        if(position.y > 0){
             neighbors.Add(new Vector2(position.x,position.y-1));
        }
        if(position.y < Width-1){
             neighbors.Add(new Vector2(position.x,position.y+1));
        }
        return neighbors;

    }
    public void SetMapItem(Vector2 position,TileMap tileType){
        int x = (int)position.x;
        int y = (int)position.y;
        currentMap[x,y] = (int)tileType;
    }
    //Generates ground and water.
    public void generateBaseLevel(){
        
        for(int x=0; x<Height; x++){
            for(int y=0; y<Width; y++){
                currentMap[x,y] =(int)TileMap.ground;
                // if(rand.NextDouble()>.95){ //TODO convert to configurable value
                //     currentMap[x,y] = (int)TileMap.lake;
                // }else{
                //     currentMap[x,y] =(int)TileMap.ground;
                // }
            }
        }
    }
    public void RemoveFoodItem(Vector2 position){
        foodMap[(int)position.x,(int)position.y] = -1;
    }
    //Generate food layer
    public void GenerateFoodLayer(){
        for(int x=0; x<Height; x++){
            for(int y=0; y<Width; y++){
                if(rand.NextDouble()>.95){ //TODO convert to configurable value
                    foodMap[x,y] = (int)TileMap.food;
                    //foodCreated?.Invoke(new Vector2(x,y));
                }else{
                    foodMap[x,y] =-1;
                }
            }
        }
    }
} 