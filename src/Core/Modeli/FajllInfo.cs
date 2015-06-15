using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace FileSharing.Core.Modeli
{
    [Serializable]
    public class FajllInfo : INotifyPropertyChanged
    {
        [Key]
        public int Id { get; set; }

        public string Emri { get; set; }

        public int Madhesia { get; set; }

        public string Pronari { get; set; }

        public Dukshmeria Dukshmeria { get; set; }

        public DateTime DataShtimit { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public bool EshtePublik
        {
            get { return Dukshmeria == Dukshmeria.Publike; }
        }

        public void RaisePropetyChanged(string propertyName)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        //public void RaisePropertyChanged<TProperty>(Expression<Func<FajllInfo, TProperty>> expression)
        //{
        //    var handler = PropertyChanged;
        //    var memberExpression = expression.Body as MemberExpression;
        //    if (handler != null && memberExpression != null)
        //    {
        //        var propertyName = memberExpression.Member.Name;
        //        handler(this, new PropertyChangedEventArgs(propertyName));
        //    }
        //}
    }
}
