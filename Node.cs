using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace MusicPlayer
{
    public class Node
    {
        public String Icon { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public ObservableCollection<Node> Nodes { get; set; }
    }
}
