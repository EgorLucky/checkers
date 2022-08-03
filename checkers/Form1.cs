using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Шашки
{
    public partial class Form1 : Form
    {
        Panel[,] Board;// массив из ячеек на доске
        List<Checker> red;// коллекция красных шашек
        List<Checker> blue;//синих шашек
        bool must_kill = false;//флаг, по которому определяется необходимость рубить противника 
        List<JToken> possible_moves = new List<JToken>();//возможные ходы для выбранной в данный момент шашки
        List<List<Panel>> possible_killing = new List<List<Panel>>();//коллекции возможных ходов для рубки противника, предназначенные для шашек, которые могут рубить
        List<Checker> who_must_eat=new List<Checker>();//шашки, которые могут рубить
        Checker selected_checker = null;//выбранная в данный момент шашка
        Panel selectedpanel = null;//ячейка, на которой находится выбранная в данный момент шашка
        string GameId;
        string FirstPlayerCode;
        string GameState;
        string AwaitableMove;
        private JToken BoardState;

        public Form1()
        {
            InitializeComponent();
        }

        Color blackCellColor = Color.Black;
        Color yellowCellColor = Color.FromArgb(255, 255, 192, 128);

        private async void Form1_Load(object sender, EventArgs e)
        {
           

            Board = new Panel[,]//внесение ячеек в массив для того, чтобы к ним можно было обращаться по индексу
                {
                {p00,p10,p20,p30,p40,p50,p60,p70},{p01,p11,p21,p31,p41,p51,p61,p71},
                {p02,p12,p22,p32,p42,p52,p62,p72},{p03,p13,p23,p33,p43,p53,p63,p73},
                {p04,p14,p24,p34,p44,p54,p64,p74},{p05,p15,p25,p35,p45,p55,p65,p75},
                {p06,p16,p26,p36,p46,p56,p66,p76},{p07,p17,p27,p37,p47,p57,p67,p77}
                };

            

            foreach(var p in Board)
            {
                p.Controls.Clear();
                p.BackColor = Panel.DefaultBackColor;
            }

            var game = await BackendService.GameCreateWithBot();
            GameId = game["id"];
            FirstPlayerCode = game["firstPlayerCode"];
            //check second player (opponent) registered
            var opponentIsRegistred = false;

            while (opponentIsRegistred == false)
            { 
                await GetState();

                opponentIsRegistred = GameState == "AllPlayersRegistred";
                if (opponentIsRegistred == false)
                    await Task.Delay(100);
            }
            //start game
            var startRequestResponse = await BackendService.GameStartWithBot(FirstPlayerCode);

            var board = startRequestResponse["boardState"]["board"];

            CheckersCreating(board);

            AwaitableMove = startRequestResponse["awaitableMove"].ToString();

            //check who moves
            if (AwaitableMove == "SecondPlayer")
            {
                //awaiting opponent move
                await WaitForOpponentMove();
                //Opponent(); 
            }
            else
            {
                BoardState = startRequestResponse["boardState"];
                this.Text = "Waiting for your move";
            }
        }

        private async Task WaitForOpponentMove()
        {
            var opponentMoved = false;
            this.Text = "Waiting for opponent move";

            while (opponentMoved == false)
            {
                await GetState();

                opponentMoved = AwaitableMove == "FirstPlayer";
                if (opponentMoved == false)
                    await Task.Delay(100);
            }

            foreach (Panel cell in Board)
            {
                cell.Controls.Clear();//очистка доски
            }

            CheckersCreating(BoardState["board"]);

            this.Text = "Waiting for your move";
        }

        async Task GetState() 
        {
            var gameInfo = await BackendService.GameGetInfo(GameId);
            GameState = gameInfo["state"];
            AwaitableMove = gameInfo["awaitableMove"];

            if(!string.IsNullOrEmpty(gameInfo["boardState"]))
                BoardState = JObject.Parse(gameInfo["boardState"]);
            else
            {

            }
        }

        static string[] horizontals = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };
        static string[] verticals = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight" };

        private void CheckersCreating(Newtonsoft.Json.Linq.JToken board)//инициализация шашек
        {
            red = new List<Checker>();
            blue = new List<Checker>();

            var redPictureBoxes = new Stack<PictureBox>(new List<PictureBox>() 
            {
                pictureBox1, pictureBox2, pictureBox3, pictureBox4, pictureBox5, pictureBox6, pictureBox7, pictureBox8, pictureBox9, pictureBox10, pictureBox11, pictureBox12      
            });

            var bluePictureBoxes = new Stack<PictureBox>(new List<PictureBox>()
            {
                pictureBox13, pictureBox14, pictureBox15, pictureBox16, pictureBox17, pictureBox18, pictureBox19, pictureBox20, pictureBox21, pictureBox22, pictureBox23, pictureBox24
            });

            foreach (var cell in board["cells"])
            {
                var i = Array.IndexOf(horizontals, cell["coordinate"]["horizontal"].ToString());
                var j = Array.IndexOf(verticals, cell["coordinate"]["vertical"].ToString());
                var panel = Board[i, j];
                if(cell["checker"].Type != Newtonsoft.Json.Linq.JTokenType.Null)
                {
                    //create checker
                    var checkerFromCell = cell["checker"];
                    if (checkerFromCell["color"].ToObject<string>() == "#FFFFFF")
                    {
                        var pictureBox = redPictureBoxes.Pop();
                        panel.Controls.Add(pictureBox);
                        red.Add(new Checker(Board, pictureBox, 0)); 
                    }
                    if (checkerFromCell["color"].ToObject<string>() == "#000000")
                    {
                        var pictureBox = bluePictureBoxes.Pop();
                        panel.Controls.Add(pictureBox);
                        blue.Add(new Checker(Board, pictureBox, 1));
                    }
                }

                var cellColor = cell["color"].ToString();

                if (cellColor == "#000000")
                    panel.BackColor = blackCellColor;
                else panel.BackColor = yellowCellColor;
            }
        }
        private async void panel1_Click(object sender, EventArgs e)// ход на выбранную ячейку
        {
            Panel s = (Panel)sender;

            (var x, var y) = GetBoardPanelIndex(s);

            var horizontal = horizontals[x];
            var vertical = verticals[y];

            var moveVector = possible_moves
                .Where(pm => pm["moveVector"]["to"]["horizontal"].ToString() == horizontal &&
                             pm["moveVector"]["to"]["vertical"].ToString() == vertical)
                .Select(pm => pm["moveVector"])
                .FirstOrDefault();

            if (selected_checker != null && moveVector != null) //если шашка выбрана, и либо ячейка, на которую хочет сходить игрок, имеется в коллекции возможных ходов,
            {
                var gameMoveWithBotResult = await BackendService.GameMoveWithBot(FirstPlayerCode, moveVector);

                if (gameMoveWithBotResult["success"].ToString() != "True")
                {
                    MessageBox.Show($"Ошибка: {gameMoveWithBotResult["message"]}");
                }

                if (gameMoveWithBotResult["newBoardState"] != null && gameMoveWithBotResult["newBoardState"].Type != JTokenType.Null)
                {
                    BoardState = gameMoveWithBotResult["newBoardState"];
                    CheckersCreating(BoardState["board"]);
                }

                if (gameMoveWithBotResult["awaitableMove"].ToString() == "FirstPlayer")
                {
                    Text = "Waiting for your move";
                }
                else
                {
                    await WaitForOpponentMove();
                }
            }
        }


        private void pictureBox12_Click(object sender, EventArgs e)//выбор пользователем шашки
        {
            selected_checker = (from u in blue where u.ch == sender select u).First(); //выборка шашки, которая соответствует изображению, на которое нажал пользователь
            if (selectedpanel != null || selectedpanel == sender)
                selectedpanel.BorderStyle = BorderStyle.None;
            selectedpanel = ((Panel)selected_checker.ch.Parent);
            selectedpanel.BorderStyle = BorderStyle.Fixed3D;
            possible_moves.Clear();

            //search selectedpanel coordinates
            (var x, var y) = GetBoardPanelIndex(selectedpanel);

            var horizontal = horizontals[x];
            var vertical = verticals[y];

            //get possible moves where moveFrom == selectedpanel coordinates
            possible_moves = BoardState["possibleMoves"]
                .Where(pm => pm["moveVector"]["from"]["horizontal"].ToString() == horizontal
                    && pm["moveVector"]["from"]["vertical"].ToString() == vertical)
                .ToList();
            //if (must_kill == false)
            //{
            //    possible_moves =  selected_checker.FindPossibleMoves();//поиск возможных ходов для данной шашки
            //}
        }

        (int, int) GetBoardPanelIndex(Panel panel)
        {
            for(int i = 0; i < Board.GetLength(0); i++)
                for(int j = 0; j < Board.GetLength(1); j++)
                    if(Board[i, j] == panel) return (i, j);

            throw new Exception();
        }


        private void toolStripButton1_Click(object sender, EventArgs e)//Начать новую игру
        {
            foreach(Panel cell in Board)
            {
                cell.Controls.Clear();//очистка доски
            }
            CheckersCreating(null);//создание шашек
            int c = 0;
            for(int i =0;i<3;i++)//размещение красных шашек
            {
                for (int j = 0; j < 8; j++)
                    if (Board[j, i].BackColor != Color.Black)
                    {
                        Board[j, i].Controls.Add(red[c].ch);
                        red[c].ch.Image = Properties.Resources.RedCheck;
                        c++;
                    }
            }
            c = 0;
            for (int i = 7; i >4; i--)//размещение синих шашек
            {
                for (int j = 0; j < 8; j++)
                    if (Board[j,i].BackColor !=Color.Black)
                    {
                        Board[j, i].Controls.Add(blue[c].ch);
                        blue[c].ch.Image = Properties.Resources.bluecheck;
                        c++;
                    }
            }
            must_kill = false;
            who_must_eat.Clear();
            possible_killing.Clear();
            //if ((new Random()).Next(2) % 2 == 0)
                //Opponent();
        }
    }
    public class Checker// класс шашки
    {
        Panel[,] Board; //доска
        enum Col { Red, Blue };
        Col color;//цвет
        public PictureBox ch;//изображение на доске
        public Panel location { get { return (Panel)ch.Parent; } }//ячейка, на которой находится шашка
        public bool queen = false;//является ли шашка дамкой?
        public Checker(Panel[,] b, PictureBox c, int col)
        {
            ch = c;
            color = (Col)col;
            Board = b;
        }
    }
}
