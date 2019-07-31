using Godot;
using System;
using System.Collections.Generic;
public class Controller : Spatial
{
    [Export]
    public float updateSpeed = 1;
    [Export]
    public NodePath StatsContainerPath;
    [Export]
    public NodePath gridMapPath;
    private GridMap enviroment;
    private List<Spatial> targets = new List<Spatial>();
    private float timer;

    public List<IUpdatable> ListOfUpdatable = new List<IUpdatable>();

    public override void _Ready()
    {
        Map currentMap = new Map(10,10,1);
        enviroment = (GridMap) GetNode(gridMapPath);
        enviroment.EnviromentMap = currentMap;  


        //TODO: Clean up this mess
        var s = (Squirrel) GetNode<RigidBody>("Squirrel");
        s.registerController(this);
        s.haveEaten += destroyEatenPlants;
        s.setStepSize(8.5f);
        s.setBounds(new Vector2(85,85));
        s.setPosition(5,5); //max is 85 on both axis
        s.setMap(currentMap);
        ListOfUpdatable.Add(s);
        var stat = (StatsContainer) GetNode(StatsContainerPath);
        stat.setAnimalReference(s);
    }
    public override void _Process(float delta){
        timer += delta;
        if(timer > updateSpeed){
            foreach(IUpdatable u in ListOfUpdatable){
                u.update();
            }
            timer = 0f;
        }
    }
    private void destroyEatenPlants(Spatial obj)
    {
       if(targets.Contains(obj)){
           targets.Remove(obj);
           obj.QueueFree();
       }
    }
}
