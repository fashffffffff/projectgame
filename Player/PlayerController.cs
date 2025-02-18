using System;
using System.Threading.Tasks;
using Godot;

public partial class PlayerController : CharacterBody2D
{
    // Константы для скорости и прыжка
    public const float Speed = 300.0f;
    public const float JumpVelocity = -400.0f;
    public bool shift = false;
	public bool repaired = false;
    public bool guide = false;



    public void NewButton()
    {
        if (!guide) {
            GetNode<RichTextLabel>("/root/Level/Player1/CanvasLayer/RichTextLabel").Visible = true;
            guide = true;
        } else {
            GetNode<RichTextLabel>("/root/Level/Player1/CanvasLayer/RichTextLabel").Visible = false;
            guide = false;
        }
    } 
    
    public override async void _Process(double delta)
{
    // Получаем текущую позицию игрока
    Vector2 position = GlobalPosition;

    // Получаем ссылку на Label по указанному пути
    var label = GetNode<Label>("/root/Level/Machine1/Write/ColorRect2/Label2");

    if ((label.Text == "Hello, world!" || label.Text == "Hello world!") && !repaired)
    {
        // Скрываем слои Write и Code
        GetNode<CanvasLayer>("/root/Level/Machine1/Code").Visible = false;
        GetNode<CanvasLayer>("/root/Level/Machine1/Write").Visible = false;

        var machineSprite = GetNode<AnimatedSprite2D>("/root/Level/Machine1/Machine1Sprite2D");

        // Запускаем анимацию починки
        machineSprite.Play("repair");

        // Ожидаем завершения анимации "repair"
        await ToSignal(machineSprite, "animation_finished");

        // После завершения анимации починки запускаем анимацию работы
        machineSprite.Play("work");
		repaired = true;

    }


        // Проверяем, что текст не равен "Hello, world!" или "Hello world!"
        if (label.Text != "Hello, world!" && label.Text != "Hello world!")
        {
            // Если координата X больше или равна 7475, телепортируем игрока на 7470
            if (position.X >= 7475)
            {
                // Телепортируем игрока по оси X, оставляем текущую ось Y
                GlobalPosition = new Vector2(7470, position.Y);
            }
        }

        // Дополнительная проверка на высоту (Y-координата) для телепортации
        if (position.Y > 800)
        {
            // Телепортируем игрока на новые координаты
            GlobalPosition = new Vector2(3150, 550);
        }
    }

    public override async void _PhysicsProcess(double delta)
    {
        // Получаем текущую скорость игрока
        Vector2 velocity = Velocity;

		bool write = GetNode<CanvasLayer>("/root/Level/Machine1/Write").Visible;

        // Обрабатываем состояние сжатия (Shift)
        if (Input.IsActionPressed("ui_down"))
        {
            GetNode<CollisionShape2D>("CollisionShape2D").Scale = new Vector2(1f, 0.5f);
            GetNode<CollisionShape2D>("CollisionShape2D").Position = new Vector2(1, 10f);
            if (!shift)
            {
                GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("shift");
            }
            shift = true;
        }
        else if (shift)
        {
            GetNode<CollisionShape2D>("CollisionShape2D").Scale = new Vector2(1, 1);
            GetNode<CollisionShape2D>("CollisionShape2D").Position = new Vector2(1, 1);
            shift = false;
        }

        // Добавляем гравитацию, если не на полу
        if (!IsOnFloor())
        {
            velocity += GetGravity() * (float)delta;
        }

        // Обработка прыжка
        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor() && !write)
        {
            velocity.Y = JumpVelocity;
        }
        else if (!IsOnFloor() && !write)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("jump");
        }

        // Обработка движения игрока по горизонтали
        float horizontalMovement = Input.GetAxis("ui_left", "ui_right");
        if (horizontalMovement != 0 && !IsOnFloor() && !write)
        {
            if (Input.IsActionPressed("ui_run"))
            {
                velocity.X = horizontalMovement * Speed * 1.25f;
            }
            else
            {
                velocity.X = horizontalMovement * Speed;
            }
        }
        else if (horizontalMovement != 0 && IsOnFloor() && !write)
        {
            if (Input.IsActionPressed("ui_run"))
            {
                velocity.X = horizontalMovement * Speed * 1.25f;
            }
            else
            {
                velocity.X = horizontalMovement * Speed;
            }
            if (shift)
            {
                GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("shift_run");
            }
            else
            {
                GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("run");
            }
        }
        else if (IsOnFloor())
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
            if (!shift)
            {
                GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play("idle");
            }
        }
        else if (IsOnFloor())
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        // Обработка направления взгляда игрока (влево или вправо)
        if (horizontalMovement == -1)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = true;
        }
        else if (horizontalMovement == 1)
        {
            GetNode<AnimatedSprite2D>("AnimatedSprite2D").FlipH = false;
        }

        // Применяем изменения скорости
        Velocity = velocity;
        MoveAndSlide();
    }
}