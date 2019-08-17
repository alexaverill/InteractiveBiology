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
    public NodePath PlantsParent;
    [Export]
    public NodePath gridMapPath;
    [Export]
    public NodePath speedSliderPath;
    private GridMap enviroment;
    private List<Spatial> targets = new List<Spatial>();
    private float timer;

    public List<IUpdatable> ListOfUpdatable = new List<IUpdatable>();
    private List<IUpdatable> ToRemove = new List<IUpdatable>();//temp list to hold the dead things.
    public Dictionary<Vector2, Plant> plants = new Dictionary<Vector2, Plant>();
    Map currentMap;
    PackedScene plantPrefab;
    PackedScene SquirrelScene;
    bool foodCreated = false;
    Slider speedSlider;
    public override void _Ready()
    {
        speedSlider = (Slider) GetNode(speedSliderPath);
        plantPrefab = (PackedScene)ResourceLoader.Load("res://Plant.tscn");
        SquirrelScene = (PackedScene)ResourceLoader.Load("res://Squirrel.tscn");
        currentMap = new Map(15, 15, 1);
        currentMap.GenerateFoodLayer();
         
        





        enviroment = (GridMap)GetNode(gridMapPath);
        enviroment.EnviromentMap = currentMap;
System.Random rand = new System.Random();
        for(var x = 0; x<5; x++){
            var node = SquirrelScene.Instance();
            AddChild(node);
            var s = (Squirrel)node;
            s.registerController(this);
            s.setStepSize(8.5f);
            s.setBounds(new Vector2(currentMap.PhysicalHeightBounds,currentMap.PhysicalWidthBounds));
             
           
            s.setPosition((int)rand.Next(0, currentMap.Height), (int)rand.Next(0, currentMap.Width)); 
            s.setMap(currentMap);
            s.died += handleUpdatableDied;
            ListOfUpdatable.Add(s);
            var stat = (StatsContainer)GetNode(StatsContainerPath);
         stat.setAnimalReference(s);
        }
        //  var stat = (StatsContainer)GetNode(StatsContainerPath);
        //  stat.setAnimalReference(s);
    }

    private void handleUpdatableDied(IUpdatable obj)
    {
        ToRemove.Add(obj);
    }

    private void PlaceFood(Vector2 vector2)
    {
        
        var node = (Spatial)plantPrefab.Instance();
        var p = (Plant)node;
        p.pos = vector2;
        p.eaten += handleEaten;
        plants.Add(vector2,p);
        try{
            var parent = (Spatial)GetNode(PlantsParent);
            parent.AddChild(node);
        }catch (Exception e){
            GD.Print(e);
        }
        //AddChild(plant);
    }

    private void handleEaten(Vector2 obj)
    {
        currentMap.RemoveFoodItem(obj);
        plants.Remove(obj);
        foreach(IUpdatable i in ListOfUpdatable){
            i.updateMap(currentMap);
        }
    }

    public override void _Process(float delta)
    {
        if (!foodCreated)
        {
            for (int x = 0; x < currentMap.Height; x++)
            {
                for (int y = 0; y < currentMap.Width; y++)
                {
                    if (currentMap.FoodRepresentation[x, y] != -1)
                    {
                        PlaceFood(new Vector2(x, y));
                    }
                }
            }
            foodCreated = true;
        }
        timer += delta;
        if (timer > speedSlider.Value)
        {
            foreach(IUpdatable i in ToRemove){
                
                ListOfUpdatable.Remove(i);
                i.remove();
            }
            ToRemove.Clear();
            foreach (IUpdatable u in ListOfUpdatable)
            {
                u.update();
            }
            timer = 0f;
        }
    }
    public IFood GetPlant(Vector2 position){
        if(plants.ContainsKey(position)){
            return plants[position];
        }
        return null;
    }
}
