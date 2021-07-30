using NetworkService.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NetworkService.ViewModel
{
    public class NetworkDisplayViewModel : BindableNotify
    {
        public static String direktorijum = @"C:\Users\colak\Desktop\HCIPZ2\NetworkService\NetworkService\Slike";

        private static ObservableCollection<Road> ww = new ObservableCollection<Road>(NetworkEntitiesViewModel.Roads);
        private ObservableCollection<Road> roadsTempDrag = new ObservableCollection<Road>(ww);
        private Road selectedRoad = new Road();
        private bool dragging = false;
        private string draggedItem;
        private Canvas currentCanvas = null;
        private bool ugasiToolTip=true;
        private bool dozvola = true;
        public static List<Canvas> canvases = new List<Canvas>();
        private ListView promenaNaListViewu;
        private MyICommand helperCanva;
        private MyICommand<Canvas> promenaProzoraCommand;



        public MyICommand<Canvas> DropCommand { get; set; }
        public MyICommand<ListView> SelectionChangedCommand { get; set; }
        public MyICommand MouseLeftButtonUpCommand { get; set; }
        public MyICommand<Canvas> DragOverCommand { get; set; }
        public MyICommand<Canvas> FreeCommand { get; set; }

        public MyICommand<ListView> PromenaCommand { get; set; }

        public ListView PromenaNaListViewu { get => promenaNaListViewu; set => promenaNaListViewu = value; }

        public MyICommand HelperCanva { get => helperCanva; set => helperCanva = value; }

        public MyICommand  UgasiToolTipCommand { get; set; }

        public MyICommand<Canvas> PromenaProzoraCommand { get => promenaProzoraCommand; set => promenaProzoraCommand = value; }





        public NetworkDisplayViewModel()
        {
            ww = new ObservableCollection<Road>(NetworkEntitiesViewModel.Roads);
            roadsTempDrag = new ObservableCollection<Road>(ww);
            SelectionChangedCommand = new MyICommand<ListView>(OnSelectionChanged);
            MouseLeftButtonUpCommand = new MyICommand(OnMouseLeftButtonUp);
            DropCommand = new MyICommand<Canvas>(OnDrop);
            DragOverCommand = new MyICommand<Canvas>(OnDrag);
            FreeCommand = new MyICommand<Canvas>(OnFree);
            PromenaCommand = new MyICommand<ListView>(OnPromena);
            HelperCanva = new MyICommand(OnHelperCanva);
            UgasiToolTipCommand = new MyICommand(OnUgasi);
            PromenaProzoraCommand = new MyICommand<Canvas>(OnWinToWin);
        }


        public ObservableCollection<Road> RoadsTempDrag
        {
            get { return roadsTempDrag; }
            set
            {
                if (this.roadsTempDrag != value)
                {
                    this.roadsTempDrag = value;
                    OnPropertyChanged("RoadsTempDrag");
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

       
        private void OnPromena(ListView listView)
        {
          promenaNaListViewu = listView;
        }

        private void OnSelectionChanged(ListView listView)
        {
            if (!dragging)
            {
                dragging = true;
                draggedItem = SelectedRoad.Slika;
                DragDrop.DoDragDrop(listView, draggedItem, DragDropEffects.Copy | DragDropEffects.Move);
            }
        }

        private void OnMouseLeftButtonUp()
        {
            draggedItem = null;
            SelectedRoad = null;
            dragging = false;
        }

        private void OnDrag(Canvas ddd)
        {
            if (ddd.Resources["taken"] != null)
            {
                ddd.AllowDrop = false;
            }
            else
            {
                ddd.AllowDrop = true;
                currentCanvas = ddd;
            }
        }

        private void OnDrop(Canvas ddd)
        {
            if (draggedItem != null) 
            {
                if(ddd.Resources["taken"] == null)
                {
                    BitmapImage map = new BitmapImage();
                    map.BeginInit();
                    map.UriSource = new Uri(Path.Combine(direktorijum, draggedItem));
                    map.EndInit();
                    ddd.Background = new ImageBrush(map);
                    ((TextBlock)ddd.Children[0]).Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
                    ((TextBlock)ddd.Children[0]).Text = SelectedRoad.Id.ToString();
                    //((TextBlock)((Canvas)e).Children[0]).Text = "Busy";
                    ddd.Resources.Add("taken", true);

                    canvases.Add(ddd);
                    this.RoadsTempDrag.Remove(SelectedRoad);

                }
               
                SelectedRoad = null;
                dragging = false;
            }
        }


        private void OnFree(Canvas canvas)
        {

            canvases.Remove(canvas);
            int id;
            try
            {
                id = Int32.Parse(((TextBlock)((Canvas)canvas).Children[0]).Text);
            }
            catch { return; }
            if (canvas.Resources["taken"] != null)
            {
                canvas.Background = Brushes.Olive;
                ((TextBlock)canvas.Children[0]).Text = "Free";
                //((TextBlock)canvas.Children[2]).Text = "";
                ((TextBlock)canvas.Children[0]).Foreground = Brushes.Black;
                canvas.Resources.Remove("taken");
            }

            foreach (var item in NetworkEntitiesViewModel.Roads)
            {
                if(item.Id.Equals(id))
                 RoadsTempDrag.Add(item);

            }
            ObservableCollection<Road> r = RoadsTempDrag;
            RoadsTempDrag = new ObservableCollection<Road>(r.OrderBy(x => x.Id).ToList());
        }

        public void OnWinToWin(Canvas ddd)
        {
            for (int i = 0; i < canvases.Count; i++)
            {
                if (canvases[i].Equals(ddd.Name))
                {
                    BitmapImage map = new BitmapImage();
                    map.BeginInit();
                    map.UriSource = new Uri(Path.Combine(direktorijum, draggedItem));
                    map.EndInit();
                    ddd.Background = new ImageBrush(map);
                    ((TextBlock)ddd.Children[0]).Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDAFF00"));
                    ((TextBlock)ddd.Children[0]).Text = SelectedRoad.Id.ToString();
                    //((TextBlock)((Canvas)e).Children[0]).Text = "Busy";
                    ddd.Resources.Add("taken", true);

                }


            }
            

        }

        private void OnHelperCanva()
        {

            System.Windows.MessageBox.Show("Izaberite sa sipska i levim klikom misa prevucite entitet u 1 od 12 prozora,SRECNO!\n" +
                "Klikom na dugme 'X' vracate entitet na spisak entiteta sortirano!!\n" +
                "U slucaju da vam se pojavi slika upozorenja, znaci da je vrednost entitet(IA ili IB) iznad svoje definisane granice!\n" +
                "Klikom na dugme 'UgasiToolTip', gasite i palite tooltip-ove", "HelperCanva", MessageBoxButton.OK, MessageBoxImage.None);
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
