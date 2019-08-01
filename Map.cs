using Godot;
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
    public int[,] MapRepresentation{
        get { return currentMap;}
    }
    System.Random rand = new System.Random();
    public Map(int width, int height, int _foodPercentage){
        _width = width;
        _height = height;
        foodPercentage = _foodPercentage/100; //convert to normalized value between 0 and 1;
        currentMap = new int[_height,_width];
        generateMap();
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
    private void generateMap(){
        
        for(int x=0; x<Height; x++){
            for(int y=0; y<Width; y++){
                if(rand.NextDouble()>.95){ //TODO convert to configurable value
                    currentMap[x,y] = (int)TileMap.food;
                }else{
                    currentMap[x,y] =(int)TileMap.ground;
                }
            }
        }
    }
}