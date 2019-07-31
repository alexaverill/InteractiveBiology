using Godot;
using System;
using System.Collections.Generic;
public class Squirrel : RigidBody, IAnimal, IUpdatable
{
    public float Hunger { get; set; }
    public float speed { get; set; }
    public Gender gender { get; set; }
    public AnimalState currentState { get; set; }
    public Spatial targetRef { get; set; }
    public Vector2 target { get; set; }
    private bool hasTarget= false;
    public float vision { get; set; }
    private Vector2 mapPosition = new Vector2();
    private Map localMap;
    private float timer = 0f;
    private int _health;
    private float stepSize = 8.5f;
    private Vector2 bounds;
    private Controller controller;
    public event Action<Spatial> haveEaten;
    public int health { 
        get{
            return _health;
         }
        set
        {
            if(value == 0){
                die();
            }
            _health = value;
        }
    }
    public void setBounds(Vector2 _bounds){
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
        QueueFree();
    }

    private bool eating = false;
    private float hungerThreshold = 5; // value to start looking for food.

    public override void _Ready()
    {
        speed = 3;
        vision = 50;//TODO: create a representation of the vision for debugging
        health = 100;
        Hunger = 0;
        currentState = AnimalState.Explore;
        
    }
    public void setMap(Map _map){
        localMap = _map;//likely may switch this to pulling from controller.
    }
    public void setPosition(int x, int y){
        //calculate position;
        mapPosition.x = x;
        mapPosition.y = y;
        float xPos = x * stepSize;
        float zPos = y * stepSize; 
        this.SetTranslation(new Vector3(xPos,3,zPos));
    }
    internal List<Spatial> getFoodTargets()
    {
        return controller.getFoodSources();
    }
    Queue<string> movements = new Queue<string>();
    public void update(){
        if(!hasTarget){
            findTarget();
            generateMoves();
            hasTarget= true;
        }
        if(movements.Count>0){
            string movement = movements.Dequeue();
            DoMove(movement);
        }else{
            hasTarget = false;
        }
    }

    private void DoMove(string movement)
    {
        GD.Print(mapPosition);
        switch(movement){
            case "right":
                mapPosition.y ++;
                break;
            case "left":
                mapPosition.y --;
                break;
            case "up":
                mapPosition.x++;
                break;
            case "down":
                mapPosition.x--;
                break;
            default:
                break;
        }
        move();
    }

    private void generateMoves()
    {
       // if(target==null) return;
       int xDiff = (int)Math.Abs(target.x - mapPosition.x);
       int yDiff = (int)Math.Abs(target.y - mapPosition.y);
        if(target.x>mapPosition.x){
            for(int x =0;x<=xDiff;x++){        
                movements.Enqueue("up");
            }
        }else{
            for(int x =0;x<xDiff;x++){           
                 movements.Enqueue("down");
            }
        }
        if(target.y>mapPosition.y){
            for(int x =0;x<=yDiff;x++){        
                movements.Enqueue("right");
            }
        }else{
            
            for(int x =0;x<yDiff;x++){           
                 movements.Enqueue("left");
            }
        }
    }

    private void findTarget()
    {
       // target = new Vector2(0,0);
        if(currentState == AnimalState.Explore){
            //pick random target;
            System.Random rand = new System.Random();
            int xVal =(int)((float)rand.NextDouble() * localMap.Height);
            int yVal = (int)((float)rand.NextDouble() * localMap.Width);
            
            target = new Vector2(xVal, yVal);
            GD.Print(target);
        }
    }
    private void move(){
        SetTranslation(new Vector3(mapPosition.x*stepSize, Transform.origin.y, mapPosition.y*stepSize));
    }
    public void eat()
    {
        eating = true;
        var plant = (IFood)targetRef;
        var increase = plant.getEaten(); 
        if(increase >0 && Hunger >0){
            Hunger -= increase;
        }else{
            eating = false;
            haveEaten.Invoke(targetRef);
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
}
