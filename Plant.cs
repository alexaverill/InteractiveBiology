using Godot;
using System;

public class Plant : Spatial, IFood
{
    public event Action<Vector2> eaten;
    private int _health;
    public int health
    {
        get;set;
        // get { return _health; }
        // set
        // {
        //     if (value <= 0)
        //     {
        //         _health = 0;
        //     }
        //     else
        //     {
        //         health = value;
        //     }
        // }
    }
    private Vector2 _pos;
    public Vector2 pos
    {
        get { return _pos; }
        set
        {
            _pos = value;
            SetTranslation(new Vector3(_pos.x * 8.5f, 5, _pos.y * 8.5f));

        }
    }
    //TODO make IUpdatable and die slowly based on heat.
    // {
    //     public event Action<Vector2> eaten;

    //     private Vector2 _pos;
    //         public Vector2 pos { 
    //             get{return _pos;} 
    //             set{
    //                 _pos = value;
    //                 SetTranslation(new Vector3(_pos.x*3,2,_pos.y*3));
    //             }
    //         }
    public int getEaten()
    {
        if (health <= 0)
        {
            eaten.Invoke(pos);
            QueueFree();
            return 0; //todo destroy this and send an event.
        }
        health -= 1;
        return 1;
    }

    public override void _Ready()
    {
        health = 2;
    }
    public override void _Process(float delta)
    {

    }
}
