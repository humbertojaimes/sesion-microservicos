// See https://aka.ms/new-console-template for more information
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using Messages;


Console.WriteLine("Hello, World!");
string _token = "1eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJJc0FkbWluIjoidHJ1ZSIsIlVzZXIiOiJhZTJjZGE4Zi1mMTAwLTRmYzAtYmZiZi1mODFlZTEwNjM2NTUiLCJuYmYiOjE2NDMxNDY5MzcsImV4cCI6MTY0NTczODkzNywiaWF0IjoxNjQzMTQ2OTM3LCJpc3MiOiJodW1iZXJ0b2phaW1lcy5uZXQiLCJhdWQiOiJodW1iZXJ0b2phaW1lcy5uZXQifQ.fjazLH7leohvqspfYWMAfGdMSE9UlJRQuQ31ENPvC6k";

var credentials = CallCredentials.FromInterceptor((context, metadata) =>
{
    if (!string.IsNullOrEmpty(_token))
    {
        metadata.Add("Authorization", $"Bearer {_token}");
    }
    return Task.CompletedTask;
});

using var channel = GrpcChannel.ForAddress("http://localhost:5000");/*, new GrpcChannelOptions
{
    Credentials = ChannelCredentials.Create( new SslCredentials(),credentials)
});*/
var client = new Messages.StudentService.StudentServiceClient(channel);

while (true)
{
    Console.WriteLine("Elige una opción");
    Console.WriteLine("1. Get All Students");
    Console.WriteLine("2. Get Student By Id");
    Console.WriteLine("3. Add Photo");

    int option = int.Parse(Console.ReadLine());

    switch (option)
    {
        case 1:
            await GetAllStudents(client);
            break;
        case 2:
            await GetStudent(client);
            break;
        case 3:
            await AddPhoto(client);
            break;

    }
}


async Task AddPhoto(StudentService.StudentServiceClient client)
{
    Metadata md = new();
    md.Add("badgenumber", "5");

    FileStream fs = File.OpenRead("dotnet.jpg");
    using (var call = client.AddPhoto())
    {
        var stream = call.RequestStream;
        while (true)
        {
            byte[] buffer = new byte[64 * 1024];
            int numRead = await fs.ReadAsync(buffer, 0, buffer.Length);
            if (numRead == 0)
            {
                break;
            }
            if (numRead < buffer.Length)
            {
                Array.Resize(ref buffer, numRead);
            }

            await stream.WriteAsync(new Messages.AddPhotoRequest() { Data = ByteString.CopyFrom(buffer) });
        }
        await stream.CompleteAsync();

        var res = await call.ResponseAsync;

        Console.WriteLine(res.IsUploaded);
    }
}

async Task GetStudent(StudentService.StudentServiceClient client)
{
    var res = await client.GetStudentByIdAsync(new GetStudentByIdRequest {  Id = 1 });
    Console.WriteLine(res.Student);
}


async Task GetAllStudents(StudentService.StudentServiceClient client)
{
    string token = "Bearer 1eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJuYmYiOjE2NDMyMzk4MzksImV4cCI6MTY0MzI0MDczOSwiaWF0IjoxNjQzMjM5ODM5fQ.01D8VRKp7ZYC_tSxhTSxQSkJgTZYHslPTpRy0UbqPnU";
    Metadata md = new();
    md.Add("Authorization", token);
    using (var call = client.GetAllStudents(new GetAllStudentsRequest(),md))
    {
        var responseStream = call.ResponseStream;
        while (await responseStream.MoveNext())
        {
            Console.WriteLine(responseStream.Current.Student);
        }
    }
}