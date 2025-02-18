using Godot;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Runtime.Loader;

public partial class Machine1 : CharacterBody2D
{
    bool on = false;
    bool off = false;

    private TextEdit textEdit;
    private Label label2;


    public override void _Ready()
    {
        // Получаем узлы
        textEdit = GetNode<TextEdit>("Write/TextEdit");
        label2 = GetNode<Label>("Write/ColorRect2/Label2");

        // Подписываемся на событие изменения текста
        textEdit.TextChanged += OnTextChanged;
    }
    

    private async void OnTextChanged()
    {
        string userCode = textEdit.Text.Trim();

        try
        {
            string result = await CompileAndRun(userCode);
            label2.Text = result; // Выводим результат в Label2
        }
        catch (Exception ex)
        {
            label2.Text = "Ошибка: " + ex.Message; // Выводим ошибку
        }
    }

    private async Task<string> CompileAndRun(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Select(a => MetadataReference.CreateFromFile(a.Location));

        var compilation = CSharpCompilation.Create(
            "UserScript" + Guid.NewGuid().ToString("N"), // Уникальное имя
            new[] { syntaxTree },
            references,
            new CSharpCompilationOptions(OutputKind.ConsoleApplication)
        );

        using (var ms = new MemoryStream())
        {
            var result = compilation.Emit(ms);
            if (!result.Success)
            {
                return "Ошибка компиляции: " + string.Join("\n", result.Diagnostics);
            }

            ms.Seek(0, SeekOrigin.Begin);
            var context = new AssemblyLoadContext(null, true); // Изолированная загрузка
            var assembly = context.LoadFromStream(ms);
            var entryPoint = assembly.EntryPoint;
            if (entryPoint == null)
            {
                return "Ошибка: Точка входа не найдена.";
            }

            using (var writer = new StringWriter())
            {
                Console.SetOut(writer);
                object? instance = entryPoint.IsStatic ? null : Activator.CreateInstance(entryPoint.DeclaringType);
                entryPoint.Invoke(instance, entryPoint.GetParameters().Length > 0 ? new object[] { new string[0] } : null);
                Console.Out.Flush();
                context.Unload(); // Выгружаем сборку
                return writer.ToString().Trim();
            }
        }
    }

    public void _on_button_pressed()
    {
        if (on)
        {
            GetNode<CanvasLayer>("Write").Visible = true;
            GetNode<CanvasLayer>("Code").Visible = false;
        }
        off = true;
    }

    public void onbodyentered(CharacterBody2D body)
    {
        on = true;
        if (body.Name == "Player1" && !off)
        {
            GetNode<CanvasLayer>("Code").Visible = true;
        }
    }

    public void onbodyexited(CharacterBody2D body)
    {
        if (body.Name == "Player1")
        {
            GetNode<CanvasLayer>("Code").Visible = false;
            GetNode<CanvasLayer>("Write").Visible = false;
        }
        on = false;
        off = false;
    }
    public void OnButtonPressed()
    {
        
            GetNode<CanvasLayer>("Code").Visible = false;
            GetNode<CanvasLayer>("Write").Visible = false;
        on = false;
        off = false;
    }
}