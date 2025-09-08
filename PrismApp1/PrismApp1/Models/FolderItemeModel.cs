using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrismApp1.Models
{
    public class FolderItemeModel : BindableBase
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? ParentFolderId { get; set; }
        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }
    }

}
