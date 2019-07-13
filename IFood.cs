using System;
using Godot;

public interface IFood{
    int health{get; set;} // amount of "food" left when getting eaten
    int getEaten();

}