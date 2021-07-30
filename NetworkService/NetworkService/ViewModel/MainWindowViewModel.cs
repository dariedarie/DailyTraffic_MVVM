using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace NetworkService.ViewModel
{
    public class MainWindowViewModel:BindableNotify
    {
        //private int count = 15; // Inicijalna vrednost broja objekata u sistemu
                                // ######### ZAMENITI stvarnim brojem elemenata
                                //           zavisno od broja entiteta u listi

        private BindableNotify currentViewModel;
        private NetworkEntitiesViewModel networkEntitiesViewModel = new NetworkEntitiesViewModel();
        private NetworkDisplayViewModel networkDisplayViewModel = new NetworkDisplayViewModel();
        private GraphViewModel graphViewModel = new GraphViewModel();
        public MyICommand<string> NavCommand { get; private set; }
        private static int indikator = 0;
        public static int indikatorIA = 15000;
        public static int indikatorIB = 7000;
        public static String direktorijum = @"C:\Users\colak\Desktop\HCIPZ2\NetworkService\NetworkService\Slike";
        

        public MainWindowViewModel()
        {
            createListener(); //Povezivanje sa serverskom aplikacijom
            NavCommand = new MyICommand<string>(OnNav);
            currentViewModel = networkEntitiesViewModel;
        }

        private void createListener()
        {
            var tcp = new TcpListener(IPAddress.Any, 25565);
            tcp.Start();

            var listeningThread = new Thread(() =>
            {
                while (true)
                {
                    var tcpClient = tcp.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(param =>
                    {
                        //Prijem poruke
                        NetworkStream stream = tcpClient.GetStream();
                        string incomming;
                        byte[] bytes = new byte[1024];
                        int i = stream.Read(bytes, 0, bytes.Length);
                        //Primljena poruka je sacuvana u incomming stringu
                        incomming = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

                        //Ukoliko je primljena poruka pitanje koliko objekata ima u sistemu -> odgovor
                        if (incomming.Equals("Need object count"))
                        {
                            //Response
                            /* Umesto sto se ovde salje count.ToString(), potrebno je poslati 
                             * duzinu liste koja sadrzi sve objekte pod monitoringom, odnosno
                             * njihov ukupan broj (NE BROJATI OD NULE, VEC POSLATI UKUPAN BROJ)
                             * */
                            Byte[] data = System.Text.Encoding.ASCII.GetBytes(NetworkEntitiesViewModel.Roads.Count.ToString());
                            stream.Write(data, 0, data.Length);
                        }
                        else
                        {
                            //U suprotnom, server je poslao promenu stanja nekog objekta u sistemu
                            Console.WriteLine(incomming); //Na primer: "Entitet_1:272"

                            //################ IMPLEMENTACIJA ####################
                            // Obraditi poruku kako bi se dobile informacije o izmeni
                            // Azuriranje potrebnih stvari u aplikaciji
                            try
                            {
                                string[] splitovanje = incomming.Split('_', ':');
                                NetworkEntitiesViewModel.Roads[int.Parse(splitovanje[1])].Value = int.Parse(splitovanje[2]);
                                File.AppendAllText(@"Log.txt", $"Road: {int.Parse(splitovanje[1])}\t|Value: {int.Parse(splitovanje[2])}\t|Time: {DateTime.Now}" + Environment.NewLine);                                                                                                                                                    //NetworkEntitiesViewModel.ValueChangeCommand.CanExecuteChanged($"{splitovanje[1]}_{splitovanje[2]}"); //saljemo dobijenu vrednost View2ViewModelu da primeni izmenu ako treba
                         
                                String slika;
                                String tempId = NetworkEntitiesViewModel.Roads[int.Parse(splitovanje[1])].Id.ToString();
                                String izbor = NetworkEntitiesViewModel.Roads[int.Parse(splitovanje[1])].Izbor;
                                for (int item = 0; item < NetworkDisplayViewModel.canvases.Count; item++)
                                {
                                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() => 
                                    {
                                        if (((TextBlock)((Canvas)NetworkDisplayViewModel.canvases[item]).Children[0]).Text.Equals(tempId))
                                        {
                                           
                                            if (izbor.Equals("IA"))
                                            {
                                                if (int.Parse(splitovanje[2]) >= indikator && int.Parse(splitovanje[2]) <= indikatorIA)
                                                {
                                                    slika = Path.Combine(direktorijum, @"../Slike/IA.png");
                                                }
                                                else
                                                {
                                                    slika = Path.Combine(direktorijum, @"../Slike/upozorenje.jpg");

                                                }

                                            }
                                            else
                                            {
                                                if (int.Parse(splitovanje[2]) >= indikator && int.Parse(splitovanje[2]) <= indikatorIB)
                                                {
                                                    slika = Path.Combine(direktorijum, @"../Slike/IB.jpg");
                                                }
                                                else
                                                {
                                                    slika = Path.Combine(direktorijum, @"../Slike/upozorenje.jpg");

                                                }
                                            }
                                            
                                            BitmapImage slikaPromene = new BitmapImage();
                                            slikaPromene.BeginInit();
                                            slikaPromene.UriSource = new Uri(slika);
                                            slikaPromene.EndInit();
                                            NetworkDisplayViewModel.canvases[item].Background = new ImageBrush(slikaPromene);
                                        }
                                    }), DispatcherPriority.ContextIdle);
                                }
                            }
                            catch { }
                           



                        }
                    }, null);
                }
            });

            listeningThread.IsBackground = true;
            listeningThread.Start();
        }

        public BindableNotify CurrentViewModel
        {
            get { return currentViewModel; }
            set
            {
                SetProperty(ref currentViewModel, value);
            }
        }

        private void OnNav(string destination)
        {
            switch (destination)
            {
                case "entity":
                    CurrentViewModel = networkEntitiesViewModel;
                    break;
                case "display":
                    CurrentViewModel = networkDisplayViewModel;
                    break;
                case "graph":
                    CurrentViewModel = graphViewModel;
                    break;




            }
        }

    }
}
