using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ITWBackup2016
{
    public class CalculateSizes
    {
        /*  |-----------------------------------------------------------------------------------|
            |   Calculating of Dir/File/Backup Size                                             |
            |-----------------------------------------------------------------------------------|

        private ListView _lviFileList_SelectedIndexChanged;
        private Label _lblFileSize, _lblDirSize, _lblBackupSize;
        private string _ErrorLabel;


        public CalculateSizes (ListView list, Label lblFileSize, Label lblDirSize, Label lblBackupSize, string ErrorLabel )
        {
            _lviFileList_SelectedIndexChanged = lviFileList_SelectedIndexChanged;
            this._lblFileSize = lblFileSize;
            this._lblDirSize = lblDirSize;
            this._lblBackupSize = lblBackupSize;
            this._Errorlabel = ErrorLabel;
        }

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
        public void completeSize()
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
    }
}
 */