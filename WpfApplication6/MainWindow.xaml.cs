using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace WpfApplication6
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public class lvlList : IEquatable<lvlList>
    {
        public string Value { get; set; }
        public int ID { get; set; }
        public bool Custom { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SavedList objAsPart = obj as SavedList;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public bool Equals(lvlList other)
        {
            if (other == null) return false;
            return true;
        }
        public override int GetHashCode()
        {
            return 0;
        }
        public override string ToString()
        {
            return "Custom: " + Custom + " value: " + Value;
        }
    }
    public class SavedList : IEquatable<SavedList>
    {
        public int lvl { get; set; }
        public string Value { get; set; }
        public string Date { get; set; }
        public bool Custom { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            SavedList objAsPart = obj as SavedList;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public bool Equals(SavedList other)
        {
            if (other == null) return false;
            return (this.lvl.Equals(other.lvl));
        }
        public override int GetHashCode()
        {
            return lvl;
        }
        public override string ToString()
        {
            return "lvl: " + lvl + " Date: " + Date + "   Value: " + Value;
        }
    }
    public class Work
    {
        public int Thislvl = -1;
        Brush color;
        bool[,] coinc = new bool[9, 9];
        bool[,] otdix = new bool[9, 9];
        bool[,] start = new bool[9, 9];
        public List<lvlList> lines = new List<lvlList>();
        public List<SavedList> SaveList = new List<SavedList>();

        string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FesteloApp\Sudoku\data.xml";
        public void refreshLines()
        {
            lines = new List<lvlList>();
            XmlTextReader reader = new XmlTextReader(path);
            bool Custom = false;
            int i = 0;
            try
            {
                for (int found = 0; reader.Read();)
                {
                    if (found == 1) found = 2;
                    if (reader.IsStartElement("level"))
                    {
                        found = 1;
                        string s = reader.Value;
                        Custom = Convert.ToBoolean(reader.GetAttribute("Custom"));
                    }
                    if (found == 2 && reader.NodeType == XmlNodeType.Text)
                    {
                        lines.Add(new lvlList() { Value = reader.Value, Custom = Custom, ID = i });
                        found = 0;
                        i++;
                    }
                }


                lines.Sort(delegate (lvlList x, lvlList y)
                {
                    if (x.Custom == y.Custom) return 0;
                    else if (x.Custom == true) return 1;
                    else return x.Custom.CompareTo(y.Custom);
                });
                reader.Close();
                if (lines.Count == 0) { File.Delete(path); refreshLines(); }
            }
            catch
            {
                MessageBoxResult MSGresult = MessageBox.Show("Первый запуск, мы создадим нужные файлы.\nПуть: " + Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FesteloApp\Sudoku\", "Первый запуск");
                //File.Create(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FesteloApp\Sudoku\data.xml");
                XmlTextWriter textWritter = null;
                try { textWritter = new XmlTextWriter(path, Encoding.UTF8); }
                catch {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FesteloApp\Sudoku\");
                    textWritter = new XmlTextWriter(path, Encoding.UTF8);
                }
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("SudokuGameData");
                textWritter.WriteEndElement();
                textWritter.Close();


                XmlDocument document = new XmlDocument();
                document.Load(path);
                XmlNode element = document.CreateElement("SaveList");
                document.DocumentElement.AppendChild(element);

                XmlNode element1 = document.CreateElement("lvlTable");
                document.DocumentElement.AppendChild(element1);

                document.Save(path);
                ReloadLVL();
                refreshLines();
            }
        }
        public void kykarekalPetyx(TextBox sender, int hor, int ver, bool zeroing = false)
        {
            if (zeroing) ColorChange(ver, hor, 1); coinc[ver, hor] = false;
            if (sender.Text != "")
            {
                int[] COUB = { 0, 1, 2 };
                for (int i = 0; i < 9; i++)
                {
                    if (sender.Text == boxes[i, hor].Text)
                    {
                        if (boxes[i, hor] != sender)
                        {
                            coinc[ver, hor] = true;
                            coinc[i, hor] = true;
                            ColorChange(ver, hor, 2);
                            ColorChange(i, hor, 2);
                        }
                    }
                    //else coinc[ver, hor] = false; coinc[i, hor] = false;
                    if (sender.Text == boxes[ver, i].Text)
                    {
                        if (boxes[ver, i] != sender)
                        {
                            ColorChange(ver, hor, 2);
                            ColorChange(ver, i, 2);
                            coinc[ver, hor] = true;
                            coinc[ver, i] = true;
                        }
                    }
                    //else coinc[ver, hor] = false; coinc[ver, i] = false;
                }
                //
                double temp1 = ver / 3;
                double temp2 = hor / 3;
                int temp3 = COUB[(int)Math.Floor(temp1)] * 3;
                int temp4 = COUB[(int)Math.Floor(temp2)] * 3;
                for (int i = 0; i < 3; i++)
                {
                    for (int j = 0; j < 3; j++)
                    {
                        if (sender.Text == boxes[temp3 + i, temp4 + j].Text)
                        {
                            if (boxes[temp3 + i, temp4 + j] != sender)
                            {
                                ColorChange(temp3 + i, temp4 + j, 2);
                                ColorChange(ver, hor, 2);
                                coinc[temp3 + i, temp4 + j] = true;
                                coinc[ver, hor] = true;
                            }
                        }
                    }
                }
            }
        }
        public MainWindow win = null;
        public Window1 win1 = null;
        public void initBoxes(int window, bool edit = false)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    boxes[i, j] = new TextBox();
                    boxes[i, j].Height = 34;
                    boxes[i, j].Width = 50;
                    boxes[i, j].MaxLength = 1;
                    boxes[i, j].FontSize = 20;
                    boxes[i, j].TextAlignment = TextAlignment.Center;
                    boxes[i, j].TextChanged += TextChanged;
                    boxes[i, j].HorizontalAlignment = HorizontalAlignment.Left;
                    boxes[i, j].VerticalAlignment = VerticalAlignment.Top;
                    boxes[i, j].PreviewKeyDown += Work_KeyDown;
                    if (window == 0) boxes[i, j].MouseDoubleClick += DoubleClick;
                    double width = boxes[i, j].Width * j;
                    double height = boxes[i, j].Height * i;
                    if (width >= 150) width = width + 2;
                    if (width >= 300) width = width + 2;
                    if (height >= 102) height = height + 2;
                    if (height >= 204) height = height + 2;
                    Thickness mesto = new Thickness(width, height, 0, 0);
                    boxes[i, j].Margin = mesto;
                    if (window == 0)
                    {
                        win.gr.Children.Add(boxes[i, j]);
                    }
                    else if (window == 1)
                    {
                        win1.gr.Children.Add(boxes[i, j]);
                    }
                }
            }
            if (window == 1)
            {
                Button btn = new Button();
                btn.VerticalAlignment = VerticalAlignment.Top;
                btn.HorizontalAlignment = HorizontalAlignment.Left;
                btn.Margin = new Thickness(0, 310, 0, 0);
                btn.Width = 450;
                if (edit)
                {
                    Button btn2 = new Button();
                    btn2.VerticalAlignment = VerticalAlignment.Top;
                    btn2.HorizontalAlignment = HorizontalAlignment.Left;
                    btn2.Margin = new Thickness(380, 310, 0, 0);
                    btn2.Width = 70;
                    btn2.Height = 19;
                    btn2.Content = "DELETE";
                    btn.Width = 380;
                    btn2.Click += button2_Click;
                    win1.gr.Children.Add(btn2);
                }
                btn.Click += button_Click;
                btn.Height = 19;
                btn.Content = "SAVE";
                win1.gr.Children.Add(btn);
            }
            color = boxes[1, 1].BorderBrush;
        }

        private void Work_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Up || e.Key == Key.Left || e.Key == Key.Down || e.Key == Key.Right)
            {
                int ver = 0, hor = 0;
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if ((sender as TextBox) == boxes[i, j]) { ver = i; hor = j; break; }
                    }
                }
                int newcoord = 1;
                if (e.Key == Key.Up || e.Key == Key.Left) newcoord = -1;
                try
                {
                    if (e.Key == Key.Down || e.Key == Key.Up) { boxes[ver + newcoord, hor].Focus(); boxes[ver + newcoord, hor].CaretIndex = boxes[ver + newcoord, hor].Text.Length; if (!boxes[ver + newcoord, hor].IsReadOnly) boxes[ver + newcoord, hor].SelectAll(); }
                    else { boxes[ver, hor + newcoord].Focus(); boxes[ver, hor + newcoord].CaretIndex = boxes[ver, hor + newcoord].Text.Length; if (!boxes[ver, hor + newcoord].IsReadOnly) boxes[ver, hor + newcoord].SelectAll(); }
                }
                catch { return; }
                e.Handled = true;
            }
        }

        private void DoubleClick(object sender, MouseButtonEventArgs e)
        {
            int hor = 0;
            int ver = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (coinc[i, j])
                    {
                        kykarekalPetyx(boxes[i, j], j, i, true);
                    }
                    if (boxes[i, j] == (sender as TextBox))
                    {
                        ver = i;
                        hor = j;
                    }
                }
            }
            if (!start[ver, hor])
            {
                otdix[ver, hor] = !otdix[ver, hor];
                if (otdix[ver, hor])
                {
                    (sender as TextBox).FontSize = 13;
                    (sender as TextBox).MaxLength = 12;
                }
                else
                {
                    (sender as TextBox).FontSize = 20;
                    (sender as TextBox).MaxLength = 1;
                }
            (sender as TextBox).Text = "";
            }
        }
        public void lvlLoad(int lvl = -1, int wind = 0, int savelvl = -1)
        {
            refreshLines();
            RefreshLVLlist();
            int rnd;
            if (lvl == -1)
            {
                Random rndA = new Random();
                rnd = rndA.Next(0, lines.Count);
            }
            else rnd = lvl;
            Thislvl = rnd;
            if (wind == 0) win.Title = "Уровень: " + (rnd + 1) + "   || Судоку от профессионала. #Festelo";
            string[] Allline = lines[rnd].Value.Split('-');
            string[] Saveline = new string[9];
            if (savelvl != -1) { Saveline = SaveList[savelvl].Value.Split('-'); }
            for (int i = 0; i < 9; i++)
            {
                char[] lvlHome = Allline[i].ToCharArray();
                char[] home = lvlHome;
                char[] saveHome = new char[9];
                if (savelvl != -1) {home = Saveline[i].ToCharArray(); }
                for (int j = 0; j < 9; j++)
                {
                    if (Convert.ToString(home[j]) == "0")
                    {
                        boxes[i, j].Text = Convert.ToString(lvlHome[j]);
                        if (wind == 0)
                        {
                            boxes[i, j].IsReadOnly = true;
                            start[i, j] = true;
                            ColorChange(i, j, 3);
                        }
                    }
                    else if (Convert.ToString(home[j]) != " ")
                    {
                        boxes[i, j].Text = Convert.ToString(home[j]);
                        bool bl = true;
                        if (lvlHome[j] != home[j]) bl = false;
                        if (wind == 0)
                        {
                            boxes[i, j].IsReadOnly = bl;
                            start[i, j] = bl;
                            if(bl)ColorChange(i, j, 3);
                        }
                    }
                    else
                    {
                        boxes[i, j].Text = "";
                        if (wind == 0)
                        {
                            boxes[i, j].IsReadOnly = false;
                            start[i, j] = false;
                            boxes[i, j].Foreground = Brushes.Black;
                        }
                    }
                }
            }
        }
        private void TextChanged(object sender, TextChangedEventArgs e)
        {
            int hor = 0;
            int ver = 0;
            int final = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (coinc[i, j])
                    {
                        kykarekalPetyx(boxes[i, j], j, i, true);
                    }
                    if (boxes[i, j] == (sender as TextBox))
                    {
                        ver = i;
                        hor = j;
                    }
                    if (!coinc[i, j] && boxes[i, j].Text != "") { final++; }
                }
            }
            if (!otdix[ver, hor])
            {
                if ((sender as TextBox).Text != "")
                {
                    if ((Convert.ToChar((sender as TextBox).Text) < '1') || (Convert.ToChar((sender as TextBox).Text) > '9'))
                    {
                        (sender as TextBox).Text = "";
                        return;
                    }
                }
                kykarekalPetyx((sender as TextBox), hor, ver);
            }
            if (final == 81 && win1 == null) { MessageBox.Show("Молодец!");  }
            (sender as TextBox).SelectAll();
        }
        public void ColorChange(int ver, int hor, int G)
        {
            if (G == 1)
            {
                boxes[ver, hor].BorderBrush = color;
            }
            else if (G == 2)
            {
                boxes[ver, hor].BorderBrush = Brushes.Red;
            }
            else
            {
                boxes[ver, hor].Foreground = Brushes.DimGray;
            }
        }
        public void RefreshLVLlist(int mode = 0, int name = 0)
        {
            SaveList = new List<SavedList>();
            XmlTextReader reader = new XmlTextReader(path);
            try
            {
                int level = 0;
                string Date = "";
                for (int found = 0; reader.Read();)
                {
                    if (found == 1) found = 2;
                    if (reader.IsStartElement("save"))
                    {
                        int k;
                        if (int.TryParse(reader.GetAttribute("lvl"), out k)) { level = k; found = 1; }
                        Date = reader.GetAttribute("date");
                    }
                    if (found == 2 && reader.NodeType == XmlNodeType.Text)
                    {
                        SaveList.Add(new SavedList() { Value = reader.Value, lvl = level, Date = Date });
                        found -= 2;
                        level = 0;
                    }
                }
                reader.Close();
            }
            catch { MessageBox.Show("ERROR#404-0"); }
        }
        public void ReloadLVL()
        {
            string OnlinePath = @"https://rocld.com/mdqv";
            string LocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\FesteloApp\Sudoku\temp.txt";
            WebClient myWebClient = new WebClient();
            try { myWebClient.DownloadFile(OnlinePath, LocalPath); }
            catch { MessageBox.Show("Ошибка при загрузке файлов.","Загрузка"); Environment.Exit(0);}
            string[] da = File.ReadAllLines(LocalPath);

            XmlDocument document = new XmlDocument();
            document.Load(path);
            for(int i = 0; i< da.Length; i++)
            {
                bool norm = true;
                for (int j = 0; j < lines.Count(); j++) { if (lines[j].Value == da[i]) norm = false; }
                if (!norm) continue;
                XmlNode element = document.CreateElement("level");
                element.InnerText = da[i];
                XmlAttribute Custom = document.CreateAttribute("Custom");
                Custom.Value = "false";
                element.Attributes.Append(Custom);
                document.GetElementsByTagName("lvlTable").Item(0).AppendChild(element);
            }
            document.Save(path);
            File.Delete(LocalPath);
            try { win.lvlItem(); }
            catch { MessageBox.Show("ERROR#405-0"); }
        }
        public void SaveLVL(int mode = 0, string name = "")
        {
            XmlDocument document = new XmlDocument();
            try { document.Load(path); }
            catch { MessageBox.Show("ERROR#404-1"); return; }
            bool ERROR = false;
            string SaveLine = "";
            if (mode == 0 || mode == 1)
            {
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (coinc[i, j]) ERROR = true;
                        else
                        {
                            if (boxes[i, j].IsReadOnly) SaveLine += "0";
                            else if (boxes[i, j].Text == "") SaveLine += " ";
                            else SaveLine += boxes[i, j].Text;
                        }
                    }
                    if (i < 8) SaveLine += "-";
                }
            }
            if (ERROR == true) { MessageBox.Show("Найдена ошибка! Сохранение не выполнено!", "Save ERROR"); return; }
            if (mode == 0)
            {
                XmlNode element = document.CreateElement("save");
                XmlAttribute lvlAt = document.CreateAttribute("lvl"); // создаём атрибут
                XmlAttribute date = document.CreateAttribute("date"); // создаём атрибут
                lvlAt.Value = Convert.ToString(Thislvl);
                date.Value = Convert.ToString(DateTime.Now);
                element.Attributes.Append(lvlAt);
                element.Attributes.Append(date);
                element.InnerText = SaveLine;
                document.GetElementsByTagName("SaveList").Item(0).AppendChild(element);
            }
            if (mode==1 || mode==2)
            {
                int id = Convert.ToInt32(name);
                List<SavedList> filtared = new List<SavedList>(SaveList.Where(lvl => lvl.lvl == 1).ToList());
                for(int i=0; i < SaveList.Count-1; i++)
                {
                    if(filtared[id] == SaveList[i]) { id = i; break; }
                }
                XmlNodeList lvls = document.GetElementsByTagName("save");
                if (mode == 2) document.GetElementsByTagName("SaveList").Item(0).RemoveChild(lvls[id]);
                else
                {
                    lvls[id].InnerText = SaveLine;
                }
            }
            document.Save(path);
            if(mode == 0)MessageBox.Show("Уровень " + (Thislvl+1) + " успешно сохранен.\n\nDate: " + DateTime.Now, "Save Complete");
            else if(mode == 1)MessageBox.Show("Изменений в уроане " + (Thislvl + 1) + " успешно сохранены.\n\nDate: " + DateTime.Now, "Save Complete");
            else if (mode == 3) MessageBox.Show("Уровень " + (Thislvl + 1) + " успешно удален.\n\nDate: " + DateTime.Now, "Save Complete");
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            XmlDocument document = new XmlDocument();
            try { document.Load(path); }
            catch { MessageBox.Show("ERROR#404-3"); return; }
            XmlNodeList lvls = document.GetElementsByTagName("level");
            document.GetElementsByTagName("lvlTable").Item(0).RemoveChild(lvls[Thislvl]);
            document.Save(path);
            Window1.deleted = true;
            win1.Close();
        }
        private void button_Click(object sender, RoutedEventArgs e)
        {
            bool ERROR = false;
            string NewLine = "";
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (coinc[i, j]) ERROR = true;
                    else
                    {
                        if (boxes[i, j].Text == "") NewLine += " ";
                        else NewLine += boxes[i, j].Text;
                    }
                }
                if(i<8)NewLine += "-";
            }
            bool NewOld = false;
            if (!ERROR)
            {
                if (Thislvl != -1) NewOld = true;

                XmlDocument document = new XmlDocument();
                try { document.Load(path); }
                catch { MessageBox.Show("ERROR#404-2"); return; }
                if (!NewOld)
                {
                    XmlNode element = document.CreateElement("level");
                    element.InnerText = NewLine;
                    XmlAttribute Custom = document.CreateAttribute("Custom"); // создаём атрибут
                    Custom.Value = "true";
                    element.Attributes.Append(Custom);
                    document.GetElementsByTagName("lvlTable").Item(0).AppendChild(element);
                }
                else
                {
                    XmlNodeList lvls = document.GetElementsByTagName("level");
                    lvls[lines[Thislvl].ID].InnerText = NewLine;
                    lvls[lines[Thislvl].ID].Attributes.GetNamedItem("Custom").Value = "true";
                }
                document.Save(path);
                MessageBox.Show("Сохранение выполнено успешно!", "Save");
            }
            else MessageBox.Show("Найдена ошибка! Сохранение не выполнено!", "Save ERROR");
        }
        TextBox[,] boxes = new TextBox[9, 9];
    }
    public partial class MainWindow : Window
    {
        public Work work = new Work();
        public MainWindow()
        {
            InitializeComponent();
            work.win = this;
            work.initBoxes(0);
            work.lvlLoad();
            lvlItem();
        }
        public void lvlItem()
        {
            work.refreshLines();
            work.RefreshLVLlist();
            subMenu.Items.Clear();
            Save.Items.Clear();
            MenuItem[] itemLVL = new MenuItem[work.lines.Count];
            List<lvlList> linesFiltared = new List<lvlList>(work.lines.Where(Custom => Custom.Custom == true).ToList());
            bool norm = false;
            for (int i = 0; i < work.lines.Count; i++)
            {
                itemLVL[i] = new MenuItem { Name = "lvl" + (i + 1), Header = "Уровень " + (i + 1), Tag = i };
                itemLVL[i].Click += lvlClick;
                if (work.lines[i].Custom && !norm) { norm = true; subMenu.Items.Add(new Separator()); }
                subMenu.Items.Add(itemLVL[i]);
            }
            List<SavedList> filtared = new List<SavedList>(work.SaveList.Where(lvl => lvl.lvl == work.Thislvl).ToList());
            MenuItem edit = new MenuItem();
            MenuItem SaveNew = new MenuItem { Name = "SaveNew", Header = "New" };
            SaveNew.Click += MenuItem_Click;
            Save.Items.Add(SaveNew);
            if (filtared.Count != 0) { Save.Items.Add(edit = new MenuItem { Name = "edit", Header = "Edit" });}
            for (int i = 0; i < filtared.Count; i++)
            {
                MenuItem[] item = new MenuItem[3];
                item[0] = new MenuItem { Name="edit"+(i+1), Header = filtared[i].Date};
                item[1] = new MenuItem { Name = "repSave" + (i + 1), Header = "Replace", Tag = "0 " + i};
                item[1].Click += MenuItem_Click;
                item[2] = new MenuItem { Name = "delSave" + (i + 1), Header = "Remove", Tag = "1 " + i};
                item[2].Click += MenuItem_Click;
                edit.Items.Add(item[0]);
                item[0].Items.Add(item[1]);
                item[0].Items.Add(item[2]);
            }
            bool[] itemBool = new bool[itemLVL.Length];
            for (int i = 0; i < work.SaveList.Count; i++)
            {
                MenuItem item = new MenuItem{ Name = "Load" + (i + 1), Header = work.SaveList[i].Date, Tag = "2 " + i };
                item.Click += MenuItem_Click;
                if(!itemBool[work.SaveList[i].lvl])
                {
                    itemBool[work.SaveList[i].lvl] = true;
                    MenuItem item2 = new MenuItem { Name = "lvlNew" + (i + 1), Header = "New", Tag = itemLVL[work.SaveList[i].lvl].Tag};
                    item2.Click += lvlClick;
                    itemLVL[work.SaveList[i].lvl].Click-=lvlClick;
                    itemLVL[work.SaveList[i].lvl].Items.Add(item2);
                }
                itemLVL[work.SaveList[i].lvl].Items.Add(item);
            }
        }

        private void lvlClick(object sender, RoutedEventArgs e)
        {
            if (work.Thislvl == (int)(sender as MenuItem).Tag) return;
            MessageBoxResult MSGresult = MessageBox.Show("Вы хотите сохранить уровень?", "Смена уровня", MessageBoxButton.YesNoCancel);
            if (MSGresult == MessageBoxResult.Yes) { work.SaveLVL(); work.lvlLoad((int)(sender as MenuItem).Tag); lvlItem(); }
            if (MSGresult == MessageBoxResult.No) { work.lvlLoad((int)(sender as MenuItem).Tag); lvlItem(); }
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as MenuItem).Name == "SaveNew") { work.SaveLVL(); lvlItem(); return; }
            else if ((sender as MenuItem).Name == "New" || (sender as MenuItem).Name == "Edit")
            {
                this.Visibility = Visibility.Hidden;
                Window1 w1;
                if ((sender as MenuItem) == New) { w1 = new Window1(); }
                else { w1 = new Window1(work.Thislvl); }
                w1.ShowDialog();
                this.Visibility = Visibility.Visible;
                lvlItem();
                if (!Window1.deleted) work.lvlLoad(work.Thislvl);
                else { work.lvlLoad(); Window1.deleted = false; }
            }
            else if (((string)(sender as MenuItem).Tag).Split(' ')[0] == "0") { work.SaveLVL(1, ((string)(sender as MenuItem).Tag).Split(' ')[1]); lvlItem(); return; }
            else if (((string)(sender as MenuItem).Tag).Split(' ')[0] == "1") { work.SaveLVL(2, ((string)(sender as MenuItem).Tag).Split(' ')[1]); lvlItem(); return; }
            else if (((string)(sender as MenuItem).Tag).Split(' ')[0] == "2")
            {
                MessageBoxResult MSGresult = MessageBox.Show("Вы хотите сохранить уровень?", "Смена уровня", MessageBoxButton.YesNoCancel);
                if (MSGresult == MessageBoxResult.Yes) work.SaveLVL();
                if (MSGresult == MessageBoxResult.Cancel) return;
                int num = Convert.ToInt32(((string)(sender as MenuItem).Tag).Split(' ')[1]);
                work.lvlLoad(work.SaveList[num].lvl, 0, num);
                lvlItem();
                return;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            work.ReloadLVL();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult MSGresult = MessageBox.Show("Вы хотите сохранить уровень?", "Выход", MessageBoxButton.YesNoCancel);
            if (MSGresult == MessageBoxResult.Yes) work.SaveLVL();
            if (MSGresult == MessageBoxResult.Cancel) e.Cancel = true;
        }
    }

    public partial class Window1 : Window
    {
        Work work = new Work();
        public static bool deleted = false;
        public Window1(int rnd = -1)
        {
            InitializeComponent();
            work.win1 = this;
            if (rnd != -1)
            {
                work.initBoxes(1, true);
                work.lvlLoad(rnd, 1);
            }
            else work.initBoxes(1);
        }
    }
}
