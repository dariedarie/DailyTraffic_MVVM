using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class NetworkEntitiesViewModel:BindableNotify
    {
        public static ObservableCollection<Road> Roads=new ObservableCollection<Road>()
        {
            
                new Road(0, "Road_1", "IA") { Value = 5553 } ,
                new Road(1, "Road_2", "IB") { Value = 3335 },
                new Road(2, "Road_3", "IA") { Value = 0 },
                new Road(3, "Road_4", "IB") { Value = 4 }
        };

        private ObservableCollection<Road> roadsTemp=new ObservableCollection<Road>(Roads);
        private MyICommand addRoadCommand;
        private MyICommand deleteRoadCommand;
        private MyICommand filterRoadCommand;
        private MyICommand filterRoadTypeCommand;
        private MyICommand helper2Command;
        private MyICommand helperCommand;
        private Road currentRoad = new Road();
        private Road filterRoad = new Road();
        private Road selectedRoad = new Road();
        private bool isChecked;
        private string filterText;
        private bool ugasiToolTip = true;
        private bool dozvola = true;
        private List<String> roadTypes = new List<String>() { "IA", "IB" };

        public MyICommand AddRoadCommand { get => addRoadCommand; set => addRoadCommand = value; }
        public MyICommand DeleteRoadCommand { get => deleteRoadCommand; set => deleteRoadCommand = value; }

        public MyICommand FilterRoadCommand { get => filterRoadCommand; set => filterRoadCommand = value; }

        public MyICommand FilterRoadTypeCommand { get => filterRoadTypeCommand; set => filterRoadTypeCommand = value; }

        public MyICommand HelperCommand { get => helperCommand; set => helperCommand = value; }
        public MyICommand Helper2Command { get => helper2Command; set => helper2Command = value; }
        public MyICommand UgasiToolTipCommand { get; set; }



        public NetworkEntitiesViewModel()
        {
            roadsTemp = new ObservableCollection<Road>(Roads);
            AddRoadCommand = new MyICommand(OnAdd);
            DeleteRoadCommand = new MyICommand(OnDelete);
            FilterRoadCommand = new MyICommand(OnFilter);
            FilterRoadTypeCommand = new MyICommand(OnFilterType);
            HelperCommand = new MyICommand(OnHelper);
            Helper2Command = new MyICommand(OnHelper2);
            UgasiToolTipCommand = new MyICommand(OnUgasi);
        }


        public string FilterText
        {
            get { return filterText; }
            set
            {
                if (this.filterText != value)
                {
                    this.filterText = value;
                    OnPropertyChanged("FilterText");
                }
            }
        }

        public ObservableCollection<Road> RoadsTemp
        {
            get { return roadsTemp; }
            set
            {
                if (this.roadsTemp != value)
                {
                    this.roadsTemp = value;
                    OnPropertyChanged("RoadsTemp");
                }
            }
        }


        public bool IsChecked
        {
            get { return isChecked; }
            set
            {
                if (this.isChecked != value)
                {
                    this.isChecked = value;
                    OnPropertyChanged("IsChecked");
                }
            }
        }



        public Road SelectedRoad
        {
            get { return selectedRoad; }
            set
            {
                if (this.selectedRoad != value)
                {
                    this.selectedRoad = value;
                    OnPropertyChanged("SelectedRoad");

                }
            }
        }

        public Road CurrentRoad
        {
            get { return currentRoad; }
            set
            {
                if (this.currentRoad != value)
                {
                    this.currentRoad = value;
                    OnPropertyChanged("CurrentRoad");       
                }
            }
        }

        public Road FilterRoad
        {
            get { return filterRoad; }
            set
            {
                if (this.filterRoad != value)
                {
                    this.filterRoad = value;
                    OnPropertyChanged("FilterRoad");
                }
            }
        }


        public List<String> RoadTypes
        {
            get { return roadTypes; }
            set
            {
                if (this.roadTypes != value)
                {
                    this.roadTypes = value;
                    OnPropertyChanged("RoadTypes");
                }
            }
        }

        

        public void OnAdd()
        {
            CurrentRoad.Validate();
            if (CurrentRoad.IsValid)
            {
                 Road road = new Road(CurrentRoad.Id, CurrentRoad.Naziv, CurrentRoad.Izbor);
                 Roads.Add(road);
                 RoadsTemp = new ObservableCollection<Road>(Roads);
                 CurrentRoad = new Road();

            }

        }


        public void OnDelete()
        {
            if (SelectedRoad != null)
            {
                Roads.Remove(SelectedRoad);
                RoadsTemp = new ObservableCollection<Road>(Roads);
            }
        }

        public void OnFilter()
        {

            if (!String.IsNullOrWhiteSpace(FilterText))
            {
                RoadsTemp = new ObservableCollection<Road>();
                if (!IsChecked)
                {
                    if(FilterRoad.Izbor==null)
                    {
                        foreach (Road item in Roads)
                        {
                            if (item.Id > Int32.Parse(filterText))
                            {
                                RoadsTemp.Add(item);
                            }

                        }
                    }
                    else
                    {
                        foreach (Road item in Roads)
                        {
                            if (FilterRoad.Izbor == item.Izbor && item.Id > Int32.Parse(filterText))
                            {
                                RoadsTemp.Add(item);
                            }

                        }
                    }
                   
                }
                else
                {
                    if (FilterRoad.Izbor == null)
                    {
                        foreach (Road item in Roads)
                        {
                            if (item.Id < Int32.Parse(filterText))
                            {
                                RoadsTemp.Add(item);
                            }

                        }
                    }
                    else
                    {
                       foreach (var item in Roads)
                        {
                            if (FilterRoad.Izbor == item.Izbor && item.Id < Int32.Parse(filterText))
                            {
                                RoadsTemp.Add(item);
                            }

                        }
                    }
                }
            }
            else
            {               
                RoadsTemp = new ObservableCollection<Road>(Roads);   
            }
            FilterText = "";
            FilterRoad = new Road();
            CurrentRoad = new Road();
        }


        public void OnFilterType()
        {
            RoadsTemp = new ObservableCollection<Road>();
            foreach (var item in Roads)
            {
                if(FilterRoad.Izbor == item.Izbor)
                {
                    RoadsTemp.Add(item);
                }
            }


        }

        public bool UgasiToolTip
        {
            get { return ugasiToolTip; }
            set
            {
                if (this.ugasiToolTip != value)
                {
                    this.ugasiToolTip = value;
                    OnPropertyChanged("UgasiToolTip");

                }
            }
        }

        public bool Dozvola
        {
            get { return dozvola; }
            set
            {
                dozvola = value;
            }
        }

        private void OnHelper()
        {
            System.Windows.MessageBox.Show("Proces DODAVANJA entiteta u tabelu:Popunite polja Id, Name, izaberite tip puta iz ComboBox-a i kliknite na dugme Dodaj.\n" +
                "Proces BRISANJA entiteta iz tabele:Selektujte entitet koji zelite da obrisete i kliknite na dugme Obrisi\n" +
                "Klikom na dugme UgasiToolTip,gasite i palite tooltip-ove", "Helper", MessageBoxButton.OK, MessageBoxImage.None);
        }
        private void OnHelper2()
        {
            System.Windows.MessageBox.Show("Proces FILTRIRANJA:1.Scenario,izaberite tip+oznacite radio batton+unesite ID+kliknite na dugme Filtriraj!\n" +
                "2.Scenario,oznacite radio batton+unesite ID+kiknite na dugme Filtriraj!\n" +"3.Scenario,odaberite tip+kliknite na dugme FiltirirajTip!\n" +
                "4.Klikom samo na dugme Filtriraj vracate na pocetno stanje tabelu!!", "HelperFilter", MessageBoxButton.OK, MessageBoxImage.None);
        }
        private void OnUgasi()
        {
            if (Dozvola == false)
            {
                UgasiToolTip = true;
                Dozvola = true;
            }
            else
            {
                UgasiToolTip = false;
                Dozvola = false;
            }

        }


    }
}
