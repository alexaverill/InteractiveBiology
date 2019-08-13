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
    Search,
    Eating
    
}
// public enum AnimalState{
//     SearchForFood,
//     MovingToFood,
//     SearchForWater,
//     MovingToWater,
//     SearchForSex,
//     MovingToSex,
//     Eating,
//     Drinking,
//     Exploring,
//     Sex,
//     Hide
// }
public  interface IAnimal: IFood
{
     float Hunger{get; set;}
     float speed { get; set;}
     Gender gender { get; set;}
     AnimalState currentState {get;set;}
     Vector2 target {get;set;}
     Vector2 mapPosition {get;set;}
     float vision {get;set;}
     //void FindNearestTarget();
     void eat();
     void drink();
}
