using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Road:ValidationBase
    {
        private int id;
        private string naziv;
        private string izbor;
        private string slika;
        private int value;

        public Road()
        {
            
        }

        public Road(int id, string naziv,string izbor)
        {
            this.id = id;
            this.naziv = naziv;
            this.izbor = izbor;

            switch (izbor)
            {
                case ("IA"):
                    slika = @"..\Slike\IA.png";
                    break;
                case ("IB"):
                    slika = @"..\Slike\IB.jpg";
                    break;

            }
        }

        public int Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged("Value");
                }
            }
        }


        public string Izbor
        {
            get { return izbor; }
            set
            {
                if (izbor != value)
                {
                    izbor = value;
                    switch (izbor)
                    {
                        case ("IA"):
                            slika = @"..\Slike\IA.png";
                            break;
                        case ("IB"):
                            slika = @"..\Slike\IB.jpg";
                            break;

                    }
                    OnPropertyChanged("Izbor");
                }
            }
        }
        public string Slika
        {
            get { return slika; }
            set
            {
                if (slika != value)
                {
                    slika = value;
                    OnPropertyChanged("Slika");
                }
            }
        }

        public int Id
        {
            get { return id; }
            set
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string Naziv
        {
            get { return naziv; }
            set
            {
                if (naziv != value)
                {
                    naziv = value;
                    OnPropertyChanged("Naziv");
                }
            }
        }

       

        protected override void ValidateSelf()
        {
            bool valid = true;
            foreach (var item in ViewModel.NetworkEntitiesViewModel.Roads)
            {
                if (item.id.Equals(Id))
                    valid = false;
            }
                
               
            if (Id<=0 || string.IsNullOrWhiteSpace(this.Id.ToString()))
            {
                this.ValidationErrors["Id"] = "Id je obavezan.";
            }
            else if(!valid)
            {
                this.ValidationErrors["Id"] = "Id vec postoji.";
            }
            else if (Id.ToString().Contains(","))
            {
                this.ValidationErrors["Id"] = "ID ne sme da sadrzi zarez.";
            }
            if (string.IsNullOrWhiteSpace(this.naziv))
            {
                this.ValidationErrors["Naziv"] = "Naziv je obavezan";
            }
            if (string.IsNullOrWhiteSpace(this.Izbor))
            {
                this.ValidationErrors["Tip"] = "Izbor tipa je obavezan";
            }
        }
    }
}
