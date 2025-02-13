using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var pedido = new List<(string referencia, int talla, int cantidad)>
        {
           ("2021E", 35,2), ("2021E", 36, 5),("2021E", 37, 3),("2021E", 38, 18),("2021E", 39,12),("2021E", 40,44),("2021E", 41,3),("2021E", 42,4),("2021E", 43,31),
           ("2022E", 38, 5),("2022E", 39, 4),("2022E", 40, 29),("2022E", 41, 30),("2022E", 42, 12),("2022E", 43,12),("2022E", 45,5),
           ("4059", 36, 2), ("4059", 37, 3),("4059", 38,1),("4059", 39,4),("4059", 40,10),("4059", 41,7),("4059", 42,2),("4059", 43,3),
           ("3046", 38, 1),("3046", 40,3),("3046", 41,4)


        };

        var referenciasDocenera = new HashSet<string> { "1000", "2021E", "2022E", "2023E" };
        var referenciasMaster = new HashSet<string> { "3045", "3046", "3048", "3049" };

        var capacidadCajas = new Dictionary<string, (int docenera34_40, int docenera41_48, int master34_40, int master41_48)>
        {
            {"1000", (17, 15, 21, 15)},
            {"2021E", (17, 15, 21, 15)},
            {"2022E", (17, 15, 21, 15)},
            {"2023E", (17, 15, 21, 15)},
            {"4058", (12, 12, 15, 15)},
            {"4059", (12, 12, 15, 15)},
            {"4059W", (12, 12, 15, 15)},
            {"5010", (12, 12, 15, 15)},
            {"5020", (12, 12, 15, 15)},
            {"3045", (9, 9, 12, 12)},
            {"3046", (9, 9, 12, 12)},
            {"3048", (9, 9, 12, 12)},
            {"3049", (9, 9, 12, 12)}
        };

        var cajas = new List<(string contenido, int pares)>();
        int cajaNumero = 1;

        foreach (var grupo in pedido.OrderBy(p => p.referencia).ThenBy(p => p.talla).GroupBy(p => p.referencia))
        {
            var referencia = grupo.Key;
            if (!capacidadCajas.ContainsKey(referencia)) continue;
            var (docenera34_40, docenera41_48, master34_40, master41_48) = capacidadCajas[referencia];

            var tallasAgrupadas = new Dictionary<int, int>();
            foreach (var item in grupo)
            {
                if (tallasAgrupadas.ContainsKey(item.talla))
                    tallasAgrupadas[item.talla] += item.cantidad;
                else
                    tallasAgrupadas[item.talla] = item.cantidad;
            }

            bool esDocenera = referenciasDocenera.Contains(referencia);
            bool esMaster = referenciasMaster.Contains(referencia);

            while (tallasAgrupadas.Values.Sum() > 0)
            {
                List<(int talla, int cantidad)> contenidoCaja = new List<(int, int)>();
                string tipoCaja = "";
                int capacidadRestante = 0;
                int paresEmpacados = 0;

                if (esMaster && tallasAgrupadas.Values.Sum() >= master34_40)
                {
                    tipoCaja = "Master";
                    capacidadRestante = master34_40;
                }
                else
                {
                    tipoCaja = "Docenera";
                    capacidadRestante = docenera34_40;
                }

                foreach (var talla in tallasAgrupadas.Keys.OrderBy(t => t).ToList())
                {
                    if (tallasAgrupadas[talla] == 0) continue;

                    int maxPorCaja = (talla >= 41) ? (esMaster ? master41_48 : docenera41_48) : capacidadRestante;
                    int aEmpacar = Math.Min(tallasAgrupadas[talla], maxPorCaja - paresEmpacados);
                    if (aEmpacar > 0)
                    {
                        contenidoCaja.Add((talla, aEmpacar));
                        tallasAgrupadas[talla] -= aEmpacar;
                        paresEmpacados += aEmpacar;
                    }

                    if (paresEmpacados >= maxPorCaja)
                        break;
                }

                if (contenidoCaja.Count > 0)
                {
                    string contenidoTexto = string.Join(", ", contenidoCaja.OrderBy(c => c.talla).Select(c => $"Talla {c.talla}: {c.cantidad} pares"));
                    cajas.Add(($"Caja {cajaNumero:D2} ({tipoCaja}) - Referencia {referencia} - {contenidoTexto}", paresEmpacados));
                    cajaNumero++;
                }
            }
        }

        for (int i = cajas.Count - 1; i > 0; i--)
        {
            if (cajas[i].pares <= 4 && cajas[i].contenido.Contains("Docenera"))
            {
                string referenciaCaja = cajas[i].contenido.Split('-')[1].Trim().Split(' ')[1];
                int capacidadMaximaMaster = capacidadCajas[referenciaCaja].master34_40;
                if (cajas[i - 1].contenido.Contains(referenciaCaja) && cajas[i - 1].pares + cajas[i].pares <= capacidadMaximaMaster)
                {
                    cajas[i - 1] = (cajas[i - 1].contenido.Replace("Docenera", "Master") + ", " + cajas[i].contenido.Split('-')[2].Trim(), cajas[i - 1].pares + cajas[i].pares);
                    cajas.RemoveAt(i);
                }
            }
        }

        foreach (var caja in cajas.OrderBy(c => int.Parse(c.contenido.Split(' ')[1])))
        {
            Console.WriteLine(caja.contenido);
        }
    }
}
