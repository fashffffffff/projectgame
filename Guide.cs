using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Guide : CharacterBody2D
{
	public void ondetectentered(CharacterBody2D body) {
		if (body.Name == "Player1") {
		GetNode<CanvasLayer>("CanvasLayer").Visible = true;
		}
	}
	public void ondetectorexit(CharacterBody2D body) {
		if (body.Name == "Player1") {
		GetNode<CanvasLayer>("CanvasLayer").Visible = false;
		}
	}
	
	public override void _PhysicsProcess(double delta)
	{

		Vector2 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}
		Velocity = velocity;
		MoveAndSlide();

	}
}