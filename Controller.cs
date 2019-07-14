using Godot;
using System;
using System.Collections.Generic;
public class Controller : Spatial
{
    [Export]
    public NodePath StatsContainerPath;
    [Export]
    public NodePath gridMapPath;
    private GridMap enviroment;
    private List<Spatial> targets = new List<Spatial>();
    public override void _Ready()
    {
        //enviroment = (GridMap) GetNode(gridMapPath);
        //get all targets!
        var nodes =GetChildren();
        foreach(Node n in nodes){
            if(n.Name.Contains("target")){
                targets.Add((Spatial)n);
            }
        }
        var s = (Squirrel) GetNode<RigidBody>("Squirrel");
        s.registerController(this);
        s.haveEaten += destroyEatenPlants;
        var stat = (StatsContainer) GetNode(StatsContainerPath);
        stat.setAnimalReference(s);
    }

    private void destroyEatenPlants(Spatial obj)
    {
       if(targets.Contains(obj)){
           targets.Remove(obj);
           obj.QueueFree();
       }
    }

    internal List<Spatial> getFoodSources()
    {
        return targets;
    }
}
