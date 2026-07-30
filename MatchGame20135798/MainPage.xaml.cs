namespace MatchGame20135798;

public partial class MainPage : ContentPage
{
    IDispatcherTimer timer;
    int tenthsOfSecondsElapsed;
    int matchesFound;

    Label lastLabelClicked;
    bool findingMatch = false;

    public MainPage()
    {
        InitializeComponent();

        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(0.1);
        timer.Tick += Timer_Tick;

        SetUpGame();
    }

    private void Timer_Tick(object? sender, EventArgs e)
    {
        tenthsOfSecondsElapsed++;
        timeLabel.Text = (tenthsOfSecondsElapsed / 10.0).ToString("0.0") + "s";

        if (matchesFound == 8)
        {
            timer.Stop();
            timeLabel.Text = timeLabel.Text + " - Play again?";
        }
    }

    private void SetUpGame()
    {
        List<string> animalEmoji = new List<string>()
        {
            "🐙", "🐙",
            "🐡", "🐡",
            "🐘", "🐘",
            "🐳", "🐳",
            "🐪", "🐪",
            "🦕", "🦕",
            "🦘", "🦘",
            "🦔", "🦔",
        };

        Random random = new Random();

        foreach (Label label in mainGrid.Children.OfType<Label>())
        {
            if (label != timeLabel && label.Text == "?")
            {
                label.IsVisible = true;
                int index = random.Next(animalEmoji.Count);
                string nextEmoji = animalEmoji[index];
                label.Text = nextEmoji;
                animalEmoji.RemoveAt(index);
            }
        }

        timer.Start();
        tenthsOfSecondsElapsed = 0;
        matchesFound = 0;
    }

    private void Label_Tapped(object? sender, EventArgs e)
    {
        Label label = (Label)sender!;

        if (findingMatch == false)
        {
            label.IsVisible = false;
            lastLabelClicked = label;
            findingMatch = true;
        }
        else if (label.Text == lastLabelClicked.Text)
        {
            matchesFound++;
            label.IsVisible = false;
            findingMatch = false;
        }
        else
        {
            lastLabelClicked.IsVisible = true;
            findingMatch = false;
        }
    }

    private void TimeLabel_Tapped(object? sender, EventArgs e)
    {
        if (matchesFound == 8)
        {
            SetUpGame();
        }
    }
}