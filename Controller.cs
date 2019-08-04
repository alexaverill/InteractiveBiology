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
    private GridMap enviroment;
    private List<Spatial> targets = new List<Spatial>();
    private float timer;

    public List<IUpdatable> ListOfUpdatable = new List<IUpdatable>();
    public Dictionary<Vector2, Plant> plants = new Dictionary<Vector2, Plant>();
    Map currentMap;
    PackedScene plantPrefab;
    bool foodCreated = false;
    public override void _Ready()
    {
        plantPrefab = (PackedScene)ResourceLoader.Load("res://Plant.tscn");
        currentMap = new Map(10, 10, 1);
        //currentMap.foodCreated += handleFoodCreated;
        currentMap.GenerateFoodLayer();





        enviroment = (GridMap)GetNode(gridMapPath);
        enviroment.EnviromentMap = currentMap;

        //TODO: Clean up this mess
        var s = (Squirrel)GetNode<RigidBody>("Squirrel");
        s.registerController(this);
        s.setStepSize(8.5f);
        s.setBounds(new Vector2(85, 85));
        s.setPosition(5, 5); //max is 85 on both axis
        s.setMap(currentMap);
        ListOfUpdatable.Add(s);
        var stat = (StatsContainer)GetNode(StatsContainerPath);
        stat.setAnimalReference(s);
    }

    private void PlaceFood(Vector2 vector2)
    {
        
        var node = (Spatial)plantPrefab.Instance();
        var p = (Plant)node;
        p.pos = vector2;
        plants.Add(vector2,p);
        try{
            var parent = (Spatial)GetNode(PlantsParent);
            parent.AddChild(node);
        }catch (Exception e){
            GD.Print(e);
        }
        //AddChild(plant);
    }

    private void handleFoodCreated(Vector2 obj)
    {

        // var plant = (CSGMesh)plantPrefab.Instance();
        // var p = (Plant) plant;
        // p.position = obj;
        //GetNode(PlantsParent).AddChild(plant);

        //node.position = obj;
        // Plant p = new Plant();
        // p.position = obj;
        // plants.Add(obj,p);
        // p.eaten += handleFoodEaten;
    }

    private void handleFoodEaten(Vector2 obj)
    {
        GD.Print("Food Eaten");
        currentMap.SetMapItem(obj, TileMap.ground);
        enviroment.EnviromentMap = currentMap;
        enviroment.resetGridMap();
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
        if (timer > updateSpeed)
        {
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
