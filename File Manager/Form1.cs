using System;
using System.IO;
using System.Windows.Forms;

namespace File_Manager
{
    public partial class Form1 : Form
    {
        private string selectedFilePath = string.Empty;

        public Form1()
        {
            InitializeComponent();
            LoadDrives();
        }

        private void LoadDrives()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    TreeNode node = new TreeNode(drive.Name);
                    node.Tag = drive.Name;
                    treeView1.Nodes.Add(node);
                    LoadDirectories(node);
                }
            }
        }

        private void LoadDirectories(TreeNode node)
        {
            string path = node.Tag as string;
            try
            {
                var directories = Directory.GetDirectories(path);
                foreach (var directory in directories)
                {
                    TreeNode newNode = new TreeNode(Path.GetFileName(directory));
                    newNode.Tag = directory;
                    node.Nodes.Add(newNode);
                }
            }
            catch (Exception) { /* Игнорируем ошибки доступа */ }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            string selectedPath = e.Node.Tag as string;
            LoadFilesInDirectory(selectedPath);
        }

        private void LoadFilesInDirectory(string path)
        {
            listView1.Items.Clear();
            try
            {
                var files = Directory.GetFiles(path);
                foreach (var file in files)
                {
                    listView1.Items.Add(Path.GetFileName(file));
                }
            }
            catch (Exception) { /* Игнорируем ошибки доступа */ }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                string selectedFileName = listView1.SelectedItems[0].Text;
                string currentDirectory = treeView1.SelectedNode.Tag as string;
                selectedFilePath = Path.Combine(currentDirectory, selectedFileName);
            }
        }

        // Копирование файла
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Выберите файл для копирования.");
                return;
            }

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationDir = folderDialog.SelectedPath;
                    string fileName = Path.GetFileName(selectedFilePath);
                    string destinationPath = Path.Combine(destinationDir, fileName);

                    try
                    {
                        File.Copy(selectedFilePath, destinationPath);
                        MessageBox.Show("Файл успешно скопирован.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка копирования: {ex.Message}");
                    }
                }
            }
        }

        // Перемещение файла
        private void buttonMove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Выберите файл для перемещения.");
                return;
            }

            using (FolderBrowserDialog folderDialog = new FolderBrowserDialog())
            {
                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string destinationDir = folderDialog.SelectedPath;
                    string fileName = Path.GetFileName(selectedFilePath);
                    string destinationPath = Path.Combine(destinationDir, fileName);

                    try
                    {
                        File.Move(selectedFilePath, destinationPath);
                        MessageBox.Show("Файл успешно перемещен.");
                        LoadFilesInDirectory(treeView1.SelectedNode.Tag as string);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка перемещения: {ex.Message}");
                    }
                }
            }
        }

        // Удаление файла
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Выберите файл для удаления.");
                return;
            }

            DialogResult result = MessageBox.Show($"Вы уверены, что хотите удалить файл '{Path.GetFileName(selectedFilePath)}'?",
                "Подтверждение удаления", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(selectedFilePath);
                    MessageBox.Show("Файл успешно удален.");
                    LoadFilesInDirectory(treeView1.SelectedNode.Tag as string);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка удаления: {ex.Message}");
                }
            }
        }

        // Переименование файла
        private void buttonRename_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("Выберите файл для переименования.");
                return;
            }

            string newName = Microsoft.VisualBasic.Interaction.InputBox("Введите новое имя файла:", "Переименование", Path.GetFileName(selectedFilePath));

            if (!string.IsNullOrEmpty(newName))
            {
                string directory = Path.GetDirectoryName(selectedFilePath);
                string newPath = Path.Combine(directory, newName);

                try
                {
                    File.Move(selectedFilePath, newPath);
                    MessageBox.Show("Файл успешно переименован.");
                    LoadFilesInDirectory(treeView1.SelectedNode.Tag as string);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка переименования: {ex.Message}");
                }
            }
        }
    }
}
