using Godot;
using System;

public class GridMap : Godot.GridMap
{
    [Export]
    public int mapWidth = 20;
    
    [Export]
    public int mapHeight = 20;
    [Export]
    public double lakePercent = 0.9;
    private enum TileMap{
      ground = 0,
      river = 1,
      waterEdge = 2,
      waterMiddle = 3,
      lake = 4,
      riverEnd = 5,
      waterCorner=6
      
    }
    private int[,] mapRepresentation;
    public int[,] Map{
      get{return mapRepresentation;}
    }
    private float timer = 0f;
    private int xPos = 0;
    private bool DEBUG = true;
    System.Random random = new System.Random();
    public override void _Ready()
    {
      mapHeight *=3;//adjusting for actual cell size;
      mapWidth *= 3; 
      var list = this.MeshLibrary.GetItemList();
      mapRepresentation = new int[mapHeight,mapWidth];
      GD.Print(list.Length);
      Clear();
      generateMap();
    }
    private void generateMap(){
      
        for(int x =0; x<mapHeight; x +=3){
          
          for(int y=0; y<mapWidth; y+=3){
            var cellType = 0;
            if(random.NextDouble()>lakePercent){
                cellType = (int)TileMap.lake;
            }
            mapRepresentation[x,y] = cellType;
            SetCellItem(x,0,y,cellType);
         }
      }
    }
    private bool runOnce = true;
    public override void _Process(float delta){
        // timer += delta;
        // if(timer > 1){
        //     SetCellItem(xPos,0,0,0);
        //     xPos +=3;
        //     timer = 0f;
        // }
    }

}
