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
    private int HungerFrameRate = 16;

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
    Queue<Vector2> movements = new Queue<Vector2>();
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
            generateMoves();
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
            Vector2 moveTo= movements.Dequeue();
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
    }
    private void generateMoves()
    {
        // if(target==null) return;
        int xDiff = (int)Math.Abs(target.x - mapPosition.x);
        int yDiff = (int)Math.Abs(target.y - mapPosition.y);
        if (target.x > mapPosition.x)
        {
            for (int x = 0; x < xDiff; x++)
            {
                movements.Enqueue("up");
            }
        }
        else
        {
            for (int x = 0; x < xDiff; x++)
            {
                movements.Enqueue("down");
            }
        }
        if (target.y > mapPosition.y)
        {
            for (int x = 0; x < yDiff; x++)
            {
                movements.Enqueue("right");
            }
        }
        else
        {

            for (int x = 0; x < yDiff; x++)
            {
                movements.Enqueue("left");
            }
        }
    }
    private Vector2 RandomTarget()
    {
        System.Random rand = new System.Random();
        int xVal = (int)((float)rand.NextDouble() * localMap.Height);
        int yVal = (int)((float)rand.NextDouble() * localMap.Width);

        return new Vector2(xVal, yVal);
    }
    private void findTarget()
    {
        if (currentState == AnimalState.Exploring)
        {
            //pick random target;
            target = RandomTarget();
            hasTarget = true;
        }
        else if (currentState == AnimalState.SearchForFood)
        {
            BreadthSearchTarget(mapPosition, TileMap.food);

        }
    }
    private void BreadthSearchTarget(Vector2 position, TileMap targetType)
    {
        Queue<Vector2> edges = new Queue<Vector2>();
        edges.Enqueue(position);
        Dictionary<Vector2, Vector2> visited = new Dictionary<Vector2, Vector2>();

        // Queue<Vector2> edges = new Queue<Vector2>();
        // edges.Enqueue(position);
        // List<Vector2> visited = new List<Vector2>(); // need to change to a dict
        Vector2 current = new Vector2();
        while (edges.Count > 0)
        {
            current = edges.Dequeue();
            if (localMap.FoodRepresentation[(int)current.x, (int)current.y] == (int)targetType)
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
                    edges.Enqueue(s);
                    visited[s] = current;
                }
                // if (!cameFrom.Contains(s))
                // {
                //     edges.Enqueue(s);
                //     visited.AddAfter(current,s);
                // }
            }
        }
        if (hasTarget)
        {
            //generate movement list;
            Queue<Vector2> path = new Queue<Vector2>();
            Vector2 start = position;

            while (current != start)
            {
                path.Enqueue(current);
                current = visited[current];
            }

            path = ReverseQueue(path);
            foreach (Vector2 v in path)
            {
                GD.Print(v);
            }
        }
        else
        {
            GD.Print("Yo I have no target");
        }
        // target = RandomTarget();
        // hasTarget = true;
    }
    //Queue.Reverse() doesnt seem to exist, something about LINQ and I am to lazy/tired to care enough to sort out why. 
    private Queue<Vector2> ReverseQueue(Queue<Vector2> q)
    {
        Queue<Vector2> outQueue = new Queue<Vector2>();
        for (int x = 0; x < q.Count; x++)
        {
            outQueue.Enqueue(q.Dequeue());
        }
        return outQueue;

    }
    public void eat()
    {
        GD.Print("I am eating");
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
