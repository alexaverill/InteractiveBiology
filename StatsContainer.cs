using Godot;
using System;

public class StatsContainer : GridContainer
{
    [Export]
    public NodePath HealthValPath;
    [Export]
    public NodePath HungerValPath;
    [Export]
    public NodePath targetValPath;
    [Export]
    public NodePath stateNodePath;
    private Label healthVal;
    private Label hungerVal;
    private Label targetVal;
    private Label stateVal;
    private IAnimal animal;
    public override void _Ready()
    {
        healthVal = (Label)GetNode(HealthValPath);
        hungerVal = (Label)GetNode(HungerValPath);
        targetVal = (Label)GetNode(targetValPath);
        stateVal = (Label)GetNode(stateNodePath);
    }
    public void setAnimalReference(IAnimal _animal){
        animal = _animal;
    }
    public override void _Process(float delta){
        if(animal == null) return;

        healthVal.Text = animal.health.ToString();
        hungerVal.Text = animal.Hunger.ToString();
        targetVal.Text = animal.targetVector.ToString();
        stateVal.Text = animal.currentState.ToString();
    }


}
