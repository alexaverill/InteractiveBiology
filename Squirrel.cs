using Godot;
using System;
using System.Collections.Generic;
public class Squirrel : RigidBody, IAnimal, IUpdatable
{
    public event Action<IUpdatable> died;
    public float Hunger { get; set; }
    public float speed { get; set; }
    public Gender gender { get; set; }
    public AnimalState currentState { get; set; }
    public Vector2 target { get; set; }
    private bool hasTarget = false;
    public float vision { get; set; }
    private Vector2 _mapPosition;
    public Vector2 mapPosition
    {
        get { return _mapPosition; }
        set { _mapPosition = value; }
    }
    private Map localMap;
    private float timer = 0f;
    private int _health;
    private float stepSize = 8.5f;
    private Vector2 bounds;
    private Controller controller;
    public event Action<Spatial> haveEaten;
    public int health
    {
        get
        {
            return _health;
        }
        set
        {
            if (value == 0)
            {
                die();
            }
            _health = value;
        }
    }

    Stack<Vector2> movements = new Stack<Vector2>();
    private int frameCount = 0;
    private bool finishedMove = true;
    private int thirstThreshold = 9;
    int ThirstFrameRate = 6;
    public float Thirst { get; set; }

    private bool eating = false;
    private float hungerThreshold = 15; // value to start looking for food.
    private int HungerFrameRate = 16;
    private int healthFrameRate = 10;

    public void setBounds(Vector2 _bounds)
    {
        bounds = _bounds;
    }
    public void setStepSize(float v)
    {
        stepSize = v;
    }

    public void registerController(Controller _controller)
    {
        controller = _controller;
    }

    private void die()
    {
        died?.Invoke(this);
    }
    public void remove()
    {
        QueueFree();
    }
System.Random rand = new System.Random();
    public override void _Ready()
    {
        speed = 3;
        vision = 50;//TODO: create a representation of the vision for debugging
        health = 100;
        Hunger = 0;
        currentState = AnimalState.Exploring;
        mapPosition = new Vector2();

    }
    public void setMap(Map _map)
    {
        localMap = _map;//likely may switch this to pulling from controller.
    }
    public void setPosition(int x, int y)
    {
        //calculate position;
        mapPosition = new Vector2(x, y);
        float xPos = x * stepSize;
        float zPos = y * stepSize;
        this.SetTranslation(new Vector3(xPos, 3.5f, zPos));
    }

    public void update()
    {
        //Update Loop
        //1. Increment hunger and thirst.
        //2. Check state to ensure that hunger and thirst are taken care of.
        //3. Set target based on state. 
        //4. Move towards target. 

        UpdateHungerThirst();//update hunger and Thirst 
        CheckState();
        if (currentState == AnimalState.Eating)
        {
            eat();
        }
        else if (currentState == AnimalState.Drinking)
        {
            drink();
        }
        else
        {
            Move();
        }
        frameCount++;
    }


    private void CheckState()
    {
        if (currentState == AnimalState.Moving)
        {
            //we continue to move
            return;
        }
        if (currentState == AnimalState.Arrived || currentState == AnimalState.Eating || currentState == AnimalState.Drinking)
        {
            //do we eat or do we explore essentially.
            if (Thirst > thirstThreshold-1)
            {
                if (isAtWater())
                {
                    currentState = AnimalState.Drinking;
                    return; // early return to ignore find target calls
                }
                else
                {
                   // GD.Print("Time to look for Water");
                    currentState = AnimalState.SearchForWater;
                }

            }
            else if (Hunger > hungerThreshold - 1)
            {
                if (IsAtFood())
                {
                    currentState = AnimalState.Eating;
                    return;
                }
                else
                {
                    currentState = AnimalState.SearchForFood;
                }
            }
            else
            {
                currentState = AnimalState.Exploring;

            }
            findTarget();
            //generateMoves();
        }

    }

    private bool IsAtFood()
    {
        if (localMap.FoodRepresentation[(int)mapPosition.x, (int)mapPosition.y] == (int)TileMap.food)
        {
            return true;
        }
        return false;
    }

    private bool isAtWater()
    {
        if (localMap.MapRepresentation[(int)mapPosition.x, (int)mapPosition.y] == (int)TileMap.lake)
        {
            return true;
        }
        return false;
    }

