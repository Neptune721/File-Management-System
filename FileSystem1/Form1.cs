using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic;

namespace FileSystem
{
    public partial class Form1 : Form
    {
        private BitMap bitMap = new BitMap(1024, false);
        private FileNode curCatalog = new FileNode("head", DateTime.Now.ToString(), true, -1);//表示当前目录
        private FileNode curNode;
        private FileNode root;//根目录
        private Stack<FileNode> stack = new Stack<FileNode>();
        private bool contentSaved = false;
        public Form1()
        {
            curNode = curCatalog;
            root = curCatalog;
            InitializeComponent();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.Rows.Add(), address = bitMap.search_Address();
            if (address >= 1024 * 1024)//内存已满
            {
                MessageBox.Show("Memory is full!");
                return;
            }
            int newFileIndex = NewFileIndex(false);
            //在表格上显示新文件的各个属性
            if (newFileIndex == 0)
                dataGridView1.Rows[index].Cells[0].Value = "New File.txt";
            else
                dataGridView1.Rows[index].Cells[0].Value = "New File(" + newFileIndex + ").txt";
            dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
            dataGridView1.Rows[index].Cells[2].Value = "Text File";
            dataGridView1.Rows[index].Cells[3].Value = "0x" + Convert.ToString(address, 16);
            dataGridView1.Rows[index].Cells[4].Value = "0B";

            FileNode newNode = new FileNode((string)dataGridView1.Rows[index].Cells[0].Value,
                (string)dataGridView1.Rows[index].Cells[1].Value, false, address);
            curNode.next = newNode;//将新文件结点链到当前文件列表中
            curNode = curNode.next;
            bitMap.Set(address / 1024, true);//更新位图
            label3.Text = "Used size : " + (address / 1024+1) + "KB";//更新已用空间
        }
        private void button2_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.Rows.Add(), address = bitMap.search_Address();
            if(address>=1024*1024)
            {
                MessageBox.Show("Memory is full!");
                return;
            }
            int newFileIndex = NewFileIndex(true);
            if (newFileIndex == 0)
                dataGridView1.Rows[index].Cells[0].Value = "New Folder";
            else
                dataGridView1.Rows[index].Cells[0].Value = "New Folder(" + newFileIndex + ")";
            dataGridView1.Rows[index].Cells[1].Value = DateTime.Now.ToString();
            dataGridView1.Rows[index].Cells[2].Value = "Folder";
            dataGridView1.Rows[index].Cells[3].Value = "0x" + Convert.ToString(address, 16);
            FileNode newFolder = new FileNode((string)dataGridView1.Rows[index].Cells[0].Value,
                (string)dataGridView1.Rows[index].Cells[1].Value, true, address);
            FileNode child = new FileNode("head", DateTime.Now.ToString(), false, -1);
            newFolder.child = child;
            child.parent = newFolder;
            curNode.next = newFolder;
            curNode = curNode.next;
            bitMap.Set(address / 1024, true);
            label3.Text = "Used size : " + (address / 1024+1) + "KB";
        }

