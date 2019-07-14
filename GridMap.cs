using Godot;
using System;

public class GridMap : Godot.GridMap
{
    [Export]
    public int mapWidth = 20;
    
    [Export]
    public int mapHeight = 20;

    private int[][] mapRepresentation;
    public override void _Ready()
    {
      var list = this.MeshLibrary.GetItemList();
      GD.Print(list.Length);
      Clear();
      for(int x =0; x<mapHeight; x +=3){
          for(int y=0; y<mapWidth; y+=3){
              SetCellItem(x,0,y,0);
          }
      }
    }

}
