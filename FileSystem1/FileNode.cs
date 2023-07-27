using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSystem
{
    public class FileNode
    {
        public string FileName { get; set; }
        public string ModifyDate { get; set; }
        public bool IsFolder { get; set; }
        public int Address { get; set; }
        public string content { get; set; }

        public FileNode next { get; set; }
        public FileNode child { get; set; }
        public FileNode parent { get; set; }

        public FileNode(string name,string modifyDate,bool isFolder, int address)
        {
            FileName = name;
            ModifyDate = modifyDate;
            IsFolder = isFolder;
            Address = address;
            content = "";
        }
    }
}
