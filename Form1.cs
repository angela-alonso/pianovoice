using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Speech.Recognition;
using System.Speech.Synthesis;
using System.IO;


namespace PianoVoice
{
    public partial class Form1 : Form
    {
        private System.Speech.Recognition.SpeechRecognitionEngine _recognizer =
        new SpeechRecognitionEngine();
        private SpeechSynthesizer synth = new SpeechSynthesizer();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            synth.Speak("Bienvenido al diseño de interfaces avanzadas. Inicializando la Aplicación");

            Grammar grammar = CreateGrammarBuilderMusic(null);
            _recognizer.SetInputToDefaultAudioDevice();
            _recognizer.UnloadAllGrammars();

            // Nivel de confianza del reconocimiento 70%
            _recognizer.UpdateRecognizerSetting("CFGConfidenceRejectionThreshold", 50);

            grammar.Enabled = true;
            _recognizer.LoadGrammar(grammar);
            _recognizer.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(_recognizer_SpeechRecognized);

            //reconocimiento asíncrono y múltiples veces
            _recognizer.RecognizeAsync(RecognizeMode.Multiple);
            synth.Speak("Aplicación preparada para reconocer su voz");
        }

        void _recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            //obtenemos un diccionario con los elementos semánticos
            SemanticValue semantics = e.Result.Semantics;
            RecognitionResult result = e.Result;
            SoundPlayer player;

            if (!semantics.ContainsKey("escalaMusical"))
            {
                synth.Speak("No info provided");
            }
            else
            {
                string notaReconocida = semantics["escalaMusical"].Value.ToString().ToLower(); // Obtener el valor semántico
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string soundFile = basePath;


                // Reproducir la nota basada en la entrada de voz
                switch (notaReconocida)
                {
                    case "do":
                        soundFile = Path.Combine(basePath, "Music", "do.wav");
                        this.notaDo.Visible = true;
                        break;
                    case "re":
                        soundFile = Path.Combine(basePath, "Music", "re.wav");
                        this.notaRe.Visible = true;
                        break;
                    case "mi":
                        soundFile = Path.Combine(basePath, "Music", "mi.wav");
                        this.notaMi.Visible = true;
                        break;
                    case "fa":
                        soundFile = Path.Combine(basePath, "Music", "fa.wav");
                        this.notaFa.Visible = true;
                        break;
                    case "sol":
                        soundFile = Path.Combine(basePath, "Music", "sol.wav");
                        this.notaSol.Visible = true;
                        break;
                    case "la":
                        soundFile = Path.Combine(basePath, "Music", "la.wav");
                        this.notaLa.Visible = true;
                        break;
                    case "si":
                        soundFile = Path.Combine(basePath, "Music", "si.wav");
                        this.notaSi.Visible = true;
                        break;
                }

                player = new SoundPlayer(soundFile);
                player.Play();

                Timer timer = new Timer();
                timer.Interval = 1500; // Duración del sonido en milisegundos
                timer.Tick += (s, args) =>
                {
                    this.Controls.OfType<Button>().ToList().ForEach(b => b.Visible = false);
                    timer.Stop(); // Detener el temporizador
                };
                timer.Start();

                Update();
            }
        }

        private Grammar CreateGrammarBuilderMusic(params int[] info)
        {
            //synth.Speak("Creando ahora la gramática");
            Choices notaChoice = new Choices();

            SemanticResultValue choiceResultValue;
            GrammarBuilder resultValueBuilder;

            choiceResultValue = new SemanticResultValue("Do", "do");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            notaChoice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Re", "re");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            notaChoice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Mi", "mi");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            notaChoice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Fa", "fa");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            notaChoice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Sol", "sol");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            notaChoice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("La", "la");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            notaChoice.Add(resultValueBuilder);

            choiceResultValue = new SemanticResultValue("Si", "si");
            resultValueBuilder = new GrammarBuilder(choiceResultValue);
            notaChoice.Add(resultValueBuilder);

            SemanticResultKey choiceResultKey = new SemanticResultKey("escalaMusical", notaChoice);
            GrammarBuilder notas = new GrammarBuilder(choiceResultKey);

            GrammarBuilder tocar = "Tocar";
            GrammarBuilder nota = "Nota";
            //GrammarBuilder acorde = "Acorde";
            //GrammarBuilder escala = "Escala";

            GrammarBuilder frase = tocar;
            frase.Append(notas);

            Grammar grammar = new Grammar(frase);
            grammar.Name = "Tocar Piano";

            return grammar;
        }
    }
}
