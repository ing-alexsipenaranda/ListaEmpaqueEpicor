using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main()
    {
        var pedido = new List<(string referencia, int talla, int cantidad)>
        {
            ("2021E", 37, 17), ("2021E", 38, 34),
           ("2021E", 39, 10), ("2021E", 40, 136),
           ("2021E", 41, 1), 
           ("4059",37,1), ("4059",45,1)
        };

        var referenciasDocenera = new HashSet<string> { "1000", "2021E", "2022E", "2023E", "4058", "4059", "4059W", "5010", "5020" };
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

        var cajas = new List<string>();
        int cajaNumero = 1;

        foreach (var grupo in pedido.OrderBy(p => p.referencia).ThenBy(p => p.talla).GroupBy(p => p.referencia))
        {
            var referencia = grupo.Key;
            if (!capacidadCajas.ContainsKey(referencia)) continue;
            var (docenera34_40, docenera41_48, master34_40, master41_48) = capacidadCajas[referencia];

            var tallasOrdenadas = grupo.OrderBy(p => p.talla).ToList();
            Dictionary<int, int> tallasPendientes = tallasOrdenadas.ToDictionary(p => p.talla, p => p.cantidad);

            while (tallasPendientes.Values.Sum() > 0)
            {
                List<(int talla, int cantidad)> contenidoCaja = new List<(int, int)>();
                string tipoCaja = "";
                int capacidadRestante = 0;
                int paresEmpacados = 0;

                foreach (var talla in tallasPendientes.Keys.OrderBy(t => t).ToList())
                {
                    if (tallasPendientes[talla] == 0) continue;

                    bool usarDocenera = referenciasDocenera.Contains(referencia);
                    bool usarMaster = referenciasMaster.Contains(referencia) || !usarDocenera;

                    int capacidadMaxima = usarDocenera ? (talla <= 40 ? docenera34_40 : docenera41_48) : 0;
                    int capacidadMaster = usarMaster ? (talla <= 40 ? master34_40 : master41_48) : 0;

                    if (paresEmpacados == 0 && usarMaster && tallasPendientes[talla] >= capacidadMaster)
                    {
                        tipoCaja = "Master";
                        capacidadRestante = capacidadMaster;
                    }
                    else if (paresEmpacados == 0 && usarDocenera)
                    {
                        tipoCaja = "Docenera";
                        capacidadRestante = capacidadMaxima;
                    }

                    int aEmpacar = Math.Min(tallasPendientes[talla], capacidadRestante - paresEmpacados);
                    if (aEmpacar > 0)
                    {
                        contenidoCaja.Add((talla, aEmpacar));
                        tallasPendientes[talla] -= aEmpacar;
                        paresEmpacados += aEmpacar;
                    }

                    if (paresEmpacados >= capacidadRestante)
                        break;
                }

                if (contenidoCaja.Count > 0)
                {
                    string contenidoTexto = string.Join(", ", contenidoCaja.Select(c => $"Talla {c.talla}: {c.cantidad} pares"));
                    cajas.Add($"Caja {cajaNumero} ({tipoCaja}) - Referencia {referencia} - {contenidoTexto}");
                    cajaNumero++;
                }
            }
        }

        foreach (var caja in cajas)
        {
            Console.WriteLine(caja);
        }
    }
}
