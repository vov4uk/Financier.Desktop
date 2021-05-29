using Financier.DataAccess.Data;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Financier.Desktop.ViewModel
{
    public class CategoriesVM : EntityBaseVM<Category>
    {
    public CategoriesVM()
    {
            this.PropertyChanged += Entities_PropertyChanged;
            this.Entities.CollectionChanged += Entities_CollectionChanged;
    }

        private void Entities_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _nodes.Clear();
            InitializeNodes(_nodes, Entities.ToList());
            RaisePropertyChanged(nameof(Nodes));
        }

        private void Entities_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Entities))
            {
                _nodes.Clear();
                InitializeNodes(_nodes, Entities.ToList());
                RaisePropertyChanged(nameof(Nodes));
            }
        }

        private ObservableCollection<Node> _nodes = new ObservableCollection<Node>();
        public ObservableCollection<Node> Nodes
        {
            get
            {
                if (_nodes == null)
                {
                    _nodes = new ObservableCollection<Node>();
                    RaisePropertyChanged(nameof(Nodes));
                }
                return _nodes;
            }
            set
            {
                _nodes = value;
                RaisePropertyChanged(nameof(Nodes));
            }
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
