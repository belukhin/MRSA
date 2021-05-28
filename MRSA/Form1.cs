using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;

namespace MRSA
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            k = PanelForDraw.CreateGraphics();
        }
        public static Graphics k;
        public static Random rand = new Random();
        public static Point[] P;
        public static List<Point> NP = new List<Point>();
        public string[] Chr;
        public static int CountOfShag;
        Stopwatch time = new Stopwatch();
        
        /// <summary>
        /// Генерирует точки и хромосомы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GeneratePoint_Click(object sender, EventArgs e)
        {
            try
            {
                timer1.Stop();
                time.Reset();
                CountOfShag = 0;
                ClearPanel_Click(sender, e);
                P = new Point[(int)CountOfPoints.Value];
                Chr = new string[(int)CountOfChr.Value];
                NP.Clear();
                textBox1.Text = CountOfShag.ToString();
                Generation.GeneratePoints(ref P);
                Generation.GenerateChromosome(ref Chr, (int)CountOfPoints.Value);
                Draw.DrawPoint(P);
              }
              catch 
              {
                  timer1.Stop();
                  MessageBox.Show(
                  "Возможные причины:\nНеверное количество точек\nДля работы алгоритма необходимо минимум 2 точки, максимум 200",
                  "Ошибка при генерации",
                  MessageBoxButtons.OK,
                  MessageBoxIcon.Error,
                  MessageBoxDefaultButton.Button1);
              }
        }

        /// <summary>
        /// Начало алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Start_Click(object sender, EventArgs e)
        {
            try
            {
                if (NP.Count > 0)//Если пользователь сам выбрал необходимые ему точки
                {
                    int i;
                List<Point> PS = new List<Point>();
                if (P != null)
                for (i = 0; i < P.Length; i++)
                        PS.Add(P[i]);
                for (i = 0; i < NP.Count; i++)
                    PS.Add(NP[i]);
                    P = new Point[PS.Count];
                CountOfPoints.Value = PS.Count;
                    Chr = new string[(int)CountOfChr.Value];
                    Generation.GenerateChromosome(ref Chr, PS.Count);
                    for (i = 0; i < PS.Count; i++)
                        P[i] = PS[i];
                NP.Clear();
                }
                textBox6.Text = $"{Generation.GenetateKrusckal(P):F2}";
                timer1.Start();
                time.Start();
            }
            catch
            {
                timer1.Stop();
                MessageBox.Show(
                "Возможные причины:\nНеверное количество точек\nДля работы алгоритма необходимо минимум 2 точки, максимум 200",
                "Ошибка при запуске",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Таймер для повтора шагов алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                TimeSpan ts = time.Elapsed;
                List<string> DopChr = new List<string>();
                string[] DopChrom = new string[1];
                textBox4.Text = $"{ts.Minutes}:{ts.Seconds}:{ts.Milliseconds}";
                timer1.Interval = 1;
                textBox1.Text = (++CountOfShag).ToString();
                Generation.GenerateChromosome(ref DopChrom, (int)CountOfPoints.Value);
                DopChr.Add(DopChrom[0]);
                string BestChrom = "";
                for (int i = 0; i < Chr[0].Length; i++)
                    if (Chr[0][i] != '+')
                        BestChrom += ("(" + (Chr[0][i] - 96) + ")");
                    else
                        BestChrom += Chr[0][i];
                textBox2.Text = BestChrom;
                textBox3.Text = $"{Tree.Len(Chr[0], P):F2}";
                if (CountOfShag % 10 == 0 || CountOfShag == 1)
                {
                    Draw.ReadChr(Chr[0], P);
                }
                int CountOfCross = 0;
                string Cr1;
                string Cr2;
                do
                {
                    int i1 = rand.Next((int)CountOfChr.Value);
                    int i2 = rand.Next((int)CountOfChr.Value);
                    int x = rand.Next(100);
                    if (x > 100 - PrOfCross.Value)
                    {
                        Cr1 = "";
                        Cr2 = "";
                        Crossover.Crossovering(Chr[i1], Chr[i2], ref Cr1, ref Cr2);
                        x = rand.Next(100);
                        if (x > 100 - PrOfMutate.Value)
                        {
                            Mutate.MutateChr(ref Cr1);
                        }
                        x = rand.Next(100);
                        if (x > 100 - PrOfMutate.Value)
                        {
                            Mutate.MutateChr(ref Cr2);
                        }
                        DopChr.Add(Cr1);
                        DopChr.Add(Cr2);
                    }
                    CountOfCross++;
                }
                while (CountOfCross < CountOfChr.Value);
                Tree.Select(ref Chr, DopChr);
                if (CountOfShag == CountOfIt.Value)
                    timer1.Stop();
            }
            catch 
            {
                timer1.Stop();
                MessageBox.Show(
                "Возможные причины:\nНеверное количество точек\nДля работы алгоритма необходимо минимум 2 точки, максимум 200",
                "Ошибка при работе алгоритма",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Приостанавливает работу алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Stop_Click(object sender, EventArgs e)
        {
            timer1.Stop();
            time.Stop();
        }

        /// <summary>
        /// Сохраняет данные о работе алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Save_Click(object sender, EventArgs e)
        {
            try
            {
                Stop_Click(sender, e);
                if (time.Elapsed.Milliseconds != 0)
                {
                    Tree.Inf(Chr[0], P);
                }
                else
                {
                    MessageBox.Show(
                    "Данных нету",
                    "Информация",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information,
                    MessageBoxDefaultButton.Button1);
                }
            }
            catch 
            {
                MessageBox.Show(
                "Возможные причины:\nНеверно указан путь к файлу",
                "Ошибка при сохранении",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);
            }
        }

        /// <summary>
        /// Закрывает программу
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void выходToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        /// <summary>
        /// Устанавливает значения, которые удобны для показа работы алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void установитьЗначенияПоУмолчаниюToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (CountOfShag > 1)
            {
                Save_Click(sender, e);
                k.Clear(Color.White);
            }
            Stop_Click(sender, e);
            CountOfPoints.Value = 20;
            CountOfChr.Value = 13;
            PrOfCross.Value = 80;
            PrOfMutate.Value = 5;
            CountOfIt.Value = 1000;
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
        }

        /// <summary>
        /// Клавиша в меню для генерации
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void генерацияToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneratePoint_Click(sender, e);
        }

        /// <summary>
        /// Клавиша в меню для запуска алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void запускАлгоритмаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Start_Click(sender, e);
        }

        /// <summary>
        /// Клавиша в меню для паузы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void паузаToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Stop_Click(sender, e);
        }

        /// <summary>
        /// Клавиша в меню для сохранения
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void сохранитьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save_Click(sender, e);
        }

        /// <summary>
        /// Открытие файла
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog a = new OpenFileDialog();
                a.Filter = "tsp files (*.tsp)|*.tsp|All files (*.*)|*.*"; //только txt файлы 
                a.ShowDialog();
                textBox5.Text = a.FileName;
                string path = a.FileName;
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    string Chromosoma = "";
                    int kol = 0;
                    bool check = true;
                    while ((line = sr.ReadLine()) != "EOF")
                    {
                        if (line.IndexOf("TYPE : TOUR") > -1)
                            check = false;
                        if (line.IndexOf("TYPE : TSP") > -1)
                            ClearPanel_Click(sender, e);
                        if (check)
                        {
                            if (line[0] >= '0' && line[0] <= '9')
                            {
                                kol++;
                                string[] StrPoint = line.Split(' ');
                                NP.Add(new Point(int.Parse(StrPoint[1]), int.Parse(StrPoint[2])));
                            }
                        }
                        else
                        {
                            if ((line[0] >= '0' && line[0] <= '9') || (line[0] == '+'))
                                if (line[0] == '+')
                                    Chromosoma += line;
                                else
                                    Chromosoma += ((char)(int.Parse(line) + 96));
                        }
                    }
                    
                    if (kol > 200)
                        throw new Exception();
                    else
                    {
                        if (check)
                        {
                            CountOfPoints.Value = kol;
                            P = new Point[NP.Count];
                            Chr = new string[(int)CountOfChr.Value];
                            for (int i = 0; i < NP.Count; i++)
                                if (Chromosoma == "")
                                    P[i] = new Point(NP[i].X, NP[i].Y);
                                else
                                    P[i] = new Point(NP[i].X, NP[i].Y);
                            Generation.GenerateChromosome(ref Chr, NP.Count);
                            Draw.DrawPoint(P);
                        }
                        else
                        {
                            if ((Chromosoma.Length + 1) / 2 != P.Length)
                                throw new Exception();
                            else
                            {
                                Draw.DrawPoint(P);
                                Draw.ReadChr(Chromosoma, P);
                            }
                        }
                        NP.Clear();
                    }
                }
            }
            catch 
            {
                MessageBox.Show(
                "Возможные причины:\nОткрыт файл TOUR прежде, чем TSP\nНеверно указан путь к файлу\nНевозможно прочитать файл\nНеверное количество точек\nДля работы алгоритма необходимо минимум 2 точки, максимум 200",
                "Ошибка при открытие файла",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);
            }
            
        }

        /// <summary>
        /// Рисование точек по клику мышки
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelForDraw_MouseClick(object sender, MouseEventArgs e)
        {
            try
            {
                Pen pen = new Pen(Color.Green, 2);
                Point D = e.Location;
                k.DrawEllipse(pen, D.X, D.Y, 2 * 2, 2 * 2);
                NP.Add(D);
            }
            catch
            {
                MessageBox.Show(
                "Возможные причины:\nДобавляли точки во время работы алгоритма\nНеверное количество точек\nДля работы алгоритма необходимо минимум 2 точки\nПревышен лимит количества точек\nМаксимум 200 точек",
                "Ошибка при рисовании точек",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error,
                MessageBoxDefaultButton.Button1);
            }
  

        }

        /// <summary>
        /// Очищает панель для рисования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearPanel_Click(object sender, EventArgs e)
        {
            if (CountOfShag > 1)
            {
                Save_Click(sender, e);
            }
            k.Clear(Color.White);
            P = new Point[0];
            textBox1.Text = "";
            CountOfShag = 0;
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            textBox6.Text = "";
            NP.Clear();
        }

        /// <summary>
        /// Возможность убрать настройки алгоритма
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox check = (CheckBox)sender;
            if (check.Checked)
                groupBox2.Visible = true;
            else
                groupBox2.Visible = false;
        }

        /// <summary>
        /// Клавиша в меню очищает панель для рисования
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void очиститьToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearPanel_Click(sender, e);
        }

        /// <summary>
        /// Показывает координаты панели
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PanelForDraw_MouseMove(object sender, MouseEventArgs e)
        {
            textBox7.Text = e.Location.X.ToString();
            textBox8.Text = e.Location.Y.ToString();
        }
    }
}
