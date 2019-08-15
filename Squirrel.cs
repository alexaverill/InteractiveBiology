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

    int ThirstFrameRate = 4;
    public float Thirst { get; set; }

    private bool eating = false;
    private float hungerThreshold = 3; // value to start looking for food.
    private int HungerFrameRate = 2;

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
        this.SetTranslation(new Vector3(xPos, 3, zPos));
    }
    Stack<Vector2> movements = new Stack<Vector2>();
    private int frameCount = 0;
    private bool finishedMove = true;

    public void update()
    {
        //Update Loop
        //1. Increment hunger and thirst.
        //2. Check state to ensure that hunger and thirst are taken care of.
        //3. Set target based on state. State chance should reset hasTarget to false/
        //4. Move towards target. 

        UpdateHungerThirst();//update hunger and Thirst 
        CheckState();
        if (currentState == AnimalState.Eating)
        {
            eat();
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
        if (currentState == AnimalState.Arrived || currentState == AnimalState.Eating)
        {
            //do we eat or do we explore essentially.
            if (Hunger > hungerThreshold - 1)
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
        if (Hunger > (hungerThreshold * 4))
        {
            health--;
        }
        //TODO Implement thirst.
    }
    private void Move()
    {
        if (movements.Count > 0)
        {
            currentState = AnimalState.Moving;
            Vector2 moveTo= movements.Pop();
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
        if (currentState == AnimalState.Exploring)
        {
            //pick random target at a slightly arbitraty distance;
            System.Random rand = new System.Random();
            int randDist = rand.Next(2,6);
            BreadthSearchTarget(localMap.MapRepresentation,mapPosition, TileMap.ground,randDist);

        }
        else if (currentState == AnimalState.SearchForFood)
        {
            BreadthSearchTarget(localMap.FoodRepresentation,mapPosition, TileMap.food);

        }
    }
    private void BreadthSearchTarget(int[,] mapRepresentation,Vector2 position, TileMap targetType, int minDistance=0)
    {
        Queue<Vector2> edges = new Queue<Vector2>();
        edges.Enqueue(position);
        Dictionary<Vector2, Vector2> visited = new Dictionary<Vector2, Vector2>();

        // Queue<Vector2> edges = new Queue<Vector2>();
        // edges.Enqueue(position);
        // List<Vector2> visited = new List<Vector2>(); // need to change to a dict
        Vector2 current = new Vector2();
        int count = 0;
        while (edges.Count > 0)
        {
            current = edges.Dequeue();
            if (mapRepresentation[(int)current.x, (int)current.y] == (int)targetType && count >= minDistance)
            {
                GD.Print("Food Found at " + current);
                target = current;
                hasTarget = true;
                break;
            }
            foreach (var s in localMap.getNeighbors(current))
            {
                if (!visited.ContainsKey(s))
                {
                    //need to always reference teh physical map representation rather then the food representation
                    //potentially want to have water in the food representation and simplify it down to a resource map on top of the terrain map.
                    if(localMap.MapRepresentation[(int)s.x,(int)s.y] !=(int) TileMap.lake){ 
                        edges.Enqueue(s);
                        visited[s] = current;
                    }
                }
            }
            count ++;
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
            BreadthSearchTarget(localMap.MapRepresentation,position,TileMap.ground,4);
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
            GD.Print("Food is completely eaten. Time to change state");
            CheckState();
        }

    }

    public void drink()
    {
        throw new NotImplementedException();
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
