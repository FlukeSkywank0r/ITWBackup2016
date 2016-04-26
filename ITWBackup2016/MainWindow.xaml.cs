using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32.TaskScheduler;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace ITWBackup2016
{
    public partial class MainWindow : Window
    {
        private static string[] OCCURRENCE_VALUES = { "On Date once", "Daily", "Weekly", "Monthly", "Yearly" };
        private static string[] BACKUP_VALUES = { "WinControl", "DiaMet", "Omnimet", "Omnimet MHT", "Minuteman" };
        private string versioninfo = "V 0.3.2. - 2016_04_25";

        public string pfadauswahl;
        public string sprache;
        public string BackupVerzeichnis;
        public string exists;

        private string ProgramFiles;
        private string ErrorLabel;
        private string SuccessLabel;
        private string lang;
        private string path1 = System.IO.Path.Combine(System.Environment.CurrentDirectory);

        private XMLInOutput _xmlInOutput;
        private CalculateSizes _calculateSizes;

        DateTime dt = DateTime.Now;

        public List<string> fileNameList;

        public MainWindow()
        {
            InitializeComponent();
            _xmlInOutput = new XMLInOutput(lviFileList, txtOutput, cmbBackupSelection, cmbOccurenceSelection);
            _xmlInOutput.loadXMLOptions();
            //_calculateSizes = new CalculateSizes(lviFileList);
            sprache = _xmlInOutput.Sprache;
            pfadauswahl = _xmlInOutput.Pfadauswahl;
            lblProgFiles.Content = "ProgramPath: " + ProgramFiles;
            lblBackupPath.Content = BackupVerzeichnis + pfadauswahl;
            lblFileSize.Content = "FileSize: ";
            lblDirSize.Content = "DirSize: ";
            lblBackupSize.Content = "BackupSize: ";

            if (_xmlInOutput.Sprache == "DE")
            {
                buttonDE_Click(null, null);
            }
            if (_xmlInOutput.Sprache == "EN")
            {
                buttonUS_Click(null, null);
            }
            if (_xmlInOutput.Sprache == "FR")
            {
                buttonFR_Click(null, null);
            }
        }

        /*  |-----------------------------------------------------------------------------------|
            |   Cultural Information                                                            |
            |-----------------------------------------------------------------------------------| */

        private void getRes(CultureInfo ci)
        {
            Assembly a = Assembly.Load("ITWBackup2016");
            ResourceManager rm = new ResourceManager("ITWBackup2016.Lang.lang", a);

            lblBackupPath.Content = rm.GetString("gewaehlterPfad ", ci) + pfadauswahl;
            lblProgFiles.Content = rm.GetString("programmePfad ", ci) + ProgramFiles;

            btnQuit.Content = rm.GetString("btnEnde", ci);
            btnDeSelectFiles.Content = rm.GetString("btnEntfernen", ci);
            btnEmptyList.Content = rm.GetString("btnListeleeren", ci);
            btnReset.Content = rm.GetString("btnReset", ci);
            btnActivateBackup.Content = rm.GetString("btnSetzen", ci);
            btnStartBackup.Content = rm.GetString("btnStarten", ci);
            btnSelectDir.Content = rm.GetString("btnVerzeichnis", ci);
            btnLoadXMLFile.Content = rm.GetString("btnLoadXMLFile", ci);

            chkLater.Content = rm.GetString("chkspaeter", ci);

            ErrorLabel = rm.GetString("ErrorLabel", ci);
            SuccessLabel = rm.GetString("SuccessLabel", ci);
            lang = rm.GetString("languageToolStripMenuItem", ci);


            /*
            Beschreibung = rm.GetString("Beschreibung", ci);
            BWillBe = rm.GetString("BWillBe", ci);
            
            
            ExecutionLabel = rm.GetString("ExecutionLabel", ci);
            Gefunden = rm.GetString("Gefunden", ci);         
            Kopiert = rm.GetString("Kopiert", ci);
            
            loadfiles = rm.GetString("loadfiles", ci);
            loadfilestitle = rm.GetString("loadfilestitle", ci);
            neuerPfad = rm.GetString("neuerPfad", ci);
            
            NichtGefunden = rm.GetString("NichtGefunden", ci);
            NichtKopiert = rm.GetString("NichtKopiert", ci);
            
            sichergroesse.Text = rm.GetString("sichergroesse", ci);
            

            TheBackup = rm.GetString("TheBackup", ci);
            TheBDate = rm.GetString("TheBDate", ci);
            UserFileData = rm.GetString("UserFileData", ci);
            Verzeichnis = rm.GetString("Verzeichnis", ci);
            verzgroesse.Text = rm.GetString("verzgroesse", ci);
            */
        }

        /*  |-----------------------------------------------------------------------------------|
            |   WPF Loading                                                                     |
            |-----------------------------------------------------------------------------------| */

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("Load previous files for backup again?", "Backup previous files?", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                if (lviFileList.Items.Count == 0)
                {
                    _xmlInOutput.loadXMLFiles();
                    completeSize();
                }
            }

            /*  |-----------------------------------------------------------------------------------|
                |   32bit / 64bit check                                                             |
                |-----------------------------------------------------------------------------------| */

            if (Environment.Is64BitOperatingSystem)
            {
                ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
                txtOutput.Text += ProgramFiles + "\r\n";
            }
            else
            {
                ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            }
        }

        /*  |-----------------------------------------------------------------------------------|
            |   Combobox controls                                                               |
            |-----------------------------------------------------------------------------------| */

        private void cmbOccurenceSelection_Loaded(object sender, RoutedEventArgs e)
        {
            // ... Get the ComboBox reference.
            var cmbOccurenceSelection = sender as System.Windows.Controls.ComboBox;
            // ... Assign the ItemsSource to the List.
            cmbOccurenceSelection.ItemsSource = OCCURRENCE_VALUES;
            // ... Make the first item selected.
            //cmbOccurenceSelection.SelectedIndex = 0;
        }

        private void cmbOccurenceSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var cmbOccurenceSelection = sender as System.Windows.Controls.ComboBox;
            /* ... Set SelectedItem as Window Title.
            string value = cmbSBox.SelectedItem as string;
            this.Title = "Selected: " + value;*/
        }

        private void cmbBackupSelection_Loaded(object sender, RoutedEventArgs e)
        {

            // ... Get the ComboBox reference.
            var cmbBackupSelection = sender as System.Windows.Controls.ComboBox;
            // ... Assign the ItemsSource to the List
            cmbBackupSelection.ItemsSource = BACKUP_VALUES;
            // ... Make the first item selected.
            // cmbBackupSelection.SelectedIndex = 0;
        }

        private void cmbBackupSelection_Changed(object sender, SelectionChangedEventArgs e)
        {
            // ... Get the ComboBox.
            var cmbBackup = sender as System.Windows.Controls.ComboBox;
            /* ... Set SelectedItem as Window Title.
            string value = cmbSBox.SelectedItem as string;
            this.Title = "Selected: " + value;*/
        }

        /*  |-----------------------------------------------------------------------------------|
            |   Listbox Drag&Drop functionality & duplicate checking                            |
            |-----------------------------------------------------------------------------------| */


        private bool search(string value)
        {
            bool found = false;
            int i = 0;

            while (!found && i<lviFileList.Items.Count )
            {
                string aux = (string)lviFileList.Items[i];
                if (aux.CompareTo(value) == 0)
                {
                    found = true;
                    System.Windows.MessageBox.Show("This item already exists");
                }

                i++;
            }
            return found;
        }

        private void lviFileList_DragDrop(object sender, System.Windows.DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop, false);
            bool found = true;
            for (int i = 0; i < files.Length; i++)
            {
                if (lviFileList.Items.Count == 0)
                {
                    // Der erste Fall
                    lviFileList.Items.Add(files[i]);
                }
                else {
                    found = search(files[i]);
                }
                if (!found)
                {
                    // nicht gefunden
                    lviFileList.Items.Add(files[i]);
                }
            }
            
            completeSize();
        }
        private void lviFileList_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
                e.Effects = System.Windows.DragDropEffects.All;
            else
            {
                String[] strGetFormats = e.Data.GetFormats();
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }

        /*  |-----------------------------------------------------------------------------------|
            |   Calculating of Dir/File/Backup Size                                             |
            |-----------------------------------------------------------------------------------| */

        public static long DirSize(DirectoryInfo d)
        {
            long Size = 0;
            // Add file sizes.
            FileInfo[] fis = d.GetFiles();
            foreach (FileInfo fi in fis)
            {
                Size += fi.Length;
            }
            // Add subdirectory sizes.
            DirectoryInfo[] dis = d.GetDirectories();
            foreach (DirectoryInfo di in dis)
            {
                Size += DirSize(di);
            }
            return (Size);
        }

        private void lviFileList_SelectedIndexChanged(object sender, EventArgs e)
        {
            double len = 0;
            byte type = 0;
            string typeName = "";

            if (lviFileList.SelectedIndex >= 0)
            {
                try
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(lviFileList.Items[lviFileList.SelectedIndex].ToString());

                    if (System.IO.Path.HasExtension(lviFileList.Items[lviFileList.SelectedIndex].ToString()))
                    {
                        len = (double)file.Length;
                        while (len > 1024)
                        {
                            len /= 1024;
                            type++;
                        }
                        switch (type)
                        {
                            case 0:
                                typeName = " bytes";
                                break;
                            case 1:
                                typeName = " kb";
                                break;
                            case 2:
                                typeName = " MB";
                                break;
                            case 3:
                                typeName = " GB";
                                break;
                            default:
                                len = (double)file.Length;
                                typeName = " bytes";
                                break;
                        }

                        string size = len.ToString("F");

                        if (size.EndsWith(".00"))
                            size = size.Remove(size.Length - 3, 3);

                        lblFileSize.Content = "FileSize: " + size + typeName;
                        lblDirSize.Content = "This is a file!";
                    }
                    else
                    {
                        lblDirSize.Content = "DirSize: " + (DirSize(new DirectoryInfo(lviFileList.Items[lviFileList.SelectedIndex].ToString())) / 1024 + " KBytes");
                        lblFileSize.Content = "This is a directory";
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show(ex.Message, ErrorLabel, MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void completeSize()
        {
            double len = 0;
            for (int i = 0; i < lviFileList.Items.Count; i++)
            {
                System.IO.FileInfo file = new System.IO.FileInfo(lviFileList.Items[i].ToString());

                if (System.IO.Path.HasExtension(lviFileList.Items[i].ToString()))
                {
                    len += (double)file.Length;
                }
                else
                {
                    len += DirSize(new DirectoryInfo(lviFileList.Items[i].ToString()));
                }
            }
            len = len / 1024 / 1024;
            len = Math.Round(len, 2);
            lblBackupSize.Content = "BackupSize: " + len + " MBytes";
        }
        /*  |-----------------------------------------------------------------------------------|
            |   GUI controls                                                                    |
            |-----------------------------------------------------------------------------------| */

        private void btnQuit_Click(object sender, RoutedEventArgs e)
        {
            _xmlInOutput.saveXMLOptions();
            _xmlInOutput.saveXMLFiles();
            this.Close();
        }

        private void btnStartBackup_Click(object sender, RoutedEventArgs e)
        {
            StartXcopy();
        }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("- Backup by Marcel Boscher -\r\n- ITW Esslingen -\r\n- Systems & Software Europe -\r\n- " + versioninfo + " -\r\n- @marcel.boscher@buehler.com", "Oh Noes!!");
        }

        private void btnEmptyList_Click(object sender, RoutedEventArgs e)
        {
            lviFileList.Items.Clear();
            lblBackupSize.Content = "";
            lblFileSize.Content = "";
            lblDirSize.Content = "";
        }

        private void btnDeSelectFiles_Click(object sender, RoutedEventArgs e)
        {
            lviFileList.Items.Remove(lviFileList.SelectedItem);
            completeSize();
        }

        private void btnSelectDir_Click(object sender, RoutedEventArgs e)
        {
            if (pfadauswahl != "")
            {
                btnSelectDir.IsEnabled = true;
            }

            var objDialog = new System.Windows.Forms.FolderBrowserDialog();
            DialogResult objResult = objDialog.ShowDialog();

            if (objResult.ToString().Equals("OK"))
            {
                System.Windows.MessageBox.Show("new path: " + objDialog.SelectedPath, "Backup Directory");
                pfadauswahl = objDialog.SelectedPath;
                txtOutput.Text += "\n" + objDialog.SelectedPath;
            }
            else
            {
                System.Windows.MessageBox.Show("The directory selection has been canceled", "Canceled", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            lblBackupPath.Content = BackupVerzeichnis + objDialog.SelectedPath;
            pfadauswahl = System.IO.Path.GetFullPath(objDialog.SelectedPath);
        }

        private void btnLoadXMLFile_Click(object sender, RoutedEventArgs e)
        {
            _xmlInOutput.loadXMLOptions();
            _xmlInOutput.loadXMLFiles();
            completeSize();
            txtOutput.Text += "Sprache: " + sprache + "\n Pfadauswahl: " + pfadauswahl + "\n";
        }

        private void buttonUS_Click(object sender, RoutedEventArgs e)
        {
            sprache = "EN";
            CultureInfo ci = new CultureInfo("en-US");
            getRes(ci);
            lblLanguage.Content = lang + ": EN";
        }

        private void buttonDE_Click(object sender, RoutedEventArgs e)
        {
            sprache = "DE";
            CultureInfo ci = new CultureInfo("de-DE");
            getRes(ci);
            lblLanguage.Content = lang + ": DE";
        }

        private void buttonFR_Click(object sender, RoutedEventArgs e)
        {
            sprache = "FR";
            CultureInfo ci = new CultureInfo("fr-FR");
            getRes(ci);
            lblLanguage.Content = lang + ": FR";
        }

        /*  |-----------------------------------------------------------------------------------|
            |   Task Scheduler                                                                  |
            |-----------------------------------------------------------------------------------| */

        private void btnActivateBackup_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string theDate = datePick.Text;
                txtOutput.Text += "The Backup " + datePick.Text;
                 
                DateTime varDate = DateTime.Parse(datePick.Text);

                txtOutput.Text += Convert.ToString(datePick);
                TaskService ts = new TaskService();
                TaskDefinition td = ts.NewTask();

                if (cmbBackupSelection.SelectedIndex == 0)
                {
                    Microsoft.Win32.TaskScheduler.Trigger t = new TimeTrigger();
                    t.StartBoundary = varDate;
                    td.Triggers.Add(t);
                    System.Windows.MessageBox.Show(varDate.ToShortDateString(), SuccessLabel, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cmbBackupSelection.SelectedIndex == 1)
                {
                    td.Triggers.Add(new WeeklyTrigger());
                    System.Windows.MessageBox.Show("", SuccessLabel, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cmbBackupSelection.SelectedIndex == 2)
                {
                    td.Triggers.Add(new WeeklyTrigger());
                    System.Windows.MessageBox.Show("", SuccessLabel, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (cmbBackupSelection.SelectedIndex == 3)
                {
                    td.Triggers.Add(new DailyTrigger { DaysInterval = 365 });
                    string s = varDate.ToString();
                    s = s.Substring(0, s.Length - 13);
                    System.Windows.MessageBox.Show("", SuccessLabel, MessageBoxButton.OK, MessageBoxImage.Information);
                }
                string path2 = path1 + @"\ITWBackupQ.exe";
                txtOutput.Text += path2;
                td.Actions.Add(new ExecAction(path2, null, null));
                ts.RootFolder.RegisterTaskDefinition("ITWBackupQ", td);
                ts.BeginInit();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show((ex.Message), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /*  |-----------------------------------------------------------------------------------|
            |   Copy Functions                                                                  |
            |-----------------------------------------------------------------------------------| */

        public void StartXcopy()
            {
                System.Windows.Controls.ListView a = lviFileList;
                fileNameList = new List<string>();
                foreach (string x in a.Items)
                {
                    fileNameList.Add(x);
                }
                foreach (string filename in fileNameList)
                {
                    string File_Name = Path.GetFileName(filename);
                    string DirString = '"' + filename + '"' + " " + '"' + pfadauswahl + @"\Backup\" + dt.ToString("yyyy-MM-dd") + @"\UserData\" + File_Name + '"' + "*";
                    string Dir2String = pfadauswahl + @"\Backup\" + dt.ToString("yyyy-MM-dd") + @"\UserData\" + File_Name + @"\";
                    string FileString = '"' + filename + '"' + " " + pfadauswahl + @"\Backup\" + dt.ToString("yyyy-MM-dd") + @"\UserData\";

                    if ((File.GetAttributes(filename) & FileAttributes.Directory) != 0)
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo();
                        startInfo.CreateNoWindow = false;
                        startInfo.UseShellExecute = false;
                        startInfo.FileName = "xcopy";
                        startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        string SolutionDirectory = filename;
                        string TargetDirectory = pfadauswahl + @"\Backup\" + dt.ToString("yyyy-MM-dd") + @"\UserData\" + File_Name + @"\*";

                        startInfo.Arguments = "\"" + SolutionDirectory + "\"" + " " + "\"" + TargetDirectory + "\"" + @" /e /y /I";
                        var attr = File.GetAttributes(filename);
                        txtOutput.Text += startInfo.Arguments;

                            if (attr.HasFlag(FileAttributes.Directory))
                            if (!Directory.Exists(DirString))
                            {
                                try
                                {   // Start the process with the info we specified.
                                    // Call WaitForExit and then the using statement will close.
                                    System.IO.Directory.CreateDirectory(Dir2String);
                                    using (Process exeProcess = Process.Start(startInfo))
                                    {
                                        exeProcess.WaitForExit();
                                    }
                                }

                                catch (Exception e)
                                {
                                    throw e;
                                }
                            }
                            else
                            {
                                try
                                {   // Start the process with the info we specified.
                                    // Call WaitForExit and then the using statement will close.
                                    using (Process exeProcess = Process.Start(startInfo))
                                    {
                                        exeProcess.WaitForExit();
                                    }
                                }
                                catch (Exception exp)
                                {
                                    throw exp;
                                }
                            }
                    }
                    else
                    {
                        System.Diagnostics.Process.Start("XCOPY.EXE ", @"/I /Y " + @"" + FileString + "");
                    }
            }
        }
    }
}

