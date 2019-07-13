using Godot;
using System;
using System.Collections.Generic;
public class Squirrel : RigidBody, IAnimal
{
    public float Hunger { get; set; }
    public float speed { get; set; }
    public Gender gender { get; set; }
    public Spatial target { get; set; }
    private bool hasTarget= false;
    public float vision { get; set; }
    public List<Spatial> listOfTargets = new List<Spatial>();
    public override void _Ready()
    {
        speed = 3;
        vision = 10;//TODO: create a representation of the vision for debugging
        
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
        // var target = GetNode<Spatial>("../target").GetGlobalTransform().origin;
        if(!hasTarget){
            FindNearestTarget();
            GD.Print("Current Target is: "+target);
        }else{
            if (target == null) return; //temporary until I create a movement function when it cannot find a target
            LookFollow(state, GetGlobalTransform(), target.Transform.origin);
            MoveTowards(state, GetGlobalTransform(),target.Transform.origin);
        }
    }

    private void MoveTowards(PhysicsDirectBodyState state, Transform transform, Vector3 targetVector)
    {
        var directionVec =( targetVector - transform.origin).Normalized();
        var distance = transform.origin.DistanceTo(targetVector);
        if(distance <=4.5f){
            listOfTargets.Remove(target);
            target.QueueFree();//simulate eating at the moment
            hasTarget = false;
        }
        //GD.Print("Distance in movetowards: "+transform.origin.DistanceTo(target));
        //if within 4 units do action on target.
        state.SetLinearVelocity(directionVec*speed);
    }

    public void FindNearestTarget()
    {
        if(listOfTargets.Count<0){
            //randomly select a point to move.
            return;
        }
        //look within sight to see if any targets are available.
        float maxDistance = vision;
        Spatial tempTarget = null;
        foreach(Spatial s in listOfTargets){
            var dist = Transform.origin.DistanceTo(s.Transform.origin);
            GD.Print(dist);
            if (dist < maxDistance && dist > 6.0f){
                maxDistance = dist;
                tempTarget = s;
            }

        }
        if(tempTarget != null){
            target = tempTarget;
            GD.Print(tempTarget.Name);
            hasTarget = true;
            return;
        }
        // if there are no targets found create a temporary target that is half to its total sight
        //target = Transform.origin; //+ new Vector3(5,4,5);
        hasTarget = true;

        

    }

    public void eat()
    {
        throw new NotImplementedException();
    }

    public void drink()
    {
        throw new NotImplementedException();
    }
}
