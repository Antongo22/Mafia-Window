using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Diagnostics;

namespace Mafia_Window
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string filename = "names.txt";
        string fileroles = "roles.txt";
        string fileresult = "result.txt";

        static List<string> players = new List<string>();
        static List<string> roles = new List<string>();
        public static Dictionary<string, string> playerRoles = new Dictionary<string, string>();

        public MainWindow()
        {
            InitializeComponent();
            SetFiles();
            GetFromFiles();
        }

        void SetFiles()
        {
            if (File.Exists(filename))
            {
                players = File.ReadAllLines(filename).ToList();
            }
            else
            {
                File.Create(filename).Close();
                players = new List<string>();
            }

            if (File.Exists(fileroles))
            {
                roles = File.ReadAllLines(fileroles).ToList();
            }
            else
            {
                File.Create(fileroles).Close();
                roles = new List<string>();
            }

            if (File.Exists(fileresult))
            {
                playerRoles = GetPlayers();
            }
            else
            {
                File.Create(fileresult).Close();
                playerRoles = new Dictionary<string, string>();
            }
        }

        Dictionary<string, string> GetPlayers()
        {
            Dictionary<string, string> keyValuePairs = new Dictionary<string, string>();

            string[] nameroles = File.ReadAllLines(fileresult);

            foreach(string namerole in nameroles)
            {
                string[] name_role = namerole.Split('-');
                keyValuePairs.Add(name_role[0].Trim(), name_role[1].Trim());
            }

            return keyValuePairs;
        }

        void GetFromFiles()
        {
            ListViewPlayers.Items.Clear();
            ListViewRoles.Items.Clear();
            ListViewNameRoles.Items.Clear();

            foreach(var p in players)
            {
                ListViewPlayers.Items.Add(p);
            }

            foreach (var r in roles)
            {
                ListViewRoles.Items.Add(r);
            }

            foreach (var pr in playerRoles)
            {
                ListViewNameRoles.Items.Add(pr.Key + " - " + pr.Value);
            }

        }

        void SaveFiles()
        {
            File.WriteAllText(filename, "");
            File.WriteAllText(fileroles, "");
            File.WriteAllText(fileresult, "");


            foreach (var p in players)
            {
                File.AppendAllText(filename, p + "\n");
            }

            foreach (var r in roles)
            {
                File.AppendAllText(fileroles, r + "\n");
            }

            foreach (var pr in playerRoles)
            {
                File.AppendAllText(fileresult, pr.Key + " - " + pr.Value + "\n");
            }
        }

        private void ListViewItem_Rlayers_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                var selectedItem = (sender as ListViewItem).Content;

                var listView = ItemsControl.ItemsControlFromItemContainer(sender as ListViewItem) as ListView;

                listView.Items.Remove(selectedItem);
                players.Remove(selectedItem.ToString());
                SaveFiles();
            }
        }

        private void ListViewItem_Roles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                var selectedItem = (sender as ListViewItem).Content;

                var listView = ItemsControl.ItemsControlFromItemContainer(sender as ListViewItem) as ListView;

                listView.Items.Remove(selectedItem);
                roles.Remove(selectedItem.ToString());
                SaveFiles();
            }
        }

        private void ListViewItem_Result_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                var selectedItem = (sender as ListViewItem).Content;

                var listView = ItemsControl.ItemsControlFromItemContainer(sender as ListViewItem) as ListView;

                listView.Items.Remove(selectedItem);
                playerRoles.Remove(selectedItem.ToString().Split('-')[0].Trim());
                SaveFiles();
            }
        }

        private void TextBoxAddPlayer_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddPlayer();
            }
        }

        private void ButtonAddPlayer_Click(object sender, RoutedEventArgs e)
        {
            AddPlayer();
        }

        void AddPlayer()
        {
            if (!String.IsNullOrEmpty(TextBoxAddPlayer.Text) && TextBoxAddPlayer.Text != " ")
            {
                ListViewPlayers.Items.Add($"{TextBoxAddPlayer.Text}");
                players.Add(TextBoxAddPlayer.Text);
                TextBoxAddPlayer.Text = "";
                SaveFiles();
                TextBoxAddPlayer.Focus();
            }
        }


        private void TextBoxAddRoles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AddRole();
            }
        }

        private void ButtonAddRoles_Click(object sender, RoutedEventArgs e)
        {
            AddRole();   
        }

        void AddRole()
        {
            if (!String.IsNullOrEmpty(TextBoxAddRoles.Text) && TextBoxAddRoles.Text != " ")
            {
                ListViewRoles.Items.Add($"{TextBoxAddRoles.Text}");
                roles.Add(TextBoxAddRoles.Text);
                TextBoxAddRoles.Text = "";
                SaveFiles();
                TextBoxAddRoles.Focus();
            }
        }

        private void ButtonPlayersClear_Click(object sender, RoutedEventArgs e)
        {
            ListViewPlayers.Items.Clear();
            players.Clear();
            SaveFiles();
        }

        private void ButtonRolesClear_Click(object sender, RoutedEventArgs e)
        {
            ListViewRoles.Items.Clear();
            roles.Clear();
            SaveFiles();
        }

        private void ButtonResultClear_Click(object sender, RoutedEventArgs e)
        {
            ListViewNameRoles.Items.Clear();
            playerRoles.Clear();
            SaveFiles();
        }

        private void ButtonCreate_Click(object sender, RoutedEventArgs e)
        {
            if (players.Count != roles.Count)
            {
                MessageBox.Show("Количество игроков и ролей должно совпадать!");
                return;
            }
            else if (players.Count <= 2)
            {
                MessageBox.Show("Количество игроков должно быть больше 2!");
                return;
            }

            ShufflePlayersAndRoles();
            ListViewNameRoles.Items.Clear();
            foreach (var pr in playerRoles)
            {
                ListViewNameRoles.Items.Add(pr.Key + " - " + pr.Value);
            }
            SaveFiles();
        }

        static void ShufflePlayersAndRoles()
        {
            List<string> shuffledRoles = new List<string>(roles);

            Random rng = new Random();
            int n = shuffledRoles.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);

                string tempRole = shuffledRoles[k];
                shuffledRoles[k] = shuffledRoles[n];
                shuffledRoles[n] = tempRole;
            }

            for (int i = 0; i < players.Count; i++)
            {
                playerRoles[players[i]] = shuffledRoles[i];
            }
        }

        private void ButtonCopy_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (string item in ListViewNameRoles.Items)
            {
                stringBuilder.AppendLine(item);
            }
            System.Windows.Clipboard.SetText(stringBuilder.ToString());
        }

        private void ButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            string filePath = System.IO.Path.Combine(Environment.CurrentDirectory, fileresult);
            Process.Start(filePath);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Проверяем, была ли нажата комбинация Ctrl + C
            if (e.Key == Key.C && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (ListViewNameRoles.IsKeyboardFocusWithin)
                {
                    var selectedItems = ListViewNameRoles.SelectedItems;
                    if (selectedItems != null && selectedItems.Count > 0)
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        foreach (var selectedItem in selectedItems)
                        {
                            stringBuilder.Append(selectedItem.ToString()); 
                            stringBuilder.Append(" ");
                        }

                        if (stringBuilder.Length > 0)
                        {
                            stringBuilder.Remove(stringBuilder.Length - 1, 1);
                        }

                        Clipboard.SetText(stringBuilder.ToString());
                    }
                }
            }
        }


    }
}
