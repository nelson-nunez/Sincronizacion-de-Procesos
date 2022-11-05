using System;
using System.Diagnostics;
using System.Security;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using System.Linq;
using System.Collections.Generic;

namespace Hilos
{
    class Program
    {
        #region Variables globales

        private static int[] vectorSecuencial= new int[10000000];

        private static int[] vectorHilos = new int[10000000];       
        
        private static int Suma = 0;
        private static int CantidadImpresa = 0;

        private static SemaphoreSlim semaphore;
        
        private static List<int> colaimpresion = new List<int>();
        #endregion

        static async Task Main(string[] args)
        {
            var p = new Program();
            Console.WriteLine("\n____________________________________");
            await p.CargarVectorSecuencial();
            await p.CargarVectorcon4Hilos();
            
            Console.WriteLine("\n____________________________________");
            await p.SumaSecuencial();
            await p.SumaconHilos();
            
            Console.WriteLine("\n____________________________________");
            await p.SumaconLockHilos();
            
            Console.WriteLine("\n____________________________________");
            await p.Ejercicio4();

            Console.WriteLine("\nPresione ENTER para salir...");
            string unasdme = Console.ReadLine();
        }

        #region Ej1
        private Task CargarVectorSecuencial()
        {
            //Inicializo temporizador
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            Console.WriteLine("\nCargando vector secuencialmente...");

            //Bucle que asigna valores random
            Random rd = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                vectorSecuencial[i]= rd.Next(0, 999);
            }

            //Detengo temporizador
            timeMeasure.Stop();
            Console.WriteLine($"El tiempo de carga del proceso secuencial es de: '{timeMeasure.ElapsedMilliseconds}ms'");
            return Task.CompletedTask;
        }
        private async Task CargarVectorcon4Hilos()
        {
            //Inicializo temporizador
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            Console.WriteLine("\nCargando vector con hilos...");

            //Inicio los Hilo
            Thread thr1 = new Thread(new ThreadStart(thread1));
            thr1.Start();
            Thread thr2 = new Thread(new ThreadStart(thread1));
            thr2.Start();
            Thread thr3 = new Thread(new ThreadStart(thread1));
            thr3.Start();
            Thread thr4 = new Thread(new ThreadStart(thread1));
            thr4.Start();
            // Esperar a que el hilo termine 
            thr1.Join();
            thr2.Join();
            thr3.Join();
            thr4.Join();
            timeMeasure.Stop();
            Console.WriteLine($"El tiempo de carga con hilos es de: '{timeMeasure.ElapsedMilliseconds}ms'");
        }

        public void thread1()
        {
            Random rd = new Random();
            for (int i = 0; i < 2500000; i++)
            {
                vectorHilos[i] = rd.Next(0, 999);
            }
        }
        public void thread2()
        {
            Random rd = new Random();
            for (int i = 2500000; i <5000000 ; i++)
            {
                vectorHilos[i] = rd.Next(0, 999);
            }
        }
        public void thread3()
        {
            Random rd = new Random();
            for (int i = 5000000; i < 7500000; i++)
            {
                vectorHilos[i] = rd.Next(0, 999);
            }
        }
        public void thread4()
        {
            Random rd = new Random();
            for (int i = 7500000; i < 10000000; i++)
            {
                vectorHilos[i] = rd.Next(0, 999);
            }
        }

        #endregion
     
        #region Ej2
        private async Task SumaSecuencial()
        {
            //Inicializo temporizador
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            Console.WriteLine("\nSumando y restando secuencialmente...");
            Suma = 0;
            //Bucle que suma
            for (int i = 0; i < 100000000; i++)
            {
                Suma++;
            }
            Console.Write($"(Suma secuencial = {Suma}");

            //Bucle que resta
            for (int i = 0; i < 100000000; i++)
            {
                Suma--;
            }
            Console.Write($" - Resta secuencial = {Suma})");

            //Detengo temporizador
            timeMeasure.Stop();
            Console.WriteLine($"\nEl tiempo del proceso de suma secuencial es de: '{timeMeasure.ElapsedMilliseconds}ms'");
        }

        private async Task SumaconHilos()
        {
            //Inicializo temporizador
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            Console.WriteLine("\nSumando y restando con hilos...");
            Suma = 0;
            //Inicio los Hilo
            Thread thr1 = new Thread(new ThreadStart(threadSuma1));
            thr1.Start();
            Thread thr2 = new Thread(new ThreadStart(threadSuma1));
            thr2.Start();
            Thread thr3 = new Thread(new ThreadStart(threadSuma2));
            thr3.Start();
            Thread thr4 = new Thread(new ThreadStart(threadSuma2));
            thr4.Start();
            // Esperar a que el hilo termine 
            thr1.Join(); 
            thr2.Join();
            thr3.Join();
            thr4.Join();

            timeMeasure.Stop();
            Console.Write($"Suma con hilos sin sincronizar = {Suma}");
            Console.WriteLine($"\nEl tiempo del proceso de suma con hilos es de: '{timeMeasure.ElapsedMilliseconds}ms'");
        }

