using System.Net;
using System.Net.Sockets;
using System.Text;
 
IPAddress localAddress = IPAddress.Parse("127.0.0.1");
Console.Write("Введите свое имя: ");
string? username = Console.ReadLine();
Console.Write("Введите порт для приема сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var localPort)) return;
Console.Write("Введите порт для отправки сообщений: ");
if (!int.TryParse(Console.ReadLine(), out var remotePort)) return;
Console.WriteLine();
 
// запускаем получение сообщений
Task.Run(ReceiveMessageAsync);
// запускаем ввод и отправку сообщений
await SendMessageAsync();
 
// отправка сообщений в группу
async Task SendMessageAsync()
{
    using UdpClient sender = new UdpClient();
    Console.WriteLine("Для отправки сообщений введите сообщение и нажмите Enter");
    // отправляем сообщения
    while (true)
    {
        var message = Console.ReadLine(); // сообщение для отправки
        // если введена пустая строка, выходим из цикла и завершаем ввод сообщений
        if (string.IsNullOrWhiteSpace(message)) break;
        // иначе добавляем к сообщению имя пользователя
        message = $"{username}: {message}";
        byte[] data = Encoding.UTF8.GetBytes(message);
        // и отправляем на 127.0.0.1:remotePort
        await sender.SendAsync(data, new IPEndPoint(localAddress, remotePort));
    }
}
// отправка сообщений
async Task ReceiveMessageAsync()
{
    // сокет для прослушки сообщений
    using UdpClient receiver = new UdpClient(localPort);
    while (true)
    {
        // получаем данные в массив data
        var result = await receiver.ReceiveAsync();
        var message = Encoding.UTF8.GetString(result.Buffer);
        // выводим сообщение
        Print(message);
    }
}
 
void Print(string message)
{
    if (OperatingSystem.IsWindows())    // если ОС Windows
    {
        var position = Console.GetCursorPosition(); // получаем текущую позицию курсора
        int left = position.Left;   // смещение в символах относительно левого края
        int top = position.Top;     // смещение в строках относительно верха
        // копируем ранее введенные символы в строке на следующую строку
        Console.MoveBufferArea(0, top, left, 1, 0, top + 1);
        // устанавливаем курсор в начало текущей строки
        Console.SetCursorPosition(0, top);
        // в текущей строке выводит полученное сообщение
        Console.WriteLine(message);
        // переносим курсор на следующую строку
        // и пользователь продолжает ввод уже на следующей строке
        Console.SetCursorPosition(left, top + 1);
    }
    else Console.WriteLine(message);
}