using Godot;
using System;
using System.Collections.Generic;
public class Controller : Spatial
{
    private List<Spatial> targets = new List<Spatial>();
    public override void _Ready()
    {
        //get all targets!
        var nodes =GetChildren();
        foreach(Node n in nodes){
            if(n.Name.Contains("target")){
                targets.Add((Spatial)n);
            }
        }
        var s = (Squirrel) GetNode<RigidBody>("Squirrel");
        s.setTargets(targets);
    }


}
