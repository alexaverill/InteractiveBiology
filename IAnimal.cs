using System;
using Godot;
public enum Gender{
    Male,
    Female
};
public  interface IAnimal 
{
     float Hunger{get; set;}
     float speed { get; set;}
     Gender gender { get; set;}
     Spatial target { get; set;}
     float vision {get;set;}
     void FindNearestTarget();
     void eat();
     void drink();
}
