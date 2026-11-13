using System;
using System.Drawing;
using System.Windows.Forms;

namespace Lab31;

public sealed class MainForm : Form
{
    private readonly TextBox _nameBox;

    public MainForm()
    {
        Text = "Лабораторная работа № 31 — Windows Forms";
        ClientSize = new Size(560, 220);
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Font = new Font("Segoe UI", 10F);

        var title = new Label
        {
            Text = "Введите имя и нажмите кнопку",
            AutoSize = true,
            Location = new Point(24, 24)
        };

        _nameBox = new TextBox
        {
            Location = new Point(24, 54),
            Width = 500,
            PlaceholderText = "Например, Арслан"
        };

        var button = new Button
        {
            Text = "Поздороваться",
            Location = new Point(24, 100),
            Size = new Size(180, 40)
        };
        button.Click += OnGreet;

        Controls.Add(title);
        Controls.Add(_nameBox);
        Controls.Add(button);

        AcceptButton = button;
        ActiveControl = _nameBox;
    }

    private void OnGreet(object? sender, EventArgs e)
    {
        string name = _nameBox.Text.Trim();
        if (name.Length == 0)
        {
            MessageBox.Show("Сначала введите имя.", "Проверка",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _nameBox.Focus();
            return;
        }

        MessageBox.Show($"Привет, {name}!", "Ответ",
            MessageBoxButtons.OK, MessageBoxIcon.Information);
    }
}