    private void UpdateHungerThirst()
    {
        if (frameCount % HungerFrameRate == 0)
        {
            Hunger++;
        }
        if (frameCount % ThirstFrameRate == 0)
        {
            Thirst++;
        }
        if (
            (Hunger > (hungerThreshold * 4)) || (Thirst > (thirstThreshold * 4))
            && frameCount % healthFrameRate == 0)
        {
            health--;
        }
    }
    private void Move()
    {
        if (movements.Count > 0)
        {
            currentState = AnimalState.Moving;
            Vector2 moveTo = movements.Pop();
            move(moveTo);
        }
        else
        {
            currentState = AnimalState.Arrived;
        }
    }
    private void move(Vector2 newPosition)
    {
        //TODO implement some form of animation on this.
        SetTranslation(new Vector3(newPosition.x * stepSize, Transform.origin.y, newPosition.y * stepSize));
        mapPosition = newPosition;
    }
    private void findTarget()
    {
        switch (currentState)
        {
            case AnimalState.Exploring:
                
                int randDist = rand.Next(2, 6);
                BreadthSearchTarget(localMap.MapRepresentation, mapPosition, TileMap.ground, randDist);
                break;
            case AnimalState.SearchForFood:
                BreadthSearchTarget(localMap.FoodRepresentation, mapPosition, TileMap.food);
                break;
            case AnimalState.SearchForWater:
                BreadthSearchTarget(localMap.MapRepresentation, mapPosition, TileMap.lake);
                break;
            default:
                break;
        }
    }
    private void BreadthSearchTarget(int[,] mapRepresentation, Vector2 position, TileMap targetType, int minDistance = 0)
    {
        Queue<Vector2> edges = new Queue<Vector2>();
        edges.Enqueue(position);
        Dictionary<Vector2, Vector2> visited = new Dictionary<Vector2, Vector2>();
        Vector2 current = new Vector2();
        int count = 0;
        while (edges.Count > 0)
        {
            current = edges.Dequeue();
            if (mapRepresentation[(int)current.x, (int)current.y] == (int)targetType && count >= minDistance)
            {
                //GD.Print(targetType+" Found at " + current);
                target = current;
                hasTarget = true;
                break;
            }
            foreach (var s in localMap.getNeighbors(current))
            {
                if (!visited.ContainsKey(s))
                {
                    //need to always reference the physical map representation rather then the food representation
                    //potentially want to have water in the food representation and simplify it down to a resource map on top of the terrain map.
                    if (targetType != TileMap.lake)
                    {
                        if (localMap.MapRepresentation[(int)s.x, (int)s.y] != (int)TileMap.lake)
                        {
                            edges.Enqueue(s);
                            visited[s] = current;
                        }
                    }
                    else
                    {
                        edges.Enqueue(s);
                        visited[s] = current;
                    }
                }
            }
            count++;
        }
        if (hasTarget)
        {
            //generate movement list;
            Stack<Vector2> path = new Stack<Vector2>();
            Vector2 start = position;

            while (current != start)
            {
                path.Push(current);
                current = visited[current];
            }
            movements = path;
        }
        else
        {
            GD.Print("Yo I have no target");
            //try to find a random target 
            BreadthSearchTarget(localMap.MapRepresentation, position, TileMap.ground, 4);
            return;
        }
    }
    public void eat()
    {
        //GD.Print("I am eating");
        eating = true;
        //get a reference to current food;
        Plant currentFood = (Plant)controller.GetPlant(mapPosition);
        if (currentFood == null)
        {
            GD.Print("No Plant at this position! " + mapPosition);
            CheckState();
            hasTarget = false;
            return;
        }
        var foodVal = currentFood.getEaten();
        if (foodVal > 0)
        {
            //if we have food to eat, eat until I am not hungy.
            if (Hunger > 0)
            {
                Hunger -= foodVal;
            }
            else
            {
                CheckState();
            }
        }
        else
        {
            eating = false;
            CheckState();
        }

    }

    public void drink()
    {

        if (Thirst > 0)
        {
            Thirst -= 1;
        }
        else
        {
            CheckState();
        }

    }

    public int getEaten()
    {
        throw new NotImplementedException();
    }

    public void updateMap(Map newMap)
    {
        localMap = newMap;
    }
}