        private void DataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    //若行已是选中状态就不再进行设置
                    if (dataGridView1.Rows[e.RowIndex].Selected == false)
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1.Rows[e.RowIndex].Selected = true;
                    }
                    //只选中一行时设置活动单元格
                    if (dataGridView1.SelectedRows.Count == 1)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    }
                    //弹出操作菜单
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void DataGridViewListCellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            int index = e.RowIndex;
            OpenFile(index);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            curCatalog = root;
            curCatalog.next = null;
            curNode = curCatalog;
            dataGridView1.Rows.Clear();
            textBox1.Text = "";
            button4.Visible = false;
        }
        private int NewFileIndex(bool isFolder)
        {
            FileNode cur = curCatalog;
            if (!isFolder)
            {
                while (cur != null)
                {
                    if (cur.FileName == "New File.txt")
                        break;
                    cur = cur.next;
                }
            }
            else
            {
                while (cur != null)
                {
                    if (cur.FileName == "New Folder")
                        break;
                    cur = cur.next;
                }
            }
            if (cur == null)
                return 0;
            int i = 2;
            cur = curCatalog;
            while (cur != null)
            {
                if (!isFolder)
                {
                    if (cur.FileName == "New File(" + i + ").txt")
                    {
                        i++;
                        cur = curCatalog;
                    }
                    else
                        cur = cur.next;
                }
                else
                {
                    if (cur.FileName == "New Folder(" + i + ")")
                    {
                        i++;
                        cur = curCatalog;
                    }
                    else
                        cur = cur.next;
                }
            }
            return i;
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult result = MessageBox.Show("Do you want to save it ?", "Hint", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            contentSaved = (result == DialogResult.OK);
            e.Cancel = false;
        }
        private DialogResult ShowInputDialog(FileNode curNode)
        {
            string preContent = curNode.content;
            Size size = new Size(800, 400);
            Form inputBox = new Form();
            inputBox.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = curNode.FileName;
            inputBox.FormClosing += new FormClosingEventHandler(Form2_FormClosing);

            TextBox textBox = new TextBox();
            textBox.Size = new Size(size.Width - 10, 23);
            textBox.Location = new Point(5, 5);
            textBox.Text = curNode.content;
            textBox.AutoSize = false;
            textBox.Height = 380;
            textBox.Multiline = true;

            inputBox.Controls.Add(textBox);

            DialogResult result = inputBox.ShowDialog();
            if (contentSaved)
            {
                curNode.content = textBox.Text;
                curNode.ModifyDate = DateTime.Now.ToString();
            }
            else
                curNode.content = preContent;
            if(curNode.content.Length > 1024)
            {
                bitMap.Set(bitMap.search_Address() / 1024, true);
            }
            return result;
        }

        private void DisplayFileList()
        {
            FileNode iterator = curCatalog;
            if (curCatalog.Address == -1)
                iterator = iterator.next;
            while (iterator != null)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = iterator.FileName;
                dataGridView1.Rows[index].Cells[1].Value = iterator.ModifyDate;
                dataGridView1.Rows[index].Cells[2].Value = iterator.IsFolder ? "Folder" : "Text File";
                dataGridView1.Rows[index].Cells[3].Value = "0x" + Convert.ToString(iterator.Address, 16);
                dataGridView1.Rows[index].Cells[4].Value = iterator.IsFolder ? "" : iterator.content.Length + "B";
                iterator = iterator.next;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            curNode = curCatalog.parent;
            curCatalog = stack.Pop();
            int index = textBox1.Text.Length - 1;
            for (; index >= 0; index--)
            {
                if (textBox1.Text.Substring(index, 1) == ">")
                {
                    textBox1.Text = textBox1.Text.Remove(index);
                    break;
                }
            }
            if (textBox1.Text == "")
                button4.Visible = false;
            dataGridView1.Rows.Clear();
            DisplayFileList();
        }
        private FileNode GetFileNode(string Name)
        {
            FileNode pre = curCatalog, cur = curCatalog.next;
            while (cur != null)
            {
                if (cur.FileName == Name)
                {
                    return cur;
                }
                else
                {
                    pre = pre.next;
                    cur = cur.next;
                }
            }
            return null;
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index;
            OpenFile(index);
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index;
            Delete((string)dataGridView1.Rows[index].Cells[0].Value);
            dataGridView1.Rows.Remove(dataGridView1.Rows[index]);
        }
        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            int index = dataGridView1.CurrentRow.Index;
            FileNode Selected = GetFileNode((string)dataGridView1.Rows[index].Cells[0].Value);
            while (true)
            {
                string input = Interaction.InputBox("Please enter the new file name : ", "Rename", Selected.FileName, 800, 400);
                FileNode fileNode = curCatalog.next;
                while (fileNode != null)
                {
                    if (fileNode.FileName == input && fileNode != Selected)
                    {
                        MessageBox.Show("Do not use the same name with other files!");
                        break;
                    }
                    fileNode = fileNode.next;
                }
                if (fileNode == null)
                {
                    Selected.FileName = input;
                    dataGridView1.Rows.Clear();
                    DisplayFileList();
                    break;
                }
            }
        }
        private void OpenFile(int index)//index为当前表格中选择的行的下标
        {
            if (index >= 0)
            {
                //通过文件名获得当前节点
                FileNode Selected = GetFileNode((string)dataGridView1.Rows[index].Cells[0].Value);
                if (!Selected.IsFolder)//文件
                {
                    ShowInputDialog(Selected);//展示输入框
                    //更新保存后的显示的文件大小和修改日期
                    dataGridView1.Rows[index].Cells[4].Value = Selected.content.Length + "B";
                    dataGridView1.Rows[index].Cells[1].Value = Selected.ModifyDate;
                }
                else//文件夹
                {
                    //更新curNode为当前节点子节点
                    curNode = Selected.child;
                    curNode.parent = Selected;
                    //存储当前目录
                    stack.Push(curCatalog);
                    curCatalog = curNode;
                    button4.Visible = true;//显示回退按钮
                    dataGridView1.Rows.Clear();
                    textBox1.Text += ">" + Selected.FileName;//更新当前路径
                    DisplayFileList();//显示当前目录的文件列表
                }
            }
        }
        private void Delete(string DelName)
        {
            FileNode pre = curCatalog, cur = curCatalog.next;
            while (cur != null)
            {
                if (cur.FileName == DelName)
                {
                    pre.next = cur.next;
                    bitMap.Set(cur.Address / 1024, false);
                    return;
                }
                else
                {
                    pre = pre.next;
                    cur = cur.next;
                }
            }
        }
    }
}
