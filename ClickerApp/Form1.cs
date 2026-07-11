using System.Runtime.InteropServices;

namespace ClickerApp;

public partial class Form1 : Form
{
    private readonly System.Windows.Forms.Timer clickTimer = new();
    private readonly NumericUpDown cpsInput = new();
    private readonly Button toggleButton = new();
    private readonly Button stopButton = new();
    private readonly Label statusLabel = new();

    [DllImport("user32.dll")]
    private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint cButtons, UIntPtr dwExtraInfo);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    public Form1()
    {
        InitializeComponent();

        Text = "ClickerApp";
        Width = 420;
        Height = 260;
        StartPosition = FormStartPosition.CenterScreen;
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;

        var titleLabel = new Label
        {
            Text = "Clicker App",
            Font = new Font("Segoe UI", 16, FontStyle.Bold),
            AutoSize = true,
            Location = new Point(20, 20)
        };

        var descriptionLabel = new Label
        {
            Text = "Start the loop to send repeated left clicks at the current cursor position.",
            AutoSize = true,
            MaximumSize = new Size(360, 0),
            Location = new Point(20, 54)
        };

        var cpsLabel = new Label
        {
            Text = "Clicks per second:",
            AutoSize = true,
            Location = new Point(20, 106)
        };

        cpsInput.Location = new Point(150, 103);
        cpsInput.Size = new Size(90, 28);
        cpsInput.Minimum = 1;
        cpsInput.Maximum = 100;
        cpsInput.Value = 10;
        cpsInput.ValueChanged += (_, _) => UpdateTimerInterval();

        toggleButton.Text = "Start";
        toggleButton.Size = new Size(90, 34);
        toggleButton.Location = new Point(20, 148);
        toggleButton.Click += ToggleClicking;

        stopButton.Text = "Stop";
        stopButton.Size = new Size(90, 34);
        stopButton.Location = new Point(122, 148);
        stopButton.Enabled = false;
        stopButton.Click += (_, _) => StopClicking();

        statusLabel.Text = "Status: Idle";
        statusLabel.AutoSize = true;
        statusLabel.Location = new Point(20, 198);

        clickTimer.Tick += (_, _) => SendLeftClick();
        UpdateTimerInterval();

        Controls.Add(titleLabel);
        Controls.Add(descriptionLabel);
        Controls.Add(cpsLabel);
        Controls.Add(cpsInput);
        Controls.Add(toggleButton);
        Controls.Add(stopButton);
        Controls.Add(statusLabel);
    }

    private void ToggleClicking(object? sender, EventArgs e)
    {
        if (clickTimer.Enabled)
        {
            StopClicking();
            return;
        }

        clickTimer.Start();
        toggleButton.Text = "Running";
        stopButton.Enabled = true;
        statusLabel.Text = "Status: Clicking";
    }

    private void StopClicking()
    {
        clickTimer.Stop();
        toggleButton.Text = "Start";
        stopButton.Enabled = false;
        statusLabel.Text = "Status: Idle";
    }

    private void UpdateTimerInterval()
    {
        var cps = (int)Math.Max(1, cpsInput.Value);
        clickTimer.Interval = Math.Max(1, 1000 / cps);
    }

    private static void SendLeftClick()
    {
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
    }
}
