using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Resources;
using System.Reflection.Metadata.Ecma335;

namespace BuscadorRecibos
{
    class Program
    {
        static void Main(string[] args)
        {
            Menu();


        }
        static void Menu()
        {
            //Menu de usuario 
            Console.WriteLine("Elija una opción('1'=Extraer Metadatos, '2'=Buscar_Recibo y Extraer en PDF, '3'=Salir): ");

            string opcionEntrada = Console.ReadLine();
            int opcion = Int32.Parse(opcionEntrada);
            
            switch(opcion)
            {
                case 1:
                    //EscribirDatosTxt();
                    Console.WriteLine("Datos Extraidos y Guardados");
                    Menu();
                    break;
                case 2:                   
                    Console.WriteLine("-----");
                    Menu();
                    break;

                case 3:
                    Console.WriteLine("Cerrando...");
                    break;
            
                default:
                    Console.WriteLine("Elija una opcion valida");
                    Menu();
                    break;

            }
            
             

           
            

        }
        static void EscribirDatosTxt()
        {
            Console.WriteLine("Introduzca una ruta de una carpeta con recibos: ");

            string ruta;
            ruta = Console.ReadLine();
            Rutas recibos = new Rutas();
            var recibos_rutas = recibos.RecuperarArchivos(ruta);

            using (System.IO.StreamWriter archivo = new System.IO.StreamWriter(@"C:\\METADATOS_RECIBOS\\Metadatos.txt"))
            foreach (string ruta_reci in recibos_rutas)
            {
                var datosPdf = DevolverdatosPagina(ruta_reci);         
           

            
            
                foreach (KeyValuePair<string, Array> dato in datosPdf)
                {
                    string pagina = dato.Key;
                    var control = dato.Value.GetValue(0);
                    var periodo = dato.Value.GetValue(1);

                    string linea = control.ToString() + "|" + periodo.ToString() + "|" + pagina + "|" + ruta_reci;


                    archivo.WriteLine(linea);
                    Console.WriteLine(linea);

                }

            }




        }


        static IDictionary<string, Array> DevolverdatosPagina(string ruta)
        {
            Dictionary<string, Array> datosPagina = new Dictionary<string, Array>();
            string[] patrones = { @"No. CONTROL: \d{8}?", @"PERIODO:\d{2}?/\d{4}?" };

            
            ArchivoPdf recibo = new ArchivoPdf(ruta);
            var pdf = recibo.LeerPdf();





            foreach (KeyValuePair<int, string> paginaRecibo in pdf) //recorre las hojas del archivo
            {
                string numPagina = paginaRecibo.Key.ToString();
                string textoReci = paginaRecibo.Value;
                //List<string> datos = new List<string>();
                string[] datos = new string[2];
                int contador = -1;
                for (int p = 0; p < patrones.Length; p++)           // recorre los patrones dados
                {
                    string patron = patrones[p];
                    Regex busqueda = new Regex(patron);
                    bool resultado = busqueda.IsMatch(textoReci);
                    contador += 1;
                    if (resultado)
                    {
                        MatchCollection coincidencias = Regex.Matches(textoReci, patron);


                        foreach (Match coincidencia in coincidencias)
                        {

                            datos[contador] = coincidencia.ToString();        //agrega el texto encontrado en la pagina
                            Console.WriteLine(coincidencia.ToString());
                        }


                    }
                }

                datosPagina.Add(numPagina, datos); //va agregando la pagina: control y periodo




            }

            return datosPagina;

        }


    }

}




