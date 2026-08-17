namespace MatchGame20135798;

public partial class MainPage : ContentPage
{
    // Temporizador que se ejecuta repetidamente para actualizar el cronometro en pantalla
    IDispatcherTimer timer;

    // Cuenta cuantas decimas de segundo han pasado desde que inicio la partida
    int tenthsOfSecondsElapsed;

    // Cuenta cuantos pares de emojis iguales ha encontrado el jugador
    int matchesFound;

    // Guarda una referencia al ultimo Label que el jugador toco, para poder compararlo con el siguiente
    Label lastLabelClicked;

    // Bandera que indica si estamos "esperando" el segundo toque para comparar un par
    bool findingMatch = false;

    // Constructor: se ejecuta una sola vez cuando se crea la pagina
    public MainPage()
    {
        InitializeComponent(); // Carga y conecta todos los controles definidos en el XAML

        timer = Dispatcher.CreateTimer(); // Crea el temporizador propio de MAUI (compatible con Android, iOS, Windows)
        timer.Interval = TimeSpan.FromSeconds(0.1); // El timer "hara tick" cada 0.1 segundos (una decima de segundo)
        timer.Tick += Timer_Tick; // Conecta el evento Tick con el metodo que se ejecutara en cada tick

        SetUpGame(); // Prepara el tablero por primera vez (reparte emojis y arranca el timer)
    }

    // Este metodo se ejecuta automaticamente cada 0.1 segundos mientras el timer este activo
    private void Timer_Tick(object? sender, EventArgs e)
    {
        tenthsOfSecondsElapsed++; // Suma 1 a la cuenta de decimas de segundo transcurridas

        // Convierte las decimas de segundo a segundos con un decimal (ej: 35 decimas -> "3.5") y lo muestra
        timeLabel.Text = (tenthsOfSecondsElapsed / 10.0).ToString("0.0") + "s";

        // Si ya se encontraron los 8 pares posibles, el juego termino
        if (matchesFound == 8)
        {
            timer.Stop(); // Detiene el cronometro, ya no debe seguir contando
            timeLabel.Text = timeLabel.Text + " - Play again?"; // Agrega un mensaje invitando a reiniciar
        }
    }

    // Prepara (o reinicia) el tablero de juego
    private void SetUpGame()
    {
        // Lista con 8 pares de emojis de animales (16 elementos en total, 2 de cada uno)
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

        // Objeto que genera numeros aleatorios, usado para desordenar los emojis
        Random random = new Random();

        // Recorre todos los Label que hay dentro del Grid principal (mainGrid)
        foreach (Label label in mainGrid.Children.OfType<Label>())
        {
            // Nos aseguramos de NO tocar el label del cronometro, solo las casillas de juego (las que dicen "?")
            if (label != timeLabel && label.Text == "?")
            {
                label.IsVisible = true; // Muestra la casilla (por si estaba oculta de una partida anterior)

                int index = random.Next(animalEmoji.Count); // Elige un indice aleatorio dentro de la lista de emojis restantes
                string nextEmoji = animalEmoji[index]; // Obtiene el emoji en ese indice aleatorio
                label.Text = nextEmoji; // Asigna ese emoji a la casilla actual

                animalEmoji.RemoveAt(index); // Elimina ese emoji de la lista para no repetirlo antes de tiempo
            }
        }

        timer.Start(); // Arranca (o reinicia) el cronometro
        tenthsOfSecondsElapsed = 0; // Reinicia el contador de tiempo a cero
        matchesFound = 0; // Reinicia el contador de pares encontrados a cero
    }

    // Se ejecuta cada vez que el jugador toca una casilla del tablero
    private void Label_Tapped(object? sender, EventArgs e)
    {
        // El "sender" es el control que disparo el evento; lo convertimos a Label para poder usar sus propiedades
        Label label = (Label)sender!;

        // Caso 1: es el PRIMER toque de este intento (no habia ninguna casilla "esperando pareja")
        if (findingMatch == false)
        {
            label.IsVisible = false; // Oculta el emoji tocado (lo "voltea")
            lastLabelClicked = label; // Guarda esta casilla como la ultima tocada, para compararla despues
            findingMatch = true; // Activa la bandera: ahora estamos esperando el segundo toque
        }
        // Caso 2: es el SEGUNDO toque, y el emoji coincide con el de la casilla anterior
        else if (label.Text == lastLabelClicked.Text)
        {
            matchesFound++; // Suma un acierto al contador de pares encontrados
            label.IsVisible = false; // Oculta tambien esta segunda casilla (par completado)
            findingMatch = false; // Apaga la bandera: el proximo toque sera un "primer toque" de nuevo
        }
        // Caso 3: es el SEGUNDO toque, pero el emoji NO coincide
        else
        {
            lastLabelClicked.IsVisible = true; // (Ver nota abajo) Vuelve a mostrar la primera casilla tocada
            findingMatch = false; // Apaga la bandera: el proximo toque sera un "primer toque" de nuevo
        }
    }

    // Se ejecuta cuando el jugador toca el Label del cronometro
    private void TimeLabel_Tapped(object? sender, EventArgs e)
    {
        // Solo permite reiniciar si el jugador YA gano la partida (encontro los 8 pares)
        if (matchesFound == 8)
        {
            SetUpGame(); // Vuelve a preparar el tablero: emojis nuevos, tiempo en cero, pares en cero
        }
    }
}