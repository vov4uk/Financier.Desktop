using Financier.DataAccess.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

namespace Financier.Desktop.Entities
{
    /// <summary>
    /// Interaction logic for Categories.xaml
    /// </summary>
    public partial class Categories : UserControl
    {
        public Categories(RangeObservableCollection<Category> categories)
        {
            var nodes = new ObservableCollection<Node>();
            InitializeNodes(nodes, categories.Where(x => x.Id > 0).ToList());

            InitializeComponent();
            treeView1.ItemsSource = nodes;
        }

        private void InitializeNodes(ObservableCollection<Node> nodes, List<Category> categories)
        {
            foreach (var category in categories.OrderBy(x => x.Left))
            {
                if (!nodes.Any(x => x.Right > category.Left))
                {
                    var subNode = new Node
                    {
                        Id = category.Id,
                        Title = category.Title,
                        Left = category.Left,
                        Right = category.Right,
                        SubCategoties = new ObservableCollection<Node>()
                    };
                    nodes.Add(subNode);

                    var sub = categories.Where(x => x.Left > category.Left && x.Right < category.Right).ToList();
                    if (sub.Any())
                    {
                        InitializeNodes(subNode.SubCategoties, sub);
                    }
                }
            }
        }

        public class Node
        {
            public int Id;
            public int Left { get; set; }
            public int Right { get; set; }
            public string Title { get; set; }
            public ObservableCollection<Node> SubCategoties { get; set; }
        }
    }
}
