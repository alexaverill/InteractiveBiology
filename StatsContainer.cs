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
    
    private Label healthVal;
    private Label hungerVal;
    private Label targetVal;
    private IAnimal animal;
    public override void _Ready()
    {
        healthVal = (Label)GetNode(HealthValPath);
        hungerVal = (Label)GetNode(HungerValPath);
        targetVal = (Label)GetNode(targetValPath);
    }
    public void setAnimalReference(IAnimal _animal){
        animal = _animal;
    }
    public override void _Process(float delta){
        if(animal == null) return;

        healthVal.Text = animal.health.ToString();
        hungerVal.Text = animal.Hunger.ToString();
        targetVal.Text = animal.targetVector.ToString();
    }


}
