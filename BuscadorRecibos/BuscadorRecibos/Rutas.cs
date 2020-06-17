using System;
using System.Collections.Generic;
using System.Text;

using System.IO;


namespace BuscadorRecibos
{
    class Rutas
    {
        public List<string> RecuperarArchivos(String ruta) 
        {
            List<string> rutas = new List<string>(); 
            var archivos = Directory.GetFiles(ruta);

            foreach (string archivo in archivos)
            {
                Console.WriteLine(archivo);
                rutas.Add(archivo);

            }

            
            return rutas;
            
        
        }

    }
}
