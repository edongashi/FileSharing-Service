using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FileSharing.Core.Modeli;

namespace FileSharing.Klienti.Modeli
{
    public class ProgresiTransferit : INotifyPropertyChanged
    {
        private double perqindja;

        private GjendjaTransferit gjendjaTransferit;

        private string statusiTekst;

        public ProgresiTransferit(string emri, KahuTransferit kahu)
        {
            FileEmri = emri;
            KahuTransferit = kahu;
        }

        public string FileEmri { get; private set; }

        public KahuTransferit KahuTransferit { get; private set; }

        public double Perqindja
        {
            get { return perqindja; }
            set
            {
                perqindja = value;
                OnPropertyChanged("Perqindja");
            }
        }

        public GjendjaTransferit GjendjaTransferit
        {
            get { return gjendjaTransferit; }
            set
            {
                gjendjaTransferit = value;
                OnPropertyChanged("GjendjaTransferit");
                OnPropertyChanged("GjendjaTransferitNumerike");
            }
        }

        public int GjendjaTransferitNumerike
        {
            get { return (int)gjendjaTransferit; }
        }

        public string StatusiText
        {
            get { return statusiTekst; }
            set
            {
                statusiTekst = value;
                OnPropertyChanged("StatusiText");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}