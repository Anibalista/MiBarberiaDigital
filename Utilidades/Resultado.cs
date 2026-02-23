
namespace Utilidades
{
    public class Resultado<T>
    {
        public bool Success { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static Resultado<T> Ok(T data, string mensaje = "") =>
            new Resultado<T> { Success = true, Data = data, Mensaje = mensaje };

        public static Resultado<T> Fail(string mensaje) =>
            new Resultado<T> { Success = false, Mensaje = mensaje };
    }
}
