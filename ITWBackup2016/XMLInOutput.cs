using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Linq;

namespace ITWBackup2016
{
    public class XMLInOutput
    {

        XmlDocument xmlDoc = new XmlDocument();

        private ListView lviFileList;
        private TextBox txtOutput;
        private ComboBox _cmbBackupSelection;
        private ComboBox _cmbOccurrenceSelection;
        private string _pfadauswahl;
        private string _sprache;
        private string pathFiles = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\XMLFiles.xml";
        private string pathOptions = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\XMLOptions.xml";

        public string Pfadauswahl
        {
            get
            {
                return _pfadauswahl;
            }

            set
            {
                _pfadauswahl = value;
            }
        }

        public string Sprache
        {
            get
            {
                return _sprache;
            }

            set
            {
                _sprache = value;
            }
        }

        public XMLInOutput(ListView list , TextBox textBox, ComboBox cmbBackupSelection, ComboBox cmbOccurenceSelection)
        {
            lviFileList = list;
            txtOutput = textBox;
            this.Pfadauswahl = string.Empty;
            this.Sprache = string.Empty;
            this._cmbBackupSelection = cmbBackupSelection;
            this._cmbOccurrenceSelection = cmbOccurenceSelection;
        }

        public void loadXMLFiles()
        {
            try
            {
                txtOutput.Text += "\n" + pathFiles + "\n";
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(pathFiles);
                System.Xml.XmlElement root = doc.DocumentElement;
                System.Xml.XmlNodeList lst = root.GetElementsByTagName("Item");

                foreach (System.Xml.XmlNode n in lst)
                {
                    try
                    {
                        lviFileList.Items.Add(n.InnerText);
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void loadXMLOptions()
        {
            try
            {
                txtOutput.Text += pathOptions + "\n";
                System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                doc.Load(pathOptions);
                System.Xml.XmlElement root = doc.DocumentElement;
                txtOutput.Text += doc.DocumentElement.SelectSingleNode(@"Data/Software").InnerText + "\n";
                txtOutput.Text += doc.DocumentElement.SelectSingleNode(@"Data/Occurrence").InnerText + "\n";

                Pfadauswahl = doc.DocumentElement.SelectSingleNode(@"Data/Path").InnerText;
                Sprache = doc.DocumentElement.SelectSingleNode(@"Data/Lang").InnerText;

                string cmbBackupS = doc.DocumentElement.SelectSingleNode(@"Data/Software").InnerText;
                _cmbBackupSelection.SelectedIndex = int.Parse(cmbBackupS);

                string cmbOccurence = doc.DocumentElement.SelectSingleNode(@"Data/Occurrence").InnerText;
                _cmbOccurrenceSelection.SelectedIndex = int.Parse(cmbOccurence);

                if (doc.DocumentElement.SelectSingleNode(@"Data/BackupUser").InnerText == "True")
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).chkUserFiles.IsChecked = true;
                }
                else
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).chkUserFiles.IsChecked = false;
                }
                if (doc.DocumentElement.SelectSingleNode(@"Data/BackupTime").InnerText == "True")
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).chkUserFiles.IsChecked = true;
                }
                else
                {
                    ((MainWindow)System.Windows.Application.Current.MainWindow).chkUserFiles.IsChecked = false;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void saveXMLFiles()
        {
            try
            {
                txtOutput.Text += "\n" + pathFiles + "\n";
                XmlTextWriter xwriter = new XmlTextWriter(pathFiles, Encoding.Unicode);
                xwriter.WriteStartDocument();
                xwriter.WriteStartElement("XMLFILE");
                foreach (String item in lviFileList.Items)
                {
                    xwriter.WriteStartElement("Item");
                    xwriter.WriteString(item);
                    xwriter.WriteEndElement();
                }
                xwriter.WriteEndElement();
                xwriter.WriteEndDocument();
                xwriter.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "ErrorLabel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void saveXMLOptions()
        {
            try
            {
                txtOutput.Text += "\n" + pathOptions + "\n";
                 XmlTextWriter xwriter = new XmlTextWriter(pathOptions, Encoding.Unicode);
                 xwriter.WriteStartDocument();
                 xwriter.WriteStartElement("XMLFILE");

                 xwriter.WriteStartElement("Data");

                 xwriter.WriteStartElement("Path");
                 xwriter.WriteString(((MainWindow)System.Windows.Application.Current.MainWindow).pfadauswahl);
                 xwriter.WriteEndElement();

                 xwriter.WriteStartElement("Lang");
                 xwriter.WriteString(((MainWindow)System.Windows.Application.Current.MainWindow).sprache);
                 xwriter.WriteEndElement();

                xwriter.WriteStartElement("Software");
                xwriter.WriteString(((MainWindow)System.Windows.Application.Current.MainWindow).cmbBackupSelection.SelectedIndex.ToString());
                xwriter.WriteEndElement();

                xwriter.WriteStartElement("BackupUser");
                xwriter.WriteString(((MainWindow)System.Windows.Application.Current.MainWindow).chkUserFiles.IsChecked.ToString());
                xwriter.WriteEndElement();

                xwriter.WriteStartElement("Occurrence");
                xwriter.WriteString(((MainWindow)System.Windows.Application.Current.MainWindow).cmbOccurenceSelection.SelectedIndex.ToString());
                xwriter.WriteEndElement();

                xwriter.WriteStartElement("BackupTime");
                xwriter.WriteString(((MainWindow)System.Windows.Application.Current.MainWindow).chkLater.IsChecked.ToString());
                xwriter.WriteEndElement();

                xwriter.WriteEndElement();

                 xwriter.WriteEndElement();
                 xwriter.WriteEndDocument();
                 xwriter.Close();
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.Message, "ErrorLabel", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
