using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    // Definimos las capacidades de cada tipo de caja según la talla
    static Dictionary<string, int> capacidadDocenera = new Dictionary<string, int>
    {
        { "34-40", 17 }, { "41-48", 15 }
    };

    static Dictionary<string, int> capacidadMaster = new Dictionary<string, int>
    {
        { "34-40", 21 }, { "41-48", 18 }
    };

    static void Main()
    {
        // Datos de pares disponibles por talla
        Dictionary<int, int> paresPorTalla = new Dictionary<int, int>
        {
            { 37, 5 }, { 38, 20 }, { 39, 45 }, { 40, 55 }, { 41, 35 },
            { 42, 70 }, { 43, 45 }, { 44, 20 }, { 45, 5 }
        };

        List<string> cajas = new List<string>();
        int numeroCaja = 1;
        Dictionary<int, int> sobrantes = new Dictionary<int, int>();

        // Procesamos cada talla priorizando cajas Master
        foreach (var talla in paresPorTalla.Keys.OrderBy(t => t))
        {
            int paresRestantes = paresPorTalla[talla];
            string categoriaTalla = (talla >= 41) ? "41-48" : "34-40";

            // Llenamos primero cajas Master
            while (paresRestantes >= capacidadMaster[categoriaTalla])
            {
                cajas.Add($"Caja {numeroCaja} (Master) - Talla {talla}: {capacidadMaster[categoriaTalla]} pares");
                paresRestantes -= capacidadMaster[categoriaTalla];
                numeroCaja++;
            }

            // Luego llenamos cajas Docenera si quedan suficientes pares
            while (paresRestantes >= capacidadDocenera[categoriaTalla])
            {
                cajas.Add($"Caja {numeroCaja} (Docenera) - Talla {talla}: {capacidadDocenera[categoriaTalla]} pares");
                paresRestantes -= capacidadDocenera[categoriaTalla];
                numeroCaja++;
            }

            // Guardamos sobrantes para combinar después
            if (paresRestantes > 0)
            {
                sobrantes[talla] = paresRestantes;
            }
        }

        // Combinamos sobrantes en nuevas cajas sin exceder capacidad
        while (sobrantes.Count > 0)
        {
            Dictionary<int, int> tallasCombinadas = new Dictionary<int, int>();
            int totalPares = 0;
            string tipoCaja = "";
            int capacidad = 0;

            foreach (var entry in sobrantes.ToList())
            {
                int talla = entry.Key;
                int pares = entry.Value;
                string categoriaTalla = (talla >= 41) ? "41-48" : "34-40";
                int capacidadMaxima = capacidadMaster[categoriaTalla];

                if (tipoCaja == "" || capacidad == capacidadMaxima)
                {
                    tipoCaja = (totalPares + pares <= capacidadMaxima) ? "Master" : "Docenera";
                    capacidad = (tipoCaja == "Master") ? capacidadMaster[categoriaTalla] : capacidadDocenera[categoriaTalla];
                }

                if (totalPares + pares <= capacidad)
                {
                    tallasCombinadas[talla] = pares;
                    totalPares += pares;
                    sobrantes.Remove(talla);
                }
            }

            // Formatear la salida detallando cada talla y su cantidad de pares en la caja
            string detallesTallas = string.Join(", ", tallasCombinadas.Select(t => $"Talla {t.Key}: {t.Value} pares"));
            cajas.Add($"Caja {numeroCaja} ({tipoCaja}) - {detallesTallas}");
            numeroCaja++;
        }

        // Imprimimos el resultado
        foreach (var caja in cajas)
        {
            Console.WriteLine(caja);
        }
    }
}
