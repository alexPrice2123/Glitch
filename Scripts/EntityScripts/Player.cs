using Godot;
using System;

public partial class Player : CharacterBody3D
{
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;
	public const float HorCamSense = 0.002f;
	public const float VerCamSense = 0.002f;

	private Node3D _head;
	private Camera3D _cam;

    public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		_head = GetNode<Node3D>("Head");
		_cam = GetNode<Camera3D>("Head/Camera3D");
    }

	public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseMotion motion && Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			// Rotate the body/head horizontally (Y-axis) and the camera vertically (X-axis)
			_head.RotateY(-motion.Relative.X * HorCamSense);
			_cam.RotateX(-motion.Relative.Y * VerCamSense);

			// Clamp the vertical camera rotation to prevent looking too far up or down
			Vector3 camRot = _cam.Rotation;
			camRot.X = Mathf.Clamp(camRot.X, Mathf.DegToRad(-80f), Mathf.DegToRad(80f));
			_cam.Rotation = camRot;
		}
    }
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
		}

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
		Vector3 direction = (_head.GlobalTransform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}
}
