using Godot;
using System;

public class Plant : CSGMesh, IFood
{
    public int health { get; set ; }

    public int getEaten()
    {
        if(health <=0) return 0; //todo destroy this and send an event.
        health -= 1;
        return 1;
    }

    public override void _Ready()
    {
        health = 100;
    }

}
