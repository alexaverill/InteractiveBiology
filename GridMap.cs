using Godot;
using System;
using System.Threading;
public class GridMap : Godot.GridMap
{
    [Export]
    public int mapWidth = 20;
    
    [Export]
    public int mapHeight = 20;

    private int[][] mapRepresentation;
    private float timer = 0f;
    private int xPos = 0;
    private bool DEBUG = true;
    public override void _Ready()
    {
      var list = this.MeshLibrary.GetItemList();
      GD.Print(list.Length);
      Clear();
      generateMap();
    }
    private void generateMap(){
        for(int x =0; x<mapHeight; x +=3){
          for(int y=0; y<mapWidth; y+=3){
              SetCellItem(x,0,y,0);
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
