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
    public Vector3 targetVector { get; set; }
    private bool hasTarget= false;
    public float vision { get; set; }
    private int[,] localMap;
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
    private float updateFrequency = 4f;
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
    public void setMap(int[,] map){
        localMap = map;//likely may switch this to pulling from controller.
    }
    public void setPosition(Vector3 position){
        this.SetTranslation(position);
    }
    internal List<Spatial> getFoodTargets()
    {
        return controller.getFoodSources();
    }
    public void update(){
        //calculate movement based on current state
        // moveForward();
        // moveLeft();
    }

    private void moveForward()
    {
        var newX = Transform.origin.z + stepSize;
        if(newX <= bounds.x){
            SetTranslation(new Vector3(newX,Transform.origin.y,Transform.origin.z));
        }
    }

    private void moveLeft()
    {
        var newZ = Transform.origin.z + stepSize;
        if(newZ <= bounds.y){
            SetTranslation(new Vector3(Transform.origin.x,Transform.origin.y,Transform.origin.z+stepSize));
        }
    }

    // private void LookFollow(PhysicsDirectBodyState state, Transform currentTransform, Vector3 targetPos){
    //     var upDir = new Vector3(0,1,0);
    //     var currentDir = currentTransform.basis.Xform(new Vector3(0,0,1));
    //     var targetDir = (targetPos - currentTransform.origin).Normalized();
    //     var rotationAngle = Mathf.Acos(currentDir.x) - Mathf.Acos(targetDir.x);
    //     state.SetAngularVelocity(upDir*(rotationAngle/state.GetStep()));
    // }
    // public override void _IntegrateForces(PhysicsDirectBodyState state){
    //     if(!hasTarget){
    //         FindNearestTarget();
    //     }else{
    //         LookFollow(state, GetGlobalTransform(), targetVector);
    //         MoveTowards(state, GetGlobalTransform());
    //     }
    // }
    // public override void _PhysicsProcess(float delta){
    //     update(delta);
    // }
    // private void update(float delta){
    //     //
    //     timer +=delta;
    //     if(timer > updateFrequency){
    //         //increment hunger and calculate health
    //         Hunger++;
    //         health -= (int)Hunger%10; // decrease health based on how hungry animal is.
    //         timer =0f;
    //     }

    //     //determine motivation;
    //     if(Hunger> hungerThreshold){
    //         currentState = AnimalState.Food;
    //     }else{
    //         currentState = AnimalState.Explore;
    //     }
    // }

    // private void MoveTowards(PhysicsDirectBodyState state, Transform transform)
    // {
    //     var directionVec =( targetVector - transform.origin).Normalized();
    //     var distance = transform.origin.DistanceTo(targetVector);
    //     if(distance <=4.5f){
    //         if(targetRef !=null){
    //             GD.Print("Time To Eat!");
    //             eat();
    //         }
    //         hasTarget = false;
    //     }
    //     //GD.Print("Distance in movetowards: "+transform.origin.DistanceTo(target));
    //     //if within 4 units do action on target.
    //     state.SetLinearVelocity(directionVec*speed);
    // }

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
        //current this causes the creature to wander in one circle
        float y = Transform.origin.y;
        float x = Transform.origin.x;
        float z = Transform.origin.z;
        //current z limits are -71 to 53. This will change once map is autogenerated.
        //current x limits are -56 to 65
        Random rand = new Random();
        float xDir = (float)rand.NextDouble();
        float zDir = (float) rand.NextDouble();
        x += vision*xDir;
        y += vision * zDir;
        x = Mathf.Clamp(x,-56f, 65f);
        y = Mathf.Clamp(y,-71f, 53f);
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
