using System;
using Godot;
public enum Gender{
    Male,
    Female
};
public enum AnimalState{
    SearchForFood,
    SearchForWater,
    SearchForSex,
    Eating,
    Drinking,
    Exploring,
    Sex,
    Hide
}
public  interface IAnimal: IFood
{
     float Hunger{get; set;}
     float speed { get; set;}
     Gender gender { get; set;}
     AnimalState currentState {get;set;}
     Spatial targetRef { get; set;}
     Vector2 target {get;set;}
     float vision {get;set;}
     //void FindNearestTarget();
     void eat();
     void drink();
}