        public void threadSuma1()
        {
            for (int i = 0; i < 50000000; i++)
            {
                Suma++;
            }
        }
        public void threadSuma2()
        {
            for (int i = 0; i < 50000000 ; i++)
            {
                Suma--;
            }
        }

        #endregion

        #region Ej3
        //https://www.pluralsight.com/guides/how-to-write-your-first-multi-threaded-application-with-c
        private async Task SumaconLockHilos()
        {
            //Inicializo temporizador
            Stopwatch timeMeasure = new Stopwatch();
            timeMeasure.Start();
            Console.WriteLine("\nSumando y restando con hilos y locks...");
            Suma = 0;
            //Inicio los Hilo           
            Thread thr1 = new Thread(new ThreadStart(threadSumaLock1));
            thr1.Start();
            Thread thr2 = new Thread(new ThreadStart(threadSumaLock1));
            thr2.Start();
            Thread thr3 = new Thread(new ThreadStart(threadSumaLock2));
            thr3.Start();
            Thread thr4 = new Thread(new ThreadStart(threadSumaLock2));
            thr4.Start();
            thr1.Join();
            thr2.Join();
            thr3.Join();
            thr4.Join();
            timeMeasure.Stop();
            Console.Write($"Suma con hilos sincronizados= {Suma}");
            Console.WriteLine($"\nEl tiempo del proceso de suma con hilos es de: '{timeMeasure.ElapsedMilliseconds}ms'");
        }

        public void threadSumaLock1()
        {
            lock (this)
            {
                for (int i = 0; i < 50000000; i++)
                {
                    Suma++;
                }
            };
        }
        public void threadSumaLock2()
        {
            lock (this)
            {
                for (int i = 0; i < 50000000; i++)
                {
                    Suma--;
                }
            };
        }

        #endregion

        #region Ej4
        //https://geeks.ms/vtortola/2008/03/02/producerconsumer-con-buffer/
        private async Task Ejercicio4()
        {
            Console.WriteLine("\nImprimiendo con hilos y locks...");
            
            //Inicio los Hilos           
            Thread thrApp1 = new Thread(new ThreadStart(App1));
            thrApp1.Start();            
            
            Thread thrApp2 = new Thread(new ThreadStart(App2));
            thrApp2.Start();            
            
            Thread thrApp3 = new Thread(new ThreadStart(App3));
            thrApp3.Start();


            //Inicio la impresora           
            CantidadImpresa = 0;
            Thread thrImpresora = new Thread(new ThreadStart(Impresora));
            thrImpresora.Start();
        }

        #region Aplicaciones que imprimen
        static void App1()
        {
            var p = new Program();
            for (Int32 i = 1; i < 101; i++)
            {
               p.CreandoImpresiones(i, 1);
            }
            Console.WriteLine("Se generaron todos las impresiones de aplicación 1");
        }        
        static void App2()
        {
            var p = new Program();
            for (Int32 i = 1; i < 101; i++)
            {
               p.CreandoImpresiones(i, 2);
            }
            Console.WriteLine("Se generaron todos las impresiones de aplicación 2");
        }       
        static void App3()
        {
            var p = new Program();
            for (Int32 i = 1; i < 101; i++)
            {
               p.CreandoImpresiones(i, 3);
            }
            Console.WriteLine("Se generaron todos las impresiones de aplicación 3");
        }
        #endregion

        public void CreandoImpresiones(int i,int appid)
        {
            #region Simulo demora
            Random rd = new Random();
            var tt = rd.Next(500, 4500);
            Thread.Sleep(tt);
            #endregion

            var position = 0;
            lock (colaimpresion)
            {
                // Si se ha alcanzado el máximo bloqueo hasta que se imprima y saque
                while (colaimpresion.Count() == 5)
                    Monitor.Wait(colaimpresion);
                //Agrego un item al final de la cola
                colaimpresion.Add(i);
                Console.WriteLine($"'Aplicación nro:{appid}' imprimió su trabajo {i}, quedando en la posición {colaimpresion.Count()} de la cola");
                Monitor.Pulse(colaimpresion);
            };
        }

        public void Impresora()
        {
            #region Simulo demora
            Random rd = new Random();
            var tt = rd.Next(500, 1500);
            Thread.Sleep(tt);
            #endregion

            do
            {
                lock (colaimpresion)
                {
                    //Espera si no hay nada en la cola
                    while (colaimpresion.Count() == 0)
                        Monitor.Wait(colaimpresion);

                    var item = colaimpresion.FirstOrDefault();
                    CantidadImpresa++;
                    Console.WriteLine($"Imprimiendo trabajo nro {CantidadImpresa}");
                    colaimpresion.Remove(item);
                    Monitor.Pulse(colaimpresion);
                };  
            } while (true);
        }
        #endregion
    }
}

