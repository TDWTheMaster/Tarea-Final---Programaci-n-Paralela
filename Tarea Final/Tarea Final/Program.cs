using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("\n--- Bienvenido a la Carrera de Procesos Paralelos ---\n");

        var random = new Random();

        // Lista de "corredores" (procesos)
        var runners = new List<string> { "Proceso A", "Proceso B", "Proceso C", "Proceso D" };
        var tasks = new List<Task>();

        // Simulación de tareas principales (padres)
        foreach (var runner in runners)
        {
            tasks.Add(Task.Factory.StartNew(() =>
            {
                Console.WriteLine($"{runner} ha comenzado su carrera.");

                // Tarea hija unida a la tarea principal
                var childTask = Task.Factory.StartNew(async () =>
                {
                    // Simula una acción asíncrona con un retraso aleatorio
                    await Task.Delay(random.Next(1000, 3000));
                    Console.WriteLine($"{runner} completó su primera etapa.");
                }, TaskCreationOptions.AttachedToParent).Unwrap();

                // Continuación si la tarea fue exitosa
                childTask.ContinueWith(t =>
                {
                    Console.WriteLine($"{runner} terminó con éxito.");
                }, TaskContinuationOptions.OnlyOnRanToCompletion);

                // Continuación si la tarea fue cancelada (simulación)
                childTask.ContinueWith(t =>
                {
                    Console.WriteLine($"{runner} fue cancelado.");
                }, TaskContinuationOptions.OnlyOnCanceled);

                // Espera a la tarea hija
                childTask.Wait();
            }));
        }

        // Continuación cuando cualquiera de las tareas termina
        Task.Factory.ContinueWhenAny(tasks.ToArray(), completedTask =>
        {
            Console.WriteLine("\n--- Una de las tareas completó primero su etapa. ---\n");
        });

        // Esperar a que todas las tareas principales finalicen
        await Task.WhenAll(tasks);

        Console.WriteLine("\n--- Carrera terminada. ---\n");
    }
}