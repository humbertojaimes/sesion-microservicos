using Grpc.Core;
using GrpcDemoC;
using Messages;
using Microsoft.AspNetCore.Authorization;

namespace GrpcDemoC.Services;

public class StudentService : Messages.StudentService.StudentServiceBase
{
    readonly StudentCatalog _studentCatalog = new();

    public override async Task<StudentResponse> GetStudentById(GetStudentByIdRequest request, ServerCallContext context)
    {
        Metadata md = context.RequestHeaders;

        foreach (var mdEntry in md)
            Console.WriteLine($"{mdEntry.Key} {mdEntry.Value}");

        return new StudentResponse()
        { Student = _studentCatalog.Students.First(s => s.Id == request.Id)};
    }

    public override async Task SaveAllStudents(IAsyncStreamReader<StudentRequest> requestStream, IServerStreamWriter<StudentResponse> responseStream, ServerCallContext context)
    {

        while (await requestStream.MoveNext())
        {
            var student = requestStream.Current.Student;
            lock (this)
            {
                _studentCatalog.Students.Add(student);
            }

            await responseStream.WriteAsync(new StudentResponse()
            {
                Student = student
            });

        }

    }


    public override async Task<AddPhotoResponse> AddPhoto(IAsyncStreamReader<AddPhotoRequest> requestStream, ServerCallContext context)
    {
        Metadata md = context.RequestHeaders;
        var id = md.FirstOrDefault(e => e.Key.ToUpperInvariant() == "ID");
        var data = new List<byte>();
        while (await requestStream.MoveNext())
        {
            Console.WriteLine("Received " +
                requestStream.Current.Data.Length + " bytes");
            data.AddRange(requestStream.Current.Data);
        }
        Console.WriteLine($"Received file with {data.Count} bytes for student {id}");

        return new AddPhotoResponse()
        {
            IsUploaded = true
        };
    }

    //[Authorize]
    public override async Task GetAllStudents(GetAllStudentsRequest request, IServerStreamWriter<StudentResponse> responseStream, ServerCallContext context)
    {
        foreach (var student in _studentCatalog.Students)
        {
            await responseStream.WriteAsync(new StudentResponse()
            {
                 Student = student
            });
        }
    }

    public override Task<StudentResponse> AddStudent(StudentRequest request, ServerCallContext context)
    {
        return base.AddStudent(request, context);
    }

}

    
