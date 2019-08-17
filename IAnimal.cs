using System;
using Godot;
public enum Gender{
    Male,
    Female
};
public enum AnimalState{
    Exploring,
    Moving,
    Arrived,
    SearchForFood,
    SearchForWater,
    Search,
    Eating,
    Drinking
    
}
public  interface IAnimal: IFood
{
     float Hunger{get; set;}
     float Thirst{get;set;}
     float speed { get; set;}
     Gender gender { get; set;}
     AnimalState currentState {get;set;}
     Vector2 target {get;set;}
     Vector2 mapPosition {get;set;}
     float vision {get;set;}
     void eat();
     void drink();
}
