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



    private void die()
    {
        QueueFree();
    }

    public List<Spatial> listOfTargets = new List<Spatial>();
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

    internal void setTargets(List<Spatial> targets)
    {
        listOfTargets = targets;
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
            //currentState = AnimalState.Food;
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
                listOfTargets.Remove(targetRef);
                targetRef.QueueFree();//simulate eating at the moment
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
            float y = Transform.origin.y;
            Random rand = new Random();
            float x = (float)rand.NextDouble()* (float)vision*2;
            float z = (float)rand.NextDouble()* (float)vision*2;
            targetVector = new Vector3(x,y,z);
            hasTarget = true;
        }
        // if(listOfTargets.Count<0){
        //     //randomly select a point to move.
        //     return;
        // }
        // //look within sight to see if any targets are available.
        // float maxDistance = vision;
        // Spatial tempTarget = null;
        // foreach(Spatial s in listOfTargets){
        //     var dist = Transform.origin.DistanceTo(s.Transform.origin);
        //     GD.Print(dist);
        //     if (dist < maxDistance && dist > 6.0f){
        //         maxDistance = dist;
        //         tempTarget = s;
        //     }

        // }
        // if(tempTarget != null){
        //     target = tempTarget;
        //     GD.Print(tempTarget.Name);
        //     hasTarget = true;
        //     return;
        // }
        // // if there are no targets found create a temporary target that is half to its total sight
        // //target = Transform.origin; //+ new Vector3(5,4,5);
        // hasTarget = true;

        

    }

    public void eat()
    {
        throw new NotImplementedException();
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
