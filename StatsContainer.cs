using Godot;
using System;

public class StatsContainer : GridContainer
{
    [Export]
    public NodePath HealthValPath;
    [Export]
    public NodePath HungerValPath;
    [Export]
    public NodePath ThirstValPath;
    [Export]
    public NodePath targetValPath;
    [Export]
    public NodePath stateNodePath;
    [Export]
    public NodePath currentPosPath;
    private Label healthVal;
    private Label hungerVal;
    private Label targetVal;
    private Label stateVal;
    private Label posVal;
    private Label thirstVal;
    private IAnimal animal;
    public override void _Ready()
    {
        healthVal = (Label)GetNode(HealthValPath);
        hungerVal = (Label)GetNode(HungerValPath);
        thirstVal = (Label)GetNode(ThirstValPath);
        targetVal = (Label)GetNode(targetValPath);
        stateVal = (Label)GetNode(stateNodePath);
        posVal = (Label)GetNode(currentPosPath);
        
    }
    public void setAnimalReference(IAnimal _animal){
        animal = _animal;
    }
    public override void _Process(float delta){
        if(animal == null) return;

        healthVal.Text = animal.health.ToString();
        hungerVal.Text = animal.Hunger.ToString();
        thirstVal.Text = animal.Thirst.ToString();
        targetVal.Text = animal.target.ToString();
        stateVal.Text = animal.currentState.ToString();
        posVal.Text = animal.mapPosition.ToString();
    }


}
