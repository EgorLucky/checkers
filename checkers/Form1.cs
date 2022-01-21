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
        List<Panel> possible_moves = new List<Panel>();//возможные ходы для выбранной в данный момент шашки
        List<List<Panel>> possible_killing = new List<List<Panel>>();//коллекции возможных ходов для рубки противника, предназначенные для шашек, которые могут рубить
        List<Checker> who_must_eat=new List<Checker>();//шашки, которые могут рубить
        Checker selected_checker = null;//выбранная в данный момент шашка
        Panel selectedpanel = null;//ячейка, на которой находится выбранная в данный момент шашка
        string GameId;
        string FirstPlayerCode;
        string GameState;
        string AwaitableMove;
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            Board = new Panel[,]//внесение ячеек в массив для того, чтобы к ним можно было обращаться по индексу
                {
                {p00,p10,p20,p30,p40,p50,p60,p70},{p01,p11,p21,p31,p41,p51,p61,p71},
                {p02,p12,p22,p32,p42,p52,p62,p72},{p03,p13,p23,p33,p43,p53,p63,p73},
                {p04,p14,p24,p34,p44,p54,p64,p74},{p05,p15,p25,p35,p45,p55,p65,p75},
                {p06,p16,p26,p36,p46,p56,p66,p76},{p07,p17,p27,p37,p47,p57,p67,p77}
                };
            CheckersCreating();

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
            //check who moves

            if (AwaitableMove == "SecondPlayer")
                Opponent();
        }

        async Task GetState() 
        {
            var gameInfo = await BackendService.GameGetInfo(GameId);
            GameState = gameInfo["state"];
            AwaitableMove = gameInfo["awaitableMove"];
        }

        private void CheckersCreating()//инициализация шашек
        {
            red = new List<Checker>
            {
                new Checker(Board,pictureBox1,0), new Checker(Board,pictureBox2,0), new Checker(Board,pictureBox3,0),
                new Checker(Board,pictureBox4,0),new Checker(Board,pictureBox5,0),new Checker(Board,pictureBox6,0),
                new Checker(Board,pictureBox7,0),new Checker(Board,pictureBox8,0),new Checker(Board,pictureBox9,0),
                new Checker(Board,pictureBox10,0),new Checker(Board,pictureBox11,0),new Checker(Board,pictureBox12,0)
            };
            blue = new List<Checker>
            {
                new Checker(Board,pictureBox13,1), new Checker(Board,pictureBox14,1), new Checker(Board,pictureBox15,1),
                new Checker(Board,pictureBox16,1),new Checker(Board,pictureBox17,1),new Checker(Board,pictureBox18,1),
                new Checker(Board,pictureBox19,1),new Checker(Board,pictureBox20,1),new Checker(Board,pictureBox21,1),
                new Checker(Board,pictureBox22,1),new Checker(Board,pictureBox23,1),new Checker(Board,pictureBox24,1)
            };
        }
        private void panel1_Click(object sender, EventArgs e)// ход на выбранную ячейку
        {
            Panel s = (Panel)sender;
            if (selected_checker != null&&(possible_moves.Contains(s) || //если шашка выбрана, и либо ячейка, на которую хочет сходить игрок, имеется в коллекции возможных ходов,
                who_must_eat.Contains(selected_checker)//либо выбранная шашка должна рубить 
                    && possible_killing[who_must_eat.IndexOf(selected_checker)].Contains(s)))//и выбранная ячейка входит в коллекцию ходов для рубления
            {
                if (possible_moves.Contains(s) && !must_kill)//если нужно сделать простой ход
                {
                    selected_checker.Move(s);//перемещение шашки
                    selectedpanel.BorderStyle = BorderStyle.None;
                    selectedpanel = null;
                }
                if (must_kill && who_must_eat.Contains(selected_checker)
                    && possible_killing[who_must_eat.IndexOf(selected_checker)].Contains(s))//если нужно рубить
                {
                    selectedpanel.BorderStyle = BorderStyle.None;
                    Killing(selected_checker, blue,s);//рубка
                    if (CheckCheckerOnEating(selected_checker, red) == true)//проверка на то, должна ли шашка после рубки, еще раз рубить
                    {
                        must_kill = true;
                        who_must_eat = new List<Checker> { selected_checker };
                    }
                }
                bool place_to_move_exists = false;
                foreach (Checker ch in blue)
                    if (ch.FindPossibleMoves().Count != 0)
                        place_to_move_exists = true;

                if (blue.Count != 0)//если шашки противника не кончились
                {
                    if (!must_kill) //если рубить не нужно
                    {
                        CheckOnEating(blue);
                        if (!place_to_move_exists&&!must_kill) //если больше нет ходов
                        {
                            MessageBox.Show("Вы выиграли!");
                            toolStripButton1_Click(this, new EventArgs());
                        }
                        else
                        { 
                            Opponent();// ход компьютера
                            CheckOnEating(red);//проверка на необходимость игроком рубления противника после его хода
                        }
                    }
                    
                }
                else//если шашки компьютера закончились
                {
                    MessageBox.Show("Вы выиграли!");
                    toolStripButton1_Click(this, new EventArgs());
                }
            }
        }
        private void Killing(Checker killer,List<Checker> opponents ,Panel newposition)//рубка
        {
            var o = killer.Coordinate();
            killer.Move(newposition);
            var n = killer.Coordinate();
            if (killer.queen == true)//рубка, если выбранная шашка является дамкой
            {
                int sx = Math.Sign(n[0] - o[0]), sy = Math.Sign(n[1] - o[1]);
                for (int i = o[0], j = o[1]; i != n[0] && j != n[1]; i += sx, j += sy)
                    if (Board[i, j].Controls.Count != 0)
                    {
                        opponents.Remove((from u in opponents where u.ch == Board[i, j].Controls[0] select u).First());
                        Board[i, j].Controls.Clear();
                        break;
                    }
            }
            else// рубка, если шашка не является дамкой
            {
                opponents.Remove((from u in opponents where u.ch == Board[(o[0] + n[0]) / 2, ((o[1] + n[1]) / 2)].Controls[0] select u).First());
                Board[(o[0] + n[0]) / 2, ((o[1] + n[1]) / 2)].Controls.Clear();
            }
            must_kill = false;
            who_must_eat.Clear();
            possible_killing.Clear();
        }
        public bool CheckCheckerOnEating(Checker ch, List<Checker> team)//проверка шашки на то, должна ли она есть?
        {
            List<Panel> w = new List<Panel>();//возможные ходы для рубки для проверяемой шашки
            var co = ch.Coordinate();
            int i = co[0], j = co[1];
            bool a = false, b = false, c = false, d = false;
            if (ch.queen == true)//если шашка дамка
            {
                if(i > 1 && j > 1)//проверка влево вниз
                {
                    for(int I=i-1,J=j-1;I>0&&J>0;I--,J--)
                    {
                        if (Board[I, J].Controls.Count != 0)
                        {
                            if (a=(from u in team where u.ch == Board[I, J].Controls[0] select u).Count() == 0
                            && I != 0 && J != 0 && Board[--I, --J].Controls.Count == 0)
                            {
                                w.Add(Board[I, J]);
                                for(I=I-1,J=J-1; I >= 0 && J >= 0&&Board[I,J].Controls.Count==0;I--,J--)
                                {
                                    w.Add(Board[I, J]);
                                }
                            }
                            break;
                        }
                    }
                }
                if (i > 1 && j <= 5)//проверка влево вверх
                {
                    for (int I = i - 1, J = j + 1; I > 0 && J < 7; I--, J++)
                    {
                        if (Board[I, J].Controls.Count != 0)
                        {
                            if (b = (from u in team where u.ch == Board[I, J].Controls[0] select u).Count() == 0
                            && I != 0 && J != 7 && Board[--I, ++J].Controls.Count == 0)
                            {
                                w.Add(Board[I, J]);
                                for (I = I - 1, J = J + 1; I > 0 && J <= 7 && Board[I, J].Controls.Count == 0; I--, J++)
                                {
                                    w.Add(Board[I, J]);
                                }
                            }
                            break;
                        }
                    }
                }
                if (i <= 5 && j <= 5)//проверка вправо вверх
                {
                    for (int I = i + 1, J = j + 1; I < 7 && J < 7; I++, J++)
                    {
                        if (Board[I, J].Controls.Count != 0)
                        {
                            if (c = (from u in team where u.ch == Board[I, J].Controls[0] select u).Count() == 0
                            && I != 7 && J != 7 && Board[++I, ++J].Controls.Count == 0)
                            {
                                w.Add(Board[I, J]);
                                for (I = I + 1, J = J + 1; I <= 7 && J <= 7 && Board[I, J].Controls.Count == 0; I++, J++)
                                {
                                    w.Add(Board[I, J]);
                                }
                            }
                            break;
                        }
                    }
                }
                if (i <= 5 && j > 1)//проверка вправо вниз
                {
                    for (int I = i + 1, J = j - 1; I < 7 && J >0; I++, J--)
                    {
                        if (Board[I, J].Controls.Count != 0)
                        {
                            if (d = (from u in team where u.ch == Board[I, J].Controls[0] select u).Count() == 0
                            && I != 7 && J != 0 && Board[++I, --J].Controls.Count == 0)
                            {
                                w.Add(Board[I, J]);
                                for (I = I + 1, J = J - 1; I <= 7 && J >=0 && Board[I, J].Controls.Count == 0; I++, J--)
                                {
                                    w.Add(Board[I, J]);
                                }
                            }
                            break;
                        }
                    }
                }
            }
            else//проверка если шашка не является дамкой
            {
                a = (i > 1 && j > 1) && Board[i - 1, j - 1].Controls.Count > 0 &&
                        (from u in team where u.ch == Board[i - 1, j - 1].Controls[0] select u).Count() == 0
                        && Board[i - 2, j - 2].Controls.Count == 0;
                if (a) w.Add(Board[i - 2, j - 2]);
                b = (i > 1 && j <= 5) && Board[i - 1, j + 1].Controls.Count > 0 &&
                        (from u in team where u.ch == Board[i - 1, j + 1].Controls[0] select u).Count() == 0
                        && Board[i - 2, j + 2].Controls.Count == 0;
                if (b) w.Add(Board[i - 2, j + 2]);
                c = (i <= 5 && j <= 5) && Board[i + 1, j + 1].Controls.Count > 0 &&
                        (from u in team where u.ch == Board[i + 1, j + 1].Controls[0] select u).Count() == 0
                        && Board[i + 2, j + 2].Controls.Count == 0;
                if (c) w.Add(Board[i + 2, j + 2]);
                d = (i <= 5 && j > 1) && Board[i + 1, j - 1].Controls.Count > 0 &&
                        (from u in team where u.ch == Board[i + 1, j - 1].Controls[0] select u).Count() == 0
                        && Board[i + 2, j - 2].Controls.Count == 0;
                if (d) w.Add(Board[i + 2, j - 2]);
                
            }
            if (a || b || c || d) //если ходы для рубки были найдены
            {
                if(!who_must_eat.Contains(ch))
                who_must_eat.Add(ch);//шашка добавляется в число тех, кто может рубить
                possible_killing.Add(w);//найденные ходы добавляются в коллекцию ходов для рубки
                return true;
            }
            else return false;
        }

        public void CheckOnEating(List<Checker> team)//проверка всех шашек с одной команды на необходимость рубки
        {
            who_must_eat.Clear();
            possible_killing.Clear();
            foreach (Checker ch in team)
            {
                if (CheckCheckerOnEating(ch, team) == true)//если хоть одна шашка из команды может рубить
                    must_kill = true;//то это значит, что надо обязательно рубить
            }
        }
        public void Opponent()//ход компьютера
        {
            CheckOnEating(blue);//проверка, должен ли компьютер рубить
            Checker checker_selected=null;//выбираемая компьютером шашка
            Panel to=null;//выбираемое место для хода
                if (!must_kill)//если рубить не нужно
                {
                    bool[] no_ways = new bool[blue.Count];
                    for (int i = 0; i < no_ways.Length; i++) no_ways[i] = false;
                    while (to == null)//выбор шашки и хода
                    {
                        checker_selected = blue[(new Random()).Next(blue.Count)];
                        var pos_moves = checker_selected.FindPossibleMoves();
                        if (pos_moves.Count != 0)
                            to = pos_moves[(new Random()).Next(pos_moves.Count)];
                        else no_ways[Array.IndexOf(blue.ToArray(), checker_selected)] = true;
                        bool no_way_for_all = true;
                        foreach (bool n in no_ways) if (n == false) no_way_for_all = false;
                        if (no_way_for_all) break;
                    }
                    if (to != null)
                        checker_selected.Move(to);//ход
                }
                else//если надо рубить
                {
                    checker_selected = who_must_eat[(new Random()).Next(who_must_eat.Count)];//выбор шашки из числа тех, кто должен рубить
                    while (CheckCheckerOnEating(checker_selected, blue) == true)//пока выбранная шашка может рубить
                    {
                        int wmk = who_must_eat.IndexOf(checker_selected);
                        int r = (new Random()).Next(possible_killing[who_must_eat.IndexOf(checker_selected)].Count);
                        to = possible_killing[wmk][r];//выбор хода
                        Killing(checker_selected, red,to);
                    }
                }
            CheckOnEating(red);//проверка красных шашек на необходимость рубки
            bool place_to_move_exists = false;
            foreach (Checker ch in red)
                if (ch.FindPossibleMoves().Count != 0)
                    place_to_move_exists = true;
            if (red.Count == 0||!place_to_move_exists&&!must_kill)//если шашки игрока закончились или больше нет ходов
            {
                MessageBox.Show("Вы проиграли.");
                toolStripButton1_Click(this, new EventArgs());
            }
            must_kill = false;
        }
        private void pictureBox12_Click(object sender, EventArgs e)//выбор пользователем шашки
        {
            selected_checker = (from u in red where u.ch == sender select u).First(); //выборка шашки, которая соответствует изображению, на которое нажал пользователь
            if (selectedpanel != null || selectedpanel == sender)
                selectedpanel.BorderStyle = BorderStyle.None;
            selectedpanel = ((Panel)selected_checker.ch.Parent);
            selectedpanel.BorderStyle = BorderStyle.Fixed3D;
            possible_moves.Clear();
            if (must_kill == false)
            {
                possible_moves = selected_checker.FindPossibleMoves();//поиск возможных ходов для данной шашки
            }
        }
        
        private void toolStripButton1_Click(object sender, EventArgs e)//Начать новую игру
        {
            foreach(Panel cell in Board)
            {
                cell.Controls.Clear();//очистка доски
            }
            CheckersCreating();//создание шашек
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
            if ((new Random()).Next(2) % 2 == 0)
                Opponent();
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
        public List<int> Coordinate()//получение координатов шашки на доске
        {
            int i = 0, j = 0; bool s = true;
            for (i = 0; i <= Board.GetLength(0) - 1 && s; i++)
                for (j = 0; j <= Board.GetLength(1) - 1 && s; j++)
                    if (Board[i, j].Controls.Count > 0 && Board[i, j] == location)
                        s = false;
            return new List<int> { i - 1, j - 1 };
        }
        public List<Panel> FindPossibleMoves()//поиск возможных простых ходов (без рубки)
        {
            var co = Coordinate();
            int i = co[0], j = co[1];
            List<Panel> result = new List<Panel>();
            if (!queen)//если шашка дамка
            {
                if (color == Col.Red) //если цвет красный
                {
                    if ((i != 0 && j != Board.GetLength(1) - 1) && Board[i - 1, j + 1].Controls.Count == 0)
                        result.Add(Board[i - 1, j + 1]);
                    if ((i != Board.GetLength(0) - 1 && j != Board.GetLength(1) - 1) && Board[i + 1, j + 1].Controls.Count == 0)
                        result.Add(Board[i + 1, j + 1]);
                }
                else//если синий
                {
                    if ((i != 0 && j != 0) && Board[i - 1, j - 1].Controls.Count == 0)
                        result.Add(Board[i - 1, j - 1]);
                    if ((i != Board.GetLength(0) - 1 && j != 0) && Board[i + 1, j - 1].Controls.Count == 0)
                        result.Add(Board[i + 1, j - 1]);
                }
            }
            else//если шашка дамка
            {
                for (int I = i - 1, J = j + 1; I >= 0 && J < 8; I--, J++)//влево вверх
                {
                    if (Board[I, J].Controls.Count != 0)
                        break;
                    else
                        result.Add(Board[I, J]);
                }
                for (int I = i + 1, J = j + 1; I < 8 && J < 8; I++, J++)//вправо вверх
                {
                    if (Board[I, J].Controls.Count != 0)
                        break;
                    else
                        result.Add(Board[I, J]);
                }
                for (int I = i + 1, J = j - 1; I < 8 && J >= 0; I++, J--)//вправо вниз
                {
                    if (Board[I, J].Controls.Count != 0)
                        break;
                    else
                        result.Add(Board[I, J]);
                }
                for (int I = i - 1, J = j - 1; I >= 0 && J >= 0; I--, J--)//влево вниз
                {
                    if (Board[I, J].Controls.Count != 0)
                        break;
                    else
                        result.Add(Board[I, J]);
                }
            }
            return result;
        }
        public void Move(Panel to)// перенос шашки 
        {
            to.Controls.Add(ch);
            if (color == Col.Red && Coordinate()[1] == 7 && !queen)
            {
                queen = true;
                ch.Image = Properties.Resources.redqueen;
            }
            if (color == Col.Blue && Coordinate()[1] == 0 && !queen)
            {
                queen = true;
                ch.Image = Properties.Resources.bluequeen;
            }
        }
    }
}
