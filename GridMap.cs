using Godot;
using System;
public class GridMap : Godot.GridMap
{

    private float timer = 0f;
    private int xPos = 0;
    private bool DEBUG = true;
    System.Random random = new System.Random();
    private Map _map;
    public Map EnviromentMap{
      get{ return _map;}
      set{
        _map = value;
        resetGridMap();
      }

    }
    private void resetGridMap(){
      for(int x=0;x<EnviromentMap.Height; x++){
        for(int y=0; y<EnviromentMap.Width; y++){
          int tileVal = EnviromentMap.MapRepresentation[x,y];
          if(tileVal == (int)TileMap.food){
              SetCellItem(x*3,1,y*3, tileVal);
              tileVal = (int)TileMap.ground;
          }
          SetCellItem(x*3,0,y*3,tileVal);
        }
      }
    }
    public override void _Ready()
    { 
      Clear();

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
