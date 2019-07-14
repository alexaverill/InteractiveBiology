using Godot;
using System;
using System.Collections.Generic;
public class Squirrel : RigidBody, IAnimal
{
    public float Hunger { get; set; }
    public float speed { get; set; }
    public Gender gender { get; set; }
    public AnimalState currentState { get; set; }
    public Spatial targetRef { get; set; }
    public Vector3 targetVector { get; set; }
    private bool hasTarget= false;
    public float vision { get; set; }
    private float timer = 0f;
    private int _health;
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

    internal void registerController(Controller _controller)
    {
        controller = _controller;
    }

    private void die()
    {
        QueueFree();
    }
    private float updateFrequency = 4f;
    private bool eating = false;
    private float hungerThreshold = 5; // value to start looking for food.

    public override void _Ready()
    {
        speed = 3;
        vision = 10;//TODO: create a representation of the vision for debugging
        health = 100;
        Hunger = 0;
        currentState = AnimalState.Explore;
        
    }

    internal List<Spatial> getFoodTargets()
    {
        return controller.getFoodSources();
    }

    private void LookFollow(PhysicsDirectBodyState state, Transform currentTransform, Vector3 targetPos){
        var upDir = new Vector3(0,1,0);
        var currentDir = currentTransform.basis.Xform(new Vector3(0,0,1));
        var targetDir = (targetPos - currentTransform.origin).Normalized();
        var rotationAngle = Mathf.Acos(currentDir.x) - Mathf.Acos(targetDir.x);
        state.SetAngularVelocity(upDir*(rotationAngle/state.GetStep()));
    }
    public override void _IntegrateForces(PhysicsDirectBodyState state){
        if(!hasTarget){
            FindNearestTarget();
        }else{
            LookFollow(state, GetGlobalTransform(), targetVector);
            MoveTowards(state, GetGlobalTransform());
        }
    }
    public override void _PhysicsProcess(float delta){
        timer += delta;
        if(timer > updateFrequency){
            updateStats();
            timer = 0f;
        }
    }

    private void updateStats()
    {
        if(!eating){
            Hunger ++;
        }
        //determine what this animals priorty is;
        if(Hunger > hungerThreshold){
            currentState = AnimalState.Food;
            health --;
        }else{
            currentState = AnimalState.Explore;
        }
    }

    private void MoveTowards(PhysicsDirectBodyState state, Transform transform)
    {
        var directionVec =( targetVector - transform.origin).Normalized();
        var distance = transform.origin.DistanceTo(targetVector);
        if(distance <=4.5f){
            if(targetRef !=null){
                GD.Print("Time To Eat!");
                eat();
            }
            hasTarget = false;
        }
        //GD.Print("Distance in movetowards: "+transform.origin.DistanceTo(target));
        //if within 4 units do action on target.
        state.SetLinearVelocity(directionVec*speed);
    }

    public void FindNearestTarget()
    {
        if( currentState == AnimalState.Explore){
            targetRef = null;
            //an exploring animal will pick a direction and move towards it, it will be within the animals vision radius
            CreateWanderTarget();
        }else if(currentState == AnimalState.Food){
            targetRef = null;
            var foodTargets = getFoodTargets();
            SearchForNearest(foodTargets);
            if(targetRef == null){
                CreateWanderTarget();
            }else{
                hasTarget = true;
            }
        }
     }

    private void CreateWanderTarget()
    {
        float y = Transform.origin.y;
        Random rand = new Random();
        float x = (float)rand.NextDouble()* (float)vision*2;
        float z = (float)rand.NextDouble()* (float)vision*2;
        targetVector = new Vector3(x,y,z);
        hasTarget = true;
    }

    private void SearchForNearest(List<Spatial> foodTargets)
    {
        float currentClosest = 100;
       foreach(Spatial s in foodTargets){
           var distance = Transform.origin.DistanceTo(s.Transform.origin);
           if (distance < 100 && distance < currentClosest){
               currentClosest = distance;
               targetRef = s;
               targetVector = s.Transform.origin;
           }
       }
       
    }

    public void eat()
    {
        var plant = (Plant)targetRef;
        var increase = plant.getEaten(); 
        if(increase >0){
            health += increase;
        }else{
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
