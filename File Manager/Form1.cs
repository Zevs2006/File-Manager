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
            catch (Exception) { /* ���������� ������ ������� */ }
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
            catch (Exception) { /* ���������� ������ ������� */ }
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

        // ����������� �����
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("�������� ���� ��� �����������.");
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
                        MessageBox.Show("���� ������� ����������.");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"������ �����������: {ex.Message}");
                    }
                }
            }
        }

        // ����������� �����
        private void buttonMove_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("�������� ���� ��� �����������.");
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
                        MessageBox.Show("���� ������� ���������.");
                        LoadFilesInDirectory(treeView1.SelectedNode.Tag as string);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"������ �����������: {ex.Message}");
                    }
                }
            }
        }

        // �������� �����
        private void buttonDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("�������� ���� ��� ��������.");
                return;
            }

            DialogResult result = MessageBox.Show($"�� �������, ��� ������ ������� ���� '{Path.GetFileName(selectedFilePath)}'?",
                "������������� ��������", MessageBoxButtons.YesNo);

            if (result == DialogResult.Yes)
            {
                try
                {
                    File.Delete(selectedFilePath);
                    MessageBox.Show("���� ������� ������.");
                    LoadFilesInDirectory(treeView1.SelectedNode.Tag as string);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��������: {ex.Message}");
                }
            }
        }

        // �������������� �����
        private void buttonRename_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(selectedFilePath) || !File.Exists(selectedFilePath))
            {
                MessageBox.Show("�������� ���� ��� ��������������.");
                return;
            }

            string newName = Microsoft.VisualBasic.Interaction.InputBox("������� ����� ��� �����:", "��������������", Path.GetFileName(selectedFilePath));

            if (!string.IsNullOrEmpty(newName))
            {
                string directory = Path.GetDirectoryName(selectedFilePath);
                string newPath = Path.Combine(directory, newName);

                try
                {
                    File.Move(selectedFilePath, newPath);
                    MessageBox.Show("���� ������� ������������.");
                    LoadFilesInDirectory(treeView1.SelectedNode.Tag as string);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"������ ��������������: {ex.Message}");
                }
            }
        }
    }
}
